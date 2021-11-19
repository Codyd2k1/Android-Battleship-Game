using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class credits : MonoBehaviour
{
    public void goBackToMainMenu()
    {
        Debug.Log("Going Back to Main Menu");
        SceneManager.LoadScene("Menu");
    }
    
}
