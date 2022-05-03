using System;
using System.Collections.Generic;
using System.Linq;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAgent : MonoBehaviour {
    
    protected NavMeshAgent agent;

    [Header("Prefab Settings")]
    [SerializeField] protected GameObject camPos = null;
    [SerializeField] protected GameObject eyePos = null;
    [SerializeField] protected int visualRange = 50;
    
    [Header("Destinations")]
    [SerializeField] protected int currentDest = 0;
    [SerializeField] protected GameObject currentDestGO;
    [SerializeField] protected int totalDestinations;
    [SerializeField] protected int initialFinalDestinationId;
    [SerializeField] protected bool shouldStop = false;
    [SerializeField] protected LocationNodeController destinationController;
    [SerializeField] protected LocationNodeController spawnController;
    
    [SerializeField] protected List<GameObject> dests;
    [SerializeField] protected List<NavMeshPath> paths = new List<NavMeshPath>();
    [SerializeField] protected bool approachingDestinationController = false;
    [SerializeField] protected bool reachedDestinationController = false;
    
    [Header("Debug")]
    [SerializeField] protected EnumDirection roadSide;
    [SerializeField] protected EnumDirection agentDirection;
    [SerializeField] private string currentState;
    [SerializeField] private GameObject lastSeenObject;
    [SerializeField] protected float objectDistance = 50;
    [SerializeField] private int seenDecay;
    [SerializeField] protected bool drawGizmos = false;
    [SerializeField] private bool isStopped = false;
    [SerializeField] private bool pathPending = false;
    [SerializeField] private bool hasPath = false;
    [SerializeField] private NavMeshPathStatus pathStatus;
    [SerializeField] private Vector3 nextPosition;
    [SerializeField] private Vector3 pathEndPosition;
    [SerializeField] protected AgentManager agentManager;
    [Space(20)]

    protected AgentStateMachine stateMachine;
    protected AgentManager afgentManager;
    protected Vector3 lookDirection;
    protected AStar aStar;
    protected bool initialized = false;
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        dests = new List<GameObject>();
        objectDistance = visualRange;
        InitStateMachine();
        SetAgentManager();
        SetLookDirection(Vector3.forward, false);
    }

    public AgentManager GetAgentManager() {
        return agentManager;
    }

    public void SetSpawnController(LocationNodeController lnc) {
        spawnController = lnc;
    }

    public void SetInitialized() {
        initialized = true;
    }

    public bool IsInitialized() {
        return initialized;
    }
    
    void FixedUpdate() {
        if (initialized) {
            AgentUpdate();
            AgentNavigate();
            CheckForObjects();
        }
        
        UpdateAgentInformation();
    }

    void ResetAgent() {
        reachedDestinationController = false;
        approachingDestinationController = false;
        dests.Clear();
        paths.Clear();
        currentDestGO = null;
        currentDest = 0;
        initialFinalDestinationId = 0;
        shouldStop = false;
        destinationController = null;
    }
    
    #region PATHING
    ///////////////////
    // NavMesh stuff //
    ///////////////////
    protected void CalculateAllPaths() {
        for (int i = 1; i < dests.Count; i++) {
            NavMeshPath path = new NavMeshPath();
            bool success = NavMesh.CalculatePath(dests[i - 1].transform.position, dests[i].transform.position, NavMesh.AllAreas, path);
            if (!success) {
                Debug.LogError("Pathing failed at " + i + " - agent will not reach destination.");
            }
            paths.Add(path);
        }
    }

    private NavMeshPath GetPathToNextPoint() {
        if (paths.Count < currentDest-1) {
            return null;
        }
        return paths[currentDest-1];
    }

    public void AddNewPathCalculation(GameObject newDestination) {
        NavMeshPath path = new NavMeshPath();
        bool success = NavMesh.CalculatePath(dests.Last().transform.position, newDestination.transform.position, NavMesh.AllAreas, path);
        if (!success) {
            Debug.LogWarning("Pathing failed at post-destination logic - agent will not reach post destination.");
        }
        paths.Add(path);
    }

    public bool IsStuck() {
        return !agent.pathPending && !agent.hasPath && !agent.isStopped;
    }
    
    //////////////////
    // A-Star stuff //
    //////////////////
    public void SetAStar(AStar aStarIn) { aStar = aStarIn; }
    protected bool NodeInRange(int node, int range) { return node >= 0 && node < range; }
    #endregion
    
    #region LOOKAROUND
    public void SetLookDirection(Vector3 vec3, bool force) {
        if (lastSeenObject != null && !force) {
            SetLookDirection();
        } else {
            Vector3 dir = transform.TransformDirection(vec3);
            if (eyePos != null) {
                dir = eyePos.transform.TransformDirection(vec3);
            }
            lookDirection = dir;
        }
    }

    public void SetLookDirection() {
        if (lastSeenObject != null) {
            lookDirection = (lastSeenObject.transform.position - agent.transform.position).normalized;
        }
        else {
            lookDirection = Vector3.forward;
        }
    }
    
    public void CheckForObjects() {
        RaycastHit hit;
        if (Physics.Raycast(eyePos.transform.position, lookDirection, out hit, visualRange)) {
            if (SeenAgent(hit.transform.gameObject)) {
                objectDistance = hit.distance;
                if (hit.transform.gameObject != gameObject) { //Don't look at self.
                    lastSeenObject = hit.transform.gameObject;
                }
            }
            Debug.DrawLine(eyePos.transform.position, hit.transform.position, Color.blue);
            seenDecay = 0;
        } else if (lastSeenObject != null) {
            if (seenDecay < 20) {
                seenDecay++;
                objectDistance = Vector3.Distance(eyePos.transform.position, lastSeenObject.transform.position);
                Debug.DrawLine(eyePos.transform.position, lastSeenObject.transform.position, Color.blue);
            }
            else {
                lastSeenObject = null;
                objectDistance = visualRange;
            }
        } else {
            seenDecay = 0;
            objectDistance = visualRange;
        }
        Debug.DrawRay(eyePos.transform.position, lookDirection*5, Color.magenta, 0);
    }

    public GameObject GetLastSeenObject() {
        return lastSeenObject;
    }

    public BaseAgent GetLastSeenAgent() {
        if (lastSeenObject != null) {
            if (SeenAgent(lastSeenObject)) {
                return lastSeenObject.GetComponent<BaseAgent>();
            }
        }

        return null;
    }

    public RaycastHit GetSeenObject() {
        RaycastHit hit;
        if (Physics.Raycast(eyePos.transform.position, lookDirection, out hit, visualRange)) {
            objectDistance = hit.distance;
            Debug.DrawLine(eyePos.transform.position, hit.transform.position, Color.green);
        }
        else {
            objectDistance = visualRange;
        }
        return hit;
    }

    public void ForceClearSeenObject() {
        seenDecay = 0;
        lastSeenObject = null;
        objectDistance = visualRange;
        SetLookDirection();
    }

    public bool SeenAgent(GameObject target) {
        if (target == null) {
            return false;
        }
        
        return target.CompareTag("Vehicle") || target.CompareTag("Pedestrian");
    }
    #endregion
     
    #region DESTINATIONS
    public void AddNewDestination(GameObject newDestination) {
        AddNewPathCalculation(newDestination);
        dests.Add(newDestination);
    }

    protected abstract void ApproachedDestinationController();
    protected abstract void ReachedDestinationController();

    protected void SetAgentDestination(GameObject dest) {
        currentDestGO = dest;
        agent.destination = dest.transform.position;
        if (dests.Contains(dest) && paths.Count > 0) {
            NavMeshPath path = currentDest > 0 ? GetPathToNextPoint() : paths[0];
            if (path != null) {
                agent.SetPath(path);
            } else {
                Debug.Log("Path was null");
            }
        }
    }

    public GameObject GetFinalKnownDestination() {
        return dests.Last();
    }

    //force a new destination. Will not calculate destination path straight away, instead adds to the back of the queue for new paths.
    public void ForceAgentDestination(GameObject dest) {
        if (currentDestGO == dest) {
            return;
        }
        currentDestGO = dest;
        agentManager.AddToRepathQueue(this);
    }

    //Same as forcing but skips the queue. Mainly used for vehicles where they can be an obstruction to other agents.
    public void ForceAgentDestinationImmediete(GameObject dest) {
        currentDestGO = dest;
    }

    public GameObject GetNextDestination() { return currentDest < dests.Count - 1 ? dests[currentDest + 1] : null; }
    public GameObject GetPreviousDestination() { return currentDest > 0 ? dests[currentDest -1] : null; }
    public GameObject GetCurrentDestination() { return currentDestGO; }
    public LocationNodeController GetDestinationController() { return destinationController; }
    
    public abstract void IncrementDestination();
    #endregion

    #region INFORMATIVE
    private EnumDirection GetCurrentRoadSide() {
        TilePos currentPos = TilePos.GetTilePosFromLocation(transform.position);
        if (TilePos.IsValid(currentPos)) {
            TileData currentTile = World.Instance.GetChunkManager().GetTile(currentPos);
            Vector3 rawTilePos = currentTile.transform.position; //e.g. tile pos is 450, 300
            Vector3 rawAgentPos = transform.position; //e.g. agent is 250, 66

            if (currentTile.GetRotation() == EnumDirection.EAST || currentTile.GetRotation() == EnumDirection.WEST) {
                if (rawAgentPos.z < rawTilePos.z) {
                    return EnumDirection.SOUTH;
                }
                return EnumDirection.NORTH;
            }

            if (rawAgentPos.x < rawTilePos.x) {
                return EnumDirection.WEST;
            } 
            return EnumDirection.EAST;
        }

        return roadSide;
    }
    
    public EnumDirection GetRoughFacingDirection() {
        float rot = transform.rotation.eulerAngles.y;

        while (rot > 360) rot -= 360;
        while (rot < 0) rot += 360;

        if (rot <= 45 || rot >= 315) {
            return EnumDirection.NORTH;
        }

        if (45 <= rot && rot <= 135) {
            return EnumDirection.EAST;
        }
        
        if (135 <= rot && rot <= 225) {
            return EnumDirection.SOUTH;
        }
        
        if (225 <= rot && rot <= 315) {
            return EnumDirection.WEST;
        }

        //bad
        Debug.LogWarning("Failed to guestimate rotation");
        return EnumDirection.NORTH;
    }

    public EnumDirection GetRoadSide() {
        return roadSide;
    }

    public bool IsOppositeRoadSide(EnumDirection otherAgent) {
        return otherAgent.Opposite() == roadSide;
    }
    
    public TileData GetCurrentTile() {
        TilePos pos = TilePos.GetTilePosFromLocation(agent.transform.position);
        if (TilePos.IsValid(pos)) {
            return World.Instance.GetChunkManager().GetTile(pos);
        }
 
        return null;
    }
    #endregion
    
    #region AGENT_DATA
    private void UpdateAgentInformation() {
        if (stateMachine.CurrentState != null) {
            currentState = stateMachine.CurrentState.GetName();
        }
        roadSide = GetCurrentRoadSide();
        totalDestinations = dests.Count;
        pathPending = agent.pathPending;
        pathStatus = agent.pathStatus;
        hasPath = agent.hasPath;
        nextPosition = agent.nextPosition;
        pathEndPosition = agent.pathEndPosition;
    }
    
    public Type GetStateType() { return stateMachine.CurrentState.GetType(); }
    public AgentBaseState GetState() { return stateMachine.CurrentState; }
    
    public bool IsAgentReady() { return paths.Count > 0; }
    public NavMeshAgent GetAgent() { return GetComponent<NavMeshAgent>(); }
    public GameObject GetEyePos() { return eyePos; }
    public GameObject GetCamPos() { return camPos; }
    public void SetEyePos(GameObject obj) { eyePos = obj; }
    public void SetCamPos(GameObject obj) { camPos = obj; }
    public AgentStateMachine GetStateMachine() { return stateMachine; }
    protected abstract void InitStateMachine();
    public abstract void SetAgentManager();
    public abstract string GetAgentTypeName();
    public abstract string GetAgentTagMessage();
    #endregion
    
    #region EVENTS
    private void OnTriggerEnter(Collider other) { AgentTriggerEnter(other); }
    private void OnTriggerExit(Collider other) { AgentTriggerExit(other); }
    protected abstract void AgentTriggerEnter(Collider other);
    protected abstract void AgentTriggerExit(Collider other);
    #endregion
    
    
    public void PrintText(string str) { Debug.Log("[Agent] " + gameObject.name + ": " + str); }
    public void PrintWarn(string str) { Debug.LogWarning("[Agent] " + gameObject.name + ": " + str); }
    public void PrintError(string str) { Debug.LogError("[Agent] " + gameObject.name + ": " + str); }
    
    public abstract void Init();
    protected abstract void AgentUpdate();
    protected abstract void AgentNavigate();
    
    
}
