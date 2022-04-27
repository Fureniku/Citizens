using System;
using UnityEngine;

namespace Scenarios {
    public abstract class ScenarioManager : MonoBehaviour {

        void Awake() {
            Debug.Log("Scenario manager awake");
            Scenarios.Instance.AddToList(this);
        }

        public abstract string GetScenarioName();
        public abstract void PrepareScenario();
        public abstract void BeginScenario();
        public abstract void ScenarioUpdate();
        public abstract void CompleteScenario();
        public abstract void CleanUp();

        private void OnDestroy() {
            CleanUp();
        }
    }
}