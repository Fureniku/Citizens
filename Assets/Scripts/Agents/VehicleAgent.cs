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

        if (aStarPath.Count >= 2) { //Realign vehicle on the correct side of the road
            TileData tdStart = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[0].x, aStarPath[0].y));
            TileData tdNext = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[1].x, aStarPath[1].y));

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
        } else {
            Debug.LogError("Vehicle " + gameObject.name + " spawned too close to its destination. Destroying.");
            Destroy(gameObject);
        }

        for (int i = 0; i < aStarPath.Count; i++) {
            TileData td = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i].x, aStarPath[i].y));
            if (i == aStarPath.Count - 2) {
                Debug.Log("Penultimate aStar position: " + td.GetName());
            }

            if (i == aStarPath.Count - 1) {
                Debug.Log("Final aStar position: " + td.GetName());
            }
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, aStarPath.Count)) entryTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i-1].x, aStarPath[i-1].y));
                if (NodeInRange(i + 1, aStarPath.Count)) exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i+1].x, aStarPath[i+1].y));

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

    protected override void AgentUpdate() {}
    protected override void AgentNavigate() {}

    public override void IncrementDestination() {
        Debug.Log("Incrementing destination");
        if (currentDest == initialFinalDestinationId) {
            Debug.Log("Reached destination controller");
            ReachedDestinationController();
        } else if (currentDest + 3 == initialFinalDestinationId) {
            Debug.Log("Approaching destination controller");
            ApproachedDestinationController();
        }
        
        if (currentDest < dests.Count - 1) { //count isn't zero-based
            Debug.Log("Actual destination incrementing stuff");
            currentDest++;
            SetAgentDestination(dests[currentDest]);

            VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
            if (node != null) {
                shouldStop = node.GiveWay();
            }
        }
        else {
            Debug.Log("Incremention out of range. " + currentDest + " is lower than " + (dests.Count - 1));
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