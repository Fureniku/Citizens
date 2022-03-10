using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRegistry : MonoBehaviour {
    
    //To add a new tile:
    // - create tile: public static readonly Tile NAME (id, local name, tiletype)
    // - initialize registry: tileRegistry[TILE.getId()] = TILE
    // - Get the tile enum (GetTile(EnumTile tile))
    // - Add to EnumTile
    // - Set EnumTile on the prefab
    // - Increment registryCount

    private static readonly GameObject[] registry = new GameObject[255];
    private static readonly List<Tile> tileRegistry = new List<Tile>();
    
    public static readonly Tile AIR = new Tile(0, "Air", TileType.AIR);
    public static readonly Tile GRASS = new Tile(1, "Grass", TileType.GRASS);
    public static readonly Tile REFERENCE = new Tile(2, "Reference", TileType.REFERENCE);
    public static readonly Tile STRAIGHT_ROAD_1x1 = new Tile(3, "Straight Road", TileType.ROAD);
    public static readonly Tile CORNER_ROAD_1x1 = new Tile(4, "Corner Road", TileType.ROAD);
    public static readonly Tile T_JUNCT_ROAD_1x1 = new Tile(5, "T Junction Road", TileType.ROAD);
    public static readonly Tile CROSSROAD_ROAD_1x1 = new Tile(6, "Crossroad", TileType.ROAD);
    public static readonly Tile CROSSROAD_CTRL_ROAD_1x1 = new Tile(7, "Controlled Crossroad", TileType.ROAD);
    public static readonly Tile SKYSCRAPER_GENERIC_1 = new Tile(8, "Generic Skyscraper 1", TileType.BUILDING);

    public static int maxId = 999;

    [SerializeField] private GameObject[] register = null;

    public static GameObject GetGrass() { return registry[GRASS.GetId()]; }

    void Start() {
        InitializeRegistry();
        
        for (int i = 0; i < register.Length; i++) {
            if (register[i].GetComponent<TileData>() != null) {
                GameObject reg = Instantiate(register[i]);
                reg.transform.parent = transform;
                reg.transform.position = new Vector3(0, -100, 0);
                Register(reg); //Instantiate to create a copy, instead of using original. Preserves original prefab.
            }
        }
        
        Debug.Log("Registration complete");

        if (World.Instance.GetWorldState() == EnumWorldState.UNSTARTED) {
            World.Instance.AdvanceWorldState();
        }
    }

    void InitializeRegistry() {
        tileRegistry.Add(AIR);
        tileRegistry.Add(GRASS);
        tileRegistry.Add(REFERENCE);
        tileRegistry.Add(STRAIGHT_ROAD_1x1);
        tileRegistry.Add(CORNER_ROAD_1x1);
        tileRegistry.Add(T_JUNCT_ROAD_1x1);
        tileRegistry.Add(CROSSROAD_ROAD_1x1);
        tileRegistry.Add(CROSSROAD_CTRL_ROAD_1x1);
        tileRegistry.Add(SKYSCRAPER_GENERIC_1);
    }

    public static int GetSize() {
        return tileRegistry.Count;
    }

    public static Tile GetTile(int id) {
        if (tileRegistry[id] == null) {
            Debug.Log("Tile at " + id + " is null!");
        }
        return tileRegistry[id];
    }

    public static void Register(GameObject go) {
        TileData tile = go.GetComponent<TileData>();
        tile.Initialize();
        tile.HideAfterRegistration();
        
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

    public static int GetIDFromGameObject(GameObject go) {
        return go.GetComponent<TileData>().GetId();
    }

    public static GameObject Instantiate(int id) {
        GameObject go = Instantiate(registry[id]);
        go.GetComponent<TileData>().Create();
        return go;
    }

    public static Tile GetTile(EnumTile tile) {
        switch (tile) {
            case EnumTile.AIR:
                return AIR;
            case EnumTile.GRASS:
                return GRASS;
            case EnumTile.REFERENCE:
                return REFERENCE;
            case EnumTile.STRAIGHT_ROAD_1x1:
                return STRAIGHT_ROAD_1x1;
            case EnumTile.CORNER_ROAD_1x1:
                return CORNER_ROAD_1x1;
            case EnumTile.T_JUNCT_ROAD_1x1:
                return T_JUNCT_ROAD_1x1;
            case EnumTile.CROSSROAD_ROAD_1x1:
                return CROSSROAD_ROAD_1x1;
            case EnumTile.CROSSROAD_CTRL_ROAD_1x1:
                return CROSSROAD_CTRL_ROAD_1x1;
            case EnumTile.SKYSCRAPER_GENERIC_1:
                return SKYSCRAPER_GENERIC_1;
            default:
                Debug.Log("Tile " + tile + " missing from GetTile function");
                return GRASS;
        }
    }
}

public class Tile {
    private readonly int id;
    private readonly string name;
    private readonly TileType tileType;

    public Tile(int id, string name, TileType tileType) {
        this.id = id;
        this.name = name;
        this.tileType = tileType;
    }

    public int GetId() {
        return id;
    }

    public string GetName() {
        return name;
    }

    public TileType GetTileType() {
        return tileType;
    }
}

public enum EnumTile {
    AIR,
    GRASS,
    REFERENCE,
    STRAIGHT_ROAD_1x1,
    CORNER_ROAD_1x1,
    T_JUNCT_ROAD_1x1,
    CROSSROAD_ROAD_1x1,
    CROSSROAD_CTRL_ROAD_1x1,
    SKYSCRAPER_GENERIC_1
}

public enum TileType {
    AIR,
    GRASS,
    REFERENCE,
    ROAD,
    BUILDING
}
