using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWorld : MonoBehaviour {

    [SerializeField] private GameObject tile;
    [SerializeField] private TileRegistry tileRegistry;
    
    private bool populatedTypes = false;
    private bool builtWorld = false;
    
    private float roadScale = 1.0f;

    private void Start() {
        tileRegistry.Initialize();
    }

    void Update() {
        if (populatedTypes && !builtWorld) {
            float tilePos = 0;
            
            for (int i = 0; i < TypeRegistries.GetTotalHouses(); i++) {
                GameObject placedTile = Instantiate(tile, new Vector3(tilePos, 0, 0), Quaternion.Euler(0, 90, 0));
                placedTile.transform.localScale = new Vector3(roadScale, roadScale, roadScale);

                GameObject placedBuilding = TileRegistry.Instantiate(TypeRegistries.GetHouse(i).GetId());
                placedBuilding.transform.position = new Vector3(tilePos, 0.0f, 10 * roadScale);
                placedBuilding.transform.localScale = new Vector3(roadScale/5, roadScale/5, roadScale/5);
                placedBuilding.transform.rotation = Quaternion.Euler(0, 90, 0);
                
                tilePos += roadScale * 10;
            }
            
            tilePos = 0;
            
            for (int i = 0; i < TypeRegistries.GetTotalShops(); i++) {
                GameObject placedTile = Instantiate(tile, new Vector3(tilePos, 0, 30 * roadScale), Quaternion.Euler(0, 90, 0));
                placedTile.transform.localScale = new Vector3(roadScale, roadScale, roadScale);

                GameObject placedBuilding = TileRegistry.Instantiate(TypeRegistries.GetShop(i).GetId());
                placedBuilding.transform.position = new Vector3(tilePos, 0.0f, (30 * roadScale) + 10 * roadScale);
                placedBuilding.transform.localScale = new Vector3(roadScale/5, roadScale/5, roadScale/5);
                placedBuilding.transform.rotation = Quaternion.Euler(0, 90, 0);
                
                tilePos += roadScale * 10;
            }

            Debug.Log("Build " + TypeRegistries.GetTotalHouses() + " houses and  " + TypeRegistries.GetTotalShops() + " shops.");
            builtWorld = true;
        }
        
        if (tileRegistry.IsComplete() && !populatedTypes) {
            TypeRegistries.PopulateRegistries();
            populatedTypes = true;
        }
    }
}
