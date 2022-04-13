using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianAgentManager : AgentManager {
    
    public override void Initialize() {
        StartCoroutine(GenAgents());
    }

    public override void Process() {
        if (spawnAgentsCreated) {
            int avr = AllAgentsReady();
            if (avr == -1) {
                EnableAgents();
            }
            else {
                message = "Plotting pedestrian paths (" + avr + " of " + agents.Count + ")";
            }
        }
    }

    public override IEnumerator GenAgents() {
        Debug.Log("Starting pedestrian generation");
        for (int i = 0; i < initialAgentCount; i++) {
            Vector3 spawnPos = DestinationRegistration.RoadSpawnerRegistry.GetAtRandom().GetWorldPos();
            float offset = World.Instance.GetChunkManager().GetGridTileSize() / 2;
            agents.Add(Instantiate(testAgent, new Vector3(spawnPos.x + offset, spawnPos.y, spawnPos.z + offset), Quaternion.identity));
            agents[i].transform.parent = transform;
            agents[i].name = "Pedestrian Agent " + (i + 1);
            agents[i].GetComponent<PedestrianAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
            agents[i].GetComponent<PedestrianAgent>().Init();
            agents[i].GetComponent<PedestrianAgent>().SaveAcceleration(agents[i].GetComponent<NavMeshAgent>().acceleration);

            message = "Created pedestrian " + i + " of " + initialAgentCount;
            yield return null;
        }

        spawnAgentsCreated = true;
        yield return null;
    }
}