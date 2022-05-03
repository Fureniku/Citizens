using UnityEngine;

public class DespawnerNode : LocationNode {

    public override void ProcessNodeLogic(BaseAgent agent) {
        agent.GetAgentManager().RemoveAgent(agent.gameObject);
    }
    
    public override void PrepareNodeLogic(BaseAgent agent) {} //No need to prepare
}