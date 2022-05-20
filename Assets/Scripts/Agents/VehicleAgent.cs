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
    
    private Vehicle vehicle;
    private bool vacant = false;
    private bool isParked = false;
    
    [SerializeField] private GameObject testAgent;

    private float maxSpeed;

    public VehicleType GetVehicleType() {
        return vehicle.GetVehicleType();
    }

    public float GetMaxSpeed() {
        return maxSpeed;
    }

    public void InitIdle() {
        vehicle = GetComponent<Vehicle>();
        isParked = true;
    }

    public override void Init() {
        vehicle = GetComponent<Vehicle>();
        TileData destTile = World.Instance.GetChunkManager().GetTile(LocationRegistration.allVehicleDestinationsRegistry.GetAtRandom());
        GameObject finalDest = destTile.gameObject;
        GameObject startPoint = gameObject;

        if (spawnController != null) {
            dests.Add(spawnController.GetSpawnerNodeVehicle().GetStartPointNode().gameObject);
            startPoint = spawnController.gameObject;
            GameObject startTile = spawnController.gameObject;

            while (finalDest == startTile) {
                finalDest = World.Instance.GetChunkManager().GetTile(LocationRegistration.allVehicleDestinationsRegistry.GetAtRandom()).gameObject;
            }
        }
        
        destinationController = finalDest.GetComponent<LocationNodeController>();
        
        if (destinationController == null) { Debug.LogError("Vehicle pathing to " + finalDest.name + " which has no destination controller."); }
        
        finalDest = destinationController.GetDestinationNodeVehicle().gameObject;
        
        List<Node> aStarPath = aStar.RequestPath(startPoint, destinationController.GetNodeLocation().gameObject);

        if (aStarPath == null) {
            Debug.LogError("Failed to create aStar path between " + startPoint.gameObject + " and " + destinationController.gameObject + ". This vehicle will fail, removing.");
            agentManager.RemoveAgent(gameObject);
        }
        
        CreatePassengers();
        InitialRoadAlignment(aStarPath, startPoint.transform.position);
        InitializeStartJunctionController(aStarPath, startPoint);
        InitializePathJunctionControllers(aStarPath, startPoint.transform.position);
        InitializeFinalJunctionController(aStarPath);

        if (destinationController != null) {
            
            dests.Add(destinationController.GetDestinationNodeVehicle().gameObject);
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

    private void CreatePassengers() {
        int passengerAgents = Random.Range(1, vehicle.GetMaxSeats() + 1);

        for (int i = 0; i < passengerAgents; i++) {
            GameObject passengerAgent = Instantiate(testAgent, transform.position, Quaternion.identity);
            Seat seat = vehicle.GetNextAvailableSeat();
            passengerAgent.GetComponent<PedestrianAgent>().SetAStar(aStar);
            passengerAgent.GetComponent<PedestrianAgent>().EnterVehicle(this, seat);
            passengerAgent.GetComponent<NavMeshAgent>().enabled = false;
            seat.SetAgentOccupancy(passengerAgent);
        }
    }

    private void InitialRoadAlignment(List<Node> aStarPath, Vector3 startPos) {
        if (aStarPath.Count >= 1) { //Realign vehicle on the correct side of the road
            TileData tdStart = World.Instance.GetChunkManager().GetTile(TilePos.GetTilePosFromLocation(startPos));
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
    }

    private void InitializePathJunctionControllers(List<Node> aStarPath, Vector3 startPos) {
        for (int i = 0; i < aStarPath.Count; i++) {
            TileData td = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i].x, aStarPath[i].y));
            
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, aStarPath.Count+1)) entryTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i-1].x, aStarPath[i-1].y));
                if (NodeInRange(i + 1, aStarPath.Count)) exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[i+1].x, aStarPath[i+1].y));

                if (entryTd == null) {
                    if (NodeInRange(i, aStarPath.Count+1)) entryTd = World.Instance.GetChunkManager().GetTile(TilePos.GetTilePosFromLocation(startPos));
                }

                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetTilePos(), td.GetTilePos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetTilePos(), exitTd.GetTilePos()); //current to exit

                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    dests.Add(entryGo);
                    dests.Add(exitGo);
                } else {
                    if (entryTd == null) Debug.LogError(agent.name + "Junction was found, but entry was null\n" + "We checked if node was in range. " + (i-1) + " is greater than zero and less than " + (aStarPath.Count+1));
                }
            }
        }
    }

    private void InitializeStartJunctionController(List<Node> aStarPath, GameObject startPoint) {
        if (startPoint != gameObject && startPoint.GetComponent<VehicleJunctionController>() != null) {
            VehicleJunctionController startJC = startPoint.GetComponent<VehicleJunctionController>();
            
            if (startJC != null) {
                dests.Add(startJC.GetInNodeRaw(EnumLocalDirection.UP));
                TileData exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[0].x, aStarPath[0].y));

                EnumDirection exit = Direction.GetDirectionOffset(startJC.GetComponent<TileData>().GetTilePos(), exitTd.GetTilePos()); //current to exit
                GameObject exitGo = startJC.GetOutNode(exit);
                dests.Add(exitGo);
            }
        }
    }

    private void InitializeFinalJunctionController(List<Node> aStarPath) {
        VehicleJunctionController vjc = destinationController.GetComponent<VehicleJunctionController>();
        if (vjc != null) { //Add the final junction entry node before the destination, if the destination is on a junction.
            TileData entryTd  = World.Instance.GetChunkManager().GetTile(new TilePos(aStarPath[aStarPath.Count-2].x, aStarPath[aStarPath.Count-2].y));
            EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetTilePos(), TilePos.GetTilePosFromLocation(vjc.transform.position));
            dests.Add(vjc.GetInNode(entry));
            dests.Add(destinationController.GetComponent<VehicleJunctionController>().GetOutNodeRaw(EnumLocalDirection.UP));
        }
    }







    private bool reattempted = false;

    public void ValidatePath() {
        if (currentDest+1 >= dests.Count) {
            if (reattempted) {
                agent.destination = dests.Last().transform.position;
            } else {
                agent.path = paths.Last();
                reattempted = true;
            }
            
            return;
        }
        Vector3 currentDestination = dests[currentDest].transform.position;
        Vector3 nextDestination = dests[currentDest+1].transform.position;
        Vector3 agentPosition = agent.transform.position;

        float currentToNext = Vector3.Distance(currentDestination, nextDestination);
        float agentToNext = Vector3.Distance(agentPosition, nextDestination);

        if (agentToNext < currentToNext) {
            IncrementDestination();
        } else {
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
                LocationNode node = destinationController.GetDestinationNodeVehicle();
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

    public bool IsParked() {
        return isParked;
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

    public override void SetAgentManager() {
        agentManager = World.Instance.GetLoadingManager().GetVehicleAgentManager().GetComponent<VehicleAgentManager>();
    }

    public override string GetAgentTypeName() {
        return "Vehicle";
    }

    public override string GetAgentTagMessage() {
        return "";
    }

    private int stuckCooldown = 0;
    private int wrongSideCooldown = 0;
    protected override void AgentUpdate() {
        if (!isParked && IsStuck()) {
            if (stuckCooldown == 0) {
                ValidatePath();
                stuckCooldown = 60;
            } else {
                stuckCooldown--;
            }
        } else {
            stuckCooldown = 0;
        }

        if (!ValidateRoadSide()) {
            wrongSideCooldown++;

            if (wrongSideCooldown > 300) {
                PrintError("On the wrong side of the road!!!!!");
            }
        } else {
            wrongSideCooldown = 0;
        }

        if (GetLastSeenObject() != null) {
            if (GetLastSeenObject().GetComponent<VehicleAgent>() != null) {
                VehicleAgent otherVehicle = GetLastSeenObject().GetComponent<VehicleAgent>();
                if (otherVehicle.GetState() is DespawningState) {
                    ForceClearSeenObject();
                }
            }
        }
    }

    private bool ValidateRoadSide() {
        TileData currentTile = GetCurrentTile();
        agentDirection = GetRoughFacingDirection();
        if (currentTile != null) {
            if (currentTile.GetTile() == TileRegistry.STRAIGHT_ROAD) {
                switch (agentDirection) {
                    case EnumDirection.NORTH:
                        if (roadSide == EnumDirection.EAST) {
                            return false;
                        }
                        break;
                    case EnumDirection.EAST:
                        if (roadSide == EnumDirection.SOUTH) {
                            return false;
                        }
                        break;
                    case EnumDirection.SOUTH:
                        if (roadSide == EnumDirection.WEST) {
                            return false;
                        }
                        break;
                    case EnumDirection.WEST:
                        if (roadSide == EnumDirection.NORTH) {
                            return false;
                        }
                        break;
                }
            }
        }

        wrongSideCooldown = 0;
        return true;
    }

    public void SetParked() {
        isParked = true;
        agentManager.MoveAgentToIdle(agent.gameObject);
    }
    
    protected override void AgentNavigate() {}

    public override void IncrementDestination() {
        reattempted = false; //from pathfinding error, if we reached a destination the error is cleared.
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
    }
    
    protected override void ApproachedDestinationController() {
        if (destinationController != null && !approachingDestinationController) {
            destinationController.ApproachDestination(this);
            approachingDestinationController = true;
        }
    }

    protected override void ReachedDestinationController() {
        if (destinationController != null && !reachedDestinationController) {
            reachedDestinationController = true;
            destinationController.ArriveAtDestination(this);
        }
    }

    #region COLLISION_EVENTS

    protected override void AgentTriggerEnter(Collider other) {
        if (!isParked) {
            //Not gonna get this stuff working in time for submission :( 
            /*if (other.CompareTag("Vehicle")) {
                PrintWarn("Trigger enter into " + other.transform.gameObject.name);
                stateMachine.SwitchToState(typeof(CrashedState));
            }
        
            if (other.CompareTag("Pedestrian")) {
                PrintWarn("Hit a pedestrian! oh no hes dead. Trigger enter into " + other.transform.gameObject.name);
            }*/
        }
    }
    
    public override void OnArrival() {
        PrintError("Vehicle was asked what to do on arrival but has nothing set.");
    }
    
    protected override void AgentTriggerExit(Collider other) { }

    public bool isInJunctionBox = false;
    
    private void OnTriggerStay(Collider other) {
        isInJunctionBox = other.CompareTag("JunctionCheck");
    }

    #endregion
}