using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class World : MonoBehaviour {

    private static World _instance;

    public static World Instance {
        get { return _instance; }
    }
    
    [SerializeField] private GridManager gridManager = null;
    private WorldData worldData = null;

    [SerializeField] private GameObject testAgent = null;
    [SerializeField] private int vehicleAgentCount;
    
    [SerializeField] private GameObject navMeshRoad = null; //Road navmesh
    [SerializeField] private GameObject navMeshSidewalk = null; //Sidewalk navmesh
    [SerializeField] private GameObject AStarPlane = null; //A-star plane

    [SerializeField] private int worldSize = 3;

    private bool isDirty = false;
    [SerializeField] private bool internalGeneratedWorld = false; //If the world was made and is a prefab now, set this to true to stop any attempts at generation.

    void Awake() {
        GC.Collect();
        Debug.Log("Initialize world");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
        
        if (!internalGeneratedWorld) {
            gridManager.Initialize();
            worldData = WorldData.Instance;
            if (worldData == null) {
                CreateWorldData();
            }

            worldData.SetNavMeshRoad(navMeshRoad);
            worldData.SetNavMeshSidewalk(navMeshSidewalk);
        }
        else {
            Debug.Log("Confirmed as internal world. Skipping generation.");
            worldData = FindObjectOfType<WorldData>();
            if (worldData == null) {
                CreateWorldData();
            }
        }
    }

    private void CreateWorldData() {
        Debug.Log("World data is null! Must create new instance!");
        GameObject go = new GameObject();
        go.AddComponent<WorldData>();
        go.name = "WorldData";
        worldData = go.GetComponent<WorldData>();
        worldData.SetWorldSize(worldSize);
    }

    public bool IsInternalGenWorld() { return internalGeneratedWorld; }

    public void SetWorldExists() => worldData.SetWorldExists();

    public bool DoesWorldExist() {
        return worldData.DoesWorldExist();
    }

    void Update() {
        if (isDirty) {
            SaveWorld();
        }

#if UNITY_EDITOR

        worldData.SetChunkGenPercent(gridManager.GetGenerationPercentage());
#endif
    }

    public WorldData GetWorldData() {
        return worldData;
    }
    
    public GridManager GetGridManager() {
        return gridManager;
    }

    public string GetWorldName() {
        return worldData.GetWorldName();
    }

    public bool SavingEnabled() {
        return worldData.SavingEnabled();
    }

    void SaveWorld() {
        if (SavingEnabled()) {
            Stopwatch stopWatch = Stopwatch.StartNew();
            Debug.Log("Starting world save...");
            for (int row = 0; row < gridManager.GetSize(); row++) {
                for (int col = 0; col < gridManager.GetSize(); col++) {
                    SaveLoadChunk.SerializeChunk(gridManager.GetChunk(row, col));
                }
            }

            stopWatch.Stop();
            Debug.Log("World saving complete! Took " + stopWatch.Elapsed + " seconds.");
        }
        isDirty = false;
    }

    public void MarkDirty() { if (SavingEnabled()) isDirty = true; }

    public GameObject GetAStarPlane() {
        return AStarPlane;
    }
}