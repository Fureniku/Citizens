using UnityEngine;

namespace Scenarios.EasterEggHunt.Competitive {
    public class EggHunterCompetitiveStalker : EggHunterAgent {
        
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
            Registry shopRegistry = DestinationRegistration.shopRegistryPedestrian;
            
            int followId = Random.Range(0, scenarioManager.GetAgentManager().GetAllAgents().Count+1);
            if (followId == hunterId) {
                if (hunterId == 0) {
                    followId++;
                } else {
                    followId--;
                }
            }
            followTarget = scenarioManager.GetAgentManager().GetAllAgents()[followId];
            
            begin = true;
        }
    }
}