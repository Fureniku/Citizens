using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class WaitingStalkerState : EggHunterBaseState {
        
        public WaitingStalkerState(EggHunterAgent agent) {
            this.stateName = "Waiting Stalker State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.CanBegin()) {
                return typeof(StalkAgentState);
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