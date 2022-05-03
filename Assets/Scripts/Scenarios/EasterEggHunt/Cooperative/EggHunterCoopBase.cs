using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scenarios.EasterEggHunt.Cooperative {
    public abstract class EggHunterCoopBase : EggHunterScenarioManager {

        [SerializeField] protected int totalLocations = 0;
        [SerializeField] private int searchedLocations = 0;
        
        public List<GameObject> searchDestinations = new List<GameObject>(); //Shared between all agents instead of individual list
        
        public void ClaimNextDestination(EggHunterAgent agent) {
            agent.ForceAgentDestination(searchDestinations[0]);
            searchDestinations.RemoveAt(0);
            searchedLocations++;
        }

        public void ClaimClosestAvailableDestination(EggHunterAgent agent) {
            float dist = 1000;
            GameObject candidate = null;

            for (int i = 0; i < searchDestinations.Count; i++) {
                //Find the unsearched shop closest to the agent
                float distTemp = Vector3.Distance(agent.transform.position, searchDestinations[i].transform.position);
                if (distTemp < dist) {
                    dist = distTemp;
                    candidate = searchDestinations[i];
                }
            }

            agent.ForceAgentDestination(candidate);
            searchDestinations.Remove(candidate);
            searchedLocations++;
        }
        
        public override void ScenarioUpdate() {
            if (isComplete) {
                Scenarios.Instance.SetInfo1("Returned agents:", returnedAgents + "/" + agentCount);
                Scenarios.Instance.SetInfo4("Time To Return:", GetPrintableReturnTime());
            } else {
                Scenarios.Instance.SetInfo1("Agents", agentCount.ToString());
                Scenarios.Instance.SetInfo4("Time Remaining:", GetPrintableTime());
            }
            
            Scenarios.Instance.SetInfo1("Agents", agentCount.ToString());
            Scenarios.Instance.SetInfo2("Eggs Found:", foundEggs + " / " + totalSpawnedEggs);
            Scenarios.Instance.SetInfo3("Locations Checked:", searchedLocations + " / " + totalLocations);

            if (AllEggsFound() || searchedLocations == totalLocations) {
                CompleteScenario();
            }
        }

        public int RemainingDestinations() {
            return searchDestinations.Count;
        }
    }
}