using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.UI;

public class SelectionInfo : MonoBehaviour {

    [SerializeField] private GameObject nameTag;
    [SerializeField] private GameObject tilePosX;
    [SerializeField] private GameObject tilePosZ;
    [SerializeField] private GameObject chunkPosX;
    [SerializeField] private GameObject chunkPosZ;
    [SerializeField] private GameObject localPosX;
    [SerializeField] private GameObject localPosZ;
    [SerializeField] private GameObject rotation;

    public void SetSelectionInfo(TileData data) {
        SetText(nameTag, data.GetName());
        SetText(tilePosX, data.GetGridPos().x);
        SetText(tilePosZ, data.GetGridPos().z);
        
        SetText(chunkPosX, TilePos.GetParentChunk(data.GetGridPos()).x);
        SetText(chunkPosZ, TilePos.GetParentChunk(data.GetGridPos()).z);
        
        SetText(localPosX, data.GetLocalPos().x);
        SetText(localPosZ, data.GetLocalPos().z);

        rotation.GetComponent<Text>().text = data.GetRotation().ToString() + "(" + data.GetRotation().GetRotation() + ")";
    }

    private void SetText(GameObject go, string txt) {
        go.GetComponent<Text>().text = txt;
    }
    
    private void SetText(GameObject go, int txt) {
        go.GetComponent<Text>().text = txt + "";
    }

}
