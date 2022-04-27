using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AgentManager : GenerationSystem {
    
    [SerializeField] protected int initialAgentCount;
    [SerializeField] protected int maxAgentCount;
    [SerializeField] protected GameObject testAgent = null; //Change to using pedestrian registry later
    [SerializeField] protected GameObject aStarPlane = null;

    protected List<GameObject> agents = new List<GameObject>();
    
    protected bool spawnAgentsCreated = false;
    private bool spawningAgentsValidated = false;

    public abstract IEnumerator GenAgents();

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

    protected void EnableAgents() {
        for (int i = 0; i < agents.Count; i++) {
            if (agents[i].GetComponent<BaseAgent>().IsAgentReady()) {
                agents[i].GetComponent<BaseAgent>().GetAgent().isStopped = false;
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
}