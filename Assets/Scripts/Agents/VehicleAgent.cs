using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class VehicleAgent : BaseAgent {

    [Header("Vehicle Appearance")]
    [SerializeField] private VehicleType vehicleType;
    [SerializeField] private VehicleColour vehicleColour;
    [SerializeField] private VehicleRegistry vehicleRegistry;
    [SerializeField] private GameObject[] colouredParts;
    private Vehicle vehicle;
    private bool vacant = false;
    private bool isParked = false;
    
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
        
        if (destinationController == null) {
            Debug.LogError("Vehicle pathing to " + finalDest.name + " which has no destination controller.");
        }
        finalDest = destinationController.GetDestinationNode().gameObject;

        Debug.Log("Requesting path between " + gameObject.name + " and " + finalDest.name);
        List<Node> aStarPath = aStar.RequestPath(gameObject, finalDest);
        Debug.Log("Initialized route overview with " + aStarPath.Count + " nodes.");
        
        int passengerAgents = Random.Range(1, vehicle.GetMaxSeats() + 1);

        for (int i = 0; i < passengerAgents; i++) {
            GameObject passengerAgent = Instantiate(testAgent, transform.position, Quaternion.identity);
            Seat seat = vehicle.GetNextAvailableSeat();
            passengerAgent.GetComponent<PedestrianAgent>().SetAStar(aStar);
            passengerAgent.GetComponent<PedestrianAgent>().EnterVehicle(this, seat);
            passengerAgent.GetComponent<NavMeshAgent>().enabled = false;
            seat.SetAgentOccupancy(passengerAgent);
        }

        if (aStarPath.Count >= 1) { //Realign vehicle on the correct side of the road
            TileData tdStart = World.Instance.GetChunkManager().GetTile(TilePos.GetTilePosFromLocation(gameObject.transform.position));
            TileData tdNext = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[0].x, aStarPath[0].y));

            EnumDirection startingDirection = Direction.GetDirectionOffset(tdStart.GetTilePos(), tdNext.GetTilePos());
            
            switch (startingDirection) {
                case EnumDirection.NORTH:
                    transform.position += new Vector3(1.65f, 0, 0);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case EnumDirection.EAST:
                    transform.position += new Vector3(0, 0, 1.65f);
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case EnumDirection.SOUTH:
                    transform.position += new Vector3(-1.65f, 0, 0);
                    break;
                case EnumDirection.WEST:
                    transform.position += new Vector3(0, 0, -1.65f);
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
            }
            agent.Warp(transform.position);
        } else {
            Debug.LogError("Vehicle " + gameObject.name + " spawned too close to its destination. This is very bad!.");
        }

        for (int i = 0; i < aStarPath.Count; i++) {
            TileData td = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i].x, aStarPath[i].y));
            if (i == aStarPath.Count - 2) {
                Debug.Log(agent.name + "Penultimate aStar position: " + td.GetName() + " @ " + td.GetTilePos());
            }

            if (i == aStarPath.Count - 1) {
                Debug.Log(agent.name + "Final aStar position: " + td.GetName() + " @ " + td.GetTilePos());
            }

            bool junctFound = false;
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                junctFound = true;
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, aStarPath.Count+1)) entryTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i-1].x, aStarPath[i-1].y));
                if (NodeInRange(i + 1, aStarPath.Count)) exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i+1].x, aStarPath[i+1].y));

                if (entryTd == null) {
                    if (NodeInRange(i, aStarPath.Count+1)) entryTd = World.Instance.GetChunkManager().GetTile(TilePos.GetTilePosFromLocation(gameObject.transform.position));
                    Debug.LogWarning(agent.name + ": Junction is very close to spawn point. Attempting current position instead of -1." + (entryTd == null ? " it's still null. " : " its not null!"));
                }

                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetTilePos(), td.GetTilePos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetTilePos(), exitTd.GetTilePos()); //current to exit

                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    dests.Add(entryGo);
                    dests.Add(exitGo);
                } else {
                    if (entryTd == null) {
                        Debug.LogError(agent.name + "Junction was found, but entry was null");
                        Debug.Log("We checked if node was in range. " + (i-1) + " is greater than zero and less than " + (aStarPath.Count+1));
                    }
                    if (exitTd == null) {
                        Debug.LogError(agent.name + "Junction was found, but exit was null");
                    }
                }
            }

            if (i == aStarPath.Count - 2 && !junctFound) {
                Debug.LogError(agent.name + "Penultimate aStar position: " + td.GetName() + " but was NOT detected to be a junction");
            }
        }

        VehicleJunctionController vjc = finalDest.GetComponent<VehicleJunctionController>();
        
        if (vjc != null) { //Add the final junction entry node before the destination, if the destination is on a junction.
            TileData entryTd  = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[aStarPath.Count-2].x, aStarPath[aStarPath.Count-2].y));
            EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetTilePos(), TilePos.GetTilePosFromLocation(finalDest.transform.position));
            dests.Add(vjc.GetInNode(entry));
        }

        if (destinationController != null) {
            dests.Add(destinationController.GetDestinationNode().gameObject);
        } else {
            dests.Add(finalDest);
        }

        initialFinalDestinationId = dests.Count;

        CalculateAllPaths();

        currentDest = -1;
        IncrementDestination();

        maxSpeed = agent.speed;

        SetLookDirection(Vector3.forward, false);
    }
    
    private void OnValidate() {
        if (colouredParts.Length > 0) {
            for (int i = 0; i < colouredParts.Length; i++) {
                colouredParts[i].GetComponent<MeshRenderer>().material = vehicleRegistry.GetMaterialNonStatic(vehicleColour);
            }
        }
    }

    public void ValidatePath() {
        if (currentDest+1 >= dests.Count) {
            Debug.Log("Agent is at end of navigation. Attempting to re-force final path.");
            agent.path = paths.Last();
            return;
        }
        Debug.Log("Attemping to validate path");
        Vector3 currentDestination = dests[currentDest].transform.position;
        Vector3 nextDestination = dests[currentDest+1].transform.position;
        Vector3 agentPosition = agent.transform.position;

        float currentToNext = Vector3.Distance(currentDestination, nextDestination);
        float agentToNext = Vector3.Distance(agentPosition, nextDestination);

        if (agentToNext < currentToNext) {
            Debug.LogWarning(agent.name + " Has skipped over destination without triggering. Skipping to next destination.");
            IncrementDestination();
        }
        else {
            Debug.LogWarning(agent.name + " has not skipped destination and is just stuck. Repathing current destination (or soon at least.)");
            agent.destination = dests[currentDest].transform.position;
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
                GameObject passengerDest = null;
                LocationNode node = destinationController.GetDestinationNode();
                if (node is ParkingEntranceNode) {
                    ParkingController parkingController = ((ParkingEntranceNode) node).GetParkingController();
                    passengerDest = parkingController.GetForwardingAgentDestination();
                }
                seat.ExitSeat(passengerDest);
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
        
        states.Add(typeof(SpawningState), new SpawningState(this)); //Vehicle is being spawned into the world
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

    public override string GetAgentTypeName() {
        return "Vehicle";
    }

    public override string GetAgentTagMessage() {
        return "";
    }

    private int stuckCooldown = 0;
    protected override void AgentUpdate() {
        if (!isParked && IsStuck()) {
            if (stuckCooldown == 0) {
                Debug.LogWarning(agent.name + "help me step-agent i'm stuck");
                ValidatePath();
                stuckCooldown = 60;
            } else {
                stuckCooldown--;
            }
        } else {
            stuckCooldown = 0;
        }
    }

    public void SetParked() {
        isParked = true;
    }
    
    protected override void AgentNavigate() {}

    public override void IncrementDestination() {
        Debug.Log("Incrementing. Current dest: " + currentDest + " , initial final: " + initialFinalDestinationId);
        if (currentDest == initialFinalDestinationId-1) { //current dest is zero based
            ReachedDestinationController();
        } else if (currentDest + 3 == initialFinalDestinationId) {
            ApproachedDestinationController();
        }
        
        if (currentDest < dests.Count - 1) { //count isn't zero-based
            currentDest++;
            SetAgentDestination(dests[currentDest]);

            VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
            if (node != null) {
                shouldStop = node.GiveWay();
            }
        }
        else {
            Debug.LogWarning("Incremention out of range. " + currentDest + " is lower than " + (dests.Count - 1));
        }
    }

    #region COLLISION_EVENTS
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
    #endregion
}

public enum VehicleType {
    CAR,
    VAN,
    TRUCK,
    BUS
}