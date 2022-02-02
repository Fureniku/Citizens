using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour {
    
    private static GameObject[,] tileMap;
    private static int size;
    private static int softBorder = 0;

    public TileGrid(int size) {
        TileGrid.size = 100;
        tileMap = new GameObject[size, size];
    }
    
    //Offset the map coordinates for the array which can't accept negative numbers
    public static GameObject GetTile(TilePos pos) {
        if (IsValidLocation(pos)) {
            return tileMap[pos.x + size / 2, pos.z + size / 2];
        }
        return null;
    }

    public static void CheckMapValidity() {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                TilePos pos = TilePos.MapOffset(i, j);
                GameObject tile = GetTile(pos);
                if (tile != null) {
                    Debug.Log("Tile " + pos.ToString() + " not null.");
                }
                else {
                    Debug.Log("Tile " + pos.ToString() + " is null! This is bad!");
                }
            }
        }
    }

    //Offset the map coordinates for the array which can't accept negative numbers
    public static bool SetTile(TilePos pos, GameObject id) {
        Debug.Log("setting tile " + pos.ToString());
        if (IsValidLocation(pos)) {
            tileMap[pos.x + size / 2, pos.z + size / 2] = id;
            return true;
        }
        else {
            Debug.Log("Location was NOT valid!");
        }

        return false;
    }
    
    public static bool IsValidLocation(TilePos pos) {
        int i = size / 2;
        if (pos.x + i > size-1-softBorder || pos.x + i < softBorder || pos.z + i > size-1-softBorder || pos.z + i < softBorder) {
            Debug.Log("Invalid location! size/2 is " + i + ", size is " + size + ", softborder: " + softBorder);
            if (pos.x + i > size - 1 - softBorder) {
                Debug.Log("Tile " + pos.ToString() + " failed  X. " + (pos.x + i) + " > " + (size - 1 - softBorder));
            }
            if (pos.x + i < softBorder) {
                Debug.Log("Tile " + pos.ToString() + " failed  X. " + (pos.x + i) + " < " + softBorder);
            }
            if (pos.z + i > size - 1 - softBorder) {
                Debug.Log("Tile " + pos.ToString() + " failed  Z. " + (pos.z + i) + " > " + (size - 1 - softBorder));
            }
            if (pos.z + i < softBorder) {
                Debug.Log("Tile " + pos.ToString() + " failed  Z. " + (pos.z + i) + " < " + softBorder);
            }

            return false;
        }
        return true;
    }
    
    public static void ClearSpace(Vector3 pos) {
        Collider[] colliders = Physics.OverlapSphere(pos, 2.0f);
        for (int i = 0; i < colliders.Length; i++) {
            Destroy(colliders[i].gameObject);
        }
    }

    public static void GetTileViaColliders(Vector3 pos) {
        Collider[] colliders = Physics.OverlapSphere(pos, 2.0f);
        for (int i = 0; i < colliders.Length; i++) {
            Debug.Log("Tile " + i + " of " + colliders.Length);
            Destroy(colliders[i].gameObject);
        }
    }

    public static GameObject GetGameObjectAtPos(Vector3 pos) {
        return GetTile(new TilePos(pos));
    }

    public static TileData GetTileAtPosition(TilePos pos) {
        GameObject tileObj = GetTile(pos);
        TileData tile = null;
        if (tileObj == null) {
            //If we're trying to get the tile at a position but there isn't one, return a default non-existant tile
            tile = TileData.tileAir;
        }
        else {
            tile = tileObj.GetComponent<TileData>();
        }
        return tile;
    }

    public static int GetMapSize() {
        return size;
    }
}
