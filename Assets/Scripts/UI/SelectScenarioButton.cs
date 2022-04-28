using UnityEngine;

namespace UI {
    public class SelectScenarioButton : MonoBehaviour {

        private int id;
        private GameObject cam;

        void Start() {
            cam = GameObject.Find("Main Camera");
        }

        public void SetId(int id) => this.id = id;

        public void SelectScenario() {
            Scenarios.Scenarios.Instance.InitializeScenario(id);
        }
    }
}