using System;
using System.Collections;
using System.Collections.Generic;
using Loading.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {
    
    private SectionManager sectionManager = null;
    private MeshCombinerManager meshCombinerManager = null;
    private LoadStateMachine stateMachine;

    [SerializeField] private GameObject registries = null;
    [SerializeField] private GameObject roadSeed = null;
    [SerializeField] private GameObject vehicleAgentManager = null;
    [SerializeField] private GameObject pedestrianAgentManager = null;
    [SerializeField] private GameObject roadNavMesh = null;
    [SerializeField] private GameObject sidewalkNavMesh = null;
    [SerializeField] private GameObject aStarGrid = null;
    [SerializeField] private GameObject camera = null;
    
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
    
    public static List<AgentManager> scenarioVehicleAgentManagers = new List<AgentManager>();
    public static List<AgentManager> scenarioPedestrianAgentManagers = new List<AgentManager>();

    public void Initialize() {
        sectionManager = GetComponent<SectionManager>();
        meshCombinerManager = GetComponent<MeshCombinerManager>();
        InitStateMachine();
    }

    private void InitStateMachine() {
        stateMachine = GetComponent<LoadStateMachine>();
        Dictionary<Type, LoadBaseState> states = new Dictionary<Type, LoadBaseState>();

        AStar aStar = aStarGrid.GetComponent<AStar>();

        NavMeshSurface roadMesh = null;
        NavMeshSurface sidewalkMesh = null;
        
        if (roadNavMesh != null)  roadMesh = roadNavMesh.GetComponent<NavMeshSurface>();
        if (sidewalkNavMesh != null) sidewalkMesh = sidewalkNavMesh.GetComponent<NavMeshSurface>();

        bool skipNavMesh = roadMesh == null || sidewalkMesh == null;

        TileRegistry tileRegistryComponent = null;
        VehicleRegistry vehicleRegistryComponent = null;
        BrandRegistry brandRegistryComponent = null;
        
        RoadSeed roadSeedComponent = null;
        VehicleAgentManager vehicleAgentManagerComponent = null;
        PedestrianAgentManager pedestrianAgentManagerComponent = null;
        

        if (registries != null) {
            tileRegistryComponent = registries.GetComponent<TileRegistry>();
            vehicleRegistryComponent = registries.GetComponent<VehicleRegistry>();
            brandRegistryComponent = registries.GetComponent<BrandRegistry>();
        }
        
        if (roadSeed != null) roadSeedComponent = roadSeed.GetComponent<RoadSeed>();
        if (vehicleAgentManager != null) vehicleAgentManagerComponent = vehicleAgentManager.GetComponent<VehicleAgentManager>();
        if (pedestrianAgentManager != null) pedestrianAgentManagerComponent = pedestrianAgentManager.GetComponent<PedestrianAgentManager>();

        states.Add(typeof(InitializeLoadState), new InitializeLoadState(0, "Initialization", typeof(GenChunksLoadState), tileRegistryComponent));
        states.Add(typeof(GenChunksLoadState), new GenChunksLoadState(1, "Chunk Generation", typeof(GenRoadsLoadState), World.Instance.SkipChunkGen()));
        states.Add(typeof(GenRoadsLoadState), new GenRoadsLoadState(2, "Road Generation", typeof(GenBuildingsLoadState), roadSeedComponent, World.Instance.SkipRoadGen()));
        states.Add(typeof(GenBuildingsLoadState), new GenBuildingsLoadState(3, "Gen Buildings", typeof(ComebineMeshLoadState), sectionManager));
        states.Add(typeof(ComebineMeshLoadState), new ComebineMeshLoadState(4, "Combine Meshes", typeof(GenNavMeshLoadState), meshCombinerManager));
        states.Add(typeof(GenNavMeshLoadState), new GenNavMeshLoadState(5, "NavMesh Generation", typeof(PopulateRegistryLoadState), aStar, roadMesh, sidewalkMesh, skipNavMesh));
        states.Add(typeof(PopulateRegistryLoadState), new PopulateRegistryLoadState(6, "Populate Registries", typeof(GenVehicleLoadState), vehicleRegistryComponent, brandRegistryComponent));
        states.Add(typeof(GenVehicleLoadState), new GenVehicleLoadState(7, "Generate Vehicles", typeof(GenPedestriansLoadState), vehicleAgentManagerComponent, World.Instance.SkipVehicleGen()));
        states.Add(typeof(GenPedestriansLoadState), new GenPedestriansLoadState(8, "Generate Pedestrians", typeof(MoveCameraState), pedestrianAgentManagerComponent, World.Instance.SkipPedestrianGen()));
        states.Add(typeof(MoveCameraState), new MoveCameraState(9, "Moving Camera", typeof(CompletedLoadState), loadingCanvas, camera));
        states.Add(typeof(CompletedLoadState), new CompletedLoadState(9, "Completed", typeof(CompletedLoadState)));
        
        stateMachine.SetStates(states);
    }
    
    void Update() {
        SetOverallProgress();
        SetStageProgress();
    }

    private void SetStageProgress() {
        string message = "";
        int percent = 0;
        GenerationSystem sys = stateMachine.CurrentState.GetSystem();
        
        // System      InitializeLoadState
        // System      GenChunksLoadState
        // System      GenRoadsLoadState
        // NOT SYSTEM  GenBuildingsLoadState
        // NOT SYSTEM  ComebineMeshLoadState
        // NOT SYSTEM  GenNavMeshLoadState
        // NOT SYSTEM  PopulateRegistryLoadState
        // System      GenVehicleLoadState
        // NOT SYSTEM  GenCiviliansLoadState
        // N/A         CompletedLoadState

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
        
        string stateStr = stateMachine.CurrentState.GetProgressString();
        overallPercent = part * stateMachine.CurrentState.GetProgressId();

        if (overallBar != null) { overallBar.GetComponent<Image>().fillAmount = (float) overallPercent / 100.0f; }
        if (overallPercentText != null) { overallPercentText.GetComponent<Text>().text = stateMachine.GetStates().Count + "/11 (" + ((int)overallPercent) + "%)"; }
        if (overallText != null) { overallText.GetComponent<Text>().text = stateStr; }
    }
    
    public GameObject GetPedestrianAgentManager() { return pedestrianAgentManager; }
    public GameObject GetVehicleAgentManager() { return vehicleAgentManager; }
}
