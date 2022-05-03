using System;
using UnityEngine;

namespace Loading.States {
    public class MoveCameraState : LoadBaseState {
        
        private GameObject loadingCanvas;
        private GameObject camera;

        private float angle = -45f;
        private float angleIncrement = 0.1f;

        public MoveCameraState(int progressId, string name, Type nextState, GameObject loadingCanvas, GameObject camera) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.loadingCanvas = loadingCanvas;
            this.camera = camera;
        }

        public override bool StateProgress() {
            if (angle < 0f) {
                angle += angleIncrement;
                camera.transform.rotation = Quaternion.Euler(angle, 45, 0);
            }

            if (angleIncrement < 0.5f) { //simulate acceleration
                angleIncrement += 0.005f;
            }

            return angle >= 0f;
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