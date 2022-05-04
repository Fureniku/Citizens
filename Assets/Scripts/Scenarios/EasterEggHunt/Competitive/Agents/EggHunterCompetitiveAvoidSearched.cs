using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt.Competitive.Agents {
    public class EggHunterCompetitiveAvoidSearched : EggHunterAgent {
        
        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();

            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(ObservantMoveToLocationState), new ObservantMoveToLocationState(this)); //Moving to next potential egg location, watching for agents entering any other buildings
            states.Add(typeof(SearchLocationState), new SearchLocationState(this)); //Search an egg location
            states.Add(typeof(ReturnEggsToBaseState), new ReturnEggsToBaseState(this)); //Bring an egg home
            states.Add(typeof(ReturnToBaseState), new ReturnToBaseState(this)); //Return to start point
            states.Add(typeof(CompleteState), new CompleteState(this)); //Return to start point
            states.Add(typeof(WaitToCrossState), new WaitToCrossState(this)); //Wait to safely cross the road
            states.Add(typeof(CrossingState), new CrossingState(this)); //Crossing the road

            stateMachine.SetStates(states);
        }

        public override void Init() {
            Registry shopRegistry = LocationRegistration.shopRegistryDestPedestrian;
            int startPoint = Random.Range(0, shopRegistry.GetListSize() + 1);
            
            //List is in order but the start point is randomized, agents will loop through all locations still.
            for (int i = 0; i < shopRegistry.GetListSize(); i++) {
                int checkPoint = i + startPoint;
                if (checkPoint >= shopRegistry.GetListSize()) {
                    checkPoint -= shopRegistry.GetListSize();
                }

                if (checkPoint < 0 && checkPoint > shopRegistry.GetListSize()) {
                    Debug.Log("Checkpoint " + checkPoint + " was out of range (" + shopRegistry.GetListSize() + "), skipping.");
                } else {
                    dests.Add(World.Instance.GetChunkManager().GetTile(shopRegistry.GetFromList(checkPoint)).gameObject);
                }
            }
            
            SetAgentDestination(dests[0]);
            agent.isStopped = true;
            base.Init();
        }

        public override void Begin() {
            agent.isStopped = false;
            begin = true;
        }
        
        public override void FinishedSearch() {
            CheckForEggs();
            previousDestination = dests[0];
            timeSinceDestination = 0;
            
            if (dests.Count > 1) {
                RemoveDestination(dests[0]);
                SetAgentDestination(dests[0]);
            } else {
                returnToBase = true;
            }
        }
        
        public override string GetAgentTypeName() {
            return "Competitive Observant Egg Hunter";
        }
    }
}