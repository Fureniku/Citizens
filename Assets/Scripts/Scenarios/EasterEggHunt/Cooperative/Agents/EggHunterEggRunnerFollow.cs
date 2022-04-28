using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;

namespace Scenarios.EasterEggHunt.Cooperative.Agents {
    public class EggHunterEggRunnerFollow : EggHunterAgent {

        private bool waitingForEggs = true;
        
        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(FollowSearcherState), new FollowSearcherState(this)); //Follow a target
            states.Add(typeof(TakeEggsToBaseState), new TakeEggsToBaseState(this)); //Return to base with eggs

            stateMachine.SetStates(states);
        }

        public override void Begin() {
            followTarget = scenarioManager.GetAgentManager().GetAllAgents()[hunterId-1];
        }

        public override void FinishedSearch() {
            throw new NotImplementedException();
        }

        public void NotWaitingForEggs() {
            waitingForEggs = false;
        }

        public bool WaitingForEggs() {
            return waitingForEggs;
        }

        public void SetWaitingForEggs() {
            waitingForEggs = true;
        }
        
        public override string GetAgentTypeName() {
            return "Egg Runner";
        }
    }
}