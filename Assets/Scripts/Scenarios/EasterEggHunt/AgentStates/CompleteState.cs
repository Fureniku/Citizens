using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class CompleteState : EggHunterBaseState {
        
        public CompleteState(EggHunterAgent agent) {
            this.stateName = "Complete State";
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