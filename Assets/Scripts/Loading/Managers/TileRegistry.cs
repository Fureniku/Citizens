﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

public class TileRegistry : GenerationSystem {
    
    //To add a new tile:
    // - create tile: public static readonly Tile NAME (id, local name, tiletype)
    // - initialize registry: tileRegistry.Add(TILE)
    // - Get the tile enum (GetTile(EnumTile tile))
    // - Add to EnumTile
    // - Set EnumTile on the prefab
    
    // - If road, set info in TileRoad too.

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
    public static readonly Tile T_JUNCT_ROAD_1x1_SINGLE_IN = new Tile(8, "T Junction Single In", TileType.ROAD);
    public static readonly Tile T_JUNCT_ROAD_1x1_SINGLE_OUT = new Tile(9, "T Junction Single Out", TileType.ROAD);
    public static readonly Tile CAR_PARK_ENTRANCE = new Tile(10, "Car Park Entrance", TileType.BUILDING_PART);
    public static readonly Tile CAR_PARK_RAMP_BASE = new Tile(11, "Car Park Ramp", TileType.BUILDING_PART);
    public static readonly Tile CAR_PARK_EDGE_BASE = new Tile(12, "Car Park Edge", TileType.BUILDING_PART);
    public static readonly Tile CAR_PARK_CORNER_BASE = new Tile(13, "Car Park Corner", TileType.BUILDING_PART);
    public static readonly Tile CAR_PARK_INNER_BASE = new Tile(14, "Car Park Inner", TileType.BUILDING_PART);
    public static readonly Tile CAR_PARK_EXIT = new Tile(15, "Car Park Exit", TileType.BUILDING_PART);
    public static readonly Tile ROAD_WORLD_EXIT = new Tile(16, "Road World Exit", TileType.ROAD);
    public static readonly Tile ZEBRA_CROSSING_1x1 = new Tile(17, "Zebra Crossing", TileType.ROAD);
    public static readonly Tile PELICAN_CROSSING_1x1 = new Tile(18, "Pelican Crossing", TileType.ROAD);
    public static readonly Tile CROSSING_APPROACH_1x1 = new Tile(19, "Crossing Approach", TileType.ROAD);
    public static readonly Tile OFFICE_1_CORNER_RECESSED_L = new Tile(20, "Office 1 Corner Recessed L", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_CORNER_RECESSED_R = new Tile(21, "Office 1 Corner Recessed R", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_CORNER_RECESSED = new Tile(22, "Office 1 Corner Recessed", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE_RECESSED = new Tile(23, "Office 1 Edge Recessed" , TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE_RECESSED_ENTRANCE = new Tile(24, "Office 1 Edge Recessed Entrance", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE_RECESSED_GARAGE = new Tile(25, "Office 1 Edge Recessed Garage", TileType.BUILDING_PART);
    public static readonly Tile SKYSCRAPER_GENERIC_1 = new Tile(26, "Generic Skyscraper 1", TileType.BUILDING);

    public static int maxId = 999;

    [SerializeField] private GameObject[] register = null;

    public static GameObject GetGrass() { return registry[GRASS.GetId()]; }

    public override void Initialize() {
        InitializeRegistry();
        StartCoroutine(Register());
    }
    
    public override void Process() {}

    IEnumerator Register() {
        for (int i = 0; i < register.Length; i++) {
            if (register[i].GetComponent<TileData>() != null) {
                GameObject reg = Instantiate(register[i], transform, true);
                reg.transform.position = new Vector3(0, -100, 0);
                reg.SetActive(false);
                Register(reg); //Instantiate to create a copy, instead of using original. Preserves original prefab.
                yield return null;
            }
        }
        Debug.Log("Registration complete");
        SetComplete();
        yield return null;
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
        tileRegistry.Add(T_JUNCT_ROAD_1x1_SINGLE_IN);
        tileRegistry.Add(T_JUNCT_ROAD_1x1_SINGLE_OUT);
        tileRegistry.Add(CAR_PARK_ENTRANCE);
        tileRegistry.Add(CAR_PARK_RAMP_BASE);
        tileRegistry.Add(CAR_PARK_EDGE_BASE);
        tileRegistry.Add(CAR_PARK_CORNER_BASE);
        tileRegistry.Add(CAR_PARK_INNER_BASE);
        tileRegistry.Add(CAR_PARK_EXIT);
        tileRegistry.Add(ROAD_WORLD_EXIT);
        tileRegistry.Add(ZEBRA_CROSSING_1x1);
        tileRegistry.Add(PELICAN_CROSSING_1x1);
        tileRegistry.Add(CROSSING_APPROACH_1x1);
        tileRegistry.Add(OFFICE_1_CORNER_RECESSED_L);
        tileRegistry.Add(OFFICE_1_CORNER_RECESSED_R);
        tileRegistry.Add(OFFICE_1_CORNER_RECESSED);
        tileRegistry.Add(OFFICE_1_EDGE_RECESSED);
        tileRegistry.Add(OFFICE_1_EDGE_RECESSED_ENTRANCE);
        tileRegistry.Add(OFFICE_1_EDGE_RECESSED_GARAGE);
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
        go.SetActive(true);
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
            case EnumTile.T_JUNCT_ROAD_1x1_SINGLE_IN:
                return T_JUNCT_ROAD_1x1_SINGLE_IN;
            case EnumTile.T_JUNCT_ROAD_1x1_SINGLE_OUT:
                return T_JUNCT_ROAD_1x1_SINGLE_OUT;
            case EnumTile.CAR_PARK_ENTRANCE:
                return CAR_PARK_ENTRANCE;
            case EnumTile.CAR_PARK_RAMP_BASE:
                return CAR_PARK_RAMP_BASE;
            case EnumTile.CAR_PARK_EDGE_BASE:
                return CAR_PARK_EDGE_BASE;
            case EnumTile.CAR_PARK_CORNER_BASE:
                return CAR_PARK_CORNER_BASE;
            case EnumTile.CAR_PARK_INNER_BASE:
                return CAR_PARK_INNER_BASE;
            case EnumTile.CAR_PARK_EXIT:
                return CAR_PARK_EXIT;
            case EnumTile.ROAD_WORLD_EXIT:
                return ROAD_WORLD_EXIT;
            case EnumTile.ZEBRA_CROSSING_1x1:
                return ZEBRA_CROSSING_1x1;
            case EnumTile.PELICAN_CROSSING_1x1:
                return PELICAN_CROSSING_1x1;
            case EnumTile.CROSSING_APPROACH_1x1:
                return CROSSING_APPROACH_1x1;
            case EnumTile.OFFICE_1_CORNER_RECESSED_L:
                return OFFICE_1_CORNER_RECESSED_L;
            case EnumTile.OFFICE_1_CORNER_RECESSED_R:
                return OFFICE_1_CORNER_RECESSED_R;
            case EnumTile.OFFICE_1_CORNER_RECESSED:
                return OFFICE_1_CORNER_RECESSED;
            case EnumTile.OFFICE_1_EDGE_RECESSED:
                return OFFICE_1_EDGE_RECESSED;
            case EnumTile.OFFICE_1_EDGE_RECESSED_ENTRANCE:
                return OFFICE_1_EDGE_RECESSED_ENTRANCE;
            case EnumTile.OFFICE_1_EDGE_RECESSED_GARAGE:
                return OFFICE_1_EDGE_RECESSED_GARAGE;
            case EnumTile.SKYSCRAPER_GENERIC_1:
                return SKYSCRAPER_GENERIC_1;
            default:
                Debug.Log("Tile " + tile + " missing from GetTile function");
                return GRASS;
        }
    }

    //TODO
    public override int GetGenerationPercentage() {
        return 0;
    }

    public override string GetGenerationString() {
        return "";
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
    T_JUNCT_ROAD_1x1_SINGLE_IN,
    T_JUNCT_ROAD_1x1_SINGLE_OUT,
    CAR_PARK_ENTRANCE,
    CAR_PARK_RAMP_BASE,
    CAR_PARK_EDGE_BASE,
    CAR_PARK_CORNER_BASE,
    CAR_PARK_INNER_BASE,
    CAR_PARK_EXIT,
    ROAD_WORLD_EXIT,
    ZEBRA_CROSSING_1x1,
    PELICAN_CROSSING_1x1,
    CROSSING_APPROACH_1x1,
    OFFICE_1_CORNER_RECESSED_L,
    OFFICE_1_CORNER_RECESSED_R,
    OFFICE_1_CORNER_RECESSED,
    OFFICE_1_EDGE_RECESSED,
    OFFICE_1_EDGE_RECESSED_ENTRANCE,
    OFFICE_1_EDGE_RECESSED_GARAGE,
    SKYSCRAPER_GENERIC_1
}

public enum TileType {
    AIR,
    GRASS,
    REFERENCE,
    ROAD,
    BUILDING,
    BUILDING_PART
}
