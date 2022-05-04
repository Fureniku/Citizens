using System;
using UnityEngine;

public class SlowForTurnState : VehicleBaseState {

    private static float approachDistance = 10;
    private float stopDistance = 1;

    private float maxSpeed;
    private float minSpeed = 10.0f;

    public SlowForTurnState(VehicleAgent agent) {
        this.stateName = "Slow For Turn State";
        this.agent = agent;
        this.maxSpeed = agent.GetAgent().speed;
        this.waitableState = true;
    }

    public override Type StateUpdate() {
        Type waitingVehicle = CheckWaitingVehicle();
        Type obstruction = CheckObstruction();

        if (waitingVehicle != null) return waitingVehicle;
        if (obstruction != null) return obstruction;
        
        ScanAhead();
        
        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);

        if (dist < stopDistance+1) {
            return typeof(TurningState);
        }

        float deltaDist = approachDistance - stopDistance; //9
        float currentDeltaDist = dist - stopDistance;
        float distanceModifier = currentDeltaDist / deltaDist;

        float speedDelta = maxSpeed - minSpeed;

        agent.SetSpeed(speedDelta * distanceModifier + minSpeed);

        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        agent.IncrementDestination();
        return null;
    }

    public static float GetApproachDist() {
        return approachDistance;
    }
}