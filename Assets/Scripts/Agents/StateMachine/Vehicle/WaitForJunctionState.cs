using System;
using UnityEngine;

public class WaitForJunctionState : VehicleBaseState {

    private static float approachDistance = 10;
    private float stopDistance = 1;

    private float maxSpeed;
    private float minSpeed = 2.0f;

    private int time = 0;
    private int maxTime = 600;

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
            float dist = Vector3.Distance(agent.transform.position, agent.GetLastSeenObject().transform.position);
                
            if (seenAgent.GetState().IsWaitableState()) {
                float deltaDist = approachDistance - stopDistance;
                float currentDeltaDist = dist - stopDistance;

                float distanceModifier = currentDeltaDist / deltaDist;
                float deltaSpeed = maxSpeed - minSpeed;

                agent.SetSpeed(deltaSpeed * distanceModifier);
                if (dist < stopDistance) {
                    agent.GetAgent().isStopped = true;
                }
            } else {
                return typeof(ApproachJunctionState);
            }

            if (time == maxTime / 3) {
                //First attempts at clearing self-clipping agents.
                if (seenAgent.GetLastSeenAgent() == agent) {
                    float agentDist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
                    float otherAgentDist = Vector3.Distance(seenAgent.transform.position, seenAgent.GetCurrentDestination().transform.position);

                    if (agentDist < otherAgentDist) { //Both agents will call this code so only the closer one will move to drive state. Other will continue waiting.
                        return typeof(ApproachJunctionState);
                    }
                } else if (dist < 1.5f) { //must be clipping.
                    return typeof(ApproachJunctionState);
                }
            }
        }

        if (time > maxTime) {
            //Agents get stuck here sometimes due to how collisions work in navmesh. When forcing they'll probably clip through eachother.
            //Although most of the time, they're already clipping. So not the end of the world.
            //Just have to hope the observer/player isn't watching when that happens...
            //I don't like this but if we don't fix it we end up with huge traffic jams.
            agent.PrintWarn("Agent was possibly stuck in wait state. Forcing to approach.");
            agent.ForceClearSeenObject();
            return typeof(ApproachJunctionState);
        }

        time++;

        return null;
    }

    public override Type StateEnter() {
        time = 0;
        if (agent.GetLastSeenObject() != null) {
            agent.GetAgent().SetDestination(agent.GetLastSeenObject().transform.position);
        }
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        agent.GetAgent().SetDestination(agent.GetCurrentDestination().transform.position);
        return null;
    }

    public static float GetApproachDist() {
        return approachDistance;
    }
}