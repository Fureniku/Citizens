using System.Collections.Generic;
using Scenarios.EasterEggHunt.Competitive;
using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public class EggHunterCooperativeScenarioManager : ScenarioManager {

        [SerializeField] private int maxEggsPerLocation = 3;
        [SerializeField] private int eggLocationChancePercent = 10;
        [SerializeField] private EggHunterAgentManager eggHunterAgentManager;
        
        [SerializeField] private float spawnRange;
        [SerializeField] private int agentCount;

        public List<GameObject> eggLocations = new List<GameObject>();

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
            }

            StartCoroutine(eggHunterAgentManager.GenerateAgents(startPoint.transform.position, spawnRange, agentCount, false));
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

        public override void CleanUp() {
            for (int i = 0; i < eggLocations.Count; i++) {
                EggHolder eggHolder = eggLocations[i].GetComponent<EggHolder>();
                if (eggHolder != null) {
                    Destroy(eggHolder);
                }
            }
        }

        private void AddEggs(TilePos pos) {
            GameObject tile = World.Instance.GetChunkManager().GetTile(pos).gameObject;
            EggHolder eggHolder = tile.GetComponent<EggHolder>();

            if (eggHolder == null) {
                tile.AddComponent<EggHolder>();
                eggHolder = tile.GetComponent<EggHolder>();
                eggLocations.Add(tile);
            }
            
            eggHolder.AddEggs(Random.Range(0, maxEggsPerLocation+1));
        }

        public AgentManager GetAgentManager() {
            return eggHunterAgentManager;
        }

        public int GetMaxAgents() {
            return agentCount;
        }
    }
}