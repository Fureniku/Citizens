using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry {
    
    private List<TilePos> list = new List<TilePos>();

    public int GetListSize() {
        return list.Count;
    }

    public void AddToList(TilePos pos) {
        if (!list.Contains(pos)) {
            list.Add(pos);
        }
    }

    public TilePos GetFromList(int id) {
        return list[id];
    }

    public TilePos GetAtRandom() {
        Debug.Log("Getting random destination from list with size " + list.Count);
        if (list.Count == 1) {
            return GetFromList(0);
        }
        int rnd = Random.Range(0, list.Count);
        return GetFromList(rnd);
    }

    public void RemoveFromList(TilePos pos) {
        list.Remove(pos);
    }

    public List<TilePos> GetList() {
        return list;
    }
}