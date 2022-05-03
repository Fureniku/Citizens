using System;
using System.Collections.Generic;
using Scenarios.EasterEggHunt;
using Scenarios.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Scenarios {
    public class Scenarios : MonoBehaviour {
        
        private static Scenarios _instance;
        public static Scenarios Instance {
            get { return _instance; }
        }

        [SerializeField] private GameObject scenarioMainCanvas = null;
        [SerializeField] private GameObject scenarioListCanvas = null;
        [SerializeField] private GameObject scenarioTrackerCanvas = null;
        [SerializeField] private GameObject scenarioConfigCanvas = null;
        [SerializeField] private GameObject scenarioStartingCanvas = null;

        [SerializeField] private GameObject startPosition = null;
        [SerializeField] private GameObject depositPoint = null;
        [SerializeField] private InputField agentCount;
        [SerializeField] private InputField timeLimit;
        //Ideally temporary, Scenarios shouldn't have anything specific to any scenario, but fine for now.
        [SerializeField] private Toggle agentChatStart;
        [SerializeField] private Toggle agentChatNoEgg;
        [SerializeField] private Toggle agentChatEggFound;
        [SerializeField] private Toggle agentChatDepositEgg;
        [SerializeField] private Text scenarioTitle;
        [SerializeField] private Text message;
        
        [SerializeField] private Text[] scenarioTracker;

        private GameObject scenario = null;
        private bool preparing = false;
        private int prepareTime = 0;
        private bool began = false;

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

        public void PrepareScenario(ScenarioManager sm) {
            Debug.Log("Preparing " + sm.GetScenarioName());
            sm.PrepareScenario();
        }

        public void Reset() {
            preparing = false;
            prepareTime = 0;
            began = false;
            scenario = null;
        }

        public void BeginScenario(ScenarioManager sm) {
            scenarioTrackerCanvas.SetActive(true);
            scenarioTracker[0].text = sm.GetScenarioName();
            sm.SetStarted();
            sm.BeginScenario();
        }
        
        public void ScenarioUpdate(ScenarioManager sm) { sm.ScenarioUpdate(); }
        public void CompleteScenario(ScenarioManager sm) { sm.CompleteScenario(); }

        public void CleanUpScenario(ScenarioManager sm) {
            scenarioListCanvas.SetActive(true);
            scenarioTrackerCanvas.SetActive(false);
            scenarioMainCanvas.SetActive(false);
            sm.CleanUp(); 
        }

        void FixedUpdate() {
            if (scenario != null) {
                ScenarioManager scenarioManager = scenario.GetComponent<ScenarioManager>();
                
                if (preparing) {
                    prepareTime++;
                    
                    int remainTime = scenarioManager.prepareTime - prepareTime;
                    if (remainTime <= 0) {
                        preparing = false;
                        prepareTime = 0;
                        began = false;
                        scenarioStartingCanvas.SetActive(false);
                        Cursor.lockState = CursorLockMode.Locked;
                    } else if (remainTime <= 60) {
                        message.text = scenarioManager.GetScenarioName() + " \nStart!";
                        if (!began) {
                            BeginScenario(scenarioManager);
                            began = true;
                        }
                    } else if (remainTime <= 120) {
                        message.text = scenarioManager.GetScenarioName() + " \n1...";
                    } else if (remainTime <= 180) {
                        message.text = scenarioManager.GetScenarioName() + " \n2...";
                    } else if (remainTime <= 240) {
                        message.text = scenarioManager.GetScenarioName() + " \n3...";
                    } else {
                        message.text = scenarioManager.GetScenarioName() + " \nPreparing...";
                    }
                }

                if (scenarioManager.HasStarted()) {
                    ScenarioUpdate(scenarioManager);
                }
            }
        }

        public void InitializeScenario(int id) {
            scenario = Instantiate(scenarios[id].gameObject, startPosition.transform, true);
            scenario.GetComponent<ScenarioManager>().SetStartPoint(startPosition);
            scenario.GetComponent<ScenarioManager>().SetDepositPoint(depositPoint);
            scenarioListCanvas.SetActive(false);
            scenarioTitle.text = scenario.GetComponent<ScenarioManager>().GetScenarioName();
            scenarioConfigCanvas.SetActive(true);
        }

        public void PrepareScenario() {
            ScenarioManager sm = scenario.GetComponent<ScenarioManager>();
            sm.SetAgentCount(Int32.Parse(agentCount.text));
            sm.SetTimeLimit(Int32.Parse(timeLimit.text));
            if (sm is EggHunterScenarioManager) {
                EggHunterScenarioManager eggHunter = (EggHunterScenarioManager) sm;
                eggHunter.chatOnStart = agentChatStart.isOn;
                eggHunter.chatOnFailedSearch = agentChatNoEgg.isOn;
                eggHunter.chatOnFoundEgg = agentChatEggFound.isOn;
                eggHunter.chatOnDepositEgg = agentChatDepositEgg.isOn;
            }
            
            PrepareScenario(sm);
            preparing = true;
            scenarioConfigCanvas.SetActive(false);
            scenarioStartingCanvas.SetActive(true);
        }

        public void SetInfo1(string title, string msg) {
            scenarioTracker[1].text = title;
            scenarioTracker[2].text = msg;
        }
        
        public void SetInfo2(string title, string msg) {
            scenarioTracker[3].text = title;
            scenarioTracker[4].text = msg;
        }
        
        public void SetInfo3(string title, string msg) {
            scenarioTracker[5].text = title;
            scenarioTracker[6].text = msg;
        }
        
        public void SetInfo4(string title, string msg) {
            scenarioTracker[7].text = title;
            scenarioTracker[8].text = msg;
        }

        public void SetSpawnerObject(GameObject obj) {
            startPosition = obj;
        }

        public GameObject GetStartPosition() {
            return startPosition;
        }
    }
}