using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class ObservantMoveToLocationState : EggHunterBaseState {
        
        public ObservantMoveToLocationState(EggHunterAgent agent) {
            this.stateName = "Observant Moving To Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            EggHunterAgent otherAgent = SearchForOtherAgents();

            if (otherAgent != null) {
                if (otherAgent.GetStateMachine().CurrentState.GetType() == typeof(SearchLocationState)) {
                    agent.RemoveDestination(otherAgent.GetCurrentDestination());
                } else if (otherAgent.GetPreviousDestination() != null) {
                    agent.RemoveDestination(otherAgent.GetPreviousDestination());
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

        public EggHunterAgent SearchForOtherAgents() {
            return null;
        }
    }
}