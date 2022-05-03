using System;

namespace Scenarios.EasterEggHunt.AgentStates {
    public abstract class EggHunterBaseState : AgentBaseState {
    
        protected new EggHunterAgent agent;
        
        public EggHunterAgent SearchForOtherAgents() {
            return null;
        }
        
        protected Type EnteredRoad() {
            if (agent.HasEnteredRoad()) {
                agent.SetPreviousState(this.GetType());
                return typeof(WaitToCrossState);
            }

            return null;
        }
    }
}