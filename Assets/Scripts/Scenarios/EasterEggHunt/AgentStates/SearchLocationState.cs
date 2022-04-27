using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class SearchLocationState : EggHunterBaseState {
        
        public SearchLocationState(EggHunterAgent agent) {
            this.stateName = "Search Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.SearchTimer()) {
                return typeof(MoveToLocationState);
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