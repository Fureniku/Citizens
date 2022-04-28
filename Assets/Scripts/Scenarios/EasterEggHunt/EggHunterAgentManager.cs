using System;
using System.Collections;
using Scenarios.EasterEggHunt.Competitive.Agents;
using Scenarios.EasterEggHunt.Cooperative;
using Scenarios.EasterEggHunt.Cooperative.Agents;
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

        public IEnumerator GenerateAgents(Vector3 spawnPos, float spawnRange, int agentCount, bool competitive, ScenarioManager scenarioManager) {
            int rng = 0;
            if (!competitive) {
                rng = Random.Range(0, 3) + 1;
            }
            
            for (int i = 0; i < agentCount; i++) {
                float spawnX = spawnPos.x + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                float spawnZ = spawnPos.z + Random.Range(0, spawnRange * 2 + 1) - spawnRange;
                Vector3 spawn = new Vector3(spawnX, 0, spawnZ);
                if (competitive) {
                    agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveFreeSearch>(spawn));
                    /*rng = Random.Range(1, 6) + 1;
                    if (rng <= 3) {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveFreeSearch>(spawn));
                    } else if (rng <= 6) {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveAvoidSearched>(spawn));
                    } else { //7
                        agents.Add(ReplaceAgentWithCustom<EggHunterCompetitiveStalker>(spawn));
                    }*/
                } else {
                    agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                    /*if (rng == 1) {
                        agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                    } else if (rng == 2) {
                        if (i % 2 == 0) {
                            agents.Add(ReplaceAgentWithCustom<EggHunterEggRunnerFollow>(spawn));
                        } else {
                            agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeFreeSearch>(spawn));
                        }
                    } else {
                        if (i % 2 == 0) {
                            agents.Add(ReplaceAgentWithCustom<EggHunterEggRunnerLocation>(spawn));
                        } else {
                            agents.Add(ReplaceAgentWithCustom<EggHunterCooperativeConquerDivide>(spawn));
                        }
                    }*/
                }

                agents[i].transform.parent = transform;
                agents[i].name = "Egg-Hunter Agent " + (i + 1);
                agents[i].GetComponent<EggHunterAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
                agents[i].GetComponent<EggHunterAgent>().Init();
                agents[i].GetComponent<EggHunterAgent>().SetHunterID(i+1);
                agents[i].GetComponent<EggHunterAgent>().SetScenarioManager((EggHunterScenarioManager) scenarioManager);

                message = "Created egg-hunter " + i + " of " + agentCount;
                yield return null;
            }
        }
    }
}