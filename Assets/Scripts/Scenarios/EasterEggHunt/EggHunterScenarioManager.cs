using System.Collections.Generic;
using Scenarios.EasterEggHunt.AgentStates;
using Scenarios.EasterEggHunt.Competitive;
using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public abstract class EggHunterScenarioManager : ScenarioManager {
        
        [SerializeField] protected int totalSpawnedEggs = 0;
        [SerializeField] protected int foundEggs = 0;
        
        [SerializeField] protected int maxEggsPerLocation = 3;
        [SerializeField] protected int eggLocationChancePercent = 10;
        [SerializeField] protected EggHunterAgentManager eggHunterAgentManager;

        public bool chatOnStart = true;
        public bool chatOnFailedSearch = true;
        public bool chatOnFoundEgg = true;
        public bool chatOnDepositEgg = true;

        protected bool isComplete = false;
        private bool isClosing = false;
        private int closeTimer = 0;

        protected int returnedAgents = 0;

        public List<GameObject> eggLocations = new List<GameObject>();

        public override void CleanUp() {
            for (int i = 0; i < eggLocations.Count; i++) {
                EggHolder eggHolder = eggLocations[i].GetComponent<EggHolder>();
                if (eggHolder != null) {
                    Destroy(eggHolder);
                }
            }

            Scenarios.Instance.Reset();
            Destroy(gameObject);
        }

        protected void AddEggs(TilePos pos) {
            GameObject tile = World.Instance.GetChunkManager().GetTile(pos).gameObject;
            EggHolder eggHolder = tile.GetComponent<EggHolder>();

            if (eggHolder == null) {
                tile.AddComponent<EggHolder>();
                eggHolder = tile.GetComponent<EggHolder>();
                eggLocations.Add(tile);
            }

            int eggs = Random.Range(1, maxEggsPerLocation + 1);
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
        
        public override void CompleteScenario() {
            if (hasStarted) {
                if (!isComplete) {
                    isComplete = true;
                    List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
                    for (int i = 0; i < agents.Count; i++) {
                        agents[i].GetComponent<EggHunterAgent>().GetStateMachine().ForceState(typeof(ReturnToBaseState));
                    }
                    World.Instance.SendChatMessage("GAME", "Egg Hunter is now over! Everyone please come back to the Town Hall.");
                }

                if (!isClosing) {
                    List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
                    returnedAgents = 0;
                    for (int i = 0; i < agents.Count; i++) {
                        if (agents[i].GetComponent<EggHunterAgent>().GetState() is CompleteState) {
                            returnedAgents++;
                        }
                    }

                    if (returnedAgents < agents.Count) {
                        return;
                    }
                    
                    World.Instance.SendChatMessage("GAME", "Congratulations everyone! We found " + foundEggs + " out of " + totalSpawnedEggs + " eggs!");
                    Debug.Log("All agents have returned!");

                    isClosing = true;
                } else {
                    closeTimer++;
                    if (closeTimer > 300) {
                        Debug.Log("Calling cleanup! Everything must die.");
                        Scenarios.Instance.CleanUpScenario(this);
                    }
                }
            }
        }
    }
}