using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt;
using UnityEngine;
using UnityEngine.UI;

namespace Scenarios {
    public class Scenarios : MonoBehaviour {
        
        private static Scenarios _instance;
        public static Scenarios Instance {
            get { return _instance; }
        }

        [SerializeField] private GameObject scenarioCanvas = null;
        [SerializeField] private GameObject scenarioConfigCanvas = null;
        [SerializeField] private GameObject scenarioStartingCanvas = null;

        [SerializeField] private GameObject startPosition = null;
        [SerializeField] private GameObject depositPoint = null;
        [SerializeField] private InputField inputField;
        [SerializeField] private Text message;

        private GameObject scenario = null;
        private bool preparing = false;
        private int prepareTime = 0;

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

        void FixedUpdate() {
            if (scenario != null && preparing) {
                prepareTime++;

                int remainTime = scenario.GetComponent<ScenarioManager>().prepareTime - prepareTime;
                if (remainTime <= 0) {
                    preparing = false;
                    prepareTime = 0;
                    scenarioStartingCanvas.SetActive(false);
                } else if (remainTime <= 60) {
                    message.text = "Start!";
                    BeginScenario(scenario.GetComponent<ScenarioManager>());
                } else if (remainTime <= 120) {
                    message.text = "1...";
                } else if (remainTime <= 180) {
                    message.text = "2...";
                } else if (remainTime <= 240) {
                    message.text = "3...";
                } else {
                    message.text = "Preparing...";
                }
            }
        }

        public void InitializeScenario(int id) {
            scenario = Instantiate(scenarios[0].gameObject, startPosition.transform, true);
            scenario.GetComponent<ScenarioManager>().SetStartPoint(startPosition);
            scenario.GetComponent<ScenarioManager>().SetDepositPoint(depositPoint);
            scenarioCanvas.SetActive(false);
            scenarioConfigCanvas.SetActive(true);
        }

        public void PrepareScenario() {
            ScenarioManager sm = scenario.GetComponent<ScenarioManager>();
            if (sm is EggHunterScenarioManager) {
                EggHunterScenarioManager scenarioManager = (EggHunterScenarioManager) sm;
                scenarioManager.SetAgentCount(Int32.Parse(inputField.text));
            }
            PrepareScenario(sm);
            preparing = true;
            scenarioConfigCanvas.SetActive(false);
            scenarioStartingCanvas.SetActive(true);
        }

        public void SetSpawnerObject(GameObject obj) {
            startPosition = obj;
        }

        public GameObject GetStartPosition() {
            return startPosition;
        }
    }
}