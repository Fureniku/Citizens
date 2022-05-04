using System;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class PedestrianAgent : BaseAgent {

    [SerializeField] private GameObject head;
    [SerializeField] private GameObject legs;
    [SerializeField] private VehicleAgent vehicle;

    //Crossing road detection things
    private Type previousState;
    private bool onRoad = true;
    private bool enteredRoad = false;

    public override void Init() {
        Debug.Log("Initializing...");
        GenerateDestination();
        CalculateAllPaths();
        initialized = true;
    }

    private void GenerateDestination() {
        GameObject finalDest = World.Instance.GetChunkManager().GetTile(LocationRegistration.allPedestrianDestinationsRegistry.GetAtRandom()).gameObject;
        destinationController = finalDest.GetComponent<LocationNodeController>();

        if (destinationController != null) {
            finalDest = destinationController.GetDestinationNodePedestrian().gameObject;
        }
        
        dests.Add(finalDest);
        SetAgentDestination(finalDest);
        
        if (GetCurrentTile().GetTile() == TileRegistry.STRAIGHT_ROAD_1x1) {
            float coinToss = Random.Range(0.0f, 1.0f);
            float offset = coinToss < 0.5f ? 3.5f : -3.5f;
            if (GetCurrentTile().GetRotation() == EnumDirection.NORTH || GetCurrentTile().GetRotation() == EnumDirection.SOUTH) {
                transform.position += new Vector3(offset, 0, 0);
            } else {
                transform.position += new Vector3(0, 0, offset);
            }
        }
    }

    public void EnterVehicle(VehicleAgent vehicleAgent, Seat seat) {
        vehicle = vehicleAgent;
        transform.parent = seat.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        legs.SetActive(false);
    }

    public void ExitVehicle(GameObject destination) {
        vehicle = null;
        agent.enabled = true;
        transform.parent = World.Instance.GetLoadingManager().GetPedestrianAgentManager().transform;
        legs.SetActive(true);
        if (destination != null) {
            if (destination.GetComponent<LocationNode>() != null) {
                destinationController = destination.GetComponent<LocationNode>().GetNodeController();
            }
            SetAgentDestination(destination);
        } else {
            GenerateDestination();
        }
    }

    public VehicleAgent GetVehicle() {
        return this.vehicle;
    }
    
    protected override void InitStateMachine() {
        stateMachine = GetComponent<AgentStateMachine>();
        Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
        states.Add(typeof(WalkState), new WalkState(this)); //Standard walking
        states.Add(typeof(WaitToCrossState), new WaitToCrossState(this)); //Wait to safely cross the road
        states.Add(typeof(CrossingState), new CrossingState(this)); //Crossing the road
        states.Add(typeof(VehiclePassengerState), new VehiclePassengerState(this)); //Agent is in a vehicle

        stateMachine.SetStates(states);
    }
    
    public override void SetAgentManager() {
        agentManager = World.Instance.GetLoadingManager().GetPedestrianAgentManager().GetComponent<PedestrianAgentManager>();
    }
    
    public new void SetLookDirection(Vector3 vec3, bool force) {
        head.transform.localRotation = Quaternion.Euler(vec3);
    }

    protected override void AgentUpdate() {
        Ray ray1 = new Ray(transform.position, Vector3.down);

        if (!onRoad) {
            if (IsOnRoad()) {
                onRoad = true;
                enteredRoad = true;
            }
        } else { //Only set to true for one frame
            enteredRoad = false;
            onRoad = IsOnRoad();
        }


        lookDirection = head.transform.rotation * Vector3.forward;
    }

    public bool HasEnteredRoad() {
        return enteredRoad;
    }


    public bool IsOnRoad() {
        Ray ray = new Ray(agent.nextPosition, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 5.0f)) {
            if (hit.collider.CompareTag("Road")) {
                return true;
            }
        }

        return false;
    }

    public void SetPreviousState(Type type) {
        previousState = type;
    }

    public Type GetPreviousState() {
        return previousState;
    }
    
    protected override void AgentNavigate() {}
    
    public override void IncrementDestination() {
        ReachedDestinationController();
    }
    
    protected override void ApproachedDestinationController() {}

    protected override void ReachedDestinationController() {
        if (destinationController != null && !reachedDestinationController) {
            destinationController.ArriveAtDestination(this);
            reachedDestinationController = true;
        }
    }

    public override void OnArrival() {
        dests.Clear();
        GameObject finalDest = World.Instance.GetChunkManager().GetTile(LocationRegistration.allPedestrianDestinationsRegistry.GetAtRandom()).gameObject;
        dests.Add(finalDest);
        destinationController = finalDest.GetComponent<LocationNodeController>();

        SetAgentDestination(finalDest);
    }

    protected override void AgentTriggerEnter(Collider other) {}
    protected override void AgentTriggerExit(Collider other) {}

    public GameObject GetHead() { return head; }
    public GameObject GetLegs() { return legs; }
    public void SetHead(GameObject obj) { head = obj; }
    public void SetLegs(GameObject obj) { legs = obj; }

    public override string GetAgentTypeName() { return ""; }
    public override string GetAgentTagMessage() { return ""; }

    public string GetFullName() {
        return GetComponent<AgentData>().GetFullName();
    }
}
