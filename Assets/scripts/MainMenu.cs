using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image muteAudioMarker;

    private void Start()
    {
        if(GameObject.Find("mainMenuAudio").GetComponent<AudioSource>().isPlaying)
        {
            muteAudioMarker.gameObject.SetActive(false);
        }
        else
        {
            muteAudioMarker.gameObject.SetActive(true);
        }
    }
    public void playGame ()
    {
        Debug.Log("Joining Game");
        SceneManager.LoadScene("Loading");
    }

    public void quitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }

    public void goToCredits()
    {
        Debug.Log("Going to Credits Scene");
        SceneManager.LoadScene("Credits");
    }

    public void muteAudio()
    {
        AudioSource audioSource = GameObject.Find("mainMenuAudio").GetComponent(typeof(AudioSource)) as AudioSource;
        Debug.Log("mute pressed!");

        if(muteAudioMarker.IsActive())
        {
            Debug.Log("mute turned off!");
            muteAudioMarker.gameObject.SetActive(false);
            audioSource.UnPause();
        }
        else if(!muteAudioMarker.IsActive())
        {
            Debug.Log("mute turned on!");
            muteAudioMarker.gameObject.SetActive(true);
            audioSource.Pause();
        }
    }

    public void helpButton()
    {
        Application.OpenURL("https://www.hasbro.com/common/instruct/Battleship.PDF");
    }
}
