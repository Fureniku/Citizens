using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class StalkerMoveToLocationState : EggHunterBaseState {
        
        public StalkerMoveToLocationState(EggHunterAgent agent) {
            this.stateName = "Moving To Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            EggHunterAgent otherAgent = SearchForOtherAgents();

            if (otherAgent != null) {
                if (otherAgent.GetCurrentDestination() != agent.GetScenarioManager().GetDepositPoint()) {
                    agent.SetFollowTarget(otherAgent.gameObject);
                    return typeof(StalkAgentState);
                }
            }

            if (Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position) < 1.5f) {
                return typeof(SearchLocationState);
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