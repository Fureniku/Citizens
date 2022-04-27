using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class MoveToLocationState : EggHunterBaseState {
        
        public MoveToLocationState(EggHunterAgent agent) {
            this.stateName = "Moving To Location State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
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