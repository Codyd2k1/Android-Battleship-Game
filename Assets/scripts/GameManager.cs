using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;
public class GameManager : MonoBehaviourPunCallbacks
{
    //new image parent objects
    [SerializeField] private Image carrierInstructions;
    [SerializeField] private Image battleshipInstructions;
    [SerializeField] private Image cruiserInstructions;
    [SerializeField] private Image destroyerInstructions;
    [SerializeField] private Image submarineInstructions;

    [SerializeField] public Image invalidEntryIncorrectNumberOfSpaces;
    [SerializeField] public Image invalidEntrySpacesNotTouching;

    [SerializeField] private Image doneWithSetupImage;
    [SerializeField] private Image waitingOnOpponentToFinishSetupImage;

    [SerializeField] private Image waitingForOpponentImage;

    [SerializeField] private Image itsYourTurnImage;
    [SerializeField] private Image opponentsTurnImage;

    [SerializeField] private Image scoreBoardImage;
    [SerializeField] private TMP_Text yourScoreNumber;
    [SerializeField] private TMP_Text opponentsScoreNumber;

    [SerializeField] public Image invalidEntryIncorrectNumberOfSpacesAttackStage;

    [SerializeField] private Image youWinImage;

    [SerializeField] private TMP_Text youLostText;

    public Dictionary<Vector2, Tile> tiles;
    public GridManager gridManager;

    Vector2 nullVector2;

    public bool setupComplete;

    public ArrayList shipPositions;

    public ArrayList shipPositionsMasterClient;

    public ArrayList shipPositionsNonMasterClient;
    private ExitGames.Client.Photon.Hashtable playerShipPositions = new ExitGames.Client.Photon.Hashtable();

    //public Button doneWithSetupButton;

    private bool masterClientPlayerSetupComplete;

    private bool nonMasterClientPlayerSetupComplete;

    public Text testRecievedEnemyShipPositions;

    //public Text waitingOnOpponentText;

    public ArrayList otherPlayersShipList;

    private bool destroyerSetupDone;

    // Attack Stage Variables 
    private bool loadAttackStageCalledMaster;
    private bool loadAttackStageCalledNonMaster;
    private ArrayList enemyShipList;
    private ArrayList myShipList;
    private Dictionary<Vector2, Tile> attackingGrid;
    private ArrayList currentTotalSelectedPieces;
    private ArrayList turnSelectedPieces;
    

    public void construct(Image _carrierInstructions,
        Image _battleshipInstructions,
        Image _cruiserInstructions,
        Image _destroyerInstructions,
        Image _submarineInstructions,
        Image _invalidEntryIncorrectNumberOfSpaces,
        Image _invalidEntrySpacesNotTouching,
        Image _doneWithSetupImage,
        Image _waitingOnOpponentToFinishSetupImage,
        Image _waitingForOpponentImage,
        Image _itsYourTurnImage,
        Image _opponentsTurnImage,
        Image _scoreBoardImage,
        Image _invalidEntryIncorrectNumberOfSpacesAttackStage,
        Image _youWinImage,
        TMP_Text _youLostText,
        TMP_Text _yourScoreNumber,
        TMP_Text _opponentsScoreNumber)
    {
        this.carrierInstructions = _carrierInstructions;
        this.battleshipInstructions = _battleshipInstructions;
        this.cruiserInstructions = _cruiserInstructions;
        this.destroyerInstructions = _destroyerInstructions;
        this.submarineInstructions = _submarineInstructions;
        this.invalidEntryIncorrectNumberOfSpaces = _invalidEntryIncorrectNumberOfSpaces;
        this.invalidEntrySpacesNotTouching = _invalidEntrySpacesNotTouching;
        this.doneWithSetupImage = _doneWithSetupImage; 
        this.waitingOnOpponentToFinishSetupImage = _waitingOnOpponentToFinishSetupImage;
        this.waitingForOpponentImage = _waitingForOpponentImage;
        this.itsYourTurnImage = _itsYourTurnImage;
        this.opponentsTurnImage = _opponentsTurnImage;
        this.scoreBoardImage = _scoreBoardImage;
        this.yourScoreNumber = _yourScoreNumber;
        this.opponentsScoreNumber = _opponentsScoreNumber;
        this.invalidEntryIncorrectNumberOfSpacesAttackStage = _invalidEntryIncorrectNumberOfSpacesAttackStage;
        this.youWinImage = _youWinImage;
        this.youLostText = _youLostText;


    }

    [SerializeField]private Button backToMainMenuButton;
    
    public int numHitShips;

    private int opponentsNumHitShips;

    public bool myTurn;



   
    [PunRPC]
    public void activateWaitingImage(string defaultInput)
    {
        waitingForOpponentImage.gameObject.SetActive(true);
        gridManager.lockEntireGrid();
        
    }

    [PunRPC]
    public void deActivateWaitingImage(string defaultInput)
    {
        waitingForOpponentImage.gameObject.SetActive(false);
        gridManager.unlockEntireGrid();

    }



    void Start()
    {
        
        Debug.Log("Starting Game...");
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gridManager.GenerateGrid();

        
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
        {
            this.photonView.RPC("activateWaitingImage", RpcTarget.All, "defaultString");
        }
        else
        {
            this.photonView.RPC("deActivateWaitingImage", RpcTarget.All, "defaultString");
        }
       
        Destroy(GameObject.Find("mainMenuAudio"));
        
        nullVector2 = new Vector2(1000, 1000);
        shipPositions = new ArrayList();
        setupComplete = false;
        masterClientPlayerSetupComplete = false;
        nonMasterClientPlayerSetupComplete = false;  
        //tiles = gridManager.getTiles();

        beginGameSetup();
    }

    void beginGameSetup()
    {
        Debug.Log("Instructions Button/Text Active");
        carrierInstructions.gameObject.SetActive(true);

    }
    public ArrayList getNumTilesSelected(int numTilesRequired)
    {
        int numTilesSelected = 0;
        ArrayList tilesCounted = new ArrayList();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2 currentPosition = new Vector2((float)x, (float)y);
                Tile tile = gridManager.getTileFromPosition(currentPosition);
                if (tile.tileMarked == true)
                {
                    if (tile.checkedAlready != true)
                    {
                        tilesCounted.Add(currentPosition);
                        
                        numTilesSelected++;
                        Debug.Log("NumTiles == " + numTilesSelected + " tile to count at: " + tile.transform.position.ToString());

                        Debug.Log("Found Tile with ship, numTilesSelected is: " + numTilesSelected);
                        tile.checkedAlready = true;
                    }
                }
            }
        }

        if (numTilesSelected != numTilesRequired)
        {
            gridManager.revertTilesCheckedAlready(tilesCounted);
            return null;
        }
        else if (numTilesSelected == numTilesRequired)
        {
            return tilesCounted;
        }
        else return null;
    }

    public void lockTileSelections(ArrayList tilesCounted)
    {
        foreach (Vector2 position in tilesCounted)
        {
            Tile tile = gridManager.getTileFromPosition(position);
            tile.disableBoxCollider();
        }
    }

    private Vector2 findFirstTilePosition(ArrayList tilesCounted)
    {
        foreach (Vector2 currentTilePosition in tilesCounted)
        {
            //generate possible connecting coordinates
            ArrayList connectingPieces = new ArrayList();
            connectingPieces.Add(new Vector2(currentTilePosition.x, currentTilePosition.y + 1));
            connectingPieces.Add(new Vector2(currentTilePosition.x, currentTilePosition.y - 1));
            connectingPieces.Add(new Vector2(currentTilePosition.x + 1, currentTilePosition.y));
            connectingPieces.Add(new Vector2(currentTilePosition.x - 1, currentTilePosition.y));
            Debug.Log("current tile position: " + currentTilePosition.ToString());
            printArrayList(connectingPieces);
            //if tilesCounted contains exactly one of the possible coordinates,
            int numConnectingTilesContainedInTilesCounted = 0;
            foreach (Vector2 connectingPosition in connectingPieces)
            {
                if (tilesCounted.Contains(connectingPosition))
                {
                    numConnectingTilesContainedInTilesCounted++;
                }
            }
            if (numConnectingTilesContainedInTilesCounted == 1)
            {
                return currentTilePosition;
            }
        }
        return nullVector2;
    }

    private void printArrayList(ArrayList listToPrint)
    {
        Debug.Log("Arraylist Size: " + listToPrint.Count);
        foreach (var item in listToPrint)
        {
            Debug.Log("Position: " + item.ToString());
        }
    }

    private bool compareVectorArrayLists(ArrayList one, ArrayList two)
    {
        if (one.Count != two.Count)
        {
            return false;
        }

        for (int i = 0; i < one.Count; i++)
        {
            if (!one[i].Equals(two[i]))
            {
                Debug.Log("array 1: " + one[i].ToString() + "Not equal to: " + two[i].ToString());
                return false;
            }
        }
        return true;

    }
    private bool findFivePiecesConnecting(ArrayList tilesCounted, Vector2 firstTilePosition, int numRequiredSelected)
    {
        printArrayList(tilesCounted);
        ArrayList possiblePositionsAbove = new ArrayList();
        ArrayList possiblePositionsRight = new ArrayList();
        ArrayList possiblePositionsBelow = new ArrayList();
        ArrayList possiblePositionsLeft = new ArrayList();

        for (int i = 0; i < numRequiredSelected; i++)
        {
            possiblePositionsAbove.Add(new Vector2(firstTilePosition.x, firstTilePosition.y + i));
            possiblePositionsRight.Add(new Vector2(firstTilePosition.x + i, firstTilePosition.y));
            possiblePositionsBelow.Add(new Vector2(firstTilePosition.x, firstTilePosition.y - i));
            possiblePositionsLeft.Add(new Vector2(firstTilePosition.x - i, firstTilePosition.y));
        }
        Debug.Log("Possible possitions above: ");
        printArrayList(possiblePositionsAbove);
        Debug.Log("Possible possitions below: ");
        printArrayList(possiblePositionsBelow);
        Debug.Log("Possible possitions Right: ");
        printArrayList(possiblePositionsRight);
        Debug.Log("Possible possitions Left: ");
        printArrayList(possiblePositionsLeft);
        if (compareVectorArrayLists(tilesCounted, possiblePositionsAbove))
        {
            return true;
        }
        else if (compareVectorArrayLists(tilesCounted, possiblePositionsBelow))
        {
            return true;
        }
        else if (compareVectorArrayLists(tilesCounted, possiblePositionsLeft))
        {
            return true;
        }
        else if (compareVectorArrayLists(tilesCounted, possiblePositionsRight))
        {
            return true;
        }
        else return false;
    }

    
    
    public void addTileSelectionsToShipList(ArrayList tilesCounted)
    {
        foreach (Vector2 tilePosition in tilesCounted)
        {
            shipPositions.Add(tilePosition);
        }
    }
    public void takeUserInputsCarrier()
    {
        ArrayList tilesCounted = getNumTilesSelected(5);
        if (tilesCounted != null)
        {
            Vector2 firstTilePosition = findFirstTilePosition(tilesCounted);
            if (firstTilePosition != nullVector2)
            {
                Debug.Log("First Tile Piece found at: " + firstTilePosition.ToString());
                if (findFivePiecesConnecting(tilesCounted, firstTilePosition, 5))
                {
                    Debug.Log("Success!");
                    lockTileSelections(tilesCounted);
                    addTileSelectionsToShipList(tilesCounted);
                    /*
                    instructionsTextCarrier.gameObject.SetActive(false);
                    instructionsButtonCarrier.gameObject.SetActive(false);
                    */
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(false);
                    carrierInstructions.gameObject.SetActive(false);
                    battleshipInstructions.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("failure, set of 5 tiles not found.");
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                    gridManager.revertTilesCheckedAlready(tilesCounted);
                }
            }
            else
            {
                Debug.Log("failure, no first tile position found");
                invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                gridManager.revertTilesCheckedAlready(tilesCounted);
            }
        }
        else
        {
            Debug.Log("failure, invalid number of tiles selected");
            invalidEntrySpacesNotTouching.gameObject.SetActive(false);
            displayTryAgainMessage();
        }

    }


    public void takeUserInputsBattleship()
    {
        ArrayList tilesCounted = getNumTilesSelected(4);
        if (tilesCounted != null)
        {
            Vector2 firstTilePosition = findFirstTilePosition(tilesCounted);
            if (firstTilePosition != nullVector2)
            {
                Debug.Log("First Tile Piece found at: " + firstTilePosition.ToString());
                if (findFivePiecesConnecting(tilesCounted, firstTilePosition, 4))
                {
                    Debug.Log("Success!");
                    lockTileSelections(tilesCounted);
                    addTileSelectionsToShipList(tilesCounted);
                    battleshipInstructions.gameObject.SetActive(false);
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(false);
                    cruiserInstructions.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("failure, set of 5 tiles not found.");
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                    gridManager.revertTilesCheckedAlready(tilesCounted);
                }
            }
            else
            {
                Debug.Log("failure, no first tile position found");
                invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                gridManager.revertTilesCheckedAlready(tilesCounted);
            }
        }
        else
        {
            Debug.Log("failure, invalid number of tiles selected");
            invalidEntrySpacesNotTouching.gameObject.SetActive(false);
            displayTryAgainMessage();
        }

    }

    public void takeUserInputsCruiser()
    {
        ArrayList tilesCounted = getNumTilesSelected(3);
        if (tilesCounted != null)
        {
            Vector2 firstTilePosition = findFirstTilePosition(tilesCounted);
            if (firstTilePosition != nullVector2)
            {
                Debug.Log("First Tile Piece found at: " + firstTilePosition.ToString());
                if (findFivePiecesConnecting(tilesCounted, firstTilePosition, 3))
                {
                    Debug.Log("Success!");
                    lockTileSelections(tilesCounted);
                    addTileSelectionsToShipList(tilesCounted);
                    cruiserInstructions.gameObject.SetActive(false);
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(false);
                    submarineInstructions.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("failure, set of 5 tiles not found.");
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                    gridManager.revertTilesCheckedAlready(tilesCounted);
                }
            }
            else
            {
                Debug.Log("failure, no first tile position found");
                invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                gridManager.revertTilesCheckedAlready(tilesCounted);
            }
        }
        else
        {
            Debug.Log("failure, invalid number of tiles selected");
            invalidEntrySpacesNotTouching.gameObject.SetActive(false);
            displayTryAgainMessage();
        }

    }

    public void takeUserInputsSubmarine()
    {
        ArrayList tilesCounted = getNumTilesSelected(3);
        if (tilesCounted != null)
        {
            Vector2 firstTilePosition = findFirstTilePosition(tilesCounted);
            if (firstTilePosition != nullVector2)
            {
                Debug.Log("First Tile Piece found at: " + firstTilePosition.ToString());
                if (findFivePiecesConnecting(tilesCounted, firstTilePosition, 3))
                {
                    Debug.Log("Success!");
                    lockTileSelections(tilesCounted);
                    addTileSelectionsToShipList(tilesCounted);
                    submarineInstructions.gameObject.SetActive(false);
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(false);
                    destroyerInstructions.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("failure, set of 5 tiles not found.");
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                    gridManager.revertTilesCheckedAlready(tilesCounted);
                }
            }
            else
            {
                Debug.Log("failure, no first tile position found");
                invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                gridManager.revertTilesCheckedAlready(tilesCounted);
            }
        }
        else
        {
            Debug.Log("failure, invalid number of tiles selected");
            invalidEntrySpacesNotTouching.gameObject.SetActive(false);
            displayTryAgainMessage();
        }

    }

    public void takeUserInputsDestroyer()
    {
        ArrayList tilesCounted = getNumTilesSelected(2);
        if (tilesCounted != null)
        {
            Vector2 firstTilePosition = findFirstTilePosition(tilesCounted);
            if (firstTilePosition != nullVector2)
            {
                Debug.Log("First Tile Piece found at: " + firstTilePosition.ToString());
                if (findFivePiecesConnecting(tilesCounted, firstTilePosition, 2))
                {
                    Debug.Log("Success!");
                    lockTileSelections(tilesCounted);
                    addTileSelectionsToShipList(tilesCounted);
                    destroyerInstructions.gameObject.SetActive(false);
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(false);
                    //activate this button to begin firing stage.
                    destroyerSetupDone = true;
                    gridManager.lockEntireGrid();
                    doneWithSetupImage.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("failure, set of 5 tiles not found.");
                    invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                    invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                    gridManager.revertTilesCheckedAlready(tilesCounted);
                }
            }
            else
            {
                Debug.Log("failure, no first tile position found");
                invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(false);
                invalidEntrySpacesNotTouching.gameObject.SetActive(true);
                gridManager.revertTilesCheckedAlready(tilesCounted);
            }
        }
        else
        {
            Debug.Log("failure, invalid number of tiles selected");
            invalidEntrySpacesNotTouching.gameObject.SetActive(false);
            displayTryAgainMessage();
        }
    }

    void displayWaitingForOpponentToFinishSetup()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(masterClientPlayerSetupComplete == true && nonMasterClientPlayerSetupComplete == false)
            {
                doneWithSetupImage.transform.Find("ContinueNextButton").gameObject.SetActive(false);
                waitingOnOpponentToFinishSetupImage.gameObject.SetActive(true);
            }
        }
        else if(!PhotonNetwork.IsMasterClient)
        {
            if(nonMasterClientPlayerSetupComplete == true && masterClientPlayerSetupComplete == false)
            {
                doneWithSetupImage.transform.Find("ContinueNextButton").gameObject.SetActive(false);
                waitingOnOpponentToFinishSetupImage.gameObject.SetActive(true);
            }
        }
    }

    public void finalizeSetupButtonPressed()
    {
        Debug.Log("Finalize Button Pressed");
        if(PhotonNetwork.IsMasterClient)
        {
            //call rpc to update master client to complete on both
            Debug.Log("This is master client");
            this.photonView.RPC("tellOtherPlayerMasterSetupComplete", RpcTarget.All, "test");
            
        }
        else{
            //call rpc to update non master client to complete on both
            Debug.Log("This is not master client");
            this.photonView.RPC("tellOtherPlayerNonMasterSetupComplete", RpcTarget.All, "test");
        }

        Debug.Log("Master client status: " + masterClientPlayerSetupComplete.ToString() + 
                ", Non master client Status: " + nonMasterClientPlayerSetupComplete.ToString());
        
        displayWaitingForOpponentToFinishSetup();
        if(nonMasterClientPlayerSetupComplete && masterClientPlayerSetupComplete)
        {
            this.photonView.RPC("sendPlayerData", RpcTarget.All, "test");
            waitingOnOpponentToFinishSetupImage.gameObject.SetActive(false);
            doneWithSetupImage.gameObject.SetActive(false);
            
        }
    }

    private void setScoringObjectsActive()
    {
        scoreBoardImage.gameObject.SetActive(true);
    }



    public void loadAttackingStage(string defaultString)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            loadAttackStageCalledMaster = true;
        }
        else
        {
            loadAttackStageCalledNonMaster = true;
        }
        doneWithSetupImage.gameObject.SetActive(false);
        waitingOnOpponentToFinishSetupImage.gameObject.SetActive(false);
        //this is the "start()" method of the attack stage.
        //generate new grid and show scores / waiting or playing..
        gridManager.hideOldGrid();
        gridManager.GenerateGrid();
        gridManager.enableAttackMode();
        storageBetweenScenes storageBetweenScenes = new storageBetweenScenes();
        enemyShipList = storageBetweenScenes.convertStringArrayListsToVectorArrayLists(otherPlayersShipList);
        setScoringObjectsActive();
        Debug.Log("Printing enemy ship list:");
        printArrayList(enemyShipList);
        currentTotalSelectedPieces = new ArrayList();
        numHitShips = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I go first");
            myTurn = true;
            //gridManager.unlockEntireGrid();
            scoreBoardImage.gameObject.SetActive(true);
            opponentsTurnImage.gameObject.SetActive(false);
            itsYourTurnImage.gameObject.SetActive(true);

            //itsYourTurnText.gameObject.SetActive(true);
            //fireButton.gameObject.SetActive(true);
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I go second");
            myTurn = false;
            //gridManager.lockEntireGrid();
            scoreBoardImage.gameObject.SetActive(true);
            opponentsTurnImage.gameObject.SetActive(true);
            itsYourTurnImage.gameObject.SetActive(false);
            //fireButton.gameObject.SetActive(false);
        }

    }

    public void goBackToMainMenuFromGame()
    {
        Debug.Log("Going Back to Main Menu");
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }

    private bool checkIfTileContainsShip(Vector2 tilesPosition)
    {
        if (enemyShipList.Contains(tilesPosition))
        {
            return true;
        }
        else return false;
    }

    public bool countTilesSelected(int requiredAmount)
    {
        turnSelectedPieces = new ArrayList();
        int numTilesSelected = 0;
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2 currentPosition = new Vector2((float)x, (float)y);
                Tile tile = gridManager.getTileFromPosition(currentPosition);
                if (tile.tileMarked)
                {
                    if (!tile.checkedAlready)
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
        if (numTilesSelected == requiredAmount)
        {
            invalidEntryIncorrectNumberOfSpacesAttackStage.gameObject.SetActive(false);
            return true;
        }
        else
        {
            invalidEntryIncorrectNumberOfSpacesAttackStage.gameObject.SetActive(true);
            gridManager.revertTilesCheckedAlready(turnSelectedPieces);
            return false;
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
        if (!(PhotonNetwork.PlayerListOthers[0].CustomProperties == null))
        {
            Debug.Log("custom properties contains score: " + PhotonNetwork.PlayerListOthers[0].CustomProperties["score"].ToString());
            opponentsScoreNumber.text = PhotonNetwork.PlayerListOthers[0].CustomProperties["score"].ToString();
        }
    }

    [PunRPC]
    public void flipTurn(string defaultString)
    {
        Debug.Log("made it to flipping turn");
        if (myTurn == true)
        {
            Debug.Log("transferring turn to next player");
            //remove "it's your turn" message
            //gridManager.lockEntireGrid();
            sendShipHitScoreToOpponent();
            opponentsTurnImage.gameObject.SetActive(true);
            itsYourTurnImage.gameObject.SetActive(false);
            //fireButton.gameObject.SetActive(false);
            myTurn = false;
            return;
        }
        else if (myTurn == false)
        {
            Debug.Log("It's now my turn");
            //display "It's your turn!" message
            //gridManager.unlockEntireGrid();
            recieveShipHitScoreFromOpponent();
            if (int.Parse(opponentsScoreNumber.text) == 17)
            {
                //display "you lost, game over" message.
                gridManager.lockEntireGrid();
                youLostText.gameObject.SetActive(true);
                backToMainMenuButton.gameObject.SetActive(true);
            }
            opponentsTurnImage.gameObject.SetActive(false);
            itsYourTurnImage.gameObject.SetActive(true);
            //fireButton.gameObject.SetActive(true);
            myTurn = true;
            return;

        }
    }

    public void takeUserInputsFireButton()
    {
        bool countTilesResult = countTilesSelected(3);
        if (countTilesResult)
        {
            Debug.Log("Found 3 tiles selected");
            //lock selected pieces and display hit pieces, then begin next players turn
            gridManager.lockSelectedPieces(turnSelectedPieces);
            numHitShips += gridManager.displayHitPieces(turnSelectedPieces, enemyShipList);
            yourScoreNumber.text = numHitShips.ToString();
            //send score to opponent
            Debug.Log("Before Sending Score");
            sendShipHitScoreToOpponent();
            Debug.Log("After Sending Score");
            if (numHitShips == 17) //win
            {
                gridManager.lockEntireGrid();
                youWinImage.gameObject.SetActive(true);
                backToMainMenuButton.gameObject.SetActive(true);
            }
            this.photonView.RPC("flipTurn", RpcTarget.Others, "test");
            Debug.Log("RPC CALLED");

            flipTurn("test");
        }
        else
        {
            Debug.Log("Did Not Find 3 tiles selected");
        }
    }


    [PunRPC]
    public void tellOtherPlayerMasterSetupComplete(string testString)
    {
        //master client and non master client
        //when we click this it should detect which session it is on, master or non master
        //if on master tell non master that mastersetup is complete
        //if on non master tell master that nonmaster is complete (booleans)
        //when both complete, transfer data
        Debug.Log("master setup bool updated to complete");

        masterClientPlayerSetupComplete = true;
        
       
    }

    
    [PunRPC]
    public void tellOtherPlayerNonMasterSetupComplete(string testString)
    {
        Debug.Log("non master setup bool updated to complete");

        nonMasterClientPlayerSetupComplete = true;
    }

   
    public void sendPositionsArrayToOtherPlayers()
    {
        playerShipPositions = new ExitGames.Client.Photon.Hashtable();
        int i = 0;
        string keyID;
        //add every element of shipList to this table
        foreach (Vector2 position in shipPositions)
        {
            keyID = i.ToString();
            playerShipPositions[keyID] = position.ToString();
            i++;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerShipPositions);
        Debug.Log("Custom Properties Sent!");
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ToString());
    }

    public ArrayList extractHashTableToArrayList(ExitGames.Client.Photon.Hashtable tableToExtract)
    {
        ArrayList extractedResults = new ArrayList();
        for(int i = 0; i < 17; i++) //17 is the number of total marks the user will place onto the board
        {
            extractedResults.Add(tableToExtract[i.ToString()]);
        }
        return extractedResults;
    }

    private void printArrayListFromPlayer(ArrayList listToPrint)
    {
        Debug.Log("Arraylist Size: " + listToPrint.Count);
        foreach (string position in listToPrint)
        {
            
            Debug.Log("Position: " + position);
            testRecievedEnemyShipPositions.text += "\nPosition: " + position;
        }

    }
    private bool playerPropsRecieved;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        this.photonView.RPC("recievePlayerData", RpcTarget.Others, "test");
    }
    public void listOtherPlayersProperties()
    {
        Debug.Log("Made it to listing other players");
        Debug.Log("Player Count: " + PhotonNetwork.PlayerList.Length.ToString());
        
        otherPlayersShipList = extractHashTableToArrayList(PhotonNetwork.PlayerListOthers[0].CustomProperties);
        if(otherPlayersShipList.Count == 0 || otherPlayersShipList == null)
        {
            Debug.Log("Other players ship list is empty.");
        }
        else{
            Debug.Log("Other player position ship list:");
            printArrayListFromPlayer(otherPlayersShipList);
            
        }
        
    }
    
    

    public void displayTryAgainMessage()
    {
        invalidEntryIncorrectNumberOfSpaces.gameObject.SetActive(true);
    }

    [PunRPC]
    void sendPlayerData(string testString)
    {
        sendPositionsArrayToOtherPlayers();
    }

    [PunRPC]
    void recievePlayerData(string test)
    {
        listOtherPlayersProperties();
        if (otherPlayersShipList != null && otherPlayersShipList.Count != 0)
        {
            playerPropsRecieved = true;
        }
    }

    [SerializeField]private Image muteAudioMarker;
    public void muteGameAudio()
    {
        AudioSource audioSource = GameObject.Find("gameAudio").GetComponent(typeof(AudioSource)) as AudioSource;
        Debug.Log("mute pressed!");

        if (muteAudioMarker.IsActive())
        {
            Debug.Log("mute turned off!");
            muteAudioMarker.gameObject.SetActive(false);
            audioSource.UnPause();
        }
        else if (!muteAudioMarker.IsActive())
        {
            Debug.Log("mute turned on!");
            muteAudioMarker.gameObject.SetActive(true);
            audioSource.Pause();
        }
    }

    // Update is called once per frame
    void Update()
    {
        tiles = gridManager.getTiles();
        attackingGrid = gridManager.getTiles();

        if(playerPropsRecieved == true)
        {
            if(PhotonNetwork.IsMasterClient == true && loadAttackStageCalledMaster == false)
            {
                loadAttackingStage("default");
            }
            else if(!PhotonNetwork.IsMasterClient && loadAttackStageCalledNonMaster == false)
            {
                loadAttackingStage("default");
            }
        }

        
    }

   
}
