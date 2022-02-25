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

    private EnumChunkState state = EnumChunkState.UNGENERATED;

    private Stopwatch stopWatch;

    void Start() {
        chunk = new GameObject[size, size];
        stopWatch = Stopwatch.StartNew();
    }

    void Update() {
        if (state == EnumChunkState.UNGENERATED) {
            Debug.Log("Starting chunk generation...");
            stopWatch.Start();
            StartCoroutine(BuildChunk());
            state = EnumChunkState.GENERATING;
        }

        if (state == EnumChunkState.RECHECK) {
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
            Debug.Log("Chunk save file does not exist. Creating new chunk. Has " + transform.childCount + " children.");
            for (int i = 0; i < transform.childCount; i++) {
                Debug.Log("Child:");
                Debug.Log(transform.GetChild(i).name);
                TileData tile = transform.GetChild(i).gameObject.GetComponent<TileData>();
                SetChunkCell(transform.GetChild(i).gameObject, tile.GetGridPos().x, tile.GetGridPos().z);
            }
        }

        GridManager.Instance.SetChunkCell(gameObject, pos.x, pos.z);
        
        stopWatch.Stop();
        Debug.Log("Chunk generation took " + stopWatch.Elapsed + " seconds.");

        state = EnumChunkState.READY;
        yield return null;
    }
    
    public void LoadChunk(GameObject[,] chunk) {
        this.chunk = chunk;
        for (int row = 0; row < 16; row++) {
            for (int col = 0; col < 16; col++) {
                FillChunkCell(chunk[row, col], row, col, 0, false);
            }
        }
    }

    public bool FillChunkCell(GameObject go, int row, int col, EnumTileDirection rotation,  bool placementChecks) {
        GameObject cell = null;

        if (go == null) {
            Debug.Log("Gameobject is null!");
            return false;
        }

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

        state = EnumChunkState.READY;
    }

    public void DeleteChunkCell(int row, int col) {
        Destroy(chunk[row, col]);
        FlagForRecheck();
    }

    public TileData GetGridTile(int row, int col) {
        GameObject go = GetChunkCellContents(row, col);
        return go.GetComponent<TileData>();
    }

    public TileData GetGridTile(TilePos pos) {
        GameObject go = GetChunkCellContents(pos);
        return go.GetComponent<TileData>();
    }

    public bool IsValidLocation(TilePos pos) { return (pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0); }
    public void SetChunkCell(GameObject go, int row, int col) { chunk[row, col] = go; }
    public GameObject GetChunkCellContents(int row, int col) { return chunk[row, col]; }
    public GameObject GetChunkCellContents(TilePos pos) { return chunk[pos.x, pos.z]; }
    public bool IsInitialized() { return state == EnumChunkState.READY || state == EnumChunkState.RECHECK; }
    public void FlagForRecheck() => state = EnumChunkState.RECHECK;
}

public enum EnumChunkState {
    UNGENERATED,
    GENERATING,
    READY,
    RECHECK
}
