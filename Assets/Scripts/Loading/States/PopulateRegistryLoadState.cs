using System;

namespace Loading.States {
    public class PopulateRegistryLoadState : LoadBaseState {
        
        private VehicleRegistry vehicleRegistry;
        private BrandRegistry brandRegistry;
        
        public PopulateRegistryLoadState(int progressId, string name, Type nextState, VehicleRegistry vehicleRegistry, BrandRegistry brandRegistry) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.vehicleRegistry = vehicleRegistry;
            this.brandRegistry = brandRegistry;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
            LocationRegistration.BuildLists();
            vehicleRegistry.Initialize();
            brandRegistry.Initialize();
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