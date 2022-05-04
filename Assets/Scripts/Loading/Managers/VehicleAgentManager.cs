using System.Collections;
using Tiles.TileManagement;
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
        Registry initialSpawnerRegistry = LocationRegistration.RoadSpawnerRegistry;
        
        int initialAgents = initialAgentCount;
        if (initialAgents > initialSpawnerRegistry.GetListSize()) {
            initialAgents = initialSpawnerRegistry.GetListSize() - 1;
            Debug.Log("Capping initial agents at " + initialAgents + " due to world size");
        }
        for (int i = 0; i < initialAgents; i++) {
            Vector3 spawnPos = initialSpawnerRegistry.GetAtRandom().GetWorldPos();
            CreateAgentInitial(spawnPos);

            message = "Created vehicle " + i + " of " + initialAgents;
            yield return null;
        }

        for (int i = 0; i < initialAgents / 2; i++) {
            CreateAgentIdle();
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
    
    private void CreateAgentIdle() {
        ParkingSpaceNode spawnPoint = null;
        int selectionAttempts = 0;

        while (spawnPoint == null && selectionAttempts < 3) { //Try to select at random
            spawnPoint = LocationRegistration.GetRandomParkingSpaceNode();
            if (spawnPoint.IsOccupied()) {
                spawnPoint = null;
                selectionAttempts++;
            }
        }

        if (spawnPoint == null) { //Random wasn't working, pick first available space
            for (int i = 0; i < LocationRegistration.carParkSpaces.Count; i++) {
                ParkingSpaceNode node = LocationRegistration.carParkSpaces[i];
                if (!node.IsOccupied()) {
                    spawnPoint = node;
                    break;
                }
            }

            if (spawnPoint == null) { //Every registered parking space on the map is full.
                Debug.LogWarning("All car parks are full. Skipping parked vehicle generation.");
                return;
            }
        }
        
        
        GameObject agent = Instantiate(VehicleRegistry.GetRandomCar(), spawnPoint.transform.position, Quaternion.Euler(0, spawnPoint.GetRotation().GetRotation(), 0));
        spawnPoint.ClaimSpace();
        spawnPoint.OccupySpace();
        Debug.Log("Creating idle agent at spawn point " + spawnPoint.name + ": " + spawnPoint.transform.position);
        agent.transform.parent = transform;
        agent.name = "VA_" + id + ": " + agent.GetComponent<Vehicle>().GetName() + " (" + agent.GetComponent<Vehicle>().GetColour() + ")";
        id++;
        agent.GetComponent<VehicleAgent>().SetAStar(aStarPlane.GetComponent<AStar>());

        agent.GetComponent<VehicleAgent>().SetSpawnController(spawnPoint.GetParkingController().GetLocationNodeController());
        agent.GetComponent<VehicleAgent>().InitIdle();
        agent.GetComponent<BaseAgent>().SetInitialized();

        agentsIdle.Add(agent);
    }

    protected override void AgentUpdate() {
        if (World.Instance.IsWorldFullyLoaded()) {
            if (spawnCooldown > 0) {
                spawnCooldown--;
            } else {
                if (currentAgentCount < maxAgentCount) {
                    TilePos pos = LocationRegistration.allVehicleSpawnersRegistry.GetAtRandom();
                    LocationNodeController lnc = World.Instance.GetChunkManager().GetTile(pos).GetComponent<LocationNodeController>();
                    CreateAgent(lnc);
                    spawnCooldown = 60;
                }
            }
        }
    }
}