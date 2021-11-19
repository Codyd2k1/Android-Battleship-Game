using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storageBetweenScenes
{
    public static ArrayList enemyShipPositions;

    public static ArrayList myShipPositions;

    private bool listsConvertedToVectors;
    
    public ArrayList getEnemyShipPositions()
    {
        return enemyShipPositions;
    }

    public void setEnemyShipPositions(ArrayList opponentShipPositions)
    {
        if(opponentShipPositions == null)
        {
            Debug.Log("Attempt to set opponent ship positions is null.");
        }
        enemyShipPositions = opponentShipPositions;
    }

    public ArrayList getMyShipPositions()
    {
        return myShipPositions;
    }

    public void setMyShipPositions(ArrayList shipPositions)
    {
        myShipPositions = shipPositions;
    }

    public void convertStringArrayListsToVectorArrayLists()
    {
        if(enemyShipPositions.Count > 0 && listsConvertedToVectors != true)
        {
            enemyShipPositions = convertStringArrayListsToVectorArrayLists(enemyShipPositions);
            listsConvertedToVectors = true;
        }
        
    }
    public ArrayList convertStringArrayListsToVectorArrayLists(ArrayList stringPositionArrayListToConvert)
    {
        ArrayList vectorArrayList = new ArrayList();
        foreach (string position in stringPositionArrayListToConvert)
        {
            Debug.Log("attempting to convert " + position + " to float");
            if(!string.IsNullOrEmpty(position))
            {
                string[] xySplitArray = position.Split(new char[] {',', '(', ')'});
                foreach (string substring in xySplitArray)
                {
                    Debug.Log("substring: " + substring);
                }
                //if(string.IsNullOrEmpty(xySplitArray[0]))
                float xPosition = float.Parse(xySplitArray[1]);
                float yPosition = float.Parse(xySplitArray[2]);
                Vector2 currentPosition = new Vector2(xPosition,yPosition);
                vectorArrayList.Add(currentPosition);
            }
        }
        return vectorArrayList;
    }



}
