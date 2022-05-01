using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;

namespace Scenarios.EasterEggHunt.Cooperative {
    public class EggHunterCoopFreeFilteredScenMan : EggHunterCoopBase {

        public override string GetScenarioName() {
            return "Egg Hunter Optimized Free Search";
        }

        public override void PrepareScenario() {
            Registry registry = LocationRegistration.shopRegistryDestPedestrian;
            
            //Filter list to be closest shop first. First, make a copy of the entire shop registry:
            List<GameObject> tempList = new List<GameObject>();
            for (int i = 0; i < registry.GetListSize(); i++) {
                tempList.Add(World.Instance.GetChunkManager().GetTile(registry.GetFromList(i)).gameObject);
            }
            //The ordered list:
            List<GameObject> orderedList = new List<GameObject>();
            
            //Do first entry manually because it's different:
            float distFirst = 1000;
            GameObject candidateFirst = null;
            int lastShopId = 0;

            for (int i = 0; i < tempList.Count; i++) {
                //Find the shop closest to the spawn point
                float distTemp = Vector3.Distance(startPoint.transform.position, tempList[i].transform.position);
                if (distTemp < distFirst) {
                    distFirst = distTemp;
                    candidateFirst = tempList[i];
                }
            }

            orderedList.Add(candidateFirst);
            tempList.Remove(candidateFirst);

            //oh no this is probably gonna be slow but neccessary (only once per scenario)
            while (tempList.Count > 0) {
                float distance = 1000;
                GameObject candidate = null;
                //Find the shop closest to the last shop
                for (int i = 0; i < tempList.Count; i++) {
                    float distTemp = Vector3.Distance(orderedList[lastShopId].transform.position, tempList[i].transform.position);
                    if (distTemp < distance) {
                        distance = distTemp;
                        candidate = tempList[i];
                    }
                }
                orderedList.Add(candidate);
                tempList.Remove(candidate);
                lastShopId++;
            }

            searchDestinations = orderedList;
            totalLocations = searchDestinations.Count;
            
            //Finally add eggs to our registry
            for (int i = 0; i < searchDestinations.Count; i++) {
                int rng = Random.Range(1, 101);
                if (rng <= eggLocationChancePercent) {
                    AddEggs(searchDestinations[i].GetComponent<TileData>().GetTilePos());
                }
            }

            StartCoroutine(eggHunterAgentManager.GenerateAgentsCoopFreeSearchOptimized(startPoint.transform.position, spawnRange, agentCount, this));
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
            Debug.Log("Scenario completed!");
            List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
            for (int i = 0; i < agents.Count; i++) {
                agents[i].GetComponent<EggHunterAgent>().GetStateMachine().ForceState(typeof(ReturnToBaseState));
            }
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