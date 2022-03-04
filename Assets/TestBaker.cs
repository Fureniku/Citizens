using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBaker : MonoBehaviour
{
    void Start() {
        GetComponent<NavMeshSurface>().BuildNavMesh();
        Debug.Log("Building navmesh");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
