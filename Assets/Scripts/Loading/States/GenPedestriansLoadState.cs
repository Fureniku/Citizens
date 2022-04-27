using System;

namespace Loading.States {
    public class GenPedestriansLoadState : LoadBaseState {
        
        public GenPedestriansLoadState(int progressId, string name, Type nextState, PedestrianAgentManager agentManager, bool skip) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.skip = skip;
            this.system = agentManager;
        }

        public override bool StateProgress() {
            system.Process();
            bool ready = system.IsComplete();
            for (int i = 0; i < LoadingManager.scenarioPedestrianAgentManagers.Count; i++) {
                LoadingManager.scenarioPedestrianAgentManagers[i].Process();
                if (!LoadingManager.scenarioPedestrianAgentManagers[i].IsComplete()) {
                    ready = false;
                }
            }

            return ready;
        }

        public override Type StateEnter() {
            system.Initialize();
            
            for (int i = 0; i < LoadingManager.scenarioPedestrianAgentManagers.Count; i++) {
                LoadingManager.scenarioPedestrianAgentManagers[i].Initialize();
            }
            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Generating Pedestrian Agents";
        }
    }
}