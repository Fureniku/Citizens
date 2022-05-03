using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Loading.States {
    public class GenNavMeshLoadState : LoadBaseState {

        private AStar aStar;
        private NavMeshSurface navMeshRoad;
        private NavMeshSurface navMeshSidewalk;

        public GenNavMeshLoadState(int progressId, string name, Type nextState, AStar aStar, NavMeshSurface navMeshRoad, NavMeshSurface navMeshSidewalk, bool skip) {
            this.progressId = progressId;
            this.stateName = name;
            this.nextState = nextState;
            this.aStar = aStar;
            this.navMeshRoad = navMeshRoad;
            this.navMeshSidewalk = navMeshSidewalk;
            this.skip = skip;
        }

        public override bool StateProgress() {
            return true;
        }

        public override Type StateEnter() {
            aStar.Initialize();

            navMeshRoad.BuildNavMesh();
            navMeshSidewalk.BuildNavMesh();

            return null;
        }

        public override Type StateExit() {
            return null;
        }
        
        public override string GetProgressString() {
            return "Generating NavMeshes";
        }
    }
}