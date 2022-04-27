using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt.Competitive {
    public class EggHunterAgentManager : AgentManager {

        public bool allAgentsGenerated = false;
        private GameObject customAgent = null;

        void Awake() {
            aStarPlane = FindObjectOfType<AStar>().gameObject;
            LoadingManager.scenarioPedestrianAgentManagers.Add(this);    
        }
        
        public override void Initialize() {}

        public override void Process() {
            SetComplete();
        }

        public override IEnumerator GenAgents() { yield return null; }

        public IEnumerator GenerateAgents(Vector3 spawnPos, float spawnRange, int agentCount) {
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                
                agents.Add(ReplaceAgentWithCustom<EggHunterAgent>(new Vector3(spawnX, spawnPos.y, spawnZ)));

                agents[i].transform.parent = transform;
                agents[i].name = "Egg-Hunter Agent " + (i + 1);
                agents[i].GetComponent<EggHunterAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
                agents[i].GetComponent<EggHunterAgent>().Init();

                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }

            allAgentsGenerated = true;
        }
    }
}