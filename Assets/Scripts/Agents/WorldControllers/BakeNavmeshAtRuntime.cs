using System;
using Loading.States;
using UnityEngine;
using UnityEngine.AI;

public class BakeNavmeshAtRuntime : MonoBehaviour {

    [SerializeField] private TileData parentTile;

    private void Awake() {
        if (!parentTile.IsRegistryVersion()) {
            GenNavMeshLoadState.extraMeshes.Add(GetComponent<NavMeshSurface>());
        }
    }
}