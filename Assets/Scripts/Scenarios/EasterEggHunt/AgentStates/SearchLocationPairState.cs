using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class SearchLocationPairState : EggHunterBaseState {
        
        public SearchLocationPairState(EggHunterAgent agent) {
            this.stateName = "Search Location Pair State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.SearchTimer()) {
                return typeof(ApproachRunnerState);
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