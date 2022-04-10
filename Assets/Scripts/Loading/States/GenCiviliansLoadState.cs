using System;

namespace Loading.States {
    public class GenCiviliansLoadState : LoadBaseState {
        
        public GenCiviliansLoadState(int progressId, string name, Type nextState) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
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