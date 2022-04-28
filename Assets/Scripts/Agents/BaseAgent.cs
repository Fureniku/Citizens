using System;
using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField, ReadOnly] protected int totalDestinations;
    [SerializeField] protected bool shouldStop = false;
    [SerializeField] protected GameObject finalPathedDestination;
    [SerializeField] protected List<GameObject> dests;
    
    [Header("Debug")]
    [SerializeField] private EnumDirection roadSide;
    [SerializeField] private string currentState;
    [SerializeField] private GameObject lastSeenObject;
    [SerializeField, ReadOnly] protected float objectDistance = 50;
    [SerializeField] private int seenDecay;
    [SerializeField] protected bool drawGizmos = false;

    protected AgentStateMachine stateMachine;
    protected Vector3 lookDirection;
    protected AStar aStar;
    protected bool initialized = false;
    
    protected LocationNodeController destinationController;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        dests = new List<GameObject>();
        objectDistance = visualRange;
        InitStateMachine();
        
        SetLookDirection(Vector3.forward, false);
    }

    public void SetAStar(AStar aStar) {
        this.aStar = aStar;
    }

    public bool ShouldStop() {
        return shouldStop;
    }

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

    void FixedUpdate() {
        roadSide = GetCurrentRoadSide();
        
        AgentUpdate();
        Navigate();

        //GetSeenObject();
        CheckForObjects();

        if (stateMachine.CurrentState != null) {
            currentState = stateMachine.CurrentState.GetName();
        }
    }

    public TileData GetCurrentTile() {
        TilePos pos = TilePos.GetTilePosFromLocation(agent.transform.position);
        if (TilePos.IsValid(pos)) {
            return World.Instance.GetChunkManager().GetTile(pos);
        }

        return null;
    }

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

    void Navigate() {
        totalDestinations = dests.Count;
        if (initialized) {
            AgentNavigate();
        }
    }
    
    protected void ReachedDestination(GameObject go) {
        float distance = Vector3.Distance(transform.position, currentDestGO.transform.position);
        if (distance < 1) {
            if (destinationController != null) {
                destinationController.ArriveAtDestination(this);
            }
        }
    }

    public void SetAgentDestination(GameObject dest) {
        currentDestGO = dest;
        agent.destination = dest.transform.position;
    }

    public GameObject GetNextDestination() {
        if (currentDest < dests.Count - 1) {
            return dests[currentDest + 1];
        }

        return null;
    }

    public Type GetStateType() { return stateMachine.CurrentState.GetType(); }
    public AgentBaseState GetState() { return stateMachine.CurrentState; }
    public GameObject GetCurrentDestinationObject() { return dests[currentDest]; }
    public bool IsAgentReady() { return initialized && GetComponent<NavMeshAgent>().hasPath; }
    public NavMeshAgent GetAgent() { return GetComponent<NavMeshAgent>(); }
    public GameObject GetEyePos() { return eyePos; }
    public GameObject GetCamPos() { return camPos; }
    public void SetEyePos(GameObject obj) { eyePos = obj; }
    public void SetCamPos(GameObject obj) { camPos = obj; }
    private void OnCollisionEnter(Collision collision) { AgentCollideEnter(collision); }
    private void OnTriggerEnter(Collider other) { AgentTriggerEnter(other); }
    protected bool NodeInRange(int node, int range) { return node > 0 && node < range; }
    public void PrintText(string str) { Debug.Log("[Agent] " + gameObject.name + ": " + str); }
    public GameObject GetCurrentDestination() { return currentDestGO; }
    public AgentStateMachine GetStateMachine() { return stateMachine; }
    public LocationNodeController GetDestinationController() { return destinationController; }

    public abstract void IncrementDestination();
    public abstract void Init();
    protected abstract void AgentUpdate();
    protected abstract void AgentNavigate();
    protected abstract void AgentCollideEnter(Collision collision);
    protected abstract void AgentCollideExit(Collision collision);
    protected abstract void AgentTriggerEnter(Collider other);
    protected abstract void AgentTriggerExit(Collider other);
    protected abstract void InitStateMachine();
    public abstract string GetAgentTypeName();
    public abstract string GetAgentTagMessage();
}
