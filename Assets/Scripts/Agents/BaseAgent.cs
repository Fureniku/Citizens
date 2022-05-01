using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles.TileManagement;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public abstract class BaseAgent : MonoBehaviour {
    
    protected NavMeshAgent agent;

    [Header("Prefab Settings")]
    [SerializeField] protected GameObject camPos = null;
    [SerializeField] protected GameObject eyePos = null;
    [SerializeField] protected int visualRange = 50;
    
    [Header("Destinations")]
    [SerializeField] protected int currentDest = 0;
    [SerializeField] protected GameObject currentDestGO;
    [SerializeField, ReadOnly] protected int totalDestinations;
    [SerializeField, ReadOnly] protected int initialFinalDestinationId;
    [SerializeField, ReadOnly] protected bool shouldStop = false;
    [SerializeField] protected LocationNodeController destinationController;
    
    [SerializeField] protected List<GameObject> dests;
    [SerializeField] protected List<NavMeshPath> paths = new List<NavMeshPath>();
    [SerializeField] protected bool approachingDestinationController = false;
    [SerializeField] protected bool reachedDestinationController = false;
    
    [Header("Debug")]
    [SerializeField] private EnumDirection roadSide;
    [SerializeField] private string currentState;
    [SerializeField] private GameObject lastSeenObject;
    [SerializeField, ReadOnly] protected float objectDistance = 50;
    [SerializeField] private int seenDecay;
    [SerializeField] protected bool drawGizmos = false;
    [SerializeField] private bool isStopped = false;
    [SerializeField] private bool pathPending = false;
    [SerializeField] private bool hasPath = false;
    [SerializeField] private NavMeshPathStatus pathStatus;
    [SerializeField] private Vector3 nextPosition;
    [SerializeField] private Vector3 pathEndPosition;
    [Space(20)]

    protected AgentStateMachine stateMachine;
    protected AgentManager agentManager;
    protected Vector3 lookDirection;
    protected AStar aStar;
    protected bool initialized = false;
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        dests = new List<GameObject>();
        objectDistance = visualRange;
        InitStateMachine();
        
        SetLookDirection(Vector3.forward, false);
    }
    
    void FixedUpdate() {
        AgentUpdate();
        if (initialized) { AgentNavigate(); }
        CheckForObjects();
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
                Debug.Log("Pathing failed at " + i + " - agent will not reach destination.");
            }
            paths.Add(path);
        }
        Debug.Log("Precalculated agent pathing with " + paths.Count + " paths.");
    }

    private NavMeshPath GetPathToNextPoint() {
        if (paths.Count < currentDest-1) {
            Debug.Log(paths.Count + " is greater than " + (currentDest - 1) + ": null!");
            return null;
        }
        Debug.Log("Next path: pos " + (currentDest-1) + " to " + currentDest + "(path " + (currentDest-1) + ") out of total paths: " + paths.Count);
        return paths[currentDest-1];
    }

    public void AddNewPathCalculation(GameObject newDestination) {
        NavMeshPath path = new NavMeshPath();
        bool success = NavMesh.CalculatePath(dests.Last().transform.position, newDestination.transform.position, NavMesh.AllAreas, path);
        if (!success) {
            Debug.LogWarning("Pathing failed at post-destination logic - agent will not reach post destination.");
        }
        else {
            Debug.LogWarning("Added new path from " + dests.Last().transform.position + " to " + newDestination.transform.position);
        }
        paths.Add(path);
    }
    //////////////////
    // A-Star stuff //
    //////////////////
    public void SetAStar(AStar aStarIn) { aStar = aStarIn; }
    protected bool NodeInRange(int node, int range) { return node > 0 && node < range; }
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
                lastSeenObject = hit.transform.gameObject;
            }
            Debug.DrawLine(eyePos.transform.position, hit.transform.position, Color.blue);
            seenDecay = 0;
        }
        else if (lastSeenObject != null) {
            if (seenDecay < 20) {
                seenDecay++;
                Debug.DrawLine(eyePos.transform.position, lastSeenObject.transform.position, Color.blue);
            }
            else {
                lastSeenObject = null;
                objectDistance = visualRange;
            }
        }
        //Debug.DrawLine(eyePos.transform.position, eyePos.transform.position + (eyePos.transform.rotation.eulerAngles * visualRange), Color.yellow);
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
    
    protected void ApproachedDestinationController() {
        if (destinationController != null && !approachingDestinationController) {
            destinationController.ApproachDestination(this);
            approachingDestinationController = true;
        }
    }

    protected void ReachedDestinationController() {
        if (destinationController != null && !reachedDestinationController) {
            destinationController.ArriveAtDestination(this);
            reachedDestinationController = true;
        }
    }

    protected void SetAgentDestination(GameObject dest) {
        if (dests.Count < 2) {
            Debug.LogError("Agent spawning too close to destination for any reasonable impact. Removing.");
            agentManager.RemoveAgent(gameObject);
        }
        
        currentDestGO = dest;
        agent.destination = dest.transform.position;
        if (dests.Contains(dest)) {
            NavMeshPath path = currentDest > 0 ? GetPathToNextPoint() : paths[0];
            if (path != null) {
                agent.SetPath(path);
            } else {
                Debug.Log("Path was null");
            }
        } else {
            Debug.Log("Destination was not in known list");
        }
    }

    //force a new destination
    public void ForceAgentDestination(GameObject dest) {
        currentDestGO = dest;
        agent.destination = dest.transform.position;
    }

    public GameObject GetNextDestination() {
        if (currentDest < dests.Count - 1) {
            return dests[currentDest + 1];
        }

        return null;
    }
    
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

    public EnumDirection GetRoadSide() {
        return roadSide;
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
    
    public bool IsAgentReady() { return initialized && GetComponent<NavMeshAgent>().hasPath; }
    public NavMeshAgent GetAgent() { return GetComponent<NavMeshAgent>(); }
    public GameObject GetEyePos() { return eyePos; }
    public GameObject GetCamPos() { return camPos; }
    public void SetEyePos(GameObject obj) { eyePos = obj; }
    public void SetCamPos(GameObject obj) { camPos = obj; }
    public AgentStateMachine GetStateMachine() { return stateMachine; }
    public void SetAgentManager(AgentManager am) { agentManager = am; }
    public AgentManager GetAgentManager() { return agentManager; }
    protected abstract void InitStateMachine();
    public abstract string GetAgentTypeName();
    public abstract string GetAgentTagMessage();
    #endregion
    
    #region EVENTS
    private void OnCollisionEnter(Collision collision) { AgentCollideEnter(collision); }
    private void OnTriggerEnter(Collider other) { AgentTriggerEnter(other); }
    protected abstract void AgentCollideEnter(Collision collision);
    protected abstract void AgentCollideExit(Collision collision);
    protected abstract void AgentTriggerEnter(Collider other);
    protected abstract void AgentTriggerExit(Collider other);
    #endregion
    
    
    public void PrintText(string str) { Debug.Log("[Agent] " + gameObject.name + ": " + str); }
    
    public abstract void Init();
    protected abstract void AgentUpdate();
    protected abstract void AgentNavigate();
    
    
}
