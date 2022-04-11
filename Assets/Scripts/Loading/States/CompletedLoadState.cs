using System;
using UnityEngine;

namespace Loading.States {
    public class CompletedLoadState : LoadBaseState {
        
        private GameObject loadingCanvas;
        
        public CompletedLoadState(int progressId, string name, Type nextState, GameObject loadingCanvas) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.loadingCanvas = loadingCanvas;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
            if (loadingCanvas != null) loadingCanvas.SetActive(false);
            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Load Complete";
        }
    }
}