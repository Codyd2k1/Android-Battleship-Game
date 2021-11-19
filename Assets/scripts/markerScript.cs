using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markerScript : MonoBehaviour
{
    public bool status;

    public bool SetStatus()
    {
        if(status == false)
        {
            status = true;
            return true;
        }
        else 
        {
            status = false;
            return false;
        }
    }
}
