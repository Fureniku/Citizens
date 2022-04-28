using System.Collections.Generic;
using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public class EggHunterCooperativeScenarioManager : EggHunterScenarioManager {

        public List<GameObject> searchDestinations = new List<GameObject>(); //Shared between all agents instead of individual list

        public override string GetScenarioName() {
            return "Egg Hunter (Cooperative)";
        }
        
        public override void PrepareScenario() {
            Registry registry = DestinationRegistration.shopRegistryPedestrian;
            for (int i = 0; i < registry.GetListSize(); i++) {
                int rng = Random.Range(1, 101);
                if (rng <= eggLocationChancePercent) {
                    AddEggs(registry.GetFromList(i));
                }
                searchDestinations.Add(World.Instance.GetChunkManager().GetTile(registry.GetFromList(i)).gameObject);
            }

            StartCoroutine(eggHunterAgentManager.GenerateAgents(startPoint.transform.position, spawnRange, agentCount, false, this));
        }

        public override void BeginScenario() {
            List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
            for (int i = 0; i < agents.Count; i++) {
                agents[i].GetComponent<EggHunterAgent>().Begin();
            }
        }

        public override void ScenarioUpdate() {
            throw new System.NotImplementedException();
        }

        public override void CompleteScenario() {
            throw new System.NotImplementedException();
        }

        public void ClaimNextDestination(EggHunterAgent agent) {
            agent.SetAgentDestination(searchDestinations[0]);
            searchDestinations.RemoveAt(0);
        }

        public int RemainingDestinations() {
            return searchDestinations.Count;
        }
    }
}