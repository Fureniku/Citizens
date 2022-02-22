using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReference : TileData {
    
    [SerializeField] private GameObject masterTile;

    public void SetMasterTile(GameObject tile) {
        masterTile = tile;
    }

    public GameObject GetMasterTile(GameObject tile) {
        return masterTile;
    }
}
