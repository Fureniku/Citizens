using UnityEngine;

public class DespawnerNode : LocationNode {

    public override void ProcessNodeLogic(BaseAgent agent) {
        Destroy(agent.gameObject);
    }
    
    public override void PrepareNodeLogic(BaseAgent agent) {} //No need to prepare
}