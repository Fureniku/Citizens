using System;

namespace Loading.States {
    public class InitializeLoadState : LoadBaseState {
        
        public InitializeLoadState(Type nextState) {
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            return false;
        }

        public override Type StateEnter() {
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}