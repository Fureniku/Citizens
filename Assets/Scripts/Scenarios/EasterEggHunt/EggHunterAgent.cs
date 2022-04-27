using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public class EggHunterAgent : PedestrianAgent {

        private bool begin = false;
        [SerializeField] private int holdingEggs = 0;

        [SerializeField] private int searchCounter = 0;

        public override void Init() {
            initialized = true;
        }
        
        private void GenerateDestination() {
            GameObject finalDest = World.Instance.GetChunkManager().GetTile(DestinationRegistration.shopRegistryPedestrian.GetAtRandom()).gameObject;
            dests.Add(finalDest);

            SetAgentDestination(finalDest);
        }
        
        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
            states.Add(typeof(WaitingState), new WaitingState(this)); //Waiting to start the game
            states.Add(typeof(MoveToLocationState), new MoveToLocationState(this)); //Moving to next potential egg location
            states.Add(typeof(SearchLocationState), new SearchLocationState(this)); //Search an egg location

            stateMachine.SetStates(states);
        }

        public void AddEggs(int eggs) {
            holdingEggs += eggs;
        }

        public bool RemoveEggs(int eggs) {
            if (eggs <= holdingEggs) {
                holdingEggs -= eggs;
                return true;
            }

            return false;
        }

        public bool SearchTimer() {
            if (searchCounter < 180) {
                searchCounter++;
                return false;
            }

            searchCounter = 0;
            return true;
        }

        public bool CanBegin() {
            return begin;
        }

        public void Begin() => begin = true;
    }

    public enum EnumSearchStrategy {
        
    }
}