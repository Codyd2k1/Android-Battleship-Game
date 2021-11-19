using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class splashScript : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;


    void Start()
    {
        StartCoroutine(playVideo());
    }
    
    private IEnumerator playVideo()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene("Menu");
    }
}
