using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    public void PlayButton_OnClick() 
    {
        SceneManager.LoadScene("TextCrawlOpeningScene", LoadSceneMode.Single);
    }
    public void AIPlayButton_OnClick() { }
    public void CreditsButton_OnClick() { }
    public void QuitButton_OnClick() { }

}
