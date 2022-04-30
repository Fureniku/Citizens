using System;
using UnityEngine;

public class ApproachJunctionState : VehicleBaseState {

    private static float approachDistance = 10;
    private float stopDistance = 1;

    private float maxSpeed;
    private float minSpeed = 2.0f;

    public ApproachJunctionState(VehicleAgent agent) {
        this.stateName = "Approach Junction State";
        this.agent = agent;
        this.maxSpeed = agent.GetAgent().speed;
        this.waitableState = true;
    }

    public override Type StateUpdate() {
        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetState().IsWaitableState()) {
                return typeof(WaitForJunctionState);
            }
        }
        
        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);

        //Approach distance: the point to start braking
        //Stop distance: the point where you should stop completely
        //Example figures:   15                2
        float deltaDist = approachDistance - stopDistance; //13
        float currentDeltaDist = dist - stopDistance; //7

        float distanceModifier = currentDeltaDist / deltaDist;
        
        //Max speed: The speed the vehicle is moving at when it starts braking
        //Min speed: The speed the vehicle is moving at when "stopped"
        //Example figures:   20         2
        float deltaSpeed = maxSpeed - minSpeed; //18
        
        //get percentage of delta dist for current delta dist
        //modify delta speed by that percentage
        //???
        //profit!

        agent.SetSpeed(deltaSpeed * distanceModifier);

        if (dist < stopDistance+1.0f) {
            agent.SetSpeed(5.0f);
            return typeof(JunctionExitWaitState);
        }

        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }

    public static float GetApproachDist() {
        return approachDistance;
    }
}