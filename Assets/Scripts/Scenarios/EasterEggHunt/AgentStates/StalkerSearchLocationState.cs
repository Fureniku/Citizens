using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class StalkerSearchLocationState : EggHunterBaseState {
        
        public StalkerSearchLocationState(EggHunterAgent agent) {
            this.stateName = "Stalker Search Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.SearchTimer()) {
                agent.FinishedSearch();
                if (agent.ReturnToBase()) {
                    return typeof(ReturnToBaseState);
                }
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