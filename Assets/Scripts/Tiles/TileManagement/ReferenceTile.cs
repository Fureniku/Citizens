using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceTile : MonoBehaviour {
    
    [SerializeField] private GameObject masterTile;

    public void SetTile(GameObject tile) {
        masterTile = tile;
    }

    public GameObject GetTile(GameObject tile) {
        return masterTile;
    }
}
