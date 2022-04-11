using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loading.States {
    public class ComebineMeshLoadState : LoadBaseState {
        
        private static List<GameObject> meshCombiners = new List<GameObject>();
        
        public ComebineMeshLoadState(int progressId, string name, Type nextState) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
            Debug.Log(meshCombiners.Count + " mesh combiners registered");
            for (int i = 0; i < meshCombiners.Count; i++) {
                Debug.Log("combining mesh " + i);
                meshCombiners[i].GetComponent<MeshCombiner>().CombineMeshes();
            }
            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Combining Meshes";
        }

        public static void RegisterMeshCombiner(GameObject meshCombiner) {
            meshCombiners.Add(meshCombiner);
        }
    }
}