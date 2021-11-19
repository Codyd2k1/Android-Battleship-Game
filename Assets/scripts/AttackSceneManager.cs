using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Obsolete("This class is no longer used due to issues with data transfer between scenes in Photon, -Cody.", true)]
public class AttackSceneManager : MonoBehaviourPunCallbacks
{
    private ArrayList enemyShipList;
    private ArrayList myShipList;
    private GridManager gridManager;
    private Dictionary<Vector2, Tile> attackingGrid;
    private ArrayList currentTotalSelectedPieces;
    private ArrayList turnSelectedPieces;
    [SerializeField]private Text itsYourTurnText;
    [SerializeField]private Text waitingOnOpponentText;
    [SerializeField]private Button fireButton;
    [SerializeField]private Text invalidEntryText;
    [SerializeField]private Text numHitShipsText;
    [SerializeField]private Text opponentsNumHitShipsText;

    [SerializeField]private Text youWinText;

    [SerializeField]private Text youLoseText;
    [SerializeField]private Button backToMainMenuButton;
    private GameManager gameManagerCopy;
    private int numHitShips;

    private int opponentsNumHitShips;

    public bool myTurn;
    // Start is called before the first frame update
    void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gridManager.GenerateGrid();

        storageBetweenScenes storageBetweenScenes =  new storageBetweenScenes();
        gameManagerCopy = GameObject.Find("GameManager").GetComponent<GameManager>();
        printArrayList(gameManagerCopy.otherPlayersShipList);
        enemyShipList = storageBetweenScenes.convertStringArrayListsToVectorArrayLists(gameManagerCopy.otherPlayersShipList);
        //first load in stored enemyship and home ship array;
        
        
        printArrayList(enemyShipList);

        
        gridManager.enableAttackMode();
        currentTotalSelectedPieces = new ArrayList();
        numHitShips = 0;
        //master client always has first turn
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I go first");
            myTurn = true;
            //gridManager.unlockEntireGrid();
            waitingOnOpponentText.gameObject.SetActive(false);
            itsYourTurnText.gameObject.SetActive(true);
            fireButton.gameObject.SetActive(true);
        }
        else if(!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I go second");
            myTurn = false;
            //gridManager.lockEntireGrid();
            waitingOnOpponentText.gameObject.SetActive(true);
            itsYourTurnText.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(false);
        }

        //players begin taking turns attacking at one another
    }

    private void printArrayList(ArrayList listToPrint)
    {
        if(listToPrint == null)
        {
            Debug.Log("Arraylist is null.");
            return;
        }
        Debug.Log("Arraylist Size: " + listToPrint.Count);
        foreach (var item in listToPrint)
        {
            Debug.Log("Position: " + item.ToString());
        }
    }
    
    [PunRPC]
    public void flipTurn(string defaultString)
    {
        Debug.Log("made it to flipping turn");
        if(myTurn == true)
        {
            Debug.Log("transferring turn to next player");
            //remove "it's your turn" message
            //gridManager.lockEntireGrid();
            sendShipHitScoreToOpponent();
            waitingOnOpponentText.gameObject.SetActive(true);
            itsYourTurnText.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(false);
            myTurn = false;
            return;
        }
        else if(myTurn == false)
        {
            Debug.Log("It's now my turn");
            //display "It's your turn!" message
            //gridManager.unlockEntireGrid();
            recieveShipHitScoreFromOpponent();
            if(int.Parse(opponentsNumHitShipsText.text) == 17)
            {
                //display "you lost, game over" message.
                gridManager.lockEntireGrid();
                youLoseText.gameObject.SetActive(true);
                backToMainMenuButton.gameObject.SetActive(true);
            }
            waitingOnOpponentText.gameObject.SetActive(false);
            itsYourTurnText.gameObject.SetActive(true);
            fireButton.gameObject.SetActive(true);
            myTurn = true;
            return;

        }
    }

    //implement the action for pressing "Fire"

    public void takeUserInputsFireButton()
    {
        bool countTilesResult = countTilesSelected(3);
        if(countTilesResult)
        {
            Debug.Log("Found 3 tiles selected");
            //lock selected pieces and display hit pieces, then begin next players turn
            gridManager.lockSelectedPieces(turnSelectedPieces);
            numHitShips += gridManager.displayHitPieces(turnSelectedPieces, enemyShipList);
            numHitShipsText.text = numHitShips.ToString();
            //send score to opponent
            Debug.Log("Before Sending Score");
            sendShipHitScoreToOpponent();
            Debug.Log("After Sending Score");
            if(numHitShips == 17) //win
            {
                gridManager.lockEntireGrid();
                youWinText.gameObject.SetActive(true);
                backToMainMenuButton.gameObject.SetActive(true);
            }
            gameManagerCopy.photonView.RPC("flipTurn", RpcTarget.Others, "test");
            Debug.Log("RPC CALLED");

            flipTurn("test");
        }
        else{
            Debug.Log("Did Not Find 3 tiles selected");
        }
    }

    private void sendShipHitScoreToOpponent()
    {
        Debug.Log("Sending ship score of " + numHitShips.ToString());
        ExitGames.Client.Photon.Hashtable playerShipsHitScore = new ExitGames.Client.Photon.Hashtable();
        playerShipsHitScore["score"] = numHitShips.ToString();
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerShipsHitScore);
    }

    private void recieveShipHitScoreFromOpponent()
    {
        Debug.Log("Recieving ship hit score...");
        if(!(PhotonNetwork.PlayerListOthers[0].CustomProperties == null))
        {
            Debug.Log("custom properties contains score: " + PhotonNetwork.PlayerListOthers[0].CustomProperties["score"].ToString());
            opponentsNumHitShipsText.text = PhotonNetwork.PlayerListOthers[0].CustomProperties["score"].ToString();
        }
    }
    private bool countTilesSelected(int requiredAmount)
    {
        turnSelectedPieces = new ArrayList();
        int numTilesSelected = 0;
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2 currentPosition = new Vector2((float)x, (float)y);
                Tile tile = gridManager.getTileFromPosition(currentPosition);
                if(tile.tileMarked)
                {
                    if(!tile.checkedAlready)
                    {
                        numTilesSelected++;
                        tile.checkedAlready = true;
                        turnSelectedPieces.Add(currentPosition);
                    }
                    //checking if tiles selected contain a ship should be done
                    //after we count that the user has selected 3 tiles.
                }
            }
        }
        Debug.Log("Found " + numTilesSelected + " tiles selected");
        if(numTilesSelected == requiredAmount)
        {
            invalidEntryText.gameObject.SetActive(false);
            return true;
        }
        else
        {
            invalidEntryText.gameObject.SetActive(true);
            gridManager.revertTilesCheckedAlready(turnSelectedPieces);
            return false;
        }
    }

    private bool checkIfTileContainsShip(Vector2 tilesPosition)
    {
        if(enemyShipList.Contains(tilesPosition))
        {
            return true;
        }
        else return false;
    }

    private void playTurn()
    {
        if(myTurn == true)
        {
            //display "select 3 pieces to attack and press fire"
        }
        
    }

    private void finalizeSetupButtonPressed()
    {
        return;
    }

    public void goBackToMainMenuFromGame()
    {
        Debug.Log("Going Back to Main Menu");
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        attackingGrid = gridManager.getTiles();

    }

    /*
    //from gridmanager:
    [PunRPC]
    public void loadAttackingStage(string defaultString)
    {
        PhotonNetwork.LoadLevel("gameAttackingScene");
    }

    [PunRPC]
    void callFinalizeSetupAgain(string test)
    {
        finalizeSetupButtonPressed();
    }

    [PunRPC]
    public void enablePositionTextComponent(string testTring)
    {
        //enable on both players screens at once.
        return;
    }

    
    [PunRPC]
    public void tellOtherPlayerNonMasterSetupComplete(string testString)
    {
        Debug.Log("non master setup bool updated to complete");
        return;
    }

    [PunRPC]
    void sendPlayerData(string testString)
    {
        return;
    }

    [PunRPC]
    void recievePlayerData(string test)
    {
        return;
    }
    */
}
