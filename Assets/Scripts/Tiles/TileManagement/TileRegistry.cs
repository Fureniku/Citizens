using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRegistry : MonoBehaviour {

    private static TileData[] registry = new TileData[255];

    public static void Register(TileData tile) {
        Debug.Log("Registering tile [" + tile.GetId() + "] (" + tile.GetName() + ")");
        registry[tile.GetId()] = tile;
    }

    public static TileData GetTileFromID(int id) {
        return registry[id];
    }
}
