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
        Registry initialSpawnerRegistry = LocationRegistration.RoadSpawnerRegistry;
        
        int initialAgents = initialAgentCount;
        if (initialAgents > initialSpawnerRegistry.GetListSize()) {
            initialAgents = initialSpawnerRegistry.GetListSize() - 1;
            Debug.Log("Capping initial agents at " + initialAgents + " due to world size");
        }
        
        for (int i = 0; i < initialAgents; i++) {
            TilePos spawnTilePos = initialSpawnerRegistry.GetAtRandom();
            Vector3 spawnPos = spawnTilePos.GetWorldPos();
            initialSpawnerRegistry.RemoveFromList(spawnTilePos);
            float offset = World.Instance.GetChunkManager().GetGridTileSize() / 2;
            agents.Add(Instantiate(testAgent, new Vector3(spawnPos.x + offset, spawnPos.y, spawnPos.z + offset), Quaternion.identity));
            agents[i].transform.parent = transform;
            agents[i].name = "Pedestrian Agent " + (i + 1);
            agents[i].GetComponent<PedestrianAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
            agents[i].GetComponent<PedestrianAgent>().Init();

            message = "Created pedestrian " + i + " of " + initialAgents;
            yield return null;
        }

        spawnAgentsCreated = true;
        yield return null;
    }

    protected override void AgentUpdate() {}
}