using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class ReturnToBaseState : EggHunterBaseState {
        
        public ReturnToBaseState(EggHunterAgent agent) {
            this.stateName = "Return To Base State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (Vector3.Distance(agent.transform.position, agent.GetScenarioManager().GetDepositPoint().transform.position) < 25.0f ||
                Vector3.Distance(agent.transform.position, agent.GetSpawnPoint()) < 10.0f) {
                if (agent.EggCount() > 0) {
                    ((EggHunterScenarioManager) agent.GetScenarioManager()).DepositEggs(agent.EggCount());
                    agent.RemoveEggs(agent.EggCount());
                }
                
                return typeof(CompleteState);
            }

            return null;
        }
        
        public override Type StateEnter() {
            agent.GetAgent().destination = agent.GetSpawnPoint();
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}