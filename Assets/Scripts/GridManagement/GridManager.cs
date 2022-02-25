using System.Collections;
using System.Diagnostics;
using Tiles.TileManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {
    
    private static GridManager _instance;
    public static GridManager Instance {
        get { return _instance; }
    }

    private string worldName = "testworld";
    
    [SerializeField, Range(1, 500)] private int size = 25;

    private GameObject[,] grid = null;

    [SerializeField, Range(1, 200)] private float gridSlotSize = 50.0f;
    [SerializeField, Range(1, 200)] private int randomSeed = 10;

    [SerializeField] GameObject defaultChunk = null;

    private bool hasStarted = false;
    private bool hasCompletedInit = false;
    private bool recheck = false;

    private bool saving = false;

    private Stopwatch stopWatch;

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

        grid = new GameObject[size, size];
        stopWatch = Stopwatch.StartNew();
    }

    void Update() {
        if (!hasStarted) {
            Debug.Log("Grid starting...");
            stopWatch.Start();
            StartCoroutine(BuildGridNew());
            hasStarted = true;
        }

        if (saving) {
            SaveWorld();
        }

        /*if (hasCompletedInit && recheck) {
            RecheckGrid();
        }*/
    }

    IEnumerator BuildGridNew() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                FillChunkCell(defaultChunk, row, col);
                yield return null;
            }
        }
        
        stopWatch.Stop();
        Debug.Log("World generation took " + stopWatch.Elapsed + " seconds.");
        TriggerSave();
        
        yield return null;
    }

    private void TriggerSave() {
        saving = true;
        stopWatch = Stopwatch.StartNew();
    }

    void SaveWorld() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                SaveLoadChunk.SerializeChunk(GetChunk(row, col), row, col);
            }
        }
        stopWatch.Stop();
        Debug.Log("World saving complete! Took " + stopWatch.Elapsed + " seconds.");
        saving = false;
    }
    
    public bool IsValidLocation(TilePos pos) {
        return (pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0);
    }

    public void LoadChunk(GameObject[,] chunk) {
        grid = chunk;

        for (int row = 0; row < 16; row++) {
            for (int col = 0; col < 16; col++) {
                grid[row, col].name = $"cell_{row}_{col}";
                grid[row, col].transform.parent = transform;
            }
        }
    }

    public void FillChunkCell(GameObject go, int row, int col) {
        GameObject cell = Instantiate(defaultChunk, transform);
        cell.name = $"chunk_{row}_{col}";
        cell.transform.position = new Vector3(gridSlotSize * row * Chunk.size, 0, gridSlotSize * col * Chunk.size) + transform.position;
        cell.GetComponent<Chunk>().SetPosition(row, col);
        grid[col, row] = cell;
    }
    
    /*IEnumerator BuildGrid() {
        bool chunkFileExist = true;
        if (chunkFileExist) {
            Debug.Log("Attempting to load chunk from save file...");
            GameObject[,] chunk = SaveLoadChunk.DeserializeChunk(TilePos.GetGridPosFromLocation(transform.position));
            
            LoadChunk(chunk);
            /*for (int row = 0; row < size; row++) {
                GameObject rowGO = GetRowParent(row);
                for (int col = 0; col < size; col++) {
                    FillGridCell(chunk[row,col], rowGO, row, col, 0, false);
                    yield return null;
                }
                rowGO.SetActive(false);
            }*//*
            
            
        }
        else {
            Debug.Log("Attempting to generate new chunk...");
        
            for (int row = 0; row < size; row++) {
                GameObject rowGO = GetRowParent(row);
                for (int col = 0; col < size; col++) {
                    //FillGridCell(TileRegistry.GetGrass(), rowGO, row, col, 0, false);
                    yield return null;
                }
                rowGO.SetActive(false);
            }
        }
        
        Debug.Log("Grid generation complete");

        for (int row = 0; row < size; row++) {
            GameObject rowGO = GetRowParent(row);
            rowGO.SetActive(true);
        }
        stopWatch.Stop();
        Debug.Log("Chunk generation took " + stopWatch.Elapsed + " seconds.");
        
        Debug.Break();
        
        hasCompletedInit = true;
        yield return null;
    }*/
    
    /*public bool FillGridCell(GameObject go, GameObject parent, int row, int col, EnumTileDirection rotation,  bool placementChecks) {
        GameObject cell = null;

        TileBuilding skyTile = go.GetComponent<TileBuilding>();
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
        cell.GetComponent<TileData>().SetInitialPos();
        return true;
    }

    public bool FillGridCell(GameObject go, int row, int col, EnumTileDirection rotation, bool placementChecks) {
        return FillGridCell(go, GetRowParent(row), row, col, rotation, placementChecks);
    }*/

    public void DeleteGridCell(int row, int col) {
        Destroy(grid[row, col]);
        recheck = true;
    }

    public void DeleteGridCell(TilePos pos) {
        DeleteGridCell(pos.x, pos.z);
    }

    /*private void RecheckGrid() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
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
    }*/

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
    
    public Chunk GetChunk(int row, int col) {
        return grid[row, col].GetComponent<Chunk>();
    }

    public Chunk GetChunk(ChunkPos pos) {
        return grid[pos.x, pos.z].GetComponent<Chunk>();
    }

    public int GetGridSize() {
        return size;
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

    /*public EnumGenerateDirection GetAvailableGenerateDirection(TilePos startPos, TileData data) {
        int gridX = data.GetWidth();
        int gridZ = data.GetLength();
        bool nePassed = true;
        bool sePassed = true;
        bool swPassed = true;
        bool nwPassed = true;
        for (int i = 0; i < gridZ; i++) {
            for (int j = 0; j < gridX; j++) {
                if (nePassed) { //x+ z+
                    if (!CheckSlot(startPos.x + j, startPos.z + i)) {
                        nePassed = false;
                    }
                }
                if (sePassed) { //x+ z-
                    if (!CheckSlot(startPos.x + j, startPos.z - i)) {
                        sePassed = false;
                    }
                }
                if (swPassed) { //x- z-
                    if (!CheckSlot(startPos.x - j, startPos.z - i)) {
                        swPassed = false;
                    }
                }
                if (nwPassed) { //x- z-
                    if (!CheckSlot(startPos.x - j, startPos.z + i)) {
                        nwPassed = false;
                    }
                }
            }
        }

        if (nePassed) { return EnumGenerateDirection.NORTH_EAST; } 
        if (sePassed) { return EnumGenerateDirection.SOUTH_EAST; } 
        if (swPassed) { return EnumGenerateDirection.SOUTH_WEST; } 
        if (nwPassed) { return EnumGenerateDirection.NORTH_WEST; }

        Debug.Log("No valid locations.");
        return EnumGenerateDirection.NONE;
    }*/

    /*private bool CheckSlot(int x, int z) {
        if (x == 0 && z == 0) return true;
        
        TilePos placeLoc = new TilePos(x, z);
        
        if (IsValidLocation(placeLoc)) {
            GameObject goTile = GetGridCellContents(placeLoc);
            TileData tile = TileData.GetFromGameObject(goTile);
            if (tile != null) {
                if (!(tile is TileGrass)) {
                    return false;
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }

        return true;
    }*/

    public string GetWorldName() {
        return worldName;
    }
}
