using System.Collections.Generic;
using Scenarios.EasterEggHunt.Competitive;
using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public abstract class EggHunterScenarioManager : ScenarioManager {
        
        [SerializeField] protected int totalSpawnedEggs = 0;
        [SerializeField] protected int foundEggs = 0;
        
        [SerializeField] protected int maxEggsPerLocation = 3;
        [SerializeField] protected int eggLocationChancePercent = 10;
        [SerializeField] protected EggHunterAgentManager eggHunterAgentManager;
        
        [SerializeField] protected float spawnRange;
        [SerializeField] protected int agentCount;
        
        public List<GameObject> eggLocations = new List<GameObject>();
        
        public void SetAgentCount(int i) {
            agentCount = i;
        }
        
        public override void CleanUp() {
            for (int i = 0; i < eggLocations.Count; i++) {
                EggHolder eggHolder = eggLocations[i].GetComponent<EggHolder>();
                if (eggHolder != null) {
                    Destroy(eggHolder);
                }
            }
        }

        protected void AddEggs(TilePos pos) {
            GameObject tile = World.Instance.GetChunkManager().GetTile(pos).gameObject;
            EggHolder eggHolder = tile.GetComponent<EggHolder>();

            if (eggHolder == null) {
                tile.AddComponent<EggHolder>();
                eggHolder = tile.GetComponent<EggHolder>();
                eggLocations.Add(tile);
            }

            int eggs = Random.Range(0, maxEggsPerLocation + 1);
            
            eggHolder.AddEggs(eggs);
            totalSpawnedEggs += eggs;
        }
        
        public int GetMaxAgents() {
            return agentCount;
        }
        
        public AgentManager GetAgentManager() {
            return eggHunterAgentManager;
        }
        
        public void DepositEggs(int eggs) {
            foundEggs += eggs;
        }

        public bool AllEggsFound() {
            return foundEggs == totalSpawnedEggs;
        }
    }
}