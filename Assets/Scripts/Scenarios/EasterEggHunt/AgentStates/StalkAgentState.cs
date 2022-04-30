using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class StalkAgentState : EggHunterBaseState {
        
        public StalkAgentState(EggHunterAgent agent) {
            this.stateName = "Stalk State";
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

                if (agent.GetFollowTarget().GetComponent<EggHunterAgent>().GetStateMachine().CurrentState.GetType() == typeof(SearchLocationState)) {
                    agent.RemoveDestination(agent.GetFollowTarget().GetComponent<EggHunterAgent>().GetCurrentDestination());
                } else if (agent.GetFollowTarget().GetComponent<EggHunterAgent>().GetCurrentDestination() == agent.GetScenarioManager().GetDepositPoint()) {
                    agent.RemoveFollowTarget();
                    return typeof(StalkerMoveToLocationState);
                }
            } else {
                return typeof(StalkerMoveToLocationState);
            }

            return null;
        }
        
        public override Type StateEnter() {
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}