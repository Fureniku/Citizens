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

        private float waitForReturn = 0;
        public override void CompleteScenario() {
            if (hasStarted) {
                if (!isComplete) {
                    isComplete = true;
                    List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
                    for (int i = 0; i < agents.Count; i++) {
                        agents[i].GetComponent<EggHunterAgent>().GetStateMachine().ForceState(typeof(ReturnToBaseState));
                    }
                    World.Instance.SendChatMessage("Game Manager", "Egg Hunter is now over! Everyone please come back to the Town Hall.");
                }

                if (!isClosing) {
                    List<GameObject> agents = eggHunterAgentManager.GetAllAgents();
                    returnedAgents = 0;
                    for (int i = 0; i < agents.Count; i++) {
                        if (agents[i].GetComponent<EggHunterAgent>().GetState() is CompleteState) {
                            returnedAgents++;
                        }
                    }

                    waitForReturn += Time.deltaTime;

                    if (returnedAgents < agents.Count && waitForReturn < 1800f) {
                        return;
                    }

                    if (waitForReturn > 1800f) {
                        int missingAgents = agents.Count - returnedAgents;
                        World.Instance.SendChatMessage("Game Manager", "Oh no! I guess a few people got lost on their way back here... " + (missingAgents == 1 ? "1 person didn't return." : missingAgents + " people didn't return."));
                    }
                    
                    World.Instance.SendChatMessage("Game Manager", "Congratulations everyone! We found " + foundEggs + " out of " + totalSpawnedEggs + " eggs!");
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

        public List<GameObject> CreateOptimisedList() {
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

            return orderedList;
        }
    }
}