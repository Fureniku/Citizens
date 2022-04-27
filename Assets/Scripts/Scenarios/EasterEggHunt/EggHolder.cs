using UnityEngine;

namespace Scenarios.EasterEggHunt {
    public class EggHolder : MonoBehaviour {

        [SerializeField] private int eggCount = 0;

        public void TakeEggs(EggHunterAgent agent, int count) {
            if (count <= eggCount) {
                eggCount -= count;
                agent.AddEggs(count);
            }
        }

        public void AddEggs(int eggs) {
            eggCount += eggs;
        }

        public void SetEggs(int eggs) {
            eggCount = eggs;
        }
    }
}