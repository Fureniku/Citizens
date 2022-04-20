using System;

namespace Loading.States {
    public class PopulateRegistryLoadState : LoadBaseState {
        
        private VehicleRegistry vehicleRegistry;
        
        public PopulateRegistryLoadState(int progressId, string name, Type nextState, VehicleRegistry vehicleRegistry) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.vehicleRegistry = vehicleRegistry;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
            DestinationRegistration.BuildLists();
            vehicleRegistry.Initialize();
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