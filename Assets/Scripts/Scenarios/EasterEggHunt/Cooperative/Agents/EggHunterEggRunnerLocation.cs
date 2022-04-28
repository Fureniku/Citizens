using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;

namespace Scenarios.EasterEggHunt.Cooperative.Agents {
    public class EggHunterEggRunnerLocation : EggHunterAgent {

        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(MoveToLocationState), new MoveToLocationState(this)); //Moving to next potential egg location
            states.Add(typeof(SearchLocationState), new SearchLocationState(this)); //Search an egg location
            
            
            
            states.Add(typeof(FollowState), new FollowState(this)); //Follow a target

            stateMachine.SetStates(states);
        }

        public override void Begin() {
            followTarget = scenarioManager.GetAgentManager().GetAllAgents()[hunterId-1];
            //TODO set wait location
        }

        public override void FinishedSearch() {
            throw new NotImplementedException();
        }
        
        public override string GetAgentTypeName() {
            return "Egg Runner";
        }
    }
}