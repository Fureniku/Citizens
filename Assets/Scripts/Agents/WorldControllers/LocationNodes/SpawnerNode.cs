using UnityEngine;

public class SpawnerNode : LocationNode {
    
    [SerializeField] private StartPointNode startPointNode;

    public override void ProcessNodeLogic(BaseAgent agent) {}
    public override void PrepareNodeLogic(BaseAgent agent) {} //No need to prepare

    public GameObject GetStartPointNode() {
        if (startPointNode != null) {
            return startPointNode.gameObject;
        }
        return gameObject;
    }
}