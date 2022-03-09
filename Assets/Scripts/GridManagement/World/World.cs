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
        worldData = WorldData.Instance;
        worldData.SetState(EnumWorldState.GEN_CHUNKS);

        worldData.SetNavMeshRoad(navMeshRoad);
        worldData.SetNavMeshSidewalk(navMeshSidewalk);
    }

    public void SetWorldExists() => worldData.SetWorldExists();

    public bool DoesWorldExist() {
        return worldData.DoesWorldExist();
    }

    void Update() {
        if (worldData.GetState() == EnumWorldState.GEN_NAVMESH) {
            Debug.Log("All road generators completed.");
            Debug.Log("Starting NavMesh generation.");
            worldData.GetNavMeshRoad().GetComponent<NavMeshSurface>().BuildNavMesh();
            worldData.GetNavMeshSidewalk().GetComponent<NavMeshSurface>().BuildNavMesh();
            DestinationRegistration.BuildLists();
            Debug.Log("NavMesh generated");
            SetWorldState(EnumWorldState.GEN_VEHICLES);
            if (!worldData.DoesWorldExist()) MarkDirty();
        }

        if (worldData.GetState() == EnumWorldState.GEN_VEHICLES) {
            Debug.Log("Given agent a target.");

            for (int i = 0; i < vehicleAgentCount; i++) {
                Vector3 spawnPos = DestinationRegistration.RoadSpawnerRegistry.GetAtRandom().GetWorldPos();
                agents.Add(Instantiate(testAgent, new Vector3(spawnPos.x - 7, spawnPos.y, spawnPos.z - 12),
                    Quaternion.identity));
                ((GameObject) agents[i]).GetComponent<TestAgent>().MoveToLocation();
            }

            SetWorldState(EnumWorldState.COMPLETE);
        }

        if (worldData.GetState() == EnumWorldState.COMPLETE && isDirty) {
            SaveWorld();
        }

#if UNITY_EDITOR

        worldData.SetChunkGenPercent(gridManager.GetGenerationPercentage());
#endif
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

    public EnumWorldState GetWorldState() {
        return worldData.GetState();
    }
    
    public void SetWorldState(EnumWorldState stateIn) {
        Debug.Log("Setting state to " + stateIn);
        if (stateIn == EnumWorldState.GEN_BUILDING) {
            Debug.Log("Building not implemented; skipping to navmesh");
            worldData.SetState(EnumWorldState.GEN_NAVMESH);
        } else {
            worldData.SetState(stateIn);
        }
    }

    public bool PassedState(EnumWorldState stateIn) {
        Debug.Log("Checking if " + stateIn + " (" + ((int) stateIn) + ") is higher than " + worldData.GetState());
        return stateIn <= worldData.GetState();
    }

    public void AdvanceWorldState() {
        switch (worldData.GetState()) {
            case EnumWorldState.UNSTARTED:
                worldData.SetState(EnumWorldState.INITIALIZED);
                break;
            case EnumWorldState.INITIALIZED:
                if (worldData.DoesWorldExist()) {
                    worldData.SetState(EnumWorldState.LOAD_WORLD);
                }
                else {
                    worldData.SetState(EnumWorldState.GEN_CHUNKS);
                }
                break;
            case EnumWorldState.GEN_CHUNKS:
                worldData.SetState(EnumWorldState.GEN_ROADS);
                break;
            case EnumWorldState.GEN_ROADS:
                worldData.SetState(EnumWorldState.GEN_BUILDING);
                break;
            case EnumWorldState.GEN_BUILDING:
                worldData.SetState(EnumWorldState.COMBINE_MESH);
                break;
            case EnumWorldState.LOAD_WORLD:
                worldData.SetState(EnumWorldState.COMBINE_MESH);
                break;
            case EnumWorldState.COMBINE_MESH:
                worldData.SetState(EnumWorldState.GEN_NAVMESH);
                break;
            case EnumWorldState.GEN_NAVMESH:
                worldData.SetState(EnumWorldState.POPULATE_REGISTRIES);
                break;
            case EnumWorldState.POPULATE_REGISTRIES:
                worldData.SetState(EnumWorldState.GEN_VEHICLES);
                break;
            case EnumWorldState.GEN_VEHICLES:
                worldData.SetState(EnumWorldState.GEN_CIVILIANS);
                break;
            case EnumWorldState.GEN_CIVILIANS:
                worldData.SetState(EnumWorldState.COMPLETE);
                break;
            default:
                worldData.SetState(EnumWorldState.COMPLETE);
                break;
        }
    }
}

public enum EnumWorldState {
    UNSTARTED,
    INITIALIZED,
    GEN_CHUNKS,
    GEN_ROADS,
    GEN_BUILDING,
    LOAD_WORLD,
    COMBINE_MESH,
    GEN_NAVMESH,
    POPULATE_REGISTRIES,
    GEN_VEHICLES,
    GEN_CIVILIANS,
    COMPLETE
}