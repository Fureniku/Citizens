using System;
using Scenarios.EasterEggHunt.Cooperative;
using Scenarios.EasterEggHunt.Cooperative.Agents;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class MoveToLocationState : EggHunterBaseState {
        
        public MoveToLocationState(EggHunterAgent agent) {
            this.stateName = "Moving To Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {

            if (agent.GetScenarioManager() is EggHunterCoopBase) {
                if (((EggHunterCoopBase) agent.GetScenarioManager()).RemainingDestinations() <= 0) {
                    return typeof(ReturnToBaseState);
                }
            }
            
            if (agent.EggCount() > 0) {
                return typeof(ReturnEggsToBaseState);
            }
            if (agent.GetCurrentDestination() == null) {
                return typeof(ReturnToBaseState);
            }
            if (Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position) < 1.5f) {
                if (agent is EggHunterCooperativePairSearch) {
                    return typeof(SearchLocationPairState);
                }
                return typeof(SearchLocationState);
            }

            if (agent.IsStuck()) {
                agent.ForceAgentDestination(agent.GetCurrentDestination());
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