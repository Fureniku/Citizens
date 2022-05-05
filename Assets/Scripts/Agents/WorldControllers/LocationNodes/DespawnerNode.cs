using UnityEngine;

public class DespawnerNode : LocationNode {

    public override void ProcessNodeLogic(BaseAgent agent) {
        Debug.Log("Despawning " + agent.name);
        agent.GetAgentManager().RemoveAgent(agent.gameObject);
    }
    
    public override void PrepareNodeLogic(BaseAgent agent) {} //No need to prepare
}