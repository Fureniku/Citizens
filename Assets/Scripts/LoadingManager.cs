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

    protected LoadStateMachine stateMachine;

    [SerializeField] private GameObject tileRegistry = null;
    [SerializeField] private GameObject roadSeed = null;
    [SerializeField] private GameObject agentManager = null;
    [SerializeField] private GameObject roadNavMesh = null;
    [SerializeField] private GameObject sidewalkNavMesh = null;
    [SerializeField] private GameObject aStarGrid = null;
    
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

    void Awake() {
        sectionManager = GetComponent<SectionManager>();
        InitStateMachine();
    }

    protected void InitStateMachine() {
        stateMachine = GetComponent<LoadStateMachine>();
        Dictionary<Type, LoadBaseState> states = new Dictionary<Type, LoadBaseState>();

        AStar aStar = aStarGrid.GetComponent<AStar>();
        NavMeshSurface roadMesh = roadNavMesh.GetComponent<NavMeshSurface>();
        NavMeshSurface sidewalkMesh = sidewalkNavMesh.GetComponent<NavMeshSurface>();
        
        states.Add(typeof(InitializeLoadState), new InitializeLoadState(0, "Initialization", typeof(GenChunksLoadState), tileRegistry.GetComponent<TileRegistry>()));
        states.Add(typeof(GenChunksLoadState), new GenChunksLoadState(1, "Chunk Generation", typeof(GenRoadsLoadState)));
        states.Add(typeof(GenRoadsLoadState), new GenRoadsLoadState(2, "Road Generation", typeof(GenBuildingsLoadState), roadSeed.GetComponent<RoadSeed>()));
        states.Add(typeof(GenBuildingsLoadState), new GenBuildingsLoadState(3, "Gen Buildings", typeof(ComebineMeshLoadState))); //Unimplemented
        states.Add(typeof(ComebineMeshLoadState), new ComebineMeshLoadState(4, "Combine Meshes", typeof(GenNavMeshLoadState))); //Unimplemented
        states.Add(typeof(GenNavMeshLoadState), new GenNavMeshLoadState(5, "NavMesh Generation", typeof(PopulateRegistryLoadState), aStar, roadMesh, sidewalkMesh)); //Part implemented
        states.Add(typeof(PopulateRegistryLoadState), new PopulateRegistryLoadState(6, "Populate Registries", typeof(GenVehicleLoadState)));
        states.Add(typeof(GenVehicleLoadState), new GenVehicleLoadState(7, "Generate Vehicles", typeof(GenCiviliansLoadState), agentManager.GetComponent<AgentManager>()));
        states.Add(typeof(GenCiviliansLoadState), new GenCiviliansLoadState(8, "Generate Civilians", typeof(CompletedLoadState)));
        states.Add(typeof(CompletedLoadState), new CompletedLoadState(9, "Completed", typeof(CompletedLoadState), loadingCanvas));
        
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
}
