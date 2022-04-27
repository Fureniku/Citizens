using System.Collections.Generic;
using UnityEngine;

namespace Scenarios {
    public class Scenarios : MonoBehaviour {
        
        private static Scenarios _instance;
        public static Scenarios Instance {
            get { return _instance; }
        }

        private void Awake() {
            Debug.Log("Scenarios awake");
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }

        public List<ScenarioManager> scenarios = new List<ScenarioManager>();

        public void AddToList(ScenarioManager scenarioManager) {
            scenarios.Add(scenarioManager);
        }

        public void PrepareScenario(int scenario) { PrepareScenario(scenarios[scenario]); }
        public void BeginScenario(int scenario) { BeginScenario(scenarios[scenario]); }
        public void ScenarioUpdate(int scenario) { ScenarioUpdate(scenarios[scenario]); }
        public void CompleteScenario(int scenario) { CompleteScenario(scenarios[scenario]); }
        public void CleanUpScenario(int scenario) { CleanUpScenario(scenarios[scenario]); }

        public void PrepareScenario(ScenarioManager scenario) {
            Debug.Log("Preparing " + scenario.GetScenarioName());
            scenario.PrepareScenario();
        }
        public void BeginScenario(ScenarioManager scenario) { scenario.BeginScenario(); }
        public void ScenarioUpdate(ScenarioManager scenario) { scenario.ScenarioUpdate(); }
        public void CompleteScenario(ScenarioManager scenario) { scenario.CompleteScenario(); }
        public void CleanUpScenario(ScenarioManager scenario) { scenario.CleanUp(); }


        private bool test = false;
        
        void Update() {
            if (!test && World.Instance.IsWorldFullyLoaded()) {
                PrepareScenario(0);
                test = true;
            }    
        }
    }
}