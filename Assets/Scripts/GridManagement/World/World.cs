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
    
    [SerializeField] private string worldName = "world";
    [SerializeField] private bool save = true;
    [SerializeField] private GridManager gridManager = null;
    [ReadOnly, SerializeField] private EnumWorldState state = EnumWorldState.UNSTARTED;
    [ReadOnly, SerializeField] private bool existingWorld = false;

    [SerializeField] private GameObject navMeshRoad = null;
    [SerializeField] private GameObject navMeshSidewalk = null;
    
    [SerializeField] private GameObject worldDataObj = null;
    private WorldData worldData = null;

    [SerializeField] private GameObject testAgent = null;

    private ArrayList agents = new ArrayList();
    
    private bool isDirty = false;

    void Awake() {
        GC.Collect();
        Debug.Log("Initialize world");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
        gridManager.Initialize();
        state = EnumWorldState.GEN_CHUNKS;
        worldData = worldDataObj.GetComponent<WorldData>();

        worldData.SetWorldName(worldName);
        worldData.SetWorldSaving(save);
        worldData.SetWorldSize(gridManager.GetSize());
    }

    public void SetWorldExists() => existingWorld = true;
    public bool DoesWorldExist() { return existingWorld; }

    void Update() {
        if (state == EnumWorldState.GEN_NAVMESH) {
            Debug.Log("All road generators completed.");
            Debug.Log("Starting NavMesh generation.");
            navMeshRoad.GetComponent<NavMeshSurface>().BuildNavMesh();
            navMeshSidewalk.GetComponent<NavMeshSurface>().BuildNavMesh();
            DestinationRegistration.RoadRegistry.BuildList();
            Debug.Log("NavMesh generated");
            SetWorldState(EnumWorldState.GEN_AGENTS);
            if (!existingWorld) MarkDirty();
        }

        if (state == EnumWorldState.GEN_AGENTS) {
            Debug.Log("Given agent a target.");

            for (int i = 0; i < 50; i++) {
                Vector3 spawnPos = DestinationRegistration.RoadRegistry.GetAtRandom().GetWorldPos();
                agents.Add(Instantiate(testAgent, new Vector3(spawnPos.x - 7, spawnPos.y, spawnPos.z - 12), Quaternion.identity));
                ((GameObject) agents[i]).GetComponent<TestAgent>().MoveToLocation();
            }

            SetWorldState(EnumWorldState.COMPLETE);
        }

        if (state == EnumWorldState.COMPLETE && isDirty && save) {
            SaveWorld();
        }
        
#if UNITY_EDITOR
        //Assign data to the worlddata object so we can monitor without performance hit
        worldData.SetWorldState(state);
        worldData.SetChunkGenPercent(gridManager.GetGenerationPercentage());
#endif
    }
    
    public GridManager GetGridManager() {
        return gridManager;
    }

    public string GetWorldName() {
        return worldName;
    }

    public bool SavingEnabled() {
        return save;
    }
    
    void SaveWorld() {
        if (save) {
            Stopwatch stopWatch = Stopwatch.StartNew();
            Debug.Log("Starting world save...");
            for (int row = 0; row < gridManager.GetSize(); row++) {
                for (int col = 0; col < gridManager.GetSize(); col++) {
                    SaveLoadChunk.SerializeChunk(gridManager.GetChunk(row, col));
                }
            }
            stopWatch.Stop();
            Debug.Log("World saving complete! Took " + stopWatch.Elapsed + " seconds.");
            isDirty = false;
        }
    }

    public void MarkDirty() => isDirty = true;

    public EnumWorldState GetWorldState() {
        return state;
    }
    
    public void SetWorldState(EnumWorldState stateIn) {
        Debug.Log("Setting state to " + stateIn);
        if (stateIn == EnumWorldState.GEN_BUILDING) {
            Debug.Log("Building not implemented; skipping to navmesh");
            state = EnumWorldState.GEN_NAVMESH;
        } else {
            state = stateIn;
        }
    }
}

public enum EnumWorldState {
    UNSTARTED,
    GEN_CHUNKS,
    GEN_ROADS,
    GEN_BUILDING,
    GEN_NAVMESH,
    GEN_AGENTS,
    COMPLETE
}