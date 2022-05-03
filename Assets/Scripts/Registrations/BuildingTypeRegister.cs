using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingTypeRegister {

    public List<Tile> buildings = new List<Tile>();

    private int lastBuilding = 0;
    private System.Random rng = new System.Random();

    public void AddEntry(Tile tile) {
        buildings.Add(tile);
    }

    public int GetTotal() {
        return buildings.Count;
    }

    public Tile GetBuilding(int id) {
        return buildings[id];
    }

    public Tile GetRandomBuilding() {
        return buildings[Random.Range(0, buildings.Count)];
    }

    public Tile GetNextBuilding() {
        Tile building;
        if (lastBuilding < buildings.Count) {
            building = buildings[lastBuilding];
            lastBuilding++;
        } else {
            buildings = buildings.OrderBy(a => rng.Next()).ToList(); //Shuffle list and repeat
            building = buildings[0];
            lastBuilding = 1;
        }

        return building;
    }
}