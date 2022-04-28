using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class MoveToLocationState : EggHunterBaseState {
        
        public MoveToLocationState(EggHunterAgent agent) {
            this.stateName = "Moving To Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
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