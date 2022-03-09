using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditButton : MonoBehaviour {

    private int id;
    private GameObject camera;

    void Start() {
        camera = GameObject.Find("Main Camera");
    }

    public void SetId(int id) => this.id = id;

    public void TriggerReplace() {
        camera.GetComponent<InputHandler>().ReplaceTile(id);
    }
}
