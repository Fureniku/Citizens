
using System;
using UnityEngine;

public class TurningState : VehicleBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;

    public TurningState(VehicleAgent agent) {
        this.stateName = "Turning State";
        waitableState = true;
        this.agent = agent;
    }

    public override Type StateUpdate() {
        Type waitingVehicle = CheckWaitingVehicle();
        Type obstruction = CheckObstruction();

        if (waitingVehicle != null) return waitingVehicle;
        if (obstruction != null) return obstruction;
        
        
        VehicleJunctionNode prevNode = agent.GetPreviousDestination().GetComponent<VehicleJunctionNode>();
        if (prevNode != null) {
            VehicleJunctionController vjc = prevNode.GetController();

            if (vjc.TurningRight(prevNode, agent.GetCurrentDestination())) {
                ScanJunctionRight();
            } else {
                ScanJunctionLeft();
            }
        }


        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);

        if (dist < 1) {
            agent.IncrementDestination();
            return typeof(DriveState);
        }

        return null;
    }
    
    private void ScanJunctionRight() {
        ScanJunction(-0.075f, 2.5f);
    }
    
    private void ScanJunctionLeft() {
        ScanJunction(-2.5f, 0.075f);
    }
    
    private void ScanJunction(float min, float max) {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
        if (reverseDir) {
            lookOffset -= 0.1f;
        } else {
            lookOffset += 0.1f;
        }

        if ((lookOffset > max && !reverseDir) || (lookOffset < min && reverseDir)) {
            reverseDir = !reverseDir;
            lookOffset = 0;
        }
    }

    public override Type StateEnter() {
        if (agent.GetAgent().speed < 10.0f) {
            agent.SetSpeed(10.0f);
        }
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}
