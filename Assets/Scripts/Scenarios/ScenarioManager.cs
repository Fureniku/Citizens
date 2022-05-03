using System;
using System.Drawing;
using Scenarios.UI;
using UnityEngine;

namespace Scenarios {
    public abstract class ScenarioManager : MonoBehaviour {

        [SerializeField] protected GameObject startPoint;
        [SerializeField] protected GameObject depositPoint;
        
        [SerializeField] protected float spawnRange;
        [SerializeField] protected int agentCount;

        [SerializeField] protected float timeLimit;

        [SerializeField] protected bool hasStarted = false;

        public int prepareTime = 300; //in ticks/60 per second
        
        void Awake() {
            Scenarios.Instance.AddToList(this);
            if (startPoint == null) {
                startPoint = Scenarios.Instance.GetStartPosition();
            }
        }

        void Update() {
            timeLimit -= Time.deltaTime;
            if (timeLimit <= 0.0f) {
                CompleteScenario();
            }
        }
        
        public abstract string GetScenarioName();
        public abstract void PrepareScenario();
        public abstract void BeginScenario();
        public abstract void ScenarioUpdate();
        public abstract void CompleteScenario();
        public abstract void CleanUp();

        private void OnDestroy() {
            CleanUp();
        }
        
        public void SetAgentCount(int i) {
            agentCount = i;
        }

        public void SetTimeLimit(int i) {
            timeLimit = i*60;
        }

        protected string GetPrintableTime() {
            TimeSpan time = TimeSpan.FromSeconds(timeLimit);
            return time.ToString("mm':'ss");
        }
        
        public GameObject GetStartPoint() {
            return startPoint;
        }
        
        public GameObject GetDepositPoint() {
            return depositPoint;
        }

        public void SetStartPoint(GameObject point) {
            startPoint = point;
        }
        
        public void SetDepositPoint(GameObject point) {
            depositPoint = point;
        }

        public void SetStarted() {
            hasStarted = true;
        }

        public bool HasStarted() {
            return hasStarted;
        }
    }
}