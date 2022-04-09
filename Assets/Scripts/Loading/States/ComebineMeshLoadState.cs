﻿using System;

namespace Loading.States {
    public class ComebineMeshLoadState : LoadBaseState {
        
        public ComebineMeshLoadState(Type nextState) {
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