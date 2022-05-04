using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;

namespace Scenarios.EasterEggHunt.Competitive {
    public class EggHunterCompetitiveScenarioManager : EggHunterScenarioManager {

        public override string GetScenarioName() {
            return "Egg Hunter (Competitive)";
        }

        public override void PrepareScenario() {
            Registry registry = LocationRegistration.shopRegistryDestPedestrian;
            for (int i = 0; i < registry.GetListSize(); i++) {
                int rng = Random.Range(1, 101);
                if (rng <= eggLocationChancePercent) {
                    AddEggs(registry.GetFromList(i));
                }
            }

            StartCoroutine(eggHunterAgentManager.GenerateAgentsCompFreeSearch(startPoint.transform.position, spawnRange, agentCount, this, 50, 50, 0));
        }

        public override void BeginScenario() {
            List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
            for (int i = 0; i < agents.Count; i++) {
                agents[i].GetComponent<EggHunterAgent>().Begin();
            }
        }
        
        public override void ScenarioUpdate() {
            string currentWinner = "N/A";
            List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
            int maxEggs = 0;
            for (int i = 0; i < agents.Count; i++) {
                int eggs = agents[i].GetComponent<EggHunterAgent>().GetTotalEggs();
                if (eggs > maxEggs) {
                    currentWinner = agents[i].GetComponent<EggHunterAgent>().GetFullName() + $" ({eggs})";
                    maxEggs = eggs;
                }
            }

            if (isComplete) {
                Scenarios.Instance.SetInfo1("Returned agents:", returnedAgents + "/" + agentCount);
                Scenarios.Instance.SetInfo4("Time To Return:", GetPrintableReturnTime());
            } else {
                Scenarios.Instance.SetInfo1("Agents", agentCount.ToString());
                Scenarios.Instance.SetInfo4("Time Remaining:", GetPrintableTime());
            }
            
            Scenarios.Instance.SetInfo2("Eggs Found:", foundEggs + " / " + totalSpawnedEggs);
            Scenarios.Instance.SetInfo3("Current Winner:", currentWinner);
        }
    }
}