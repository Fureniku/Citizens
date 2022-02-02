using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{
    private static WorldGenerator _instance;
    public static WorldGenerator Instance {
        get { return _instance; }
    }

    [SerializeField] private GameObject defaultTile;
    [SerializeField] private int mapSize = 100;
    [SerializeField] private GameObject tileParent;
    //[SerializeField] private GameObject roadGenParent;

    [SerializeField] private GameObject[] roadGenerators;
    
    private static TileGrid grid;

    void Awake() {
        Debug.Log("Initialize world generator");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }

    void Start() {
        grid = GetGrid();
        InitializeWorld();

        for (int i = 0; i < roadGenerators.Length; i++) {
            roadGenerators[i].GetComponent<RoadGenerator>().BeginRoadGeneration();
        }
    }

    public TileGrid GetGrid() {
        if (grid == null) {
            Debug.Log("Creating grid");
            return new TileGrid(mapSize);
        }
        return grid;
    }

    private void InitializeWorld() {
        Debug.Log("Starting world initiliazation");
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                TilePos pos = new TilePos(i - mapSize / 2, j - mapSize / 2);
                
                
                PlaceTile(defaultTile, pos.GetVector3(), 0, pos);
            }
        }
        Debug.Log("World initialized");
    }

    //Handle replacing tiles in the world during generation
    public void PlaceTile(GameObject go, Vector3 goPos, int rot, TilePos pos) {
        Destroy(TileGrid.GetTile(pos)); //First, clear existing tiles from the slot
        
        GameObject generated = Instantiate(go, goPos, Quaternion.identity); //Create our new tile in the same position
        
        generated.transform.parent = tileParent.transform; //Parent it to our tiles object (mainly for editor simplicity)
        TileData placedTile = TileData.GetFromGameObject(generated); //Get the tile data from the gameobject


        if (placedTile.IsHalfRotation() && rot >= 180) { //Write rotation data
            placedTile.SetRotation(rot - 180); //Some tiles are identical at opposite rotations
        }
        else {
            placedTile.SetRotation(rot);
        }
        
        
        placedTile.SetGridPos(pos); //Write the grid coordinates to the tile
        if (!TileGrid.SetTile(pos, generated)) {
            Destroy(generated);
            Debug.Log("!!! Tile generation at " + pos.ToString() + " failed!");
        }
    }

    public void SeedRoadGenerator() {
        
    }
}