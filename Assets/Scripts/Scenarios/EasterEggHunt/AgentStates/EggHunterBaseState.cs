using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public abstract class EggHunterBaseState : AgentBaseState {
    
        protected new EggHunterAgent agent;
        
        private float lookOffset = 0f;
        private bool reverseDir = false;
        private int waitTime = 0;
        private float lookSpeed = 4.0f;
        private float maxLook = 80.0f;
        
        public EggHunterAgent SearchForOtherAgents() {
            Vector3 dir = new Vector3(0, lookOffset, 0);
            agent.SetLookDirection(dir, true);
            if (waitTime == 0) {
                if (reverseDir) {
                    lookOffset -= lookSpeed;
                } else {
                    lookOffset += lookSpeed;
                }
            } else {
                waitTime--;
            }

            if ((lookOffset > maxLook && !reverseDir) || (lookOffset < -maxLook && reverseDir)) {
                reverseDir = !reverseDir;
                waitTime = 30;
            }

            if (agent.GetSeenObject().transform != null) {
                GameObject go = agent.GetSeenObject().transform.gameObject;
                if (go != null && go.GetComponent<EggHunterAgent>() != null) {
                    return go.GetComponent<EggHunterAgent>();
                }
            }
            return null;
        }
        
        protected Type EnteredRoad() {
            if (agent.HasEnteredRoad()) {
                agent.SetPreviousState(this.GetType());
                return typeof(WaitToCrossState);
            }

            return null;
        }
    }
}