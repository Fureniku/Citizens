using UnityEngine;

namespace UI {
    public class SelectScenarioButton : MonoBehaviour {

        private int id;
        private GameObject cam;

        void Start() {
            cam = GameObject.Find("Main Camera");
        }

        public void SetId(int idIn) => id = idIn;

        public void SelectScenario() {
            Scenarios.Scenarios.Instance.InitializeScenario(id);
            Debug.Log("Selecting scenario " + id);
        }
    }
}