using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRegistry : MonoBehaviour {

    private static GameObject[] registry = new GameObject[255];

    [SerializeField] private GameObject[] register = null;
    
    private static TileRegistry _instance = null;
    public static TileRegistry Instance {
        get { return _instance; }
    }

    public static GameObject GetGrass() { return registry[1]; }
    
    //Grass                 1
    //1x1 Reference Tile    2
    //1x1 Striaght Road     3
    //1x1 Corner Road       4
    //1x1 T-junction        5
    //1x1 Crossroad         6
    //1x1 Crossroad Ctrld   7
    

    void Start() {
        for (int i = 0; i < register.Length; i++) {
            if (register[i].GetComponent<TileData>() != null) {
                Register(register[i]);
            }
        }
        Debug.Log("Registration complete");
    }

    public static void Register(GameObject go) {
        TileData tile = go.GetComponent<TileData>();
        
        Debug.Log("Registering tile [" + tile.GetId() + "] (" + tile.GetName() + ")");

        if (registry[tile.GetId()] != null) {
            Debug.Log("Overwriting existing tile with ID " + tile.GetId() + "! Replacing " + GetTileFromID(tile.GetId()).GetName() + " with " + tile.GetName());
        }

        registry[tile.GetId()] = go;
    }

    public static TileData GetTileFromID(int id) {
        return registry[id].GetComponent<TileData>();
    }

    public static GameObject GetGameObjectFromID(int id) {
        return registry[id];
    }
}
