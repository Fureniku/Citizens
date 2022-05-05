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
    
    [SerializeField] private GameObject state;
    
    [SerializeField] private GameObject finalDestPosX;
    [SerializeField] private GameObject finalDestPosZ;
    [SerializeField] private GameObject finalDestName;
    


    public void SetSelectionInfo(VehicleAgent agent) {
        SetText(nameTag, agent.gameObject.name);
        SetText(vehicleType, "Car");
        TilePos destinationPos = TilePos.GetTilePosFromLocation(agent.GetAgent().destination);
        SetText(destPosX, destinationPos.x);
        SetText(destPosZ, destinationPos.z);
        
        TilePos pos = TilePos.GetTilePosFromLocation(agent.transform.position);
        SetText(tilePosX, pos.x);
        SetText(tilePosZ, pos.z);
        
        SetText(state, agent.GetState().GetName());

        LocationNode finalNode = agent.GetFinalKnownDestination().GetComponent<LocationNode>();
        if (finalNode != null) {
            TileData finalDestination = finalNode.GetNodeController().GetParentTile();
        
            SetText(finalDestPosX, finalDestination.GetTilePos().x);
            SetText(finalDestPosZ, finalDestination.GetTilePos().z);
            SetText(finalDestName, finalDestination.GetName());
        } else {
            SetText(finalDestPosX, "");
            SetText(finalDestPosZ, "");
            SetText(finalDestName, "Unknown");
        }
    }

    private void SetText(GameObject go, string txt) {
        go.GetComponent<Text>().text = txt;
    }
    
    private void SetText(GameObject go, int txt) {
        go.GetComponent<Text>().text = txt + "";
    }

}
