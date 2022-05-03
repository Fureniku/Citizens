using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt.Competitive.Agents {
    public class EggHunterCompetitiveStalker : EggHunterAgent {

        protected override void InitStateMachine() {
            stateMachine = GetComponent<AgentStateMachine>();
            Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
            states.Add(typeof(WaitingStalkerState), new WaitingStalkerState(this)); //Waiting to start the game
            states.Add(typeof(StalkAgentState), new StalkAgentState(this)); //Following an agent, noting where they check
            states.Add(typeof(StalkerMoveToLocationState), new StalkerMoveToLocationState(this)); //Following an agent, noting where they check
            states.Add(typeof(StalkerSearchLocationState), new StalkerSearchLocationState(this)); //Search an egg location
            states.Add(typeof(ReturnToBaseState), new ReturnToBaseState(this)); //Return to start point
            states.Add(typeof(CompleteState), new CompleteState(this)); //Return to start point
            states.Add(typeof(WaitToCrossState), new WaitToCrossState(this)); //Wait to safely cross the road
            states.Add(typeof(CrossingState), new CrossingState(this)); //Crossing the road

            stateMachine.SetStates(states);
        }
        
        public override void Init() {
            Registry shopRegistry = LocationRegistration.shopRegistryDestPedestrian;

            List<GameObject> followableList = new List<GameObject>();
            for (int i = 0; i < scenarioManager.GetAgentManager().GetAllAgents().Count; i++) {
                EggHunterAgent eggHunterAgent = scenarioManager.GetAgentManager().GetAllAgents()[i].GetComponent<EggHunterAgent>();
                if (!(eggHunterAgent is EggHunterCompetitiveStalker)) {
                    followableList.Add(eggHunterAgent.gameObject);
                }
            }
            
            for (int i = 0; i < shopRegistry.GetListSize(); i++) {
                dests.Add(World.Instance.GetChunkManager().GetTile(shopRegistry.GetFromList(i)).gameObject);
            }
            
            followTarget = followableList[Random.Range(0, followableList.Count+1)];
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
            return "Competitive Egg Hunter Stalker";
        }
    }
}