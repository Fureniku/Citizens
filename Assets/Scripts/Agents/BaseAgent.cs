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
        aStar = World.Instance.GetAStarPlane().GetComponent<AStar>();
        objectDistance = visualRange;
        InitStateMachine();
        
        SetLookDirection(Vector3.forward);
    }

    void FixedUpdate() {
        AgentUpdate();
        Navigate();

        GetSeenObject();

        if (stateMachine.CurrentState != null) {
            currentState = stateMachine.CurrentState.GetName();
        }
    }

    public void SetLookDirection(Vector3 vec3) {
        Vector3 dir = transform.TransformDirection(vec3);
        if (eyePos != null) {
            dir = eyePos.transform.TransformDirection(vec3);
        }
        lookDirection = dir;
    }

    public RaycastHit GetSeenObject() {
        RaycastHit hit;
        if (Physics.Raycast(eyePos.transform.position, lookDirection, out hit, visualRange)) {
            objectDistance = hit.distance;
        }
        else {
            objectDistance = visualRange;
        }
        return hit;
    }

    public bool SeenAgent(RaycastHit hit) {
        if (hit.collider == null) {
            return false;
        }
        return hit.collider.CompareTag("Vehicle") || hit.collider.CompareTag("Pedestrian");
    }

    void Navigate() {
        totalDestinations = dests.Count;
        if (initialized) {
            AgentNavigate();
        }
    }
    
    protected void ReachedDestination() {
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

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision!");
        AgentCollideEnter(collision);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger!");
        AgentTriggerEnter(other);
    }

    protected abstract void IncrementDestination();
    public abstract void Init();
    protected abstract void AgentUpdate();
    protected abstract void AgentNavigate();
    protected abstract void AgentCollideEnter(Collision collision);
    protected abstract void AgentCollideExit(Collision collision);
    protected abstract void AgentTriggerEnter(Collider other);
    protected abstract void AgentTriggerExit(Collider other);
    protected abstract void InitStateMachine();
}
