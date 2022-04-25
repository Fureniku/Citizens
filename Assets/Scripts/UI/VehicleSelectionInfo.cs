using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionInfo : MonoBehaviour {

    [SerializeField] private GameObject nameTag;
    [SerializeField] private GameObject vehicleType;
    [SerializeField] private GameObject tilePosX;
    [SerializeField] private GameObject tilePosZ;
    
    [SerializeField] private GameObject destPosX;
    [SerializeField] private GameObject destPosZ;
    
    [SerializeField] private GameObject distance;


    public void SetSelectionInfo(VehicleAgent agent) {
        SetText(nameTag, agent.gameObject.name);
        SetText(vehicleType, "Car");
        TilePos destinationPos = TilePos.GetTilePosFromLocation(agent.GetAgent().destination);
        SetText(destPosX, destinationPos.x);
        SetText(destPosZ, destinationPos.z);
        
        TilePos pos = TilePos.GetTilePosFromLocation(agent.transform.position);
        SetText(tilePosX, pos.x);
        SetText(tilePosZ, pos.z);
                
        SetText(distance, TilePos.TileDistance(pos,destinationPos));
    }

    private void SetText(GameObject go, string txt) {
        go.GetComponent<Text>().text = txt;
    }
    
    private void SetText(GameObject go, int txt) {
        go.GetComponent<Text>().text = txt + "";
    }

}
