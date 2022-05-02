using System.Collections.Generic;
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

            StartCoroutine(eggHunterAgentManager.GenerateAgentsCompFreeSearch(startPoint.transform.position, spawnRange, agentCount, this));
        }

        public override void BeginScenario() {
            List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
            for (int i = 0; i < agents.Count; i++) {
                agents[i].GetComponent<EggHunterAgent>().Begin();
            }
        }
        
        public override void ScenarioUpdate() {
            Scenarios.Instance.SetInfo1("Agents", agentCount.ToString());
            Scenarios.Instance.SetInfo2("Eggs Found:", foundEggs + " / " + totalSpawnedEggs);
            Scenarios.Instance.SetInfo3("Locations Checked:", "N/A");
            Scenarios.Instance.SetInfo4("Time Remaining:", "unimplemented");
        }

        public override void CompleteScenario() {
            throw new System.NotImplementedException();
        }

        public override void CleanUp() {
            for (int i = 0; i < eggLocations.Count; i++) {
                if (eggLocations[i] != null) {
                    EggHolder eggHolder = eggLocations[i].GetComponent<EggHolder>();
                    if (eggHolder != null) {
                        Destroy(eggHolder);
                    }
                }
            }
        }
    }
}