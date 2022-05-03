using System.Collections;
using Scenarios.EasterEggHunt.Competitive.Agents;
using Scenarios.EasterEggHunt.Cooperative.Agents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt {
    public class EggHunterAgentManager : AgentManager {

        void Awake() {
            aStarPlane = FindObjectOfType<AStar>().gameObject;
            LoadingManager.scenarioPedestrianAgentManagers.Add(this);    
        }
        
        public override void Initialize() {}

        public override void Process() {
            SetComplete();
        }

        public override IEnumerator GenAgents() { yield return null; }
        
        //Competitive
        public IEnumerator GenerateAgentsCompFreeSearch(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveFreeSearch>(spawn));
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
        
        public IEnumerator GenerateAgentsCompObservantSearch(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveAvoidSearched>(spawn));
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
        
        public IEnumerator GenerateAgentsCompStalkerSearch(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveStalker>(spawn));
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }

        //Cooperative
        public IEnumerator GenerateAgentsCoopFreeSearch(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
        
        public IEnumerator GenerateAgentsCoopFreeSearchOptimized(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                agents.Add(ReplaceAgentWithCustom<EggHunterCoopFreeSearchOptimized>(spawn));
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
        
        public IEnumerator GenerateAgentsCoopPairedSearch(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                if (i % 2 == 0) {
                    agents.Add(ReplaceAgentWithCustom<EggHunterEggRunnerFollow>(spawn));
                } else {
                    agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                }
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
        
        public IEnumerator GenerateAgentsCoopConquerDivide(Vector3 spawnPos, float spawnRange, int agentCount, ScenarioManager scenarioManager) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                if (i % 2 == 0) {
                    agents.Add(ReplaceAgentWithCustom<EggHunterEggRunnerLocation>(spawn));
                } else {
                    agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeConquerDivide>(spawn));
                }
                FinalizeAgent(agents[i], i, scenarioManager);
                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }

        private void FinalizeAgent(GameObject agent, int i, ScenarioManager sm) {
            EggHunterScenarioManager eggSM = (EggHunterScenarioManager) sm;
            agents[i].transform.parent = transform;
            agents[i].name = "Egg-Hunter Agent " + (i + 1);
            agents[i].GetComponent<EggHunterAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
            agents[i].GetComponent<EggHunterAgent>().Init();
            agents[i].GetComponent<EggHunterAgent>().SetHunterID(i+1);
            agents[i].GetComponent<EggHunterAgent>().SetScenarioManager(eggSM);
            agents[i].GetComponent<EggHunterAgent>().SetAgentManager();
            
            if (eggSM.chatOnStart) World.Instance.SendChatMessage(agents[i].GetComponent<EggHunterAgent>().GetFullName(), EggHunterAgent.GetRandomMessage(EggHunterAgent.initMessages));
        }
        
        protected override void AgentUpdate() {}
    }
}