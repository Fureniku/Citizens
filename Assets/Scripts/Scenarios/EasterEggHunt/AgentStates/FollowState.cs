using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class FollowState : EggHunterBaseState {
        
        public FollowState(EggHunterAgent agent) {
            this.stateName = "Follow State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.GetFollowTarget() != null) {
                float dist = Vector3.Distance(agent.transform.position, agent.GetFollowTarget().transform.position);
                if (dist < agent.GetFollowDistance()) {
                    agent.GetAgent().isStopped = true;
                } else {
                    agent.GetAgent().isStopped = false;
                    agent.ForceAgentDestination(agent.GetFollowTarget());
                }
            } else {
                return typeof(MoveToLocationState);
            }

            return EnteredRoad();
        }
        
        public override Type StateEnter() {
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}