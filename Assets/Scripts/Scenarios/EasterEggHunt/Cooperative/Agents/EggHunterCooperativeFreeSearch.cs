using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;

namespace Scenarios.EasterEggHunt {
    
    public class EggHunterCooperativeFreeSearch : EggHunterAgent {
        
        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(MoveToLocationState), new MoveToLocationState(this)); //Moving to next potential egg location
            states.Add(typeof(SearchLocationState), new SearchLocationState(this)); //Search an egg location
            states.Add(typeof(ReturnToBaseState), new ReturnToBaseState(this)); //Search an egg location
            states.Add(typeof(CompleteState), new CompleteState(this)); //Search an egg location

            stateMachine.SetStates(states);
        }

        public override void Begin() {
            ((EggHunterCooperativeScenarioManager) scenarioManager).ClaimNextDestination(this);
            begin = true;
        }

        public override void FinishedSearch() {
            if (((EggHunterCooperativeScenarioManager) scenarioManager).RemainingDestinations() > 0) {
                ((EggHunterCooperativeScenarioManager) scenarioManager).ClaimNextDestination(this);
            } else {
                SetAgentDestination(null);
            }
        }
        
        public override string GetAgentTypeName() {
            return "Egg Hunter";
        }
    }
}