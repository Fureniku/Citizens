using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public class EggHolder : MonoBehaviour {

        [SerializeField] private int eggCount = 0;

        public void TakeEggs(EggHunterAgent agent, int count) {
            Debug.Log("Attempting to take " + count + " for " + agent.name);
            if (count <= eggCount) {
                Debug.Log("Success!");
                eggCount -= count;
                agent.AddEggs(count);
            }
            else {
                Debug.Log("Oh no! The shop still holds " + eggCount + " eggs.");
            }
        }

        public int GetEggCount() {
            return eggCount;
        }

        public void AddEggs(int eggs) {
            eggCount += eggs;
        }

        public void SetEggs(int eggs) {
            eggCount = eggs;
        }
    }
}