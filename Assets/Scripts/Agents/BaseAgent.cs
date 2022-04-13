using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAgent : MonoBehaviour {
    
    protected NavMeshAgent agent;

    [SerializeField] protected GameObject camPos = null;

    private float accelerationSave = 0; //Saved acceleration value used for delaying vehicle motion start.

    [SerializeField] protected int currentDest = 0;
    [SerializeField] protected GameObject currentDestGO;

    [SerializeField] protected bool destroyOnArrival = false;
    [SerializeField] protected bool shouldStop = false;
    [SerializeField] protected bool drawGizmos = false;
    [SerializeField] protected GameObject eyePos = null;
    [SerializeField] protected int visualRange = 50;
    [SerializeField, ReadOnly] protected float objectDistance = 50;

    [SerializeField] private string currentState;

    protected AgentStateMachine stateMachine;
    protected Vector3 lookDirection;
    protected AStar aStar;
    protected bool initialized = false;

    protected List<GameObject> dests;
    [SerializeField, ReadOnly] protected int totalDestinations;
    
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

    void FixedUpdate() {
        AgentUpdate();
        Navigate();

        //GetSeenObject();
        CheckForObjects();

        if (stateMachine.CurrentState != null) {
            currentState = stateMachine.CurrentState.GetName();
        }
    }

    public void SetLookDirection(Vector3 vec3, bool force) {
        if (lastSeenObject != null && !force) {
            SetLookDirection();
        }
        else {
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

    [SerializeField] private GameObject lastSeenObject;
    [SerializeField] private int seenDecay;
    
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

    public Type GetStateType() {
        return stateMachine.CurrentState.GetType();
    }

    public AgentBaseState GetState() {
        return stateMachine.CurrentState;
    }

    public GameObject GetCurrentDestinationObject() {
        return dests[currentDest];
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
    
    protected void ReachedDestination() {
        Debug.Log(name + " has reached their destination.");
        if (destroyOnArrival) {
            Destroy(gameObject);
        }
    }
    
    public bool IsAgentReady() {
        return initialized && GetComponent<NavMeshAgent>().hasPath;
    }

    public NavMeshAgent GetAgent() {
        return GetComponent<NavMeshAgent>();
    }

    public GameObject GetCamPos() {
        return camPos;
    }
    
    //Save the initiial acceleration value and temporarily set to zero to stop agent moving until we're ready.
    public void SaveAcceleration(float acc) {
        accelerationSave = acc;
        GetComponent<NavMeshAgent>().acceleration = 0;
    }
    
    public void RestoreAcceleration() => GetComponent<NavMeshAgent>().acceleration = accelerationSave;
    
    protected bool NodeInRange(int node, int range) {
        return node > 0 && node < range;
    }

    public void PrintText(string str) {
        Debug.Log("[Agent] " + gameObject.name + ": " + str);
    }

    public GameObject GetCurrentDestination() {
        return dests[currentDest];
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision!");
        AgentCollideEnter(collision);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger!");
        AgentTriggerEnter(other);
    }

    public abstract void IncrementDestination();
    public abstract void Init();
    protected abstract void AgentUpdate();
    protected abstract void AgentNavigate();
    protected abstract void AgentCollideEnter(Collision collision);
    protected abstract void AgentCollideExit(Collision collision);
    protected abstract void AgentTriggerEnter(Collider other);
    protected abstract void AgentTriggerExit(Collider other);
    protected abstract void InitStateMachine();
}
