using System;

namespace Loading.States {
    public class PopulateRegistryLoadState : LoadBaseState {
        
        public PopulateRegistryLoadState(int progressId, string name, Type nextState) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
            DestinationRegistration.BuildLists();
            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Populating Registries";
        }
    }
}