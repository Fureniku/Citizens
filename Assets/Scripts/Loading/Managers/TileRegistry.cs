using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

public class TileRegistry : GenerationSystem {
    
    //To add a new tile:
    // - create tile: public static readonly Tile NAME (id, local name, tiletype)
    // - Add to EnumTile
    // - Set EnumTile on the prefab

    private static readonly GameObject[] registry = new GameObject[255];
    public static readonly Tile[] tileRegistry = new Tile[255];

    #region TILES
    //Roads
    public static readonly Tile AIR = new Tile(0, "Air", TileType.AIR);
    public static readonly Tile GRASS = new Tile(1, "Grass", TileType.GRASS);
    public static readonly Tile REFERENCE = new Tile(2, "Reference", TileType.REFERENCE);
    public static readonly Tile STRAIGHT_ROAD = new Tile(3, "Straight Road", TileType.ROAD);
    public static readonly Tile CORNER_ROAD = new Tile(4, "Corner Road", TileType.ROAD);
    public static readonly Tile T_JUNCT_ROAD = new Tile(5, "T Junction", TileType.ROAD);
    public static readonly Tile T_JUNCT_ROAD_CONTROLLED = new Tile(6, "Controlled T Junction", TileType.ROAD);
    public static readonly Tile CROSSROAD_ROAD = new Tile(7, "Crossroad", TileType.ROAD);
    public static readonly Tile CROSSROAD_ROAD_CONTROLLED = new Tile(8, "Controlled Crossroad", TileType.ROAD);
    public static readonly Tile T_JUNCT_ROAD_SINGLE_IN = new Tile(9, "T Junction Single In", TileType.ROAD);
    public static readonly Tile T_JUNCT_ROAD_SINGLE_OUT = new Tile(10, "T Junction Single Out", TileType.ROAD);
    public static readonly Tile ZEBRA_CROSSING = new Tile(11, "Zebra Crossing", TileType.ROAD);
    public static readonly Tile PELICAN_CROSSING = new Tile(12, "Pelican Crossing", TileType.ROAD);
    public static readonly Tile ROAD_WORLD_EXIT = new Tile(13, "Road World Exit", TileType.ROAD);

    //Large buildings (non-generated)
    public static readonly Tile HOSPITAL_8x8 = new Tile(20, "Large Hospital", TileType.BUILDING_MAJOR);
    public static readonly Tile TOWN_HALL_8x8 = new Tile(21, "Town Hall", TileType.ROAD);
    public static readonly Tile UNIVERSITY_8x8 = new Tile(22, "University", TileType.ROAD);

    //Houses
    public static readonly Tile TOWN_HOUSE_1 = new Tile(40, "Town House 1", TileType.BUILDING_HOUSE);
    public static readonly Tile TOWN_HOUSE_2 = new Tile(41, "Town House 2", TileType.BUILDING_HOUSE);
    public static readonly Tile TOWN_HOUSE_3 = new Tile(42, "Town House 3", TileType.BUILDING_HOUSE);
    public static readonly Tile TOWN_HOUSE_4 = new Tile(43, "Town House 4", TileType.BUILDING_HOUSE);

    //Shops
   
    public static readonly Tile ZORO = new Tile(61, "Zoro", TileType.BUILDING_SHOP);
    public static readonly Tile PRIDAVE = new Tile(62, "Pridave", TileType.BUILDING_SHOP);
    public static readonly Tile PREVIOUS = new Tile(63, "Previous", TileType.BUILDING_SHOP);
    public static readonly Tile RURAL_COSTUMERS = new Tile(64, "Rural Costumers", TileType.BUILDING_SHOP);
    public static readonly Tile G_AND_L = new Tile(65, "G&L", TileType.BUILDING_SHOP);
    public static readonly Tile VERYWET = new Tile(66, "Verywet", TileType.BUILDING_SHOP);
    public static readonly Tile LAKE_CONTINENT = new Tile(67, "Lake Continent", TileType.BUILDING_SHOP);
    public static readonly Tile OLD_SOUND = new Tile(68, "Old Sound", TileType.BUILDING_SHOP);

    public static readonly Tile SHOP_BESTCO = new Tile(69, "Best Co", TileType.BUILDING_SHOP);
    //bainsurys
    //morrisons
    public static readonly Tile SHOP_812 = new Tile(72, "8-Twelve", TileType.BUILDING_SHOP);
    //reserved
    //reserved
    //danpora
    //I. Frederick
    //silverforge
    //illegal world
    //nerd withdraw
    //firerocks
    //spare
    public static readonly Tile SHOP_WCDONALDS = new Tile(82, "WcDonald's", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_BURGERQUEEN = new Tile(83, "Burger Queen", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_SFC = new Tile(84, "SFC", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_PIZZA_HOUSE = new Tile(85, "Pizza House", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_TGITUESDAYS = new Tile(86, "TGI Tuesdays", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_WAGAPAPA = new Tile(87, "wagapapa", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_GRANDPADOS = new Tile(88, "Grandpado's", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_TENGIRLS = new Tile(89, "Ten Girls Burgers & Fries", TileType.BUILDING_SHOP);
    public static readonly Tile SHOP_DOMPATH = new Tile(90, "Dompath", TileType.BUILDING_SHOP);
    
    public static readonly Tile COFFEE_SHOP_1 = new Tile(91, "300 Degrees Coffee", TileType.BUILDING_SHOP);
    //barstucks
    //costly coffee
    //caffe hero
    //reserved
    
    //homeware 1
    //homeware 2
    //homeware 3 98
    
    //vg bloggs
    //newsagents 2
    //newsagents 3
    //newsagents 4
    //newsagents 5
    //newsagents 6 104
    
    //pear
    //stevespoke
    //four
    //h2o
    //NN
    //bhajis
    //computer planet
    //
    //
    // 114
    
    //music 1
    //music 2
    //music 3
    //music 4
    
    //furniture 1
    //furniture 2
    //furniture 3
    //furniture 4
    //furniture 5
    
    //gloves
    //hypermeds
    //chemist 3 126
    
    
    
    //Multi-part large buildings
    public static readonly Tile MULTI_CAR_PARK_ENTRANCE = new Tile(150, "Car Park Entrance", TileType.BUILDING_PART);
    public static readonly Tile MULTI_CAR_PARK_RAMP = new Tile(151, "Car Park Ramp", TileType.BUILDING_PART);
    public static readonly Tile MULTI_CAR_PARK_EDGE = new Tile(152, "Car Park Edge", TileType.BUILDING_PART);
    public static readonly Tile MULTI_CAR_PARK_CORNER = new Tile(153, "Car Park Corner", TileType.BUILDING_PART);
    public static readonly Tile MULTI_CAR_PARK_INNER = new Tile(154, "Car Park Inner", TileType.BUILDING_PART);
    public static readonly Tile MULTI_CAR_PARK_EXIT = new Tile(155, "Car Park Exit", TileType.BUILDING_PART);
    
    public static readonly Tile OFFICE_1_CORNER_RECESSED_L = new Tile(156, "Office 1 Corner Recessed L", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_CORNER_RECESSED_R = new Tile(157, "Office 1 Corner Recessed R", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_CORNER_RECESSED = new Tile(158, "Office 1 Corner Recessed", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE_RECESSED = new Tile(159, "Office 1 Edge Recessed", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE_RECESSED_ENTRANCE = new Tile(160, "Office 1 Edge Recessed Entrance", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE_RECESSED_GARAGE = new Tile(161, "Office 1 Edge Recessed Garage", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_EDGE = new Tile(162, "Office 1 Edge", TileType.BUILDING_PART);
    public static readonly Tile OFFICE_1_INNER_ROOF = new Tile(163, "Office 1 Inner", TileType.BUILDING_PART);
    
    //Skyscrapers... if I ever make any!
    public static readonly Tile SKYSCRAPER_GENERIC_1 = new Tile(200, "Generic Skyscraper 1", TileType.BUILDING_PART);
    #endregion
    
    [SerializeField] private GameObject[] register = null;

    public static Tile[] GetRegistry() { return tileRegistry; }

    public override void Initialize() {
        StartCoroutine(Register());
    }
    
    public override void Process() {}

    IEnumerator Register() {
        for (int i = 0; i < register.Length; i++) {
            if (register[i] != null && register[i].GetComponent<TileData>() != null) {
                GameObject reg = Instantiate(register[i], transform, true);
                reg.transform.position = new Vector3(0, -100, 0);
                reg.SetActive(false);
                Register(reg); //Instantiate to create a copy, instead of using original. Preserves original prefab.
                yield return null;
            }
        }
        Debug.Log("Registration complete");
        TypeRegistries.PopulateRegistries();
        SetComplete();
        yield return null;
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
            Debug.LogError("Overwriting existing tile with ID " + tile.GetId() + "! Replacing " + GetTileFromID(tile.GetId()).GetName() + " with " + tile.GetName());
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
        if (registry[id] == null) {
            Debug.LogError("Instantiating " + id + " which is null!!");
            return null;
        }
        GameObject go = Instantiate(registry[id]);
        go.SetActive(true);
        go.GetComponent<TileData>().CreateFromRegistry();
        return go;
    }
    
    //No point in implementing, would just slow down load times and this parts very fast anyway, no one would see it.
    public override int GetGenerationPercentage() { return 50; }
    public override string GetGenerationString() { return "Initializing tile registry"; }
}

public class Tile {
    private readonly int id;
    private readonly string name;
    private readonly TileType tileType;

    public Tile(int id, string name, TileType tileType) {
        this.id = id;
        this.name = name;
        this.tileType = tileType;
        
        TileRegistry.tileRegistry[id] = this;
    }

    public int GetId() { return id; }
    public string GetName() { return name; }
    public TileType GetTileType() { return tileType; }
}/*

public enum EnumTile {
    /* 000 #1# AIR,
    /* 001 #1# GRASS,
    /* 002 #1# REFERENCE,
    /* 003 #1# STRAIGHT_ROAD,
    /* 004 #1# CORNER_ROAD,
    /* 005 #1# T_JUNCT_ROAD,
    /* 006 #1# T_JUNCT_ROAD_CONTROLLED,
    /* 007 #1# CROSSROAD_ROAD,
    /* 008 #1# CROSSROAD_ROAD_CONTROLLED,
    /* 009 #1# T_JUNCT_ROAD_SINGLE_IN,
    /* 010 #1# T_JUNCT_ROAD_SINGLE_OUT, 
    /* 011 #1# ZEBRA_CROSSING,
    /* 012 #1# PELICAN_CROSSING,
    /* 013 #1# ROAD_WORLD_EXIT,
    /* 014 #1# 
    /* 015 #1# MULTI_CAR_PARK_RAMP,
    /* 016 #1# MULTI_CAR_PARK_EDGE,
    /* 017 #1# MULTI_CAR_PARK_CORNER,
    /* 018 #1# MULTI_CAR_PARK_INNER,
    /* 019 #1# MULTI_CAR_PARK_EXIT,
    /* 020 #1# 
    /* 021 #1# 
    /* 022 #1# 
    /* 023 #1# 
    /* 024 #1# 
    /* 025 #1# 
    /* 026 #1# 
    /* 027 #1# 
    /* 028 #1# 
    /* 029 #1# 
    /* 030 #1# MULTI_CAR_PARK_ENTRANCE,
    /* 031 #1# 
    /* 032 #1# 
    /* 033 #1# 
    /* 034 #1# 
    /* 035 #1# 
    /* 036 #1# 
    /* 037 #1# 
    /* 038 #1# 
    /* 039 #1# 
    /* 04 #1# 
    
    
    
    
    
    
    
    
    
    OFFICE_1_CORNER_RECESSED_L,
    OFFICE_1_CORNER_RECESSED_R,
    OFFICE_1_CORNER_RECESSED,
    OFFICE_1_EDGE_RECESSED,
    OFFICE_1_EDGE_RECESSED_ENTRANCE,
    OFFICE_1_EDGE_RECESSED_GARAGE,
    OFFICE_1_EDGE,
    OFFICE_1_INNER_ROOF,
    TOWN_HOUSE_1,
    TOWN_HOUSE_2,
    TOWN_HOUSE_3,
    TOWN_HOUSE_4,
    COFFEE_SHOP_1,
    ZORO,
    PRIDAVE,
    PREVIOUS,
    RURAL_COSTUMERS,
    G_AND_L,
    VERYWET,
    LAKE_CONTINENT,
    OLD_SOUND,
    HOSPITAL_8x8,
    TOWN_HALL_8x8,
    UNIVERSITY_8x8,
    SHOP_BESTCO,
    SHOP_812,
    SHOP_WCDONALDS,
    SHOP_BURGERQUEEN,
    SHOP_SFC,
    SHOP_PIZZA_HOUSE,
    SHOP_TGITUESDAYS,
    SHOP_WAGAPAPA,
    SHOP_GRANDPADOS,
    SHOP_TENGIRLS,
    SHOP_DOMPATH,
    SKYSCRAPER_GENERIC_1
}*/

public enum TileType {
    AIR,
    GRASS,
    REFERENCE,
    ROAD,
    BUILDING_SHOP,
    BUILDING_HOUSE,
    BUILDING_PART,
    BUILDING_MAJOR,
    LOWERED
}
