using System;
using UnityEngine;

public class ObstructionSpottedState : AgentBaseState {
    
    private float approachDistance = 25;
    private float stopDistance = 3;
    
    private float maxSpeed;
    private float minSpeed = 2.0f;
    
    public ObstructionSpottedState(BaseAgent agent) {
        this.stateName = "Obstruction Spotted State";
        this.agent = agent;
        this.maxSpeed = agent.GetAgent().speed;
    }
    
    public override Type StateUpdate() {
        if (agent.GetLastSeenAgent() != null) {
            if (agent.GetLastSeenAgent() is VehicleAgent) {
                VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

                if (seenAgent.GetStateType() == typeof(ApproachJunctionState) || seenAgent.GetStateType() == typeof(JunctionExitWaitState) || seenAgent.GetStateType() == typeof(WaitForJunctionState)) {
                    return typeof(WaitForJunctionState);
                }
            
                if (seenAgent.GetState() is WaitForVehicleState || seenAgent.GetState() is ObstructionSpottedState) {
                    if (seenAgent.GetLastSeenAgent() == agent) {
                        float agentDist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
                        float otherAgentDist = Vector3.Distance(seenAgent.transform.position, seenAgent.GetCurrentDestination().transform.position);

                        if (agentDist < otherAgentDist) { //Both agents will call this code so only the closer one will move to drive state. Other will continue waiting.
                            return typeof(DriveState);
                        }
                    }
                }
            } else if (agent.GetLastSeenAgent() is PedestrianAgent) {
                PedestrianAgent seenAgent = (PedestrianAgent) agent.GetLastSeenAgent();
                if (!(agent.GetState() is CrossingState)) {
                    return typeof(DriveState);
                }
            }
        }
        
        float distance = agent.GetSeenObject().distance;
        
        float deltaDist = approachDistance - stopDistance; //13
        float currentDeltaDist = distance - stopDistance; //7

        float distanceModifier = currentDeltaDist / deltaDist;
        float deltaSpeed = maxSpeed - minSpeed;

        
        if (distance < approachDistance && distance > 0) {
            if (distance < stopDistance) {
                agent.GetAgent().isStopped = true;
            }
            else {
                agent.GetAgent().isStopped = false;
                agent.GetAgent().speed = deltaSpeed * distanceModifier;
            }
            ScanAhead();
        }

        if (agent.GetLastSeenObject() == null) {
            return typeof(DriveState);
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
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    
    private void ScanAhead() {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
        if (reverseDir) {
            lookOffset -= 0.015f;
        } else {
            lookOffset += 0.015f;
        }

        if ((lookOffset > 0.075f && !reverseDir) || (lookOffset < -0.075f && reverseDir)) {
            reverseDir = !reverseDir;
        }
    }
}