using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;

namespace Scenarios.EasterEggHunt.Cooperative.Agents {
    
    public class EggHunterCooperativeFreeSearch : EggHunterAgent {
        
        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(MoveToLocationState), new MoveToLocationState(this)); //Moving to next potential egg location
            states.Add(typeof(SearchLocationState), new SearchLocationState(this)); //Search an egg location
            states.Add(typeof(ReturnEggsToBaseState), new ReturnEggsToBaseState(this)); //Search an egg location
            states.Add(typeof(ReturnToBaseState), new ReturnToBaseState(this)); //Search an egg location
            states.Add(typeof(CompleteState), new CompleteState(this)); //Search an egg location
            states.Add(typeof(WaitToCrossState), new WaitToCrossState(this)); //Wait to safely cross the road
            states.Add(typeof(CrossingState), new CrossingState(this)); //Crossing the road

            stateMachine.SetStates(states);
        }

        public override void Begin() {
            ((EggHunterCoopBase) scenarioManager).ClaimNextDestination(this);
            begin = true;
        }

        public override void FinishedSearch() {
            CheckForEggs();
            if (((EggHunterCoopBase) scenarioManager).RemainingDestinations() > 0) {
                ((EggHunterCoopBase) scenarioManager).ClaimNextDestination(this);
            } else {
                stateMachine.ForceState(typeof(ReturnToBaseState));
            }
        }
        
        public override string GetAgentTypeName() {
            return "Egg Hunter";
        }
    }
}