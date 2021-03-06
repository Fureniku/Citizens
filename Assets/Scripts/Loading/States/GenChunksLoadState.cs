using System;

namespace Loading.States {
    public class GenChunksLoadState : LoadBaseState {
        
        public GenChunksLoadState(int progressId, string name, Type nextState, bool skip) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.system = World.Instance.GetChunkManager();
            this.skip = skip;
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
            return "Generating Chunks";
        }
    }
}