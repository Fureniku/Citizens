using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.UI;

public class PedestrianSelectionInfo : MonoBehaviour {

    [SerializeField] private GameObject nameTag;
    [SerializeField] private GameObject age;
    [SerializeField] private GameObject gender;
    [SerializeField] private GameObject profession;
    [SerializeField] private GameObject currentState;
    [SerializeField] private GameObject agentType;
    [SerializeField] private GameObject agentMessage;
    
    [SerializeField] private GameObject tilePosX;
    [SerializeField] private GameObject tilePosZ;
    
    [SerializeField] private GameObject destPosX;
    [SerializeField] private GameObject destPosZ;
    
    [SerializeField] private GameObject destName;
    
    [SerializeField] private GameObject distance;


    public void SetSelectionInfo(PedestrianAgent agent) {
        AgentData agentData = agent.GetComponent<AgentData>();
        
        SetText(nameTag, agentData.GetFullName());
        SetText(age, agentData.GetAge());
        SetText(gender, agentData.GetGenderName());
        SetText(profession, agentData.GetProfessionName());
        
        SetText(currentState, agent.GetState().GetName());
        SetText(agentType, agent.GetAgentTypeName());
        SetText(agentMessage, agent.GetAgentTagMessage());
        
        TilePos destinationPos = TilePos.GetTilePosFromLocation(agent.GetAgent().destination);
        SetText(destPosX, destinationPos.x);
        SetText(destPosZ, destinationPos.z);

        TileData destination = World.Instance.GetChunkManager().GetTile(destinationPos);
        SetText(destName, destination.GetName());
        
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
