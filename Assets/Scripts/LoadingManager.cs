using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {
    
    [SerializeField] private GameObject roadSeed = null;
    [SerializeField] private GameObject agentManager = null;
    [SerializeField] private GameObject roadNavMesh = null;
    [SerializeField] private GameObject sidewalkNavMesh = null;
    
    [Space(20)]
    
    [SerializeField] private GameObject loadingCanvas = null;
    [Space(4)]
    [SerializeField] private GameObject stageMessageText = null;
    [SerializeField] private GameObject stagePercentText = null;
    [SerializeField] private GameObject stageBar = null;
    [Space(4)]
    [SerializeField] private GameObject overallBar = null;
    [SerializeField] private GameObject overallText = null;
    [SerializeField] private GameObject overallPercentText = null;

    private double overallPercent;

    public static readonly UnityEvent initializedEvent = new UnityEvent();
    public static readonly UnityEvent genChunksStartEvent = new UnityEvent();
    public static readonly UnityEvent genRoadsStartEvent = new UnityEvent();
    public static readonly UnityEvent genBuildingsStartEvent = new UnityEvent();
    public static readonly UnityEvent loadWorldStartEvent = new UnityEvent();
    public static readonly UnityEvent combineMeshStartEvent = new UnityEvent();
    public static readonly UnityEvent genNavMeshStartEvent = new UnityEvent();
    public static readonly UnityEvent populateRegistryStartEvent = new UnityEvent();
    public static readonly UnityEvent genVehiclesStartEvent = new UnityEvent();
    public static readonly UnityEvent genCiviliansStartEvent = new UnityEvent();
    public static readonly UnityEvent completedLoadEvent = new UnityEvent();
    
    void Start() {
        initializedEvent.AddListener(OnInitialized);
        genChunksStartEvent.AddListener(OnGenChunks);
        genRoadsStartEvent.AddListener(OnGenRoads);
        genBuildingsStartEvent.AddListener(OnGenBuildings);
        loadWorldStartEvent.AddListener(OnLoadWorld);
        combineMeshStartEvent.AddListener(OnCombineMesh);
        genNavMeshStartEvent.AddListener(OnGenNavmesh);
        populateRegistryStartEvent.AddListener(OnPopulateRegistries);
        genVehiclesStartEvent.AddListener(OnGenVehicles);
        genCiviliansStartEvent.AddListener(OnGenCivilians);
        completedLoadEvent.AddListener(OnComplete);
    }

    void Update() {
        SetOverallProgress();
        SetStageProgress();
    }

    private void SetStageProgress() {
        string message = "";
        int percent = 0;
        GenerationSystem sys = null;
        switch (World.Instance.GetWorldState()) {
            case EnumWorldState.GEN_CHUNKS:
                sys = World.Instance.GetGridManager();
                break;
            case EnumWorldState.GEN_ROADS:
                sys = roadSeed.GetComponent<RoadSeed>();
                break;
            case EnumWorldState.GEN_BUILDING:
                break;
            case EnumWorldState.LOAD_WORLD:
                break;
            case EnumWorldState.COMBINE_MESH:
                break;
            case EnumWorldState.GEN_NAVMESH:
                break;
            case EnumWorldState.POPULATE_REGISTRIES:
                break;
            case EnumWorldState.GEN_VEHICLES:
                sys = agentManager.GetComponent<AgentManager>();
                break;
            case EnumWorldState.GEN_CIVILIANS:
                break;
        }

        if (sys != null) {
            message = sys.GetGenerationString();
            percent = sys.GetGenerationPercentage();
        }
        else {
            message = "Unknown system";
            percent = 0;
        }

        if (stageMessageText != null) { stageMessageText.GetComponent<Text>().text = message; }
        if (stagePercentText != null) { stagePercentText.GetComponent<Text>().text = percent + "%"; }
        if (stageBar != null) { stageBar.GetComponent<Image>().fillAmount = percent / 100.0f; }
    }

    private void SetOverallProgress() {
        double part = 100.0 / 10.0;
        string stateStr = "";
        switch (World.Instance.GetWorldState()) {
            case EnumWorldState.INITIALIZED:
                overallPercent = part * 0;
                stateStr = "Initializing World";
                break;
            case EnumWorldState.GEN_CHUNKS:
                overallPercent = part * 1;
                stateStr = "Generating Chunks";
                break;
            case EnumWorldState.GEN_ROADS:
                overallPercent = part * 2;
                stateStr = "Generating Roads";
                break;
            case EnumWorldState.GEN_BUILDING:
                overallPercent = part * 3;
                stateStr = "Generating Buildings";
                break;
            case EnumWorldState.LOAD_WORLD:
                overallPercent = part * 4;
                stateStr = "Loading World";
                break;
            case EnumWorldState.COMBINE_MESH:
                overallPercent = part * 5;
                stateStr = "Combining Meshes";
                break;
            case EnumWorldState.GEN_NAVMESH:
                overallPercent = part * 6;
                stateStr = "Generating NavMeshes";
                break;
            case EnumWorldState.POPULATE_REGISTRIES:
                overallPercent = part * 7;
                stateStr = "Populating Registries";
                break;
            case EnumWorldState.GEN_VEHICLES:
                overallPercent = part * 8;
                stateStr = "Generating Vehicle Agents";
                break;
            case EnumWorldState.GEN_CIVILIANS:
                overallPercent = part * 9;
                stateStr = "Generating Pedestrian Agents";
                break;
            case EnumWorldState.COMPLETE:
                overallPercent = part * 10;
                stateStr = "Load Complete";
                break;
        }

        if (overallBar != null) { overallBar.GetComponent<Image>().fillAmount = (float) overallPercent / 100.0f; }
        if (overallPercentText != null) { overallPercentText.GetComponent<Text>().text = ((int) World.Instance.GetWorldState()) + "/11 (" + ((int)overallPercent) + "%)"; }
        if (overallText != null) { overallText.GetComponent<Text>().text = stateStr; }
    }
    
#region EVENTS
    private void OnInitialized() {
        Debug.Log("###### Initiialized Event called ######");
    }
        
    private void OnGenChunks() {
        Debug.Log("###### Gen Chunks Event called ######");
    }
        
    private void OnGenRoads() {
        Debug.Log("###### Gen Roads Event called ######");
        if (roadSeed != null) {
            roadSeed.SetActive(true);
        }
        else {
            World.Instance.AdvanceWorldState();
        }
    }
        
    private void OnGenBuildings() {
        Debug.Log("###### Gen Buildings Event called ###### (SKIPPING!)");
        World.Instance.AdvanceWorldState();//TODO temporary
    }
        
    private void OnLoadWorld() {
        Debug.Log("###### Load World Event called ######");
    }
        
    private void OnCombineMesh() {
        Debug.Log("###### Combine Mesh Event called ###### (SKIPPING!)");
        World.Instance.AdvanceWorldState();//TODO temporary
    }
        
    private void OnGenNavmesh() {
        Debug.Log("###### Gen Navmesh Event called ######");
        if (roadNavMesh != null) {
            roadNavMesh.SetActive(true);
        }

        if (sidewalkNavMesh != null) {
            sidewalkNavMesh.SetActive(true);
        }

        if (roadNavMesh == null && sidewalkNavMesh == null) {
            World.Instance.AdvanceWorldState();
        }
    }
        
    private void OnPopulateRegistries() {
        Debug.Log("###### Populate Registries Event called ###### (SKIPPING)");
        World.Instance.AdvanceWorldState();//TODO temporary
    }
        
    private void OnGenVehicles() {
        Debug.Log("###### Gen Vehicles Event called ######");
        if (agentManager == null) {
            World.Instance.AdvanceWorldState();
        }
    }
        
    private void OnGenCivilians() {
        Debug.Log("###### Gen Civilians Event called ######");
        if (agentManager == null) {
            World.Instance.AdvanceWorldState();
        }
    }

    private void OnComplete() {
        if (loadingCanvas != null) {
            loadingCanvas.SetActive(false);
        }
        Debug.Log("###### Complete Load Event called ######");
    }
    

#endregion
}
