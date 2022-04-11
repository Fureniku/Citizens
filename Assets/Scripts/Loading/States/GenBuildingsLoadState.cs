using System;
using UnityEngine.ParticleSystemJobs;

namespace Loading.States {
    public class GenBuildingsLoadState : LoadBaseState {
        
        public GenBuildingsLoadState(int progressId, string name, Type nextState, SectionManager sectionManager, bool skip) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.system = sectionManager;
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
            return "Generating Buildings";
        }
    }
}