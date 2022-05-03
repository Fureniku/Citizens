using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;

namespace Scenarios.EasterEggHunt.Cooperative {
    public class EggHunterCoopFreeScenMan : EggHunterCoopBase {

        public override string GetScenarioName() {
            return "Egg Hunter Free Search";
        }

        public override void PrepareScenario() {
            Registry registry = LocationRegistration.shopRegistryDestPedestrian;
            for (int i = 0; i < registry.GetListSize(); i++) {
                int rng = Random.Range(1, 101);
                if (rng <= eggLocationChancePercent) {
                    AddEggs(registry.GetFromList(i));
                }
                searchDestinations.Add(World.Instance.GetChunkManager().GetTile(registry.GetFromList(i)).gameObject);
            }

            StartCoroutine(eggHunterAgentManager.GenerateAgentsCoopFreeSearch(startPoint.transform.position, spawnRange, agentCount, this));
        }

        public override void BeginScenario() {
            List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
            for (int i = 0; i < agents.Count; i++) {
                agents[i].GetComponent<EggHunterAgent>().Begin();
            }
        }
    }
}