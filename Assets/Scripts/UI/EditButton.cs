using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditButton : MonoBehaviour {

    private int id;
    private GameObject cam;

    void Start() {
        cam = GameObject.Find("Main Camera");
    }

    public void SetId(int id) => this.id = id;

    public void TriggerReplace() {
        cam.GetComponent<InputHandler>().ReplaceTile(id);
    }
}
