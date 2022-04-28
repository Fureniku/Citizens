using System;
using System.Collections.Generic;
using System.Linq;
using Scenarios.EasterEggHunt.AgentStates;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenarios.EasterEggHunt {
    public abstract class EggHunterAgent : PedestrianAgent {

        protected bool begin = false;
        [SerializeField] protected int holdingEggs = 0;
        [SerializeField] protected int maxHeldEggs = 3;
        [SerializeField] protected int searchCounter = 0;
        [SerializeField] protected float maxSpeed = 3.5f;
        [SerializeField] protected int hunterId = 0;
        [SerializeField] protected EnumSearchStrategy searchStrategy;

        [SerializeField] protected GameObject followTarget = null;
        [SerializeField] protected GameObject partner = null;
        [SerializeField] protected GameObject meetPoint = null;
        [SerializeField] protected float followDistance = 5.0f;
        [SerializeField] protected EggHunterCooperativeScenarioManager scenarioManager;

        [SerializeField] protected bool returnToBase;
        
        [SerializeField] protected GameObject previousDestination = null;
        [SerializeField] protected int timeSinceDestination = 0;

        public override void Init() {
            scenarioManager = FindObjectOfType<EggHunterCooperativeScenarioManager>();
            initialized = true;
        }

        protected override void AgentUpdate() {
            base.AgentUpdate();
            agent.speed = maxSpeed - (holdingEggs * 0.75f);
            timeSinceDestination++;
        }

        public void SetHunterID(int id) => hunterId = id;
        public GameObject GetFollowTarget() { return followTarget; }
        public float GetFollowDistance() { return followDistance; }
        public int EggCount() { return holdingEggs; }

        public bool GiveEgg(EggHunterAgent otherAgent) {
            int eggSpace = otherAgent.maxHeldEggs - otherAgent.holdingEggs;
            if (eggSpace > 0 && holdingEggs >= 1) {
                holdingEggs -= 1;
                otherAgent.holdingEggs += 1;
            }

            return false;
        }

        public void AddEggs(int eggs) {
            holdingEggs += eggs;
        }

        public bool RemoveEggs(int eggs) {
            if (eggs <= holdingEggs) {
                holdingEggs -= eggs;
                return true;
            }

            return false;
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

        public abstract void Begin();
        public abstract void FinishedSearch();
    }

    public enum EnumSearchStrategy {
        COMP_1, //Search the world in order of building registration, and check every building
        COMP_2, //Search the world in order of building registration, only skip buildings they've seen another agent search
        COMP_3, //Follow another agent, note which buildings they search, and as soon as they find an egg continue their search path (interrupting/sabotaging that agents future search attempts)
        
        COOP_1, //Everyone searches freely, but skips already searched areas
        COOP_2, //Everyone searches freely in pairs, when an egg is found one takes it home while the other continues the search
        COOP_3  //The world is divided into sections. Each section has a group of searchers and a group of runners. Runners wait at a location, searchers give runners eggs then runners return them home
    }
}