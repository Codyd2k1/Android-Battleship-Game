using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int grid_width, grid_height;

    public Tile tile;

    public Dictionary<Vector2, Tile> tiles;

    /*void Start(){
        GenerateGrid();
    }*/
    

    public void GenerateGrid()
    {   
        tiles = new Dictionary<Vector2, Tile>();
        for(int x = 0; x < grid_width; x++)
        {  
            for(int y = 0; y < grid_height; y++)
            {
                var tileSpawned = Instantiate(tile, new Vector3(x,y), Quaternion.identity);
                tileSpawned.name = $"Tile {x} {y}";
                var isOdd = (x % 2 == 0 && y % 2 != 0) || (y % 2 == 0 && x % 2 != 0);
                tileSpawned.Init(isOdd);
                tiles[new Vector2(x,y)] = tileSpawned;
            }
        }
    }


    public Tile getTileFromPosition(Vector2 position)
    {
        if(tiles.TryGetValue(position, out var tile))
        {
            return tile;
        }
        else
        {
            return null;
        }
    }

    public Dictionary<Vector2, Tile> getTiles(){
        return tiles;
    }

    
    public void lockEntireGrid()
    {
        foreach (System.Collections.Generic.KeyValuePair<UnityEngine.Vector2, Tile> tilePair in tiles)
        {
            tilePair.Value.disableBoxCollider();
        }
    }

    public void unlockEntireGrid()
    {
        foreach (System.Collections.Generic.KeyValuePair<UnityEngine.Vector2, Tile> tilePair in tiles)
        {
            tilePair.Value.enableBoxCollider();
        }
    }

    public void enableAttackMode()
    {
        foreach (System.Collections.Generic.KeyValuePair<UnityEngine.Vector2, Tile> tilePair in tiles)
        {
            tilePair.Value.enableAttackMode = true;
        }
    }

    public void revertTilesCheckedAlready(ArrayList tilesCounted)
    {
        foreach (Vector2 position in tilesCounted)
        {
            Tile tile = getTileFromPosition(position);
            tile.checkedAlready = false;
            tile.OnMouseDown();
        }
    }

    public void lockSelectedPieces(ArrayList turnSelectedPieces)
    {
        foreach (Vector2 piecesPosition in turnSelectedPieces)
        {
            Tile currentTile = getTileFromPosition(piecesPosition);
            currentTile.disableBoxCollider();
        }
    }
    public int displayHitPieces(ArrayList turnSelectedPieces, ArrayList enemyShipList)
    {
        if(enemyShipList == null)
        {
            Debug.Log("Enemy Ship list is empty?");
            return 0;
        }
        int numHitShips = 0;
        foreach (Vector2 piecesPosition in turnSelectedPieces)
        {
            if(enemyShipList.Contains(piecesPosition))
            {
                Tile currentTile = getTileFromPosition(piecesPosition);
                currentTile.displayShipHitMarker();
                numHitShips++;
            }
        }
        return numHitShips;
    }
    /*
    public void unlockEntireGrid()
    {
        foreach (System.Collections.Generic.KeyValuePair<UnityEngine.Vector2, Tile> tilePair in tiles)
        {
            tilePair.Value.enableBoxCollider();
        }
    }
    */

    public void hideOldGrid()
    {
        Debug.Log("hiding old grid");
        foreach (System.Collections.Generic.KeyValuePair<UnityEngine.Vector2, Tile> tilePair in tiles)
        {
            tilePair.Value.gameObject.SetActive(false);
        }
    }
}
