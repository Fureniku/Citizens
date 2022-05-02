using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class AgentManager : GenerationSystem {
    
    [SerializeField] protected int initialAgentCount;
    [SerializeField] protected int currentAgentCount;
    [SerializeField] protected int maxAgentCount;
    [SerializeField] protected GameObject testAgent = null; //Change to using pedestrian registry later
    [SerializeField] protected GameObject aStarPlane = null;

    protected List<GameObject> agents = new List<GameObject>();

    [SerializeField] protected List<BaseAgent> repathQueue = new List<BaseAgent>();
    
    protected bool spawnAgentsCreated = false;
    private bool spawningAgentsValidated = false;

    public abstract IEnumerator GenAgents();
    protected abstract void AgentUpdate();

    protected int AllAgentsReady() {
        if (!spawningAgentsValidated && spawningAgentsValidated) {
            for (int i = 0; i < agents.Count; i++) {
                if (!agents[i].GetComponent<BaseAgent>().IsAgentReady()) {
                    return i;
                }
            }
            spawningAgentsValidated = true;
        }
        return -1;
    }

    private int pathCooldown = 0;
    
    void FixedUpdate() {
        currentAgentCount = transform.childCount;
        if (pathCooldown == 0) {
            SetPathForQueuedAgent();
        } else {
            pathCooldown--;
        }
    }

    void SetPathForQueuedAgent() {
        if (repathQueue.Count > 0) {
            BaseAgent agent = repathQueue[0];
            agent.GetAgent().destination = agent.GetCurrentDestination().transform.position;
            repathQueue.RemoveAt(0);
            pathCooldown = 10;
        }
    }

    public void AddToRepathQueue(BaseAgent agent) {
        if (repathQueue.Contains(agent)) {
            repathQueue.Remove(agent);
        }
        repathQueue.Add(agent);
    }

    protected void EnableAgents() {
        for (int i = 0; i < agents.Count; i++) {
            if (agents[i].GetComponent<BaseAgent>().IsAgentReady()) {
                agents[i].GetComponent<BaseAgent>().GetAgent().isStopped = false;
                agents[i].GetComponent<BaseAgent>().SetInitialized();
            }
        }
        SetComplete();
    }
    
    public override int GetGenerationPercentage() {
        float vehicleGen = (float)agents.Count / initialAgentCount;
        int avr = AllAgentsReady();
        float pathGen = avr > -1 ? (float) avr / agents.Count : 1.0f;
        return (int) (vehicleGen * 25) + (int) (pathGen * 75);
    }

    public override string GetGenerationString() {
        return message;
    }

    public List<GameObject> GetAllAgents() {
        return agents;
    }

    public void RemoveAgent(GameObject agent) {
        agents.Remove(agent);
        Destroy(agent);
    }
    
    protected GameObject ReplaceAgentWithCustom<T>(Vector3 spawnPos) where T : PedestrianAgent {
        GameObject newAgent = Instantiate(testAgent, spawnPos, Quaternion.identity);
        GameObject eyePos = newAgent.GetComponent<PedestrianAgent>().GetEyePos();
        GameObject camPos = newAgent.GetComponent<PedestrianAgent>().GetCamPos();
        GameObject head = newAgent.GetComponent<PedestrianAgent>().GetHead();
        GameObject legs = newAgent.GetComponent<PedestrianAgent>().GetLegs();
        Destroy(newAgent.GetComponent<PedestrianAgent>());
        newAgent.AddComponent<T>();
            
        newAgent.GetComponent<T>().SetEyePos(eyePos);
        newAgent.GetComponent<T>().SetCamPos(camPos);
        newAgent.GetComponent<T>().SetHead(head);
        newAgent.GetComponent<T>().SetLegs(legs);

        return newAgent;
    }
}