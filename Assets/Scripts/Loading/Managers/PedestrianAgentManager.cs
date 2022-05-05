using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianAgentManager : AgentManager {
    
    public override void Initialize() {
        initialAgentCount = WorldData.Instance.GetInitPeds();
        maxAgentCount = WorldData.Instance.GetMaxPeds();
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
            CreateInitialAgent(spawnPos);
            message = "Created pedestrian " + i + " of " + initialAgents;
            yield return null;
        }

        spawnAgentsCreated = true;
        yield return null;
    }
    
    private void CreateInitialAgent(Vector3 spawnPos) {
        float offset = World.Instance.GetChunkManager().GetGridTileSize() / 2;
        GameObject agent = Instantiate(testAgent, new Vector3(spawnPos.x + offset, spawnPos.y, spawnPos.z + offset), Quaternion.identity);
        agent.transform.parent = transform;
        agent.name = "PA_" + id + ": " + agent.GetComponent<AgentData>().GetFullName();
        id++;
        agent.GetComponent<PedestrianAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
        agent.GetComponent<PedestrianAgent>().Init();

        agents.Add(agent);
    }
    
    private void CreateAgent(LocationNodeController spawnPoint) {
        GameObject agent = Instantiate(testAgent, spawnPoint.GetSpawnerNodePedestrian().transform.position, Quaternion.identity);
        agent.transform.parent = transform;
        agent.name = "PA_" + id + ": " + agent.GetComponent<AgentData>().GetFullName();
        id++;
        agent.GetComponent<PedestrianAgent>().SetAStar(aStarPlane.GetComponent<AStar>());
        agent.GetComponent<PedestrianAgent>().SetSpawnController(spawnPoint);
        agent.GetComponent<PedestrianAgent>().Init();
        agent.GetComponent<BaseAgent>().SetInitialized();

        agents.Add(agent);
    }

    protected override void AgentUpdate() {
        if (World.Instance.IsWorldFullyLoaded()) {
            if (spawnCooldown > 0) {
                spawnCooldown--;
            } else {
                if (currentAgentCount < maxAgentCount) {
                    TilePos pos = LocationRegistration.allPedestrianSpawnersRegistry.GetAtRandom();
                    LocationNodeController lnc = World.Instance.GetChunkManager().GetTile(pos).GetComponent<LocationNodeController>();
                    CreateAgent(lnc);
                    spawnCooldown = maxSpawnCooldown;
                }
            }
        }
    }
}