using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color originalColor, oddColor;
    public SpriteRenderer renderer;
    public bool tileMarked;
    public bool checkedAlready;
    public GameObject markerPlaceShip;

    public GameObject markerAttackShip;
    public GameObject markerShipHit;

    public BoxCollider2D boxCollider2D;
    public Vector3 boxColliderOriginalSize;
    public Transform transform;

    public bool checkedForConnectingShip;

    public bool enableAttackMode;
    public bool hasBeenAttacked;
    public void Init(bool isOdd)
    {
        if(isOdd)
        {
            renderer.color = oddColor;
        }
        else
        {
            renderer.color = originalColor;
        }

    }

    public void OnMouseDown(){
        Debug.Log("tile clicked");
        if(!enableAttackMode)
        {
            if(markerPlaceShip.activeSelf)
            {
                markerPlaceShip.SetActive(false);
                tileMarked = false;
                return;
            }
            markerPlaceShip.SetActive(true);
            tileMarked = true;
            return;
        }
        else
        {
            if(markerAttackShip.activeSelf)
            {
                markerAttackShip.SetActive(false);
                tileMarked = false;
                return;
            }
            markerAttackShip.SetActive(true);
            tileMarked = true;
            return;
        }
    }

    public void displayShipHitMarker()
    {
        markerAttackShip.SetActive(false);
        markerPlaceShip.SetActive(false);
        markerShipHit.SetActive(true);
        disableBoxCollider();
    }

    public bool gettileMarked()
    {
        return tileMarked;
    }

    public bool getCheckedAlready()
    {
        return checkedAlready;
    }
    
    public void disableBoxCollider()
    {
        boxCollider2D.enabled = false;
    }

    public void enableBoxCollider()
    {
        boxCollider2D.enabled = true;
    }
    public Transform getTransform()
    {
        return transform;
    }
    
}
