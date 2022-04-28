using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Scenarios.UI {
    public class ScenarioButtonPopulator : MonoBehaviour {
    
        private bool populated = false;
        [SerializeField] private GameObject button;
    
        public void Awake() {
            if (!populated) {
                Debug.Log("Creating " + Scenarios.Instance.scenarios.Count + " buttons");
                for (int i = 0; i < Scenarios.Instance.scenarios.Count; i++) {
                    GameObject newBtn = Instantiate(button, transform);
                    newBtn.name = Scenarios.Instance.scenarios[i].GetScenarioName();
                    newBtn.transform.GetChild(0).GetComponent<Text>().text = Scenarios.Instance.scenarios[i].GetScenarioName();
                    newBtn.GetComponent<SelectScenarioButton>().SetId(i);
                }
                populated = true;
            }
        }
    }
}
