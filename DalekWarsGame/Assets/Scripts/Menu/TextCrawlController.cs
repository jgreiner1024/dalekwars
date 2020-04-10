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
        introMusic.Play();
        Color introColor = introText.color;
        
        introColor.a = 0f;
        introText.color = introColor;
        introCanvas.gameObject.SetActive(true);

        //fade in intro text
        for (float a = 0; a <= 1; a += 0.01f)
        {
            introColor.a = a;
            introText.color = introColor;
            yield return new WaitForSeconds(0.01f);
        }

        
        yield return new WaitForSeconds(1.5f);


        float alpha = 1f;
        //fade out intro text
        for (; alpha >= 0; alpha -= 0.01f)
        {
            introColor.a = alpha;
            introText.color = introColor;
            yield return new WaitForSeconds(0.01f);
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

        //move logo away from the camera
        for (float zpos = 0; zpos < 1000; zpos++)
        {
            logoPosition.z = zpos;
            logoCanvas.transform.position = logoPosition;
            
            //fade out logo close to the end
            if(zpos > 800)
            {
                alpha -= 0.01f;
                logoColor.a = alpha;
                logoText.color = logoColor;

            }


            if(zpos > 250)
            {
                crawlPosition.y = ypos;
                crawlCanvas.transform.localPosition = crawlPosition;
                ypos += 0.2f;
            }

            yield return new WaitForSeconds(0.01f);
        }

        logoCanvas.gameObject.SetActive(false);


        

        

        //move along the Y axis to move up the rotated game object away from the camera
        for (; ypos < 1700; ypos += 0.25f)
        {
            crawlPosition.y = ypos;
            crawlCanvas.transform.localPosition = crawlPosition;
            yield return new WaitForSeconds(0.01f);
        }

        crawlCanvas.gameObject.SetActive(false);
        
        //a small wait so we don't cut off the music
        yield return new WaitForSeconds(0.5f);

        MoveScene();
    }

    private void MoveScene()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
