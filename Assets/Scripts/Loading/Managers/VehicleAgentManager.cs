using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAgentManager : AgentManager {

    private int id; //Only used for naming!
    private int spawnCooldown;
    
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
        Registry initialSpawnerRegistry = LocationRegistration.RoadSpawnerRegistry;
        
        int initialAgents = initialAgentCount;
        if (initialAgents > initialSpawnerRegistry.GetListSize()) {
            initialAgents = initialSpawnerRegistry.GetListSize() - 1;
            Debug.Log("Capping initial agents at " + initialAgents + " due to world size");
        }
        for (int i = 0; i < initialAgents; i++) {
            Vector3 spawnPos = initialSpawnerRegistry.GetAtRandom().GetWorldPos();
            //CreateAgentInitial(spawnPos);

            message = "Created vehicle " + i + " of " + initialAgents;
            yield return null;
        }

        spawnAgentsCreated = true;
        yield return null;
    }

    private void CreateAgentInitial(Vector3 spawnPos) {
        float offset = World.Instance.GetChunkManager().GetGridTileSize() / 2;
        GameObject agent = Instantiate(VehicleRegistry.GetRandomCar(), new Vector3(spawnPos.x + offset, spawnPos.y, spawnPos.z + offset), Quaternion.identity);
        agent.transform.parent = transform;
        agent.name = "VA_" + id + ": " + agent.GetComponent<Vehicle>().GetName() + " (" + agent.GetComponent<Vehicle>().GetColour() + ")";
        id++;
        agent.GetComponent<VehicleAgent>().SetAStar(aStarPlane.GetComponent<AStar>());

        agent.GetComponent<VehicleAgent>().Init();
        agent.GetComponent<VehicleAgent>().GetAgent().isStopped = true;

        agents.Add(agent);
    }
    
    private void CreateAgent(LocationNodeController spawnPoint) {
        GameObject agent = Instantiate(VehicleRegistry.GetRandomCar(), spawnPoint.GetSpawnerNodeVehicle().transform.position, Quaternion.identity);
        agent.transform.parent = transform;
        agent.name = "VA_" + id + ": " + agent.GetComponent<Vehicle>().GetName() + " (" + agent.GetComponent<Vehicle>().GetColour() + ")";
        id++;
        agent.GetComponent<VehicleAgent>().SetAStar(aStarPlane.GetComponent<AStar>());

        agent.GetComponent<VehicleAgent>().SetSpawnController(spawnPoint);
        agent.GetComponent<VehicleAgent>().Init();
        agent.GetComponent<BaseAgent>().SetInitialized();

        agents.Add(agent);
    }

    void FixedUpdate() {
        if (World.Instance.IsWorldFullyLoaded()) {
            if (spawnCooldown > 0) {
                spawnCooldown--;
            }
            else {
                if (agents.Count < maxAgentCount) {
                    TilePos pos = LocationRegistration.worldEntryVehicle.GetAtRandom();
                    LocationNodeController lnc = World.Instance.GetChunkManager().GetTile(pos).GetComponent<LocationNodeController>();
                    CreateAgent(lnc);
                    Debug.Log("Spawning fresh agent at " + pos);
                    spawnCooldown = 120;
                }
            }
        }
    }
}