using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GridManager : GenerationSystem {
    
    [SerializeField, Range(1, 500)] private int size = 25;

    private GameObject[,] grid = null;

    [SerializeField, Range(1, 200)] private float gridSlotSize = 50.0f;
    [SerializeField, Range(1, 200)] private int randomSeed = 10;

    [SerializeField] GameObject defaultChunk = null;
    [SerializeField] GameObject emptyChunk = null;

    private int currentChunks = 0;
    private int maxChunks = 0;
    
    public readonly float defaultScale = 50f;

    [ReadOnly, SerializeField] private GridManagerState state = GridManagerState.UNINITIALIZED;

    private Stopwatch stopWatch;

    public void Initialize() {
        state = GridManagerState.INITIALIZED;
    }
    
    void Start() {
        Random.InitState(randomSeed);

        grid = new GameObject[size, size];
        stopWatch = Stopwatch.StartNew();
        maxChunks = size * size;
    }

    void FixedUpdate() {
        if (state == GridManagerState.INITIALIZED && World.Instance.GetWorldState() == EnumWorldState.GEN_CHUNKS) {
            state = GridManagerState.LOADING;
            Debug.Log("Grid starting...");
            stopWatch.Start();
            StartCoroutine(BuildGrid());
        }
        
        if (state == GridManagerState.ENABLED && World.Instance.GetWorldState() == EnumWorldState.GEN_CHUNKS) {
            if (World.Instance.DoesWorldExist()) {
                World.Instance.SetWorldState(EnumWorldState.GEN_NAVMESH);
            }
            else {
                World.Instance.SetWorldState(EnumWorldState.GEN_ROADS);
            }
            state = GridManagerState.READY;
        }
        
        if (state == GridManagerState.ENABLING) { //Enabled must be set for the next frame, not current.
            EnableWorld();
        }

        GetGenerationPercentage();
        GetGenerationString();
    }

    IEnumerator BuildGrid() {
        for (int row = 0; row < size; row++) {
            for (int col = 0; col < size; col++) {
                if (SaveLoadChunk.FileExists(new ChunkPos(row, col))) {
                    World.Instance.SetWorldExists();
                    FillChunkCell(emptyChunk, row, col);
                }
                else {
                    FillChunkCell(defaultChunk, row, col);
                }
                
                grid[row,col].SetActive(false);
                currentChunks++;

                yield return null;
            }
        }

        state = GridManagerState.ENABLING;

        yield return null;
    }

    void EnableWorld() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
            //yield return null;
        }
        stopWatch.Stop();
        Debug.Log("World generation took " + stopWatch.Elapsed + " seconds.");
        state = GridManagerState.ENABLED;
        //yield return null;
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

    public bool IsValidChunk(ChunkPos pos) { return pos.x < size && pos.x >= 0 && pos.z < size && pos.z >= 0; }
    public bool IsValidTile(TilePos pos) { return IsValidChunk(TilePos.GetParentChunk(pos)); }
    public int GetSize() { return size; }
    public Chunk GetChunk(int row, int col) { return GetChunk(new ChunkPos(row, col)); }
    public Chunk GetChunk(ChunkPos pos) { return grid[pos.x, pos.z].GetComponent<Chunk>(); }
    public float GetGridTileSize() { return gridSlotSize; }
    public bool IsInitialized() { return state == GridManagerState.READY; }
    
    /////////////////////////////////// Abstract inheritence stuff ///////////////////////////////////
    public override int GetGenerationPercentage() {
        int add = 0;
        if (state == GridManagerState.ENABLING) {
            add = 5;
        } else if (state == GridManagerState.ENABLED) {
            add = 10;
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
            case GridManagerState.ENABLED:
                message = "Chunks are ready!";
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
    ENABLED,
    READY
}
