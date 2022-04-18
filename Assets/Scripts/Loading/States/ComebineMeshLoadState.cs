using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loading.States {
    public class ComebineMeshLoadState : LoadBaseState {

        public ComebineMeshLoadState(int progressId, string name, Type nextState, MeshCombinerManager meshCombinerManager) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.system = meshCombinerManager;
        }

        public override bool StateProgress() {
            if (system == null) { return true; }
            system.Process();
            return system.IsComplete();
        }

        public override Type StateEnter() {
            if (system != null) { system.Initialize(); }
            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Combining Meshes";
        }
    }
}