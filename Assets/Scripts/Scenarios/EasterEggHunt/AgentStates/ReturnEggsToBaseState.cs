using System;
using Scenarios.EasterEggHunt.Cooperative;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class ReturnEggsToBaseState : EggHunterBaseState {
        
        public ReturnEggsToBaseState(EggHunterAgent agent) {
            this.stateName = "Return Eggs To Base State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (Vector3.Distance(agent.transform.position, agent.GetScenarioManager().GetDepositPoint().transform.position) < 3.0f) {
                if (agent.EggCount() > 0) {
                    ((EggHunterScenarioManager) agent.GetScenarioManager()).DepositEggs(agent.EggCount());
                    agent.RemoveEggs(agent.EggCount());
                }
                
                if (((EggHunterCoopBase) agent.GetScenarioManager()).RemainingDestinations() > 0) {
                    ((EggHunterCoopBase) agent.GetScenarioManager()).ClaimClosestAvailableDestination(agent);
                }
                
                return typeof(MoveToLocationState);
            }

            return EnteredRoad();
        }
        
        public override Type StateEnter() {
            agent.ForceAgentDestination(agent.GetScenarioManager().GetDepositPoint());
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}