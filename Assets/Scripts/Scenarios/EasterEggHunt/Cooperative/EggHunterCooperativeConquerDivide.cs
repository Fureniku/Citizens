using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public class EggHunterCooperativeConquerDivide : EggHunterAgent {
        
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
            
            bool runner = hunterId % 2 == 0;
            int quarter = shopRegistry.GetListSize() / 4;
            int agentQuarter = scenarioManager.GetMaxAgents() / 4;

            int start = quarter * (agentQuarter - 1);
            int end = start + quarter;
            if (end > shopRegistry.GetListSize()) {
                end = shopRegistry.GetListSize();
            }

            for (int i = start; i < end; i++) {
                //Find closest shop for meet point
                float dist = 1000f;
                if (Vector3.Distance(transform.position, shopRegistry.GetFromList(i).GetWorldPos()) < dist) {
                    dist = Vector3.Distance(transform.position, shopRegistry.GetFromList(i).GetWorldPos());
                    meetPoint = World.Instance.GetChunkManager().GetTile(shopRegistry.GetFromList(i)).gameObject;
                }

                if (runner) {
                            
                }
                else {
                    dests.Add(World.Instance.GetChunkManager().GetTile(shopRegistry.GetFromList(i)).gameObject);
                }
            }
            
            begin = true;
        }
    }
}