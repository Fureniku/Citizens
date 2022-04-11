using System;
using System.Collections;
using System.Diagnostics;
using Tiles.TileManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ChunkManager : GenerationSystem {
    
    private int size = 25;

    private GameObject[,] grid = null;

    private float gridSlotSize = 50.0f;
    [SerializeField, Range(1, 200)] private int randomSeed = 10;

    [SerializeField] GameObject defaultChunk = null;
    [SerializeField] GameObject emptyChunk = null;

    private int currentChunks = 0;
    private int maxChunks = 0;
    
    public readonly float defaultScale = 50f;

    [ReadOnly, SerializeField] private GridManagerState state = GridManagerState.UNINITIALIZED;

    private Stopwatch stopWatch;

    public override void Initialize() {
        state = GridManagerState.INITIALIZED;
    }
    
    void Start() {
        Random.InitState(randomSeed);
        size = WorldData.Instance.GetWorldSize();
        Debug.Log("Constructing world with size " + size);

        grid = new GameObject[size, size];
        stopWatch = Stopwatch.StartNew();
        maxChunks = size * size;
        if (World.Instance.IsInternalGenWorld()) {
            SetInternalWorldChunks();
        }
    }

    public override void Process() {
        if (state == GridManagerState.INITIALIZED) {
            state = GridManagerState.LOADING;
            Debug.Log("Grid starting...");
            stopWatch.Start();
            StartCoroutine(BuildGrid());
        }

        if (state == GridManagerState.ENABLING) { //Enabled must be set for the next frame, not current.
            EnableWorld();
        }

        GetGenerationPercentage();
        GetGenerationString();
    }

    public void SetInternalWorldChunks() {
        Debug.Log("Building grid from internal world. World has " + transform.childCount + " children.");
        for (int i = 0; i < transform.childCount; i++) {
            Chunk child = transform.GetChild(i).GetComponent<Chunk>();
            if (child != null) {
                grid[child.GetPosition().x, child.GetPosition().z] = transform.GetChild(i).gameObject;
            }
        }
    }

    IEnumerator BuildGrid() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                if (SaveLoadChunk.FileExists(new ChunkPos(row, col))) {
                    World.Instance.SetWorldExists();
                    FillChunkCell(emptyChunk, row, col);
                } else {
                    FillChunkCell(defaultChunk, row, col);
                }
                
                grid[row,col].SetActive(false);
                currentChunks++;

                yield return null;
            }
        }
        Debug.Log("Grid building should be complete");
        state = GridManagerState.ENABLING;

        yield return null;
    }

    void EnableWorld() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        stopWatch.Stop();
        Debug.Log("World generation took " + stopWatch.Elapsed + " seconds.");
        state = GridManagerState.READY;
        SetComplete();
    }

    public void FillChunkCell(GameObject go, int row, int col) {
        GameObject cell = Instantiate(go, transform);
        cell.name = $"chunk_{row}_{col}";
        cell.transform.position = new Vector3(gridSlotSize * row * Chunk.size + (gridSlotSize/2), 0, gridSlotSize * col * Chunk.size + (gridSlotSize/2)) + transform.position;
        cell.GetComponent<Chunk>().SetPosition(new ChunkPos(row, col));
        grid[row, col] = cell;
    }

    public void SetChunkCell(GameObject go, int row, int col) => grid[row, col] = go;

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
                    if (!CheckTileIsGrass(startPos.x + j, startPos.z + i)) {
                        nePassed = false;
                    }
                }
                if (sePassed) { //x+ z-
                    if (!CheckTileIsGrass(startPos.x + j, startPos.z - i)) {
                        sePassed = false;
                    }
                }
                if (swPassed) { //x- z-
                    if (!CheckTileIsGrass(startPos.x - j, startPos.z - i)) {
                        swPassed = false;
                    }
                }
                if (nwPassed) { //x- z-
                    if (!CheckTileIsGrass(startPos.x - j, startPos.z + i)) {
                        nwPassed = false;
                    }
                }
            }
        }

        if (nePassed) { return EnumGenerateDirection.NORTH_EAST; } 
        if (sePassed) { return EnumGenerateDirection.SOUTH_EAST; } 
        if (swPassed) { return EnumGenerateDirection.SOUTH_WEST; } 
        if (nwPassed) { return EnumGenerateDirection.NORTH_WEST; }

        return EnumGenerateDirection.NONE;
    }

    private bool CheckTileIsGrass(int x, int z) {
        if (x == 0 && z == 0) return true;
        
        TilePos placeLoc = new TilePos(x, z);
        
        if (IsValidTile(placeLoc)) {
            TileData tile = GetTile(placeLoc);
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

    public TileData GetTile(TilePos pos) {
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        int x = pos.x - (chunkPos.x * 16);
        int z = pos.z - (chunkPos.z * 16);
        return GetTile(chunkPos, x, z);
    }

    public TileData GetTile(ChunkPos pos, int x, int z) {
        Chunk chunk = GetChunk(pos);
        return chunk.GetGridTile(x, z);
    }

    public void SetTile(TilePos pos, int id, EnumDirection rot) {
        Chunk chunk = GetChunk(ChunkPos.GetChunkPosFromLocation(pos.GetWorldPos()));
        LocalPos lp = LocalPos.FromTilePos(pos);
        chunk.FillChunkCell(id, lp, rot);
    }
    
    public void SetTile(TilePos pos, int id, EnumDirection rot, Transform parent) {
        Chunk chunk = GetChunk(ChunkPos.GetChunkPosFromLocation(pos.GetWorldPos()));
        LocalPos lp = LocalPos.FromTilePos(pos);
        chunk.FillChunkCell(id, lp, rot, parent);
    }

    public bool IsValidChunk(ChunkPos pos) { return pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0; }
    public bool IsValidTile(TilePos pos) { return IsValidChunk(TilePos.GetParentChunk(pos)); }
    public int GetSize() { return size; }
    public Chunk GetChunk(int row, int col) { return GetChunk(new ChunkPos(row, col)); }
    public Chunk GetChunk(ChunkPos pos) { return grid[pos.x, pos.z].GetComponent<Chunk>(); }
    public float GetGridTileSize() { return gridSlotSize; }

    /////////////////////////////////// Abstract inheritence stuff ///////////////////////////////////
    public override int GetGenerationPercentage() {
        int add = 0;
        if (state == GridManagerState.ENABLING) {
            add = 5;
        }
        percentage = (currentChunks / maxChunks) * 90 + add;
        return percentage;
    }

    public override string GetGenerationString() {
        switch (state) {
            case GridManagerState.LOADING:
                message = "Chunks generating...";
                break;
            case GridManagerState.ENABLING:
                message = "Chunks enabling...";
                break;
            default:
                message = "STATE: " + state;
                break;
        }

        return message;
    }

    ////////////////////////////////////////// Editor stuff //////////////////////////////////////////
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        
        Vector3 pos0 = new Vector3();
        Vector3 pos1 = new Vector3();
        pos0.y = 5;
        pos1.y = 5;
        Gizmos.DrawLine(pos0, pos1);

        for (int i = -size; i < (size*2)+1; i++) {
            pos0.x = -(size * gridSlotSize * Chunk.size);
            pos1.x = size * gridSlotSize * Chunk.size * 2;
            pos0.z = i * gridSlotSize * Chunk.size;
            pos1.z = i * gridSlotSize * Chunk.size;
            Gizmos.DrawLine(pos0, pos1);
        }
        
        for (int i = -size; i < (size*2)+1; i++) {
            pos0.x = i * gridSlotSize * Chunk.size;
            pos1.x = i * gridSlotSize * Chunk.size;
            pos0.z = -(size * gridSlotSize * Chunk.size);
            pos1.z = size * gridSlotSize * Chunk.size * 2;
            Gizmos.DrawLine(pos0, pos1);
        }
    }
}

public enum GridManagerState {
    UNINITIALIZED,
    INITIALIZED,
    LOADING,
    ENABLING,
    READY
}
