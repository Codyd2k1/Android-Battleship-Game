using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameAudioScript : MonoBehaviour
{
    private static gameAudioScript instance = null;
    public static gameAudioScript Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
