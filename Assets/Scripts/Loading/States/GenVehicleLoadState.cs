using System;

namespace Loading.States {
    public class GenVehicleLoadState : LoadBaseState {
        
        public GenVehicleLoadState(int progressId, string name, Type nextState, VehicleAgentManager agentManager, bool skip) {
            this.progressId = progressId;
            this.stateName = name;
            this.system = agentManager;
            this.skip = skip;
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            system.Process();
            bool ready = system.IsComplete();
            for (int i = 0; i < LoadingManager.scenarioVehicleAgentManagers.Count; i++) {
                LoadingManager.scenarioVehicleAgentManagers[i].Process();
                if (!LoadingManager.scenarioVehicleAgentManagers[i].IsComplete()) {
                    ready = false;
                }
            }

            return ready;
        }

        public override Type StateEnter() {
            system.Initialize();
            
            for (int i = 0; i < LoadingManager.scenarioVehicleAgentManagers.Count; i++) {
                LoadingManager.scenarioVehicleAgentManagers[i].Initialize();
            }
            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Generating Vehicle Agents";
        }
    }
}