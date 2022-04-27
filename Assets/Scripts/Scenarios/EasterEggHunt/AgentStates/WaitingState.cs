using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class WaitingState : EggHunterBaseState {
        
        public WaitingState(EggHunterAgent agent) {
            this.stateName = "Waiting State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.CanBegin()) {
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