using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingManager : MonoBehaviour {
    

    [SerializeField] private GameObject roadSeed = null;
    [SerializeField] private GameObject roadNavMesh = null;
    [SerializeField] private GameObject sidewalkNavMesh = null;

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

    private void OnInitialized() {
        Debug.Log("###### Initiialized Event called ######");
    }
    
    private void OnGenChunks() {
        Debug.Log("###### Gen Chunks Event called ######");
    }
    
    private void OnGenRoads() {
        Debug.Log("###### Gen Roads Event called ######");
        roadSeed.SetActive(true);
    }
    
    private void OnGenBuildings() {
        Debug.Log("###### Gen Buildings Event called ######");
    }
    
    private void OnLoadWorld() {
        Debug.Log("###### Load World Event called ######");
    }
    
    private void OnCombineMesh() {
        Debug.Log("###### Combine Mesh Event called ######");
    }
    
    private void OnGenNavmesh() {
        Debug.Log("###### Gen Navmesh Event called ######");
        roadNavMesh.SetActive(true);
        sidewalkNavMesh.SetActive(true);
    }
    
    private void OnPopulateRegistries() {
        Debug.Log("###### Populate Registries Event called ######");
    }
    
    private void OnGenVehicles() {
        Debug.Log("###### Gen Vehicles Event called ######");
    }
    
    private void OnGenCivilians() {
        Debug.Log("###### Gen Civilians Event called ######");
    }

    private void OnComplete() {
        Debug.Log("###### Complete Load Event called ######");
    }
}
