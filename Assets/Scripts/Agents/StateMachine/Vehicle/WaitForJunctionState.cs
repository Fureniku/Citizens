using System;
using UnityEngine;

public class WaitForJunctionState : VehicleBaseState {

    private static float approachDistance = 10;
    private float stopDistance = 3;

    private float maxSpeed;
    private float minSpeed = 2.0f;

    public WaitForJunctionState(VehicleAgent agent) {
        this.stateName = "Wait For Junction State";
        this.agent = agent;
        this.maxSpeed = agent.GetAgent().speed;
        this.waitableState = true;
    }

    public override Type StateUpdate() {
        if (agent.GetLastSeenObject() == null) {
            return typeof(ApproachJunctionState);
        }

        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetState().IsWaitableState()) {
                float dist = Vector3.Distance(agent.transform.position, agent.GetLastSeenObject().transform.position);
        
                float deltaDist = approachDistance - stopDistance;
                float currentDeltaDist = dist - stopDistance;

                float distanceModifier = currentDeltaDist / deltaDist;
                float deltaSpeed = maxSpeed - minSpeed;

                agent.SetSpeed(deltaSpeed * distanceModifier);
                if (dist < stopDistance) {
                    agent.GetAgent().isStopped = true;
                }
            }
            else {
                Debug.Log("Switch back to approach junction state");
                return typeof(ApproachJunctionState);
            }
        }
        
        
        return null;
    }

    public override Type StateEnter() {
        if (agent.GetLastSeenObject() != null) {
            agent.GetAgent().SetDestination(agent.GetLastSeenObject().transform.position);
        }
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        agent.GetAgent().SetDestination(agent.GetCurrentDestinationObject().transform.position);
        return null;
    }

    public static float GetApproachDist() {
        return approachDistance;
    }
}