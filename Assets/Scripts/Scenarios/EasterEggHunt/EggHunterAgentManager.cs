using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt.Competitive {
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

        public IEnumerator GenerateAgents(Vector3 spawnPos, float spawnRange, int agentCount, bool competitive) {
            int rng = 0;
            if (!competitive) {
                rng = Random.Range(0, 3) + 4;
            }
            
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, spawnPos.y, spawnZ);
                if (competitive) {
                    rng = Random.Range(0, 3) + 1;
                    if (rng == 1) {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveFreeSearch>(spawn));
                    } else if (rng == 2) {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveAvoidSearched>(spawn));
                    } else {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveStalker>(spawn));
                    }
                } else {
                    if (rng == 4) {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                    } else if (rng == 5) {
                        if (i % 2 == 0) {
                            agents.Add(ReplaceAgentWithCustom<EggHunterEggRunner>(spawn));
                        } else {
                            agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                        }
                    } else {
                        if (i % 2 == 0) {
                            agents.Add(ReplaceAgentWithCustom<EggHunterEggRunner>(spawn));
                        } else {
                            agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeConquerDivide>(spawn));
                        }
                    }
                }

                agents[i].transform.parent = transform;
                agents[i].name = "Egg-Hunter Agent " + (i + 1);
                agents[i].GetComponent<EggHunterAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
                agents[i].GetComponent<EggHunterAgent>().Init();
                agents[i].GetComponent<EggHunterAgent>().SetHunterID(i+1);

                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
    }
}