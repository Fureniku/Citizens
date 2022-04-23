using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAgentManager : AgentManager {
    
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
                message = "Plotting vehicle paths (" + avr + " of " + agents.Count + ")";
            }
        }
    }

    public override IEnumerator GenAgents() {
        Debug.Log("Starting vehicle generation");
        Registry initialSpawnerRegistry = DestinationRegistration.RoadDestinationRegistry;
        
        int initialAgents = initialAgentCount;
        if (initialAgents > initialSpawnerRegistry.GetListSize()) {
            initialAgents = initialSpawnerRegistry.GetListSize() - 1;
            Debug.Log("Capping initial agents at " + initialAgents + " due to world size");
        }
        for (int i = 0; i < initialAgents; i++) {
            GameObject agent = VehicleRegistry.GetRandomCar();
            TilePos spawnTilePos = initialSpawnerRegistry.GetAtRandom();
            Vector3 spawnPos = spawnTilePos.GetWorldPos();
            initialSpawnerRegistry.RemoveFromList(spawnTilePos);
            float offset = World.Instance.GetChunkManager().GetGridTileSize() / 2;
            agents.Add(Instantiate(agent, new Vector3(spawnPos.x + offset, spawnPos.y, spawnPos.z + offset), Quaternion.identity));
            agents[i].transform.parent = transform;
            agents[i].name = "Vehicle Agent " + (i + 1);
            agents[i].GetComponent<VehicleAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
            agents[i].GetComponent<VehicleAgent>().Init();
            agents[i].GetComponent<VehicleAgent>().SaveAcceleration(agents[i].GetComponent<NavMeshAgent>().acceleration);

            message = "Created vehicle " + i + " of " + initialAgents;
            yield return null;
        }

        spawnAgentsCreated = true;
        yield return null;
    }
}