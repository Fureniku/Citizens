using System;
using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class VehicleAgent : BaseAgent {

    [Header("Vehicle Appearance")]
    [SerializeField] private VehicleType vehicleType;
    [SerializeField] private VehicleColour vehicleColour;
    [SerializeField] private VehicleRegistry vehicleRegistry;
    [SerializeField] private GameObject[] colouredParts;
    private Vehicle vehicle;
    private bool vacant = false;

    [SerializeField] private GameObject testAgent;

    private float maxSpeed;

    public VehicleType GetVehicleType() {
        return vehicleType;
    }

    public float GetMaxSpeed() {
        return maxSpeed;
    }

    public override void Init() {
        vehicle = GetComponent<Vehicle>();
        GameObject finalDest = World.Instance.GetChunkManager().GetTile(DestinationRegistration.hospitalRegistry.GetAtRandom()).gameObject;
        destinationController = finalDest.GetComponent<LocationNodeController>();
        
        if (destinationController != null) {
            finalDest = destinationController.GetDestinationNode();
        }
        
        Debug.Log("Initializing vehicle path to " + finalDest);
        finalPathedDestination = finalDest;
        
        List<Node> path = aStar.RequestPath(gameObject, finalDest);
        
        int agents = Random.Range(1, vehicle.GetMaxSeats() + 1);

        for (int i = 0; i < agents; i++) {
            GameObject passengerAgent = Instantiate(testAgent, transform.position, Quaternion.identity);
            Seat seat = vehicle.GetNextAvailableSeat();
            passengerAgent.GetComponent<PedestrianAgent>().SetAStar(aStar);
            passengerAgent.GetComponent<PedestrianAgent>().EnterVehicle(this, seat);
            passengerAgent.GetComponent<NavMeshAgent>().enabled = false;
            seat.SetAgentOccupancy(passengerAgent);
        }

        if (path.Count > 2) { //Realign vehicle on the correct side of the road
            TileData tdStart = World.Instance.GetChunkManager().GetTile(new TilePos(path[0].x, path[0].y));
            TileData tdNext = World.Instance.GetChunkManager().GetTile(new TilePos(path[1].x, path[1].y));

            EnumDirection startingDirection = Direction.GetDirectionOffset(tdStart.GetTilePos(), tdNext.GetTilePos());
            
            switch (startingDirection) {
                case EnumDirection.NORTH:
                    transform.position += new Vector3(2f, 0, 0);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case EnumDirection.EAST:
                    transform.position += new Vector3(0, 0, 2f);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case EnumDirection.SOUTH:
                    transform.position += new Vector3(-2f, 0, 0);
                    break;
                case EnumDirection.WEST:
                    transform.position += new Vector3(0, 0, -2f);
                    break;
            }
            agent.Warp(transform.position);
        }

        for (int i = 0; i < path.Count; i++) {
            TileData td = World.Instance.GetChunkManager().GetTile(new TilePos(path[i].x, path[i].y));
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, path.Count)) entryTd = World.Instance.GetChunkManager().GetTile(new TilePos(path[i-1].x, path[i-1].y));
                if (NodeInRange(i + 1, path.Count)) exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(path[i+1].x, path[i+1].y));

                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetTilePos(), td.GetTilePos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetTilePos(), exitTd.GetTilePos()); //current to exit

                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    dests.Add(entryGo);
                    dests.Add(exitGo);
                }
            }
        }

        VehicleJunctionController vjc = finalDest.GetComponent<VehicleJunctionController>();
        
        
        if (vjc != null) { //Add the final junction entry node before the destination, if the destination is on a junction.
            TileData entryTd  = World.Instance.GetChunkManager().GetTile(new TilePos(path[path.Count-2].x, path[path.Count-2].y));
            EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetTilePos(), TilePos.GetTilePosFromLocation(finalDest.transform.position));
            dests.Add(vjc.GetInNode(entry));
        }

        if (destinationController != null) {
            dests.Add(destinationController.GetDestinationNode());
        }
        else {
            dests.Add(finalDest);
        }

        SetAgentDestination(dests[0]);
        
        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
        }

        maxSpeed = agent.speed;

        initialized = true;
    }
    
    private void OnValidate() {
        if (colouredParts.Length > 0) {
            for (int i = 0; i < colouredParts.Length; i++) {
                colouredParts[i].GetComponent<MeshRenderer>().material = vehicleRegistry.GetMaterialNonStatic(vehicleColour);
            }
        }
    }

    public void SetSpeed(float speed) {
        if (speed < maxSpeed) {
            agent.speed = speed;
        }
        else {
            agent.speed = maxSpeed;
        }
    }

    public void EjectPassengers() {
        for (int i = 0; i < vehicle.GetMaxSeats(); i++) {
            Seat seat = vehicle.GetSeat(i);
            if (!seat.IsAvailable()) {
                seat.ExitSeat();
            }
        }

        vacant = true;
    }

    public bool IsVacant() {
        return vacant;
    }
    
    protected override void InitStateMachine() {
        stateMachine = GetComponent<AgentStateMachine>();
        Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
        states.Add(typeof(DriveState), new DriveState(this)); //Standard driving with observation in front
        states.Add(typeof(ObstructionSpottedState), new ObstructionSpottedState(this)); //Something ahead is blocking the vehicle
        states.Add(typeof(CrashedState), new CrashedState(this)); //Vehicle collided with something
        states.Add(typeof(JunctionExitWaitState), new JunctionExitWaitState(this)); //Waiting at a junction exit, checking if its safe to go
        states.Add(typeof(ApproachJunctionState), new ApproachJunctionState(this)); //Approaching a junction; slowing down
        states.Add(typeof(WaitForJunctionState), new WaitForJunctionState(this)); //Waiting behind another vehicle at a junction
        states.Add(typeof(WaitForVehicleState), new WaitForVehicleState(this)); //Waiting behind another vehicle in general
        states.Add(typeof(SlowForTurnState), new SlowForTurnState(this)); //Slow down when approaching a turn where we wouldn't have to completely stop
        states.Add(typeof(TurningState), new TurningState(this)); //In the process of turning in a junction (slower driving)
        states.Add(typeof(AccelerateState), new AccelerateState(this)); //Gradually increase vehicle speed (manual control is better than Unity's agent system)
        states.Add(typeof(ParkingState), new ParkingState(this)); //Approaching a parking space
        states.Add(typeof(ParkedState), new ParkedState(this)); //Vehicle has parked
        states.Add(typeof(DespawningState), new DespawningState(this)); //TERMINAL STATE: Approach a despawner for destruction
        
        stateMachine.SetStates(states);
    }

    protected override void AgentUpdate() {
        
    }

    protected override void AgentNavigate() {

    }

    public override void IncrementDestination() {
        if (currentDest < dests.Count-1) { //count isn't zero-based
            currentDest++;
            SetAgentDestination(dests[currentDest]);

            VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
            if (node != null) {
                shouldStop = node.GiveWay();
            }
        } else {
            ReachedDestination(currentDestGO);
        }
    }
    
    public override void SetAgentDestruction(GameObject dest) {
        currentDestGO = dest;
        agent.destination = dest.transform.position;
        stateMachine.ForceState(typeof(DespawningState));
    }

    public void SetAgentParking(GameObject parkingSpot) {
        currentDestGO = parkingSpot;
        agent.destination = parkingSpot.transform.position;
        stateMachine.ForceState(typeof(ParkingState));
    }

    protected override void AgentCollideEnter(Collision collision) {
        PrintText("Crashed into " + collision.collider.transform.gameObject.name);
        stateMachine.SwitchToState(typeof(CrashedState));
    }

    protected override void AgentCollideExit(Collision collision) { }

    protected override void AgentTriggerEnter(Collider other) {
        PrintText("Trigger enter into " + other.transform.gameObject.name);
        stateMachine.SwitchToState(typeof(CrashedState));
    }
    
    protected override void AgentTriggerExit(Collider other) { }

    /*private void OnDrawGizmos() {
        if (drawGizmos) {
            Gizmos.color = Color.red;
            if (eyePos != null) {
                Gizmos.DrawRay(eyePos.transform.position, lookDirection * objectDistance);
            }
            else {
                Gizmos.DrawRay(transform.position, lookDirection * objectDistance);
            }
        }
    }*/
}

public enum VehicleType {
    CAR,
    VAN,
    TRUCK,
    BUS
}