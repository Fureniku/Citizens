using System;
using System.Drawing;
using UnityEngine;

namespace Scenarios {
    public abstract class ScenarioManager : MonoBehaviour {

        [SerializeField] protected GameObject startPoint;
        [SerializeField] protected GameObject depositPoint;

        public int prepareTime = 300; //in ticks/60 per second
        
        void Awake() {
            Debug.Log("Scenario manager awake");
            Scenarios.Instance.AddToList(this);
            if (startPoint == null) {
                startPoint = Scenarios.Instance.GetStartPosition();
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
    }
}