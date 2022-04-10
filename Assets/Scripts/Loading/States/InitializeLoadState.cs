using System;
using UnityEngine;
using UnityEngine.UI;

namespace Loading.States {
    public class InitializeLoadState : LoadBaseState {
        
        //private ButtonPopulator buttonPopulator;

        public InitializeLoadState(int progressId, string name, Type nextState, TileRegistry registry) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            system = registry;
            //buttonPopulator = GameObject.Find("EditButtonList").GetComponent<ButtonPopulator>();
        }

        public override bool StateProgress() {
            return system.IsComplete();
        }

        public override Type StateEnter() {
            system.Initialize();
            return null;
        }

        public override Type StateExit() {
            /*if (buttonPopulator != null) {
                buttonPopulator.Process();
            }
            else {
                Debug.Log("Button populator is null!");
            }*/
            return null;
        }

        public override string GetProgressString() {
            return "Initializing World";
        }
    }
}