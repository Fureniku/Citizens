using System;
using System.ComponentModel;
using Tiles.TileManagement;
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

            if (agent.GetRoadSide().Opposite() == seenAgent.GetRoadSide() || agent.GetRoughFacingDirection().Opposite() == seenAgent.GetRoughFacingDirection()) {
                return typeof(DriveState);
            }
            
            if (seenAgent.GetState() is WaitForVehicleState) {
                if (seenAgent.GetLastSeenAgent() == agent) {
                    float agentDist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
                    float otherAgentDist = Vector3.Distance(seenAgent.transform.position, seenAgent.GetCurrentDestination().transform.position);

                    if (agentDist < otherAgentDist) { //Both agents will call this code so only the closer one will move to drive state. Other will continue waiting.
                        return typeof(DriveState);
                    }
                }
            }

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
        
        float destDist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
        if (destDist < 1) {
            agent.IncrementDestination();
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