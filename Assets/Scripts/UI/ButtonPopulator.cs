using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPopulator : MonoBehaviour {
    
    private bool populated = false;
    [SerializeField] private GameObject button;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (!populated && World.Instance.PassedState(EnumWorldState.INITIALIZED)) {
            Debug.Log("Creating " + TileRegistry.GetSize() + " buttons");
            for (int i = 0; i < TileRegistry.GetSize(); i++) {
                GameObject newBtn = Instantiate(button, transform);
                newBtn.name = TileRegistry.GetTile(i).GetName();
                newBtn.transform.GetChild(0).GetComponent<Text>().text = TileRegistry.GetTile(i).GetName();
                newBtn.GetComponent<EditButton>().SetId(i);
            }
            populated = true;
        }
    }
}
