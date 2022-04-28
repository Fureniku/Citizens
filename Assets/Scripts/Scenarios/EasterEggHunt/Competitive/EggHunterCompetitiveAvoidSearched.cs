using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;

namespace Scenarios.EasterEggHunt.Competitive {
    public class EggHunterCompetitiveAvoidSearched : EggHunterAgent {
        
        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();

            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(ObservantMoveToLocationState), new ObservantMoveToLocationState(this)); //Moving to next potential egg location, watching for agents entering any other buildings
            states.Add(typeof(SearchLocationState), new SearchLocationState(this)); //Search an egg location
            states.Add(typeof(ReturnToBaseState), new ReturnToBaseState(this)); //Return to start point
            states.Add(typeof(CompleteState), new CompleteState(this)); //Return to start point
            
            states.Add(typeof(FollowState), new FollowState(this)); //Follow a target

            stateMachine.SetStates(states);
        }
        
        public override void Begin() {
            Registry shopRegistry = DestinationRegistration.shopRegistryPedestrian;
            
            for (int i = 0; i < shopRegistry.GetListSize(); i++) {
                dests.Add(World.Instance.GetChunkManager().GetTile(shopRegistry.GetFromList(i)).gameObject);
            }
            
            SetAgentDestination(dests[0]);
            begin = true;
        }
        
        public override void FinishedSearch() {
            previousDestination = dests[0];
            timeSinceDestination = 0;
            
            if (dests.Count > 1) {
                RemoveDestination(dests[0]);
                SetAgentDestination(dests[0]);
            } else {
                returnToBase = true;
            }
        }
    }
}