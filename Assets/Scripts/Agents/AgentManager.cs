using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentManager : GenerationSystem {
    
    [SerializeField] private GameObject vehicleParent;
    [SerializeField] private GameObject pedestrianParent;
    [Space(5)]
    [SerializeField] private int initialVehicleCount;
    [SerializeField] private int maxVehicleCount;
    [SerializeField] private GameObject testVehicle = null; //Change to using vehicle registry later
    [Space(5)]
    [SerializeField] private int initialPedestrianCount;
    [SerializeField] private int maxPedestrianCount;
    [SerializeField] private GameObject testPedestrian = null; //Change to using pedestrian registry later

    private bool spawnVehiclesCreated = false;
    private bool spawningVehiclesValidated = false;
    private float initialMaxSpeed = 0;

    private List<GameObject> agents = new List<GameObject>();

    void Start() {
        LoadingManager.genVehiclesStartEvent.AddListener(GenVehicles);
    }

    void Update() {
        if (spawnVehiclesCreated && World.Instance.GetWorldState() != EnumWorldState.COMPLETE) {
            int avr = AllVehiclesReady();
            if (avr == -1) {
                EnableVehicles();
                World.Instance.SetWorldState(EnumWorldState.COMPLETE);
            }
            else {
                message = "Plotting vehicle paths (" + avr + " of " + agents.Count + ")";
            }
        }
    }

    void GenVehicles() {
        Debug.Log("Starting vehicle generation");
        for (int i = 0; i < initialVehicleCount; i++) {
            Vector3 spawnPos = DestinationRegistration.RoadSpawnerRegistry.GetAtRandom().GetWorldPos();
            agents.Add(Instantiate(testVehicle, new Vector3(spawnPos.x - 7, spawnPos.y, spawnPos.z - 12), Quaternion.identity));
            agents[i].GetComponent<TestAgent>().Init();
            agents[i].GetComponent<TestAgent>().SaveAcceleration(agents[i].GetComponent<NavMeshAgent>().acceleration);
            agents[i].transform.parent = vehicleParent.transform;
            agents[i].name = "Vehicle Agent " + (i+1);
            message = "Created vehicle " + i + " of " + initialVehicleCount;
        }

        spawnVehiclesCreated = true;
    }

    int AllVehiclesReady() {
        if (!spawningVehiclesValidated && spawnVehiclesCreated) {
            for (int i = 0; i < agents.Count; i++) {
                if (!agents[i].GetComponent<TestAgent>().IsAgentReady()) {
                    return i;
                }
            }
            Debug.Log("all vehicles are ready.");
            spawningVehiclesValidated = true;
        }
        return -1;
    }

    void EnableVehicles() {
        for (int i = 0; i < agents.Count; i++) {
            if (agents[i].GetComponent<TestAgent>().IsAgentReady()) {
                agents[i].GetComponent<TestAgent>().RestoreAcceleration();
            }
        }
    }

    void GenPedestrians() {
        
    }
    
    /////////////////////////////////// Abstract inheritence stuff ///////////////////////////////////
    public override int GetGenerationPercentage() {
        float vehicleGen = (float)agents.Count / initialVehicleCount;
        int avr = AllVehiclesReady();
        float pathGen = avr > -1 ? (float) avr / agents.Count : 1.0f;
        return (int) (vehicleGen * 25) + (int) (pathGen * 75);
    }

    public override string GetGenerationString() {
        return message;
    }
}