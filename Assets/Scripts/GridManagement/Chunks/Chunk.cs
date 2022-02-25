using System.Collections;
using System.Diagnostics;
using Tiles.TileManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Chunk : MonoBehaviour {
    
    public static readonly int size = 16;

    private GameObject[,] chunk = null;

    [SerializeField] private int chunkX = 0; //Chunks coordinate in world grid
    [SerializeField] private int chunkZ = 0;
    
    private bool hasStarted = false;
    private bool hasCompletedInit = false;
    private bool recheck = false;

    private Stopwatch stopWatch;

    void Start() {
        chunk = new GameObject[size, size];
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

    public void SetPosition(int x, int z) {
        chunkX = x;
        chunkZ = z;
    }
    
    IEnumerator BuildChunk() {
        ChunkPos pos = ChunkPos.GetChunkPosFromLocation(transform.position);
        bool chunkFileExist = SaveLoadChunk.FileExists(pos);
        if (chunkFileExist) {
            Debug.Log("Attempting to load chunk from save file...");
            GameObject[,] chunk = SaveLoadChunk.DeserializeChunk(ChunkPos.GetChunkPosFromLocation(transform.position));
            
            LoadChunk(chunk);
        }
        else {
            Debug.Log("Chunk save file does not exist. Creating new chunk.");
            foreach (Transform child in transform) {
                TileData tile = child.gameObject.GetComponent<TileData>();
                chunk[tile.GetGridPos().x, tile.GetGridPos().z] = child.gameObject;
            }
        }

        stopWatch.Stop();
        Debug.Log("Chunk generation took " + stopWatch.Elapsed + " seconds.");
  
        hasCompletedInit = true;
        yield return null;
    }
    
    public void LoadChunk(GameObject[,] chunk) {
        this.chunk = chunk;
        for (int row = 0; row < 16; row++) {
            for (int col = 0; col < 16; col++) {
                this.chunk[row, col].name = $"cell_{row}_{col}";
                this.chunk[row, col].transform.parent = transform;
            }
        }
    }

    public bool IsValidLocation(TilePos pos) {
        return (pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0);
    }
    
    public bool FillChunkCell(GameObject go, int row, int col, EnumTileDirection rotation,  bool placementChecks) {
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
        
        cell = Instantiate(go, transform, true);
        cell.name = $"cell_{row}_{col}";

        if (chunk[row, col] != null) { 
            DeleteChunkCell(row, col);
        }
        
        chunk[row, col] = cell;
        
        cell.transform.position = new Vector3(GridManager.Instance.GetGridTileSize() * row, 0, GridManager.Instance.GetGridTileSize() * col) + transform.position;
        cell.transform.rotation = Quaternion.Euler(0,rotation.GetRotation(),0);
        TilePos.GetGridPosFromLocation(cell.transform.position);
        cell.GetComponent<TileData>().SetInitialPos();
        return true;
    }


    public void DeleteChunkCell(int row, int col) {
        Destroy(chunk[row, col]);
        recheck = true;
    }

    public void DeleteChunkCell(TilePos pos) {
        DeleteChunkCell(pos.x, pos.z);
    }

    private void RecheckChunk() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                if (chunk[row, col] == null) {
                    Debug.Log("Repairing grid at " + row + ", " + col);
                    FillChunkCell(TileRegistry.GetGrass(), row, col, 0, false);
                } else if (chunk[row, col].GetComponent<TileData>() == null) {
                    Destroy(chunk[row, col]);
                    FillChunkCell(TileRegistry.GetGrass(), row, col, 0, false);
                }
            }
        }

        recheck = false;
    }

    public GameObject GetChunkCellContents(int row, int col) {
        return chunk[row, col];
    }

    public GameObject GetChunkCellContents(TilePos pos) {
        return chunk[pos.x, pos.z];
    }

    public TileData GetGridTile(int row, int col) {
        GameObject go = GetChunkCellContents(row, col);
        return go.GetComponent<TileData>();
    }

    public TileData GetGridTile(TilePos pos) {
        GameObject go = GetChunkCellContents(pos);
        return go.GetComponent<TileData>();
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

public enum EnumChunkState {
    UNGENERATED,
    GENERATING,
    GENERATED,
    RECHECK
}
