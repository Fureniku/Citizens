using System;
using UnityEngine;

public class ApproachZebraCrossingState : PedestrianBaseState {
    
    private GameObject crossingPoint = null;
    
    public ApproachZebraCrossingState(PedestrianAgent agent) {
        this.stateName = "Approach Zebra Crossing State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (crossingPoint != null) {
            float dist = Vector3.Distance(agent.transform.position, crossingPoint.transform.position);

            if (dist < 1) {
                return typeof(WaitZebraCrossingState);
            }
            return null;
        }

        return typeof(WalkState);
    }
    
    public override Type StateEnter() {
        TileData td = agent.GetCurrentTile();

        if (td.GetTile() == TileRegistry.ZEBRA_CROSSING_1x1) {
            CrossingController crossingController = td.GetComponent<CrossingController>();

            crossingPoint = crossingController.GetClosestCrossing(agent.transform.position);
            //agent.SetAgentDestination(crossingPoint);
        }
        else {
            return typeof(WalkState);
        }
        
        return null;
    }

    public override Type StateExit() {
        crossingPoint = null;
        agent.GetAgent().isStopped = true;
        agent.IncrementDestination();
        return null;
    }
}