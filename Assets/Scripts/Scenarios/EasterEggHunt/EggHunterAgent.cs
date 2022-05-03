using System;
using System.Collections.Generic;
using System.Linq;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt {
    public abstract class EggHunterAgent : PedestrianAgent {

        protected bool begin = false;
        [SerializeField] protected int holdingEggs = 0;
        [SerializeField] protected int maxHeldEggs = 3;
        [SerializeField] protected int totalEggsFound = 0;
        [SerializeField] protected int searchCounter = 0;
        [SerializeField] protected float maxSpeed = 4f;
        [SerializeField] protected float eggWeight = 0.25f;
        protected int hunterId = 0;

        protected Vector3 spawnPoint;

        protected GameObject followTarget = null;
        protected float followDistance = 5.0f;
        [SerializeField] protected EggHunterScenarioManager scenarioManager;

        [SerializeField] protected bool returnToBase;
        
        [SerializeField] protected GameObject previousDestination = null;
        [SerializeField] protected int timeSinceDestination = 0;

        public override void Init() {
            initialized = true;
        }

        public void SetScenarioManager(EggHunterScenarioManager scenarioMngr) {
            scenarioManager = scenarioMngr;
        }

        protected override void AgentUpdate() {
            base.AgentUpdate();
            agent.speed = maxSpeed - (holdingEggs * eggWeight);
            if (previousDestination != null) {
                timeSinceDestination++;
            }
        }
        
        public override void SetAgentManager() {
            if (scenarioManager == null) {
                return;
            }
            agentManager = scenarioManager.GetAgentManager();
        } 

        public void SetHunterID(int id) => hunterId = id;
        public GameObject GetFollowTarget() { return followTarget; }
        public void SetFollowTarget(GameObject follow) { followTarget = follow; }
        public void RemoveFollowTarget() { followTarget = null; }
        public float GetFollowDistance() { return followDistance; }
        public int EggCount() { return holdingEggs; }

        public void GiveEgg(EggHunterAgent otherAgent) {
            int eggSpace = otherAgent.maxHeldEggs - otherAgent.holdingEggs;
            if (eggSpace > 0 && holdingEggs >= 1) {
                holdingEggs -= 1;
                otherAgent.holdingEggs += 1;
            }
        }

        public void AddEggs(int eggs) {
            Debug.Log("Adding eggs");
            holdingEggs += eggs;
        }

        public int GetTotalEggs() {
            return totalEggsFound;
        }

        public void RemoveEggs(int eggs) {
            if (eggs <= holdingEggs) {
                holdingEggs -= eggs;
                totalEggsFound += eggs;
            }
        }

        public bool SearchTimer() {
            if (searchCounter < 180) {
                searchCounter++;
                return false;
            }

            searchCounter = 0;
            return true;
        }

        public bool CanBegin() {
            spawnPoint = transform.position;
            return begin;
        }

        public Vector3 GetSpawnPoint() {
            return spawnPoint;
        }
        
        public bool ReturnToBase() {
            return returnToBase;
        }

        public ScenarioManager GetScenarioManager() {
            return scenarioManager;
        }
        
        public void RemoveDestination(GameObject go) {
            if (dests[0] == go) {
                dests.Remove(go);
                SetAgentDestination(dests[0]);
            } else if (dests.Contains(go)) {
                dests.Remove(go);
            }
        }

        public GameObject GetPreviousDestination() {
            return timeSinceDestination < 180 ? previousDestination : null;
        }

        public void CheckForEggs() {
            EggHolder eggHolder = currentDestGO.GetComponent<EggHolder>();
            if (currentDestGO.GetComponent<TileData>() != null) {
                string shopName = currentDestGO.GetComponent<TileData>().GetName();
                if (eggHolder != null) {
                    int eggCount = eggHolder.GetEggCount();
                    int takeEggs = Mathf.Max(maxHeldEggs - eggCount, eggCount);
                    currentDestGO.GetComponent<EggHolder>().TakeEggs(this, takeEggs);

                    if (scenarioManager.chatOnFoundEgg) {
                        if (takeEggs == 1) {
                            World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(foundOneEgg), takeEggs, shopName));
                        } else {
                            World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(foundEggs), takeEggs, shopName));
                        }
                    }
                    return;
                }
                if (scenarioManager.chatOnFailedSearch) World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(foundNoEggs), 0, shopName));
            } else {
                if (scenarioManager.chatOnDepositEgg) World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(depositingEggs), holdingEggs, "Base"));
            }
            
        }

        public abstract void Begin();
        public abstract void FinishedSearch();

        public override string GetAgentTagMessage() {
            return "Current Eggs: " + holdingEggs;
        }

        public static string ParseString(string str, int eggCount, string shopName) {
            string strOut = str.Replace("%e", eggCount.ToString());
            return strOut.Replace("%s", shopName);
        }

        public static string GetRandomMessage(string[] str) {
            return str[Random.Range(0, str.Length)];
        }

        public static readonly string[] initMessages = {
            "I'm ready to find some eggs!",
            "I'm gonna find the most eggs!",
            "I can't promise I wont eat the eggs I find...",
            "If I don't find any eggs I'm gonna cry :'(",
        };
        
        private static readonly string[] foundNoEggs = {
            "No eggs at %s.",
            "Not a single egg in %s!",
            "Found an egg at %s! Oh wait, nope, never mind.",
            "I could tell you %s had 2000 eggs, but I'd be lying.",
            "Mom can you pick me up? There's no eggs at %s. Whoops wrong chat!",
            "EGG-HUNTER-BOT-3000 DID NOT DETECT EGGS IN THIS LOCATION: %s",
        };
        
        private static readonly string[] foundEggs = {
            "I found %e eggs at %s!",
            "%e more eggs from %s",
            "%e eggs courtesy of %s, bringing 'em home.",
            "Retrieved %e eggs from %s.",
        };
        
        private static readonly string[] foundOneEgg = {
            "I found an egg at %s!",
            "One more egg, located in %s.",
            "Add another to the pile! %s kindly gave me an egg!",
        };
        
        private static readonly string[] depositingEggs = {
            "Depoisted %e eggs at base",
        };
    }
}