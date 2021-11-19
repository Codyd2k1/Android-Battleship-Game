using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;


//test naming conventions: test[feature to test] test ID

public class editModeMechanicsTest
{
    private GameManager gameManager;

    private void setupGameManager()
    {
        var gameObject = new GameObject();
        gameManager = gameObject.AddComponent<GameManager>();
        Image gameImage = gameObject.AddComponent<Image>();
        TMPro.TMP_Text tmpText = gameObject.AddComponent<TMP_Text>();
        gameManager.construct(gameImage, gameImage, gameImage, gameImage, gameImage, gameImage,
            gameImage, gameImage, gameImage, gameImage, gameImage, gameImage, gameImage, gameImage, gameImage, tmpText, tmpText, tmpText);
        gameManager.gridManager = gameObject.AddComponent<GridManager>();
        gameManager.gridManager.grid_width = 10;
        gameManager.gridManager.grid_height = 10;
        gameManager.gridManager.tile = Resources.Load<Tile>("TestImages/Tile");
        gameManager.gridManager.GenerateGrid();
        gameManager.numHitShips = 0;
        gameManager.shipPositions = new ArrayList();

    }

    private void selectNumPieces(int numPieces)
    {
        for (int i = 0; i < numPieces; i++)
        {
            Tile selectedTile = gameManager.gridManager.getTileFromPosition(new Vector2(0, (float)i));
            selectedTile.markerPlaceShip = new GameObject();
            selectedTile.markerPlaceShip.gameObject.SetActive(false);
            selectedTile.OnMouseDown();
        }
    }

    [Test]
    public void testInputValidationTooManySelectedSetupId1()
    {
        setupGameManager();
        selectNumPieces(10);
        gameManager.takeUserInputsCarrier();
        Assert.IsTrue(gameManager.invalidEntryIncorrectNumberOfSpaces.gameObject.activeSelf);

    }

    [Test]
    public void testInputValidationTooFewSelectedSetupId1()
    {
        setupGameManager();
        selectNumPieces(2);
        gameManager.takeUserInputsCarrier();
        Assert.IsTrue(gameManager.invalidEntryIncorrectNumberOfSpaces.gameObject.activeSelf);
    }

    [Test]
    public void testInputValidationSpacesNotTouchingSelectedSetupId1()
    {
        setupGameManager();
        selectNumPieces(4);
        Tile randomlySelectedTile = gameManager.gridManager.getTileFromPosition(new Vector2(0, (float)8));
        randomlySelectedTile.OnMouseDown();
        gameManager.takeUserInputsCarrier();
        Assert.IsTrue(gameManager.invalidEntrySpacesNotTouching.gameObject.activeSelf);
    }


    [Test]
    public void testPlayerCanPlaceShipsId2()
    {
        var gameObject = new GameObject();

        var tile = gameObject.AddComponent<Tile>();
        tile.markerPlaceShip = new GameObject();
        tile.markerPlaceShip.gameObject.SetActive(false);
        tile.OnMouseDown();

        Assert.IsTrue(tile.tileMarked);
    }

    private ArrayList getSpaceSelectionList(int numShipsWanted)
    {
        ArrayList spacesSelected = new ArrayList();
        for (int i = 1; i <= numShipsWanted; i++)
        {
            Vector2 currentPosition = new Vector2(0, (float)i);
            Tile selectedTile = gameManager.gridManager.getTileFromPosition(currentPosition);
            selectedTile.markerPlaceShip = new GameObject();
            selectedTile.markerPlaceShip.gameObject.SetActive(false);
            selectedTile.OnMouseDown();
            spacesSelected.Add(currentPosition);

        }
        return spacesSelected;
    }

    private ArrayList getSpaceSelectionList(int numShipsWanted, int valueToStartOn)
    {
        ArrayList spacesSelected = new ArrayList();
        for (int i = 0; i <= numShipsWanted; i++)
        {
            Vector2 currentPosition = new Vector2(0, (float)valueToStartOn + i);
            Tile selectedTile = gameManager.gridManager.getTileFromPosition(currentPosition);
            selectedTile.markerPlaceShip = new GameObject();
            selectedTile.markerPlaceShip.gameObject.SetActive(false);
            selectedTile.OnMouseDown();
            spacesSelected.Add(currentPosition);

        }
        return spacesSelected;
    }

    [Test]
    public void testPlayerCanAttackShipsId3()
    {
        setupGameManager();
        ArrayList enemyShipList = getSpaceSelectionList(5);
        
        gameManager.takeUserInputsCarrier();

        gameManager.gridManager.enableAttackMode();

        ArrayList turnSelectedPieces = getSpaceSelectionList(3);
        gameManager.countTilesSelected(3);
        gameManager.numHitShips += gameManager.gridManager.displayHitPieces(turnSelectedPieces, enemyShipList);
        Assert.AreEqual(3, gameManager.numHitShips);

    }

    [Test]
    public void testPlayerCantExceedMaximumShipsId4()
    {
        setupGameManager();
        ArrayList shipSelectionsList = getSpaceSelectionList(9);
        ArrayList shipSelectionsListTwo = getSpaceSelectionList(9);
        ArrayList shipSelectionsListThree = getSpaceSelectionList(9);
        gameManager.takeUserInputsCarrier();
        Assert.IsTrue(gameManager.invalidEntryIncorrectNumberOfSpaces.gameObject.activeSelf);
    }

    [Test]
    public void testPlayerCantPlaceShipsOnOtherShipsId5()
    {
        setupGameManager();
        ArrayList selectedSpaces = getSpaceSelectionList(5);
        gameManager.takeUserInputsCarrier();

        foreach (Vector2 position in selectedSpaces)
        {
            Assert.IsFalse(gameManager.gridManager.getTileFromPosition(position).boxCollider2D.enabled);
        }
        
    }

    [Test]
    public void testSystemUpdatesShipLocationsId8()
    {
        setupGameManager();
        ArrayList selectedSpaces = getSpaceSelectionList(5);
        gameManager.takeUserInputsCarrier();

        Assert.AreEqual(gameManager.shipPositions, selectedSpaces);
    }

    [Test]
    public void testSystemUpdatesAtttackLocationsId9()
    {
        setupGameManager();
        ArrayList enemyShipList = getSpaceSelectionList(5);

        gameManager.takeUserInputsCarrier();

        gameManager.gridManager.enableAttackMode();

        ArrayList turnSelectedPieces = getSpaceSelectionList(3);
        gameManager.countTilesSelected(3);

        int numAttacksLocations = 0;
        foreach (System.Collections.Generic.KeyValuePair<UnityEngine.Vector2, Tile> tilePair in gameManager.gridManager.tiles)
        {
            if(tilePair.Value.markerAttackShip.gameObject.activeSelf)
            { numAttacksLocations++; }
        }
        Assert.AreEqual(3, numAttacksLocations);
    }

    [Test]
    public void testMissedShotMarkedAsMissId10()
    {
        setupGameManager();
        ArrayList enemyShipList = getSpaceSelectionList(5);

        gameManager.takeUserInputsCarrier();

        gameManager.gridManager.enableAttackMode();

        ArrayList turnSelectedPieces = getSpaceSelectionList(3, 6);
        gameManager.countTilesSelected(3);
        gameManager.numHitShips += gameManager.gridManager.displayHitPieces(turnSelectedPieces, enemyShipList);
        Assert.AreEqual(0, gameManager.numHitShips);

    }

    [Test]
    public void testHitShotMarkedAsHitId11()
    {
        setupGameManager();
        ArrayList enemyShipList = getSpaceSelectionList(5);

        gameManager.takeUserInputsCarrier();

        gameManager.gridManager.enableAttackMode();

        ArrayList turnSelectedPieces = getSpaceSelectionList(3, 5);
        gameManager.countTilesSelected(3);
        gameManager.numHitShips += gameManager.gridManager.displayHitPieces(turnSelectedPieces, enemyShipList);
        Assert.AreEqual(1, gameManager.numHitShips);

    }

    [Test]
    public void testShipFullyHitMarkedAsDestroyedId13()
    {
        setupGameManager();
        ArrayList enemyShipList = getSpaceSelectionList(5);

        gameManager.takeUserInputsCarrier();

        gameManager.gridManager.GenerateGrid();
        gameManager.gridManager.enableAttackMode();

        ArrayList firstTurnSelectedPieces = getSpaceSelectionList(3);
        gameManager.countTilesSelected(3);
        gameManager.numHitShips += gameManager.gridManager.displayHitPieces(firstTurnSelectedPieces, enemyShipList);
        ArrayList secondTurnSelectedPieces = getSpaceSelectionList(3, 4);
        gameManager.numHitShips += gameManager.gridManager.displayHitPieces(secondTurnSelectedPieces, enemyShipList);

        foreach (Vector2 shipPosition in enemyShipList)
        {
            Assert.IsTrue(gameManager.gridManager.getTileFromPosition(shipPosition).markerShipHit.gameObject.activeSelf);
        }
    }


    [Test]
    public void testScoreUpdatedWhenShipMarkedAsDestroyedId14()
    {
        setupGameManager();
        int originalScore = gameManager.numHitShips;
        ArrayList enemyShipList = getSpaceSelectionList(5);

        gameManager.takeUserInputsCarrier();

        gameManager.gridManager.GenerateGrid();
        gameManager.gridManager.enableAttackMode();

        ArrayList turnSelectedPieces = getSpaceSelectionList(3);
        gameManager.countTilesSelected(3);
        gameManager.numHitShips += gameManager.gridManager.displayHitPieces(turnSelectedPieces, enemyShipList);

        Assert.That(originalScore < gameManager.numHitShips);
    }

    [Test]
    public void testUserTouchConvertedToPositionId19()
    {
        setupGameManager();
        Tile tileUserTouched = gameManager.gridManager.getTileFromPosition(new Vector2(5, 5));
        tileUserTouched.OnMouseDown();
        Assert.IsNotNull(tileUserTouched.transform.position);
    }

}
