using System;

namespace Loading.States {
    public class GenVehicleLoadState : LoadBaseState {

        
        public GenVehicleLoadState(int progressId, string name, Type nextState, AgentManager agentManager) {
            this.progressId = progressId;
            this.stateName = name;
            this.system = agentManager;
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            system.Process();
            return system.IsComplete();
        }

        public override Type StateEnter() {
            system.Initialize();
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