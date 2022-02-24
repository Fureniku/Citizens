using System.Collections;
using System.Diagnostics;
using Tiles.TileManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Chunk : MonoBehaviour {
    
    [SerializeField, Range(5, 500)] private int size = 16;

    private GameObject[,] chunkArray = null;

    [SerializeField, Range(1, 200)] private float gridSlotSize = 50.0f;
    [SerializeField, Range(1, 200)] private int randomSeed = 10;

    [SerializeField] GameObject defaultTilePrefab = null;

    private bool hasStarted = false;
    private bool hasCompletedInit = false;
    private bool recheck = false;

    private Stopwatch stopWatch;

    void Start() {
        Random.InitState(randomSeed);

        chunkArray = new GameObject[size, size];
        stopWatch = Stopwatch.StartNew();
    }

    void Update() {
        if (!hasStarted) {
            Debug.Log("Starting chunk generation...");
            stopWatch.Start();
            StartCoroutine(BuildChunk());
            hasStarted = true;
        }

        if (hasCompletedInit && recheck) {
            RecheckChunk();
        }
    }
    
    IEnumerator BuildChunk() {
        Debug.Log("Starting build chunk of total size " + size*size);
        for (int row = 0; row < size; row++) {
            GameObject rowGo = GetRowParent(row);
            for (int col = 0; col < size; col++) {
                FillChunkCell(defaultTilePrefab, rowGo, row, col, 0, false);
                yield return null;
            }
            rowGo.SetActive(false);
        }
        Debug.Log("Chunk generation complete");

        for (int row = 0; row < size; row++) {
            GameObject rowGo = GetRowParent(row);
            rowGo.SetActive(true);
        }
        stopWatch.Stop();
        Debug.Log("Chunk generation took " + stopWatch.Elapsed + " seconds.");
        Debug.Break();
        hasCompletedInit = true;
    }

    public bool IsValidLocation(TilePos pos) {
        return (pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0);
    }
    
    public bool FillChunkCell(GameObject go, GameObject parent, int row, int col, EnumTileDirection rotation,  bool placementChecks) {
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

        if (chunkArray[row, col] != null) { 
            DeleteChunkCell(row, col);
        }
        
        chunkArray[row, col] = cell;
        
        cell.transform.position = new Vector3(gridSlotSize * row, 0, gridSlotSize * col) + transform.position;
        cell.transform.rotation = Quaternion.Euler(0,rotation.GetRotation(),0);
        TilePos.GetGridPosFromLocation(cell.transform.position);
        return true;
    }

    public bool FillChunkCell(GameObject go, int row, int col, EnumTileDirection rotation, bool placementChecks) {
        return FillChunkCell(go, GetRowParent(row), row, col, rotation, placementChecks);
    }

    public void DeleteChunkCell(int row, int col) {
        Destroy(chunkArray[row, col]);
        recheck = true;
    }

    public void DeleteChunkCell(TilePos pos) {
        DeleteChunkCell(pos.x, pos.z);
    }

    private void RecheckChunk() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                if (chunkArray[row, col] == null) {
                    Debug.Log("Repairing grid at " + row + ", " + col);
                    FillChunkCell(TileRegistry.GetGrass(), row, col, 0, false);
                } else if (chunkArray[row, col].GetComponent<TileData>() == null) {
                    Destroy(chunkArray[row, col]);
                    FillChunkCell(TileRegistry.GetGrass(), row, col, 0, false);
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
    
    public GameObject GetChunkCellContents(int row, int col) {
        return chunkArray[row, col];
    }

    public GameObject GetChunkCellContents(TilePos pos) {
        return chunkArray[pos.x, pos.z];
    }

    public TileData GetGridTile(int row, int col) {
        GameObject go = GetChunkCellContents(row, col);
        return go.GetComponent<TileData>();
    }

    public TileData GetGridTile(TilePos pos) {
        GameObject go = GetChunkCellContents(pos);
        return go.GetComponent<TileData>();
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

    public EnumGenerateDirection GetAvailableGenerateDirection(TilePos startPos, TileData data) {
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
    }

    private bool CheckSlot(int x, int z) {
        if (x == 0 && z == 0) return true;
        
        TilePos placeLoc = new TilePos(x, z);
        
        if (IsValidLocation(placeLoc)) {
            GameObject goTile = GetChunkCellContents(placeLoc);
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
    }
}
