using System;
using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {
    
    private static GridManager _instance;
    public static GridManager Instance {
        get { return _instance; }
    }
    
    [SerializeField, Range(5, 50)] private int width = 25;
    [SerializeField, Range(5, 50)] private int height = 25;

    private GameObject[,] grid = null;

    [SerializeField, Range(1, 200)] private float gridSlotSize = 50.0f;
    [SerializeField, Range(1, 200)] private int randomSeed = 10;
    private int prevSeed = -1;

    //[SerializeField] private string randomShaderTypeName = "Transparent/Diffuse";

    [SerializeField] private GameObject defaultTilePrefab = null;

    private bool hasStarted = false;
    private bool hasCompletedInit = false;
    private bool recheck = false;

    void Awake() {
        Debug.Log("Initialize grid manager");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }
    
    void Start() {
        Random.InitState(randomSeed);
        prevSeed = randomSeed;

        grid = new GameObject[height, width];
    }

    void Update() {
        if (!hasStarted) {
            Debug.Log("Grid starting...");
            StartCoroutine(BuildGrid());
            hasStarted = true;
        }

        if (hasCompletedInit && recheck) {
            RecheckGrid();
        }
    }

    IEnumerator BuildGrid() {
        Debug.Log("Starting build grid of total size " + height*width);
        for (int row = 0; row < height; row++) {
            GameObject rowGO = GetRowParent(row);
            for (int col = 0; col < width; col++) {
                FillGridCell(defaultTilePrefab, rowGO, row, col, 0, false);
                yield return null;
            }
        }
        Debug.Log("Grid generation complete");
        Debug.Break();
        hasCompletedInit = true;
    }

    public bool IsValidLocation(TilePos pos) {
        return (pos.x < width && pos.x >= 0 && pos.z < height && pos.z >= 0);
    }
    
    public bool FillGridCell(GameObject go, GameObject parent, int row, int col, EnumTileDirection rotation,  bool placementChecks) {
        GameObject cell = null;

        SkyscraperGenerator skyTile = go.GetComponent<SkyscraperGenerator>();
        if (skyTile != null) {
            Debug.Log("Filling grid cell with skyscraper! Pos: " + row + ", " + col + ", size: " + skyTile.GetWidth() + ", " + skyTile.GetLength());
        }

        //If requested, check that the current tile is grass before placement.
        //Used for multi-grid spawner placements.
        if (placementChecks) {
            if (IsValidLocation(new TilePos(row, col))) {
                if (!(GetGridTile(row, col) is TileGrass)) {
                    Debug.Log("Current position tile is not grass! abort!");
                    return false;
                }
            }
        }
        
        cell = Instantiate(go, parent.transform, true);
        cell.name = $"cell_{row}_{col}";

        if (grid[row, col] != null) { 
            DeleteGridCell(row, col);
        }
        
        grid[row, col] = cell;
        
        cell.transform.position = new Vector3(gridSlotSize * row, 0, gridSlotSize * col) + transform.position;
        cell.transform.rotation = Quaternion.Euler(0,rotation.GetRotation(),0);
        TilePos.GetGridPosFromLocation(cell.transform.position);
        return true;
    }

    public bool FillGridCell(GameObject go, int row, int col, EnumTileDirection rotation, bool placementChecks) {
        return FillGridCell(go, GetRowParent(row), row, col, rotation, placementChecks);
    }

    public void DeleteGridCell(int row, int col) {
        Destroy(grid[row, col]);
        recheck = true;
    }

    public void DeleteGridCell(TilePos pos) {
        DeleteGridCell(pos.x, pos.z);
    }

    private void RecheckGrid() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                if (grid[row, col] == null) {
                    Debug.Log("Repairing grid at " + row + ", " + col);
                    FillGridCell(TileRegistry.GetGrass(), row, col, 0, false);
                } else if (grid[row, col].GetComponent<TileData>() == null) {
                    Destroy(grid[row, col]);
                    FillGridCell(TileRegistry.GetGrass(), row, col, 0, false);
                }
            }
        }

        recheck = false;
    }

    private GameObject GetRowParent(int row) {
        Transform rowTransform = transform.Find($"row_{row}");
        GameObject rowParent = null;
        if (rowTransform == null) {
            rowParent = new GameObject($"row_{row}");
            rowParent.transform.parent = transform;
        }
        else {
            rowParent = rowTransform.gameObject;
        }

        return rowParent;
    }
    
    public GameObject GetGridCellContents(int row, int col) {
        return grid[row, col];
    }

    public GameObject GetGridCellContents(TilePos pos) {
        return grid[pos.x, pos.z];
    }

    public TileData GetGridTile(int row, int col) {
        GameObject go = GetGridCellContents(row, col);
        return go.GetComponent<TileData>();
    }

    public TileData GetGridTile(TilePos pos) {
        GameObject go = GetGridCellContents(pos);
        return go.GetComponent<TileData>();
    }
    
    public int GetMaxGridX() {
        return width;
    }

    public int GetMaxGridY() {
        return height;
    }

    public float GetGridTileSize() {
        return gridSlotSize;
    }

    public bool IsInitialized() {
        return hasCompletedInit;
    }

    public void FlagForRecheck() {
        recheck = true;
    }

    public EnumGenerateDirection GetAvailableGenerateDirection(TilePos startPos, TileData data) {
        int gridX = data.GetWidth();
        int gridZ = data.GetLength();
        Debug.Log("Getting available generation direction from " + startPos.x + ", " + startPos.z);
        bool nePassed = true;
        bool sePassed = true;
        bool swPassed = true;
        bool nwPassed = true;
        Debug.Log("Checking XPOS, ZPOS tile");
        for (int i = 0; i < gridZ; i++) {
            for (int j = 0; j < gridX; j++) {
                if (nePassed) { //x+ z+
                    if (!CheckSlot(startPos.x + j, startPos.z + i)) {
                        Debug.Log("XPOS, ZPOS failed. Breaking out of loop. This shouldn't appear again for this generation.");
                        nePassed = false;
                    }
                }
                if (sePassed) { //x+ z-
                    if (!CheckSlot(startPos.x + j, startPos.z - i)) {
                        Debug.Log("XPOS, ZPOS failed. Breaking out of loop. This shouldn't appear again for this generation.");
                        sePassed = false;
                    }
                }
                if (swPassed) { //x- z-
                    if (!CheckSlot(startPos.x - j, startPos.z - i)) {
                        Debug.Log("XPOS, ZPOS failed. Breaking out of loop. This shouldn't appear again for this generation.");
                        swPassed = false;
                    }
                }
                if (nwPassed) { //x- z-
                    if (!CheckSlot(startPos.x - j, startPos.z + i)) {
                        Debug.Log("XPOS, ZPOS failed. Breaking out of loop. This shouldn't appear again for this generation.");
                        nwPassed = false;
                    }
                }
            }
        }
        
        Debug.Log("GenChecks complete. NE: " + nePassed + ", SE: " + sePassed  + ", SW: " + swPassed  + ", NW: " + nwPassed);

        if (nePassed) { return EnumGenerateDirection.NORTH_EAST; } 
        if (sePassed) { return EnumGenerateDirection.SOUTH_EAST; } 
        if (swPassed) { return EnumGenerateDirection.SOUTH_WEST; } 
        if (nwPassed) { return EnumGenerateDirection.NORTH_WEST; }

        Debug.Log("No valid locations.");
        return EnumGenerateDirection.NONE;
    }

    private bool CheckSlot(int x, int z) {
        if (x == 0 && z == 0) return true;
        
        TilePos placeLoc = new TilePos(x, z);
        
        if (IsValidLocation(placeLoc)) {
            GameObject goTile = GetGridCellContents(placeLoc);
            TileData tile = TileData.GetFromGameObject(goTile);
            if (tile != null) {
                Debug.Log("it's a "+ tile.GetName());
                if (!(tile is TileGrass)) {
                    Debug.Log("Tile isn't grass. No space, skipping to next. There should be no more XPOS, ZPOS failures.");
                    return false;
                }
            }
            else {
                Debug.Log("Tile is null! Very bad!");
                return false;
            }
        }
        else {
            Debug.Log("Invalid location; fail!");
            return false;
        }

        return true;
    }
}
