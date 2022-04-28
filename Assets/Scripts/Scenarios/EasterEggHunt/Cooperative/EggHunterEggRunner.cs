namespace Scenarios.EasterEggHunt {
    public class EggHunterEggRunner : EggHunterAgent {
        
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
            bool follower = hunterId % 2 == 0;
            if (follower) {
                followTarget = scenarioManager.GetAgentManager().GetAllAgents()[hunterId-1];
            } else {
                partner = scenarioManager.GetAgentManager().GetAllAgents()[hunterId+1];
                for (int i = 0; i < shopRegistry.GetListSize(); i++) {
                    dests.Add(World.Instance.GetChunkManager().GetTile(shopRegistry.GetFromList(i)).gameObject);
                }
            }
        }
    }
}