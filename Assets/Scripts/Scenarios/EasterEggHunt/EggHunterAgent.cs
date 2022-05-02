﻿using System;
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

        protected GameObject followTarget = null;
        protected float followDistance = 5.0f;
        protected EggHunterScenarioManager scenarioManager;

        [SerializeField] protected bool returnToBase;
        
        [SerializeField] protected GameObject previousDestination = null;
        [SerializeField] protected int timeSinceDestination = 0;

        public override void Init() {
            World.Instance.SendChatMessage(GetFullName(), GetRandomMessage(initMessages));
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
            holdingEggs += eggs;
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
            return begin;
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

        public void TimeUp() {
            stateMachine.ForceState(typeof(ReturnToBaseState));
        }

        public GameObject GetPreviousDestination() {
            return timeSinceDestination < 180 ? previousDestination : null;
        }

        public void CheckForEggs() {
            EggHolder eggHolder = currentDestGO.GetComponent<EggHolder>();
            string shopName = currentDestGO.GetComponent<TileData>().GetName();
            if (eggHolder != null) {
                int eggCount = eggHolder.GetEggCount();
                int takeEggs = Mathf.Max(maxHeldEggs - eggCount, eggCount);
                currentDestGO.GetComponent<EggHolder>().TakeEggs(this, takeEggs);
                AddEggs(takeEggs);
                
                if (takeEggs == 1) {
                    World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(foundOneEgg), takeEggs, shopName));
                } else {
                    World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(foundEggs), takeEggs, shopName));
                }
                Debug.Log("Took " + takeEggs + " eggs!");
                return;
            }
            World.Instance.SendChatMessage(GetFullName(), ParseString(GetRandomMessage(foundNoEggs), 0, shopName));
            Debug.Log("No eggs, moving on.");
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

        public static string[] initMessages = {
            "I'm ready to find some eggs!",
            "I'm gonna find the most eggs!",
            "I can't promise I wont eat the eggs I find...",
            "If I don't find any eggs I'm gonna cry :'(",
        };
        
        public static string[] foundNoEggs = {
            "No eggs at %s.",
            "Not a single egg in %s!",
            "Found an egg at %s! Oh wait, nope, never mind.",
            "I could tell you %s had 2000 eggs, but I'd be lying.",
            "Mom can you pick me up? There's no eggs at %s. Whoops wrong chat!",
            "EGG-HUNTER-BOT-3000 DID NOT DETECT EGGS IN THIS LOCATION: %s",
        };
        
        public static string[] foundEggs = {
            "I found %e eggs at %s!",
            "%e more eggs from %s",
            "%e eggs courtesy of %s, bringing 'em home.",
            "Retrieved %e eggs from %s.",
        };
        
        public static string[] foundOneEgg = {
            "I found an egg at %s!",
            "One more egg, located in %s.",
            "Add another to the pile! %s kindly gave me an egg!",
        };
    }
}