using System;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class PedestrianAgent : BaseAgent {

    [SerializeField] private GameObject head;
    [SerializeField] private GameObject legs;
    [SerializeField] private VehicleAgent vehicle;

    public override void Init() {
        GenerateDestination();
        CalculateAllPaths();
        initialized = true;
    }

    private void GenerateDestination() {
        GameObject finalDest = World.Instance.GetChunkManager().GetTile(LocationRegistration.allPedestrianDestinationsRegistry.GetAtRandom()).gameObject;
        dests.Add(finalDest);

        if (GetCurrentTile().GetTile() == TileRegistry.STRAIGHT_ROAD_1x1) {
            float coinToss = Random.Range(0.0f, 1.0f);
            float offset = coinToss < 0.5f ? 3.5f : -3.5f;
            if (GetCurrentTile().GetRotation() == EnumDirection.NORTH || GetCurrentTile().GetRotation() == EnumDirection.SOUTH) {
                transform.position += new Vector3(offset, 0, 0);
            } else {
                transform.position += new Vector3(0, 0, offset);
            }
        }

        SetAgentDestination(finalDest);
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
        states.Add(typeof(ApproachZebraCrossingState), new ApproachZebraCrossingState(this)); //Approach a zebra crossing
        states.Add(typeof(WaitZebraCrossingState), new WaitZebraCrossingState(this)); //Wait at a zebra crossing
        states.Add(typeof(CrossingState), new CrossingState(this)); //Crossing the road (as to not switch back into an approach state)
        states.Add(typeof(VehiclePassengerState), new VehiclePassengerState(this)); //Agent is in a vehicle

        stateMachine.SetStates(states);
    }
    
    public override void SetAgentManager() {
        agentManager = World.Instance.GetLoadingManager().GetPedestrianAgentManager().GetComponent<PedestrianAgentManager>();
    }
    
    public new void SetLookDirection(Vector3 vec3, bool force) {
        if (GetLastSeenObject() != null && !force) {
            SetLookDirection();
        } else {
            head.transform.rotation = Quaternion.Euler(vec3);
        }
    }

    protected override void AgentUpdate() {
        Ray ray1 = new Ray(transform.position, Vector3.down);

        bool onSidewalk = false;

        if (Physics.Raycast(ray1, out RaycastHit hit1, 5.0f)) {
            if (hit1.collider.CompareTag("Sidewalk")) {
                onSidewalk = true;
            }
        }
        
        if (!onSidewalk) {
            Debug.LogWarning("Walking on something not sidewalky!!");
        }
    }
    
    protected override void AgentNavigate() {}
    
    public override void IncrementDestination() {
        if (currentDest < dests.Count-1) { //count isn't zero-based
            currentDest++;
            SetAgentDestination(dests[currentDest]);

            VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
            if (node != null) {
                shouldStop = node.GiveWay();
            }
        } else {
            ReachedDestinationController();
        }
    }
    
    protected override void ApproachedDestinationController() {
        if (destinationController != null && !approachingDestinationController) {
            destinationController.ApproachDestination(this);
            approachingDestinationController = true;
        }
    }

    protected override void ReachedDestinationController() {
        if (destinationController != null && !reachedDestinationController) {
            destinationController.ArriveAtDestination(this);
            reachedDestinationController = true;
        }
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
