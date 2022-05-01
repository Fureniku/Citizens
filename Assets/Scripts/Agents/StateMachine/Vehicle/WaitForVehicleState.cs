using System;
using UnityEngine;

public class WaitForVehicleState : VehicleBaseState {

    private static float approachDistance = 20;
    private float stopDistance = 5;

    private float maxSpeed;
    private float minSpeed = 2.0f;

    public WaitForVehicleState(VehicleAgent agent) {
        this.stateName = "Wait For Vehicle State";
        this.agent = agent;
        this.maxSpeed = agent.GetAgent().speed;
        this.waitableState = true;
    }

    public override Type StateUpdate() {
        if (agent.GetLastSeenObject() == null) {
            return typeof(DriveState);
        }
        
        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetState().IsWaitableState()) {
                float dist = Vector3.Distance(agent.transform.position, agent.GetLastSeenObject().transform.position);
        
                float deltaDist = approachDistance - stopDistance;
                float currentDeltaDist = dist - stopDistance;

                float distanceModifier = currentDeltaDist / deltaDist;
                float deltaSpeed = maxSpeed - minSpeed;

                if (dist < stopDistance) {
                    agent.GetAgent().isStopped = true;
                } else {
                    agent.GetAgent().isStopped = false;
                }
                
                agent.SetSpeed(deltaSpeed * distanceModifier);
            }
            else {
                return typeof(DriveState);
            }
        }
        
        
        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }

    public static float GetApproachDist() {
        return approachDistance;
    }
}