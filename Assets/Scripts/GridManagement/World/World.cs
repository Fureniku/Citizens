﻿using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class World : MonoBehaviour {

    private static World _instance;
    public static World Instance {
        get { return _instance; }
    }
    
    [SerializeField] private ChunkManager chunkManager = null;
    [SerializeField] private int worldSize = 3;
    [SerializeField] private bool internalGeneratedWorld = false; //If the world was made and is a prefab now, set this to true to stop any attempts at generation.
    
    private WorldData worldData = null;
    private bool isDirty = false;

    void Awake() {
        GC.Collect();
        Debug.Log("Initialize world");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        
        if (!internalGeneratedWorld) {
            chunkManager.Initialize();
            worldData = WorldData.Instance;
            if (worldData == null) {
                CreateWorldData();
            }
        } else {
            Debug.Log("Confirmed as internal world. Skipping generation.");
            worldData = FindObjectOfType<WorldData>();
            if (worldData == null) {
                CreateWorldData();
            }
        }
    }

    private void CreateWorldData() {
        Debug.Log("World data is null! Must create new instance!");
        GameObject go = new GameObject();
        go.AddComponent<WorldData>();
        go.name = "WorldData";
        worldData = go.GetComponent<WorldData>();
        worldData.SetWorldSize(worldSize);
    }

    public bool IsInternalGenWorld() { return internalGeneratedWorld; }
    public void SetWorldExists() => worldData.SetWorldExists();
    public bool DoesWorldExist() { return worldData.DoesWorldExist(); }
    public WorldData GetWorldData() { return worldData; }
    public ChunkManager GetChunkManager() { return chunkManager; }
    public string GetWorldName() { return worldData.GetWorldName(); }
    public void MarkDirty() { if (SavingEnabled()) isDirty = true; }
    private bool SavingEnabled() { return worldData.SavingEnabled(); }
    
    void Update() {
        if (isDirty) {
            SaveWorld();
        }
    }

    void SaveWorld() {
        if (SavingEnabled()) {
            Stopwatch stopWatch = Stopwatch.StartNew();
            Debug.Log("Starting world save...");
            for (int row = 0; row < chunkManager.GetSize(); row++) {
                for (int col = 0; col < chunkManager.GetSize(); col++) {
                    SaveLoadChunk.SerializeChunk(chunkManager.GetChunk(row, col));
                }
            }

            stopWatch.Stop();
            Debug.Log("World saving complete! Took " + stopWatch.Elapsed + " seconds.");
        }
        isDirty = false;
    }
}