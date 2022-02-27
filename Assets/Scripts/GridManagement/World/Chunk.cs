using System.Collections;
using System.Diagnostics;
using Tiles.TileManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Chunk : MonoBehaviour {
    
    public static readonly int size = 16;

    private GameObject[,] chunk = null;

    [SerializeField] private ChunkPos chunkPos = ChunkPos.ZERO; //Chunks coordinate in world grid

    [SerializeField] private bool GenerateFreshChunks = false;

    private EnumChunkState state = EnumChunkState.UNGENERATED;

    private Stopwatch stopWatch;

    void Start() {
        chunk = new GameObject[size, size];
        stopWatch = Stopwatch.StartNew();
    }

    void Update() {
        if (state == EnumChunkState.UNGENERATED) {
            stopWatch.Start();
            if (GenerateFreshChunks) {
                GenerateFreshChunk();
            }
            else {
                BuildChunk();
            }
            
            state = EnumChunkState.GENERATING;
        }

        if (state == EnumChunkState.RECHECK) {
            RecheckChunk();
        }
    }


    void BuildChunk() {
        ChunkPos pos = ChunkPos.GetChunkPosFromLocation(transform.position);
        bool chunkFileExist = SaveLoadChunk.FileExists(pos);
        if (chunkFileExist) {
            GameObject[,] chunk = SaveLoadChunk.DeserializeChunk(ChunkPos.GetChunkPosFromLocation(transform.position));
            
            LoadChunk(chunk);
        }
        else {
            for (int i = 0; i < transform.childCount; i++) {
                TileData tile = transform.GetChild(i).gameObject.GetComponent<TileData>();
                SetChunkCell(transform.GetChild(i).gameObject, tile.GetLocalPos().x, tile.GetLocalPos().z);
            }
        }

        World.Instance.GetGridManager().SetChunkCell(gameObject, pos.x, pos.z);
        
        stopWatch.Stop();
        Debug.Log("Chunk generation took " + stopWatch.Elapsed + " seconds.");

        state = EnumChunkState.READY;
    }
    
    public void LoadChunk(GameObject[,] chunk) {
        this.chunk = chunk;
        for (int row = 0; row < 16; row++) {
            for (int col = 0; col < 16; col++) {
                FillChunkCell(chunk[row, col], new LocalPos(row, col), 0, false);
            }
        }
    }

    public bool FillChunkCell(GameObject go, LocalPos pos, EnumTileDirection rotation,  bool placementChecks) {
        GameObject cell = null;

        if (!pos.IsValidPos()) {
            return false;
        }

        if (go == null) {
            Debug.Log("Gameobject is null!");
            return false;
        }

        TileBuilding skyTile = go.GetComponent<TileBuilding>();
        if (skyTile != null) {
            Debug.Log("Filling grid cell with skyscraper! Pos: " + pos.x + ", " + pos.z + ", size: " + skyTile.GetWidth() + ", " + skyTile.GetLength());
        }

        //If requested, check that the current tile is grass before placement.
        //Used for multi-grid spawner placements.
        //TODO does this still work with tilepos?
        if (placementChecks) {
            if (IsValidLocation(new TilePos(pos.x, pos.z))) {
                if (!(GetGridTile(pos.x, pos.z) is TileGrass)) {
                    Debug.Log("Current position tile is not grass! abort!");
                    return false;
                }
            }
        }
        
        cell = Instantiate(go, transform, true);
        cell.name = $"cell_{pos.x}_{pos.z}";

        if (GetChunkCellContents(pos.x, pos.z) != null) { 
            DeleteChunkCell(pos.x, pos.z, true);
        }
        
        chunk[pos.x, pos.z] = cell;
        
        cell.transform.position = new Vector3(World.Instance.GetGridManager().GetGridTileSize() * pos.x, 0, World.Instance.GetGridManager().GetGridTileSize() * pos.z) + transform.position;
        cell.transform.rotation = Quaternion.Euler(0,rotation.GetRotation(),0);
        TilePos.GetGridPosFromLocation(cell.transform.position);
        cell.GetComponent<TileData>().SetInitialPos();
        return true;
    }

    public void FillChunkCell(GameObject go, TilePos pos, EnumTileDirection rotation, bool placementChecks) {
        FillChunkCell(go, LocalPos.FromTilePos(pos), rotation, placementChecks);
    }

    private void RecheckChunk() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                if (chunk[row, col] == null) {
                    Debug.Log("Repairing grid at " + row + ", " + col);
                    FillChunkCell(TileRegistry.GetGrass(), new LocalPos(row, col), 0, false);
                } else if (chunk[row, col].GetComponent<TileData>() == null) {
                    Destroy(chunk[row, col]);
                    FillChunkCell(TileRegistry.GetGrass(), new LocalPos(row, col), 0, false);
                }
            }
        }

        state = EnumChunkState.READY;
    }

    public void DeleteChunkCell(int row, int col, bool flag) {
        Destroy(chunk[row, col]);
        if (flag) { } //FlagForRecheck();
    }

    public TileData GetGridTile(int row, int col) {
        return GetGridTile(new LocalPos(row, col));
    }

    public TileData GetGridTile(LocalPos pos) {
        GameObject go = GetChunkCellContents(pos);
        return go == null ? null : go.GetComponent<TileData>();
    }

    public void SetChunkCell(GameObject go, int row, int col) {
        DeleteChunkCell(row, col, false);
        chunk[row, col] = go;
        go.GetComponent<TileData>().SetInitialPos();
    }

    public bool IsValidLocation(TilePos pos) { return (pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0); }

    public GameObject GetChunkCellContents(int row, int col) {
        return GetChunkCellContents(new LocalPos(row, col));
    }
    
    public GameObject GetChunkCellContents(LocalPos pos) { 
        if (!pos.IsValidPos()) {
            //TODO Go to the world and get the relevant chunk for this.
            Debug.Log("Couldn't get chunk cell contents; out of range: " + pos.x + ", " + pos.z);
            return null;
        }
        return chunk[pos.x, pos.z];
    }
    
    public bool IsInitialized() { return state == EnumChunkState.READY || state == EnumChunkState.RECHECK; }
    public void FlagForRecheck() => state = EnumChunkState.RECHECK;
    public void SetPosition(ChunkPos pos) => chunkPos = pos;
    public ChunkPos GetPosition() { return chunkPos; }

    //Used to create a new chunk prefab on the occasion that it's needed.
    private void GenerateFreshChunk() {
        Debug.Log("Entirely fresh chunk requested");
        for (int row = 0; row < 16; row++) {
            for (int col = 0; col < 16; col++) {
                DeleteChunkCell(row, col, false);
                FillChunkCell(TileRegistry.GetGrass(), new LocalPos(row, col), 0, false);
            }
        }
        state = EnumChunkState.READY;
    }
}

public enum EnumChunkState {
    UNGENERATED,
    GENERATING,
    READY,
    RECHECK
}
