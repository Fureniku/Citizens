using System;

namespace Loading.States {
    public class GenChunksLoadState : LoadBaseState {
        
        public GenChunksLoadState(Type nextState) {
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            return false;
        }

        public override Type StateEnter() {
            throw new NotImplementedException();
        }

        public override Type StateExit() {
            throw new NotImplementedException();
        }
    }
}