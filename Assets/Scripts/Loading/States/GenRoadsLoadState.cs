using System;

namespace Loading.States {
    public class GenRoadsLoadState : LoadBaseState {

        public GenRoadsLoadState(int progressId, string name, Type nextState, RoadSeed roadGen, bool skip) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.system = roadGen;
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
            return "Generating Roads";
        }
    }
}