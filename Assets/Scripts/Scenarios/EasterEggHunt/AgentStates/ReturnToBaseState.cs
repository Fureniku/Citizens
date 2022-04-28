using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class ReturnToBaseState : EggHunterBaseState {
        
        public ReturnToBaseState(EggHunterAgent agent) {
            this.stateName = "Return To Base State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (Vector3.Distance(agent.transform.position, agent.GetScenarioManager().GetStartPoint().transform.position) < 5.0f) {
                return typeof(CompleteState);
            }

            return null;
        }
        
        public override Type StateEnter() {
            agent.SetAgentDestination(agent.GetScenarioManager().GetStartPoint());
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}