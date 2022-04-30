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

        protected GameObject followTarget = null;
        protected float followDistance = 5.0f;
        protected EggHunterScenarioManager scenarioManager;

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
            if (eggHolder != null) {
                int eggCount = eggHolder.GetEggCount();
                int takeEggs = Mathf.Max(maxHeldEggs - eggCount, eggCount);
                currentDestGO.GetComponent<EggHolder>().TakeEggs(this, takeEggs);
                AddEggs(takeEggs);
                Debug.Log("Took " + takeEggs + " eggs!");
                return;
            }
            Debug.Log("No eggs, moving on.");
        }

        public abstract void Begin();
        public abstract void FinishedSearch();

        public override string GetAgentTagMessage() {
            return "Current Eggs: " + holdingEggs;
        } 
    }
}