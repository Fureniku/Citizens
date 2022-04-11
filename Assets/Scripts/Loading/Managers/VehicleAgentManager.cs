﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAgentManager : AgentManager {

    public override void Process() {
        if (spawnAgentsCreated) {
            int avr = AllAgentsReady();
            if (avr == -1) {
                EnableAgents();
            }
            else {
                message = "Plotting vehicle paths (" + avr + " of " + agents.Count + ")";
            }
        }
    }

    public override void Initialize() {
        StartCoroutine(GenAgents());
    }

    public override IEnumerator GenAgents() {
        Debug.Log("Starting vehicle generation");
        for (int i = 0; i < initialAgentCount; i++) {
            Vector3 spawnPos = DestinationRegistration.RoadSpawnerRegistry.GetAtRandom().GetWorldPos();
            float offset = World.Instance.GetChunkManager().GetGridTileSize() / 2;
            agents.Add(Instantiate(testAgent, new Vector3(spawnPos.x + offset, spawnPos.y, spawnPos.z + offset), Quaternion.identity));
            agents[i].transform.parent = transform;
            agents[i].name = "Vehicle Agent " + (i + 1);
            agents[i].GetComponent<VehicleAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
            agents[i].GetComponent<VehicleAgent>().Init();
            agents[i].GetComponent<VehicleAgent>().SaveAcceleration(agents[i].GetComponent<NavMeshAgent>().acceleration);

            message = "Created vehicle " + i + " of " + initialAgentCount;
            yield return null;
        }

        spawnAgentsCreated = true;
        yield return null;
    }
}