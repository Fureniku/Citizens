using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRegistry : MonoBehaviour {

    private static GameObject[] registry = new GameObject[255];
    
    public static readonly Tile GRASS = new Tile(1);
    public static readonly Tile REFERENCE = new Tile(2);
    public static readonly Tile STRAIGHT_ROAD_1x1 = new Tile(4);
    public static readonly Tile CORNER_ROAD_1x1 = new Tile(5);
    public static readonly Tile T_JUNCT_ROAD_1x1 = new Tile(6);
    public static readonly Tile CROSSROAD_ROAD_1x1 = new Tile(7);
    public static readonly Tile CROSSROAD_CTRL_ROAD_1x1 = new Tile(8);

    public static int maxId = 999;

    [SerializeField] private GameObject[] register = null;
    
    private static TileRegistry _instance = null;
    public static TileRegistry Instance {
        get { return _instance; }
    }

    public static GameObject GetGrass() { return registry[GRASS.GetId()]; }

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

public class Tile {
    private int id = -1;

    public Tile(int id) {
        this.id = id;
    }

    public int GetId() {
        return id;
    }
}
