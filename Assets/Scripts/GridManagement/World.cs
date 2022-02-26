using System;
using UnityEngine;

public class World : MonoBehaviour {
    
    private static World _instance;
    public static World Instance {
        get { return _instance; }
    }
    
    [SerializeField] private string worldName = "world";
    [SerializeField] private bool save = true;
    [SerializeField] private GridManager gridManager = null;
    
    void Awake() {
        GC.Collect();
        Debug.Log("Initialize world");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
        gridManager.Initialize();
    }

    public GridManager GetGridManager() {
        return gridManager;
    }

    public string GetWorldName() {
        return worldName;
    }

    public bool SavingEnabled() {
        return save;
    }
}