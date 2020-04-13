using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TextCrawlController : MonoBehaviour
{
    [SerializeField]
    private Canvas introCanvas;

    [SerializeField]
    private TextMeshProUGUI  introText;

    [SerializeField]
    private Canvas logoCanvas;

    [SerializeField]
    private TextMeshProUGUI logoText;
    
    [SerializeField]
    private Canvas crawlCanvas;

    private Coroutine textCrawl;
    private AudioSource introMusic;

    private void Awake()
    {
        introMusic = GetComponent<AudioSource>();
    }

    private void Start()
    {
        
        textCrawl = StartCoroutine(TextCrawlRoutine());
    }

    // Start is called before the first frame update
    public void SkipButton_OnClick()
    {
        StopCoroutine(textCrawl);
        MoveScene();
    }

    private IEnumerator TextCrawlRoutine()
    {
        //times used for animation timing to be more frame accurate
        float elapsedTime = 0f;
        float duration = 0f;

        introMusic.Play();
        Color introColor = introText.color;
        
        introColor.a = 0f;
        introText.color = introColor;
        introCanvas.gameObject.SetActive(true);


        //fade in intro text
        duration = 1f;
        for (elapsedTime = 0f; elapsedTime <= duration; elapsedTime += Time.deltaTime)
        {
            introColor.a = (elapsedTime / duration) * 1f; 
            introText.color = introColor;
            yield return new WaitForEndOfFrame();
        }

        duration = 2.2f;
        for (elapsedTime = 0f; elapsedTime <= duration; elapsedTime += Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
        }

        float alpha = 1f;
        //fade out intro text
        duration = 1f;
        for (elapsedTime = 0f; elapsedTime <= duration; elapsedTime += Time.deltaTime)
        {
            introColor.a = 1f - ((elapsedTime / duration) * 1f);
            introText.color = introColor;
            yield return new WaitForEndOfFrame();
        }
        introCanvas.gameObject.SetActive(false);



        Color logoColor = logoText.color;
        Vector3 logoPosition = logoCanvas.transform.position;
        Vector3 crawlPosition = crawlCanvas.transform.localPosition;

        //reset logo color
        alpha = 1f;
        logoColor.a = alpha;
        logoText.color = logoColor;

        //reset the logo position then set active
        logoPosition.z = 0f;
        logoCanvas.transform.position = logoPosition;
        logoCanvas.gameObject.SetActive(true);

        //reset the crawl position
        crawlPosition.y = 0f;
        crawlCanvas.transform.localPosition = crawlPosition;
        crawlCanvas.gameObject.SetActive(true);

        float ypos = 0;
        duration = 10f;
        float crawlDuration = 85.3f;
        for (elapsedTime = 0f; elapsedTime <= (duration + 0.5f); elapsedTime += Time.deltaTime)
        {
            logoPosition.z = 1000 * (elapsedTime / duration);
            logoCanvas.transform.position = logoPosition;

            //after 8 seconds start fading out for 2 seconds
            if (elapsedTime > 8f)
            {
                logoColor.a = 2f - (((elapsedTime - 8f) / (duration - 8f)) * 2f) ;
                logoText.color = logoColor;
            }

            //start moving the text crawl
            if(elapsedTime > 2.5f)
            {
                crawlPosition.y = ((elapsedTime - 2.5f) / crawlDuration) * 1700f;
                crawlCanvas.transform.localPosition = crawlPosition;
            }

            if(elapsedTime >= 10f && duration <= 10f)
            {
                duration = 2.5f + crawlDuration;
                logoCanvas.gameObject.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }


        crawlCanvas.gameObject.SetActive(false);



        //continue moving the text crawl
        //duration += 4.25f;
        ////move along the Y axis to move up the rotated game object away from the camera
        //for (; elapsedTime <= duration; elapsedTime += Time.deltaTime)
        //{
        //    crawlPosition.y = ypos;
        //    crawlCanvas.transform.localPosition = crawlPosition;
        //    yield return new WaitForSeconds(0.01f);
        //}

        
        
        //a small wait so we don't cut off the music
        yield return new WaitForSeconds(0.5f);

        MoveScene();
    }

    private void MoveScene()
    {
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
