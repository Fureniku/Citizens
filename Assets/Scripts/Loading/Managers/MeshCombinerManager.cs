using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombinerManager : GenerationSystem {
    
    private static List<GameObject> meshCombiners = new List<GameObject>();
    
    public override int GetGenerationPercentage() {
        return 0;
    }

    public override string GetGenerationString() {
        return "Combining meshes";
    }

    public override void Initialize() {
        Debug.Log(meshCombiners.Count + " mesh combiners registered");
        StartCoroutine(Combine());
    }

    public override void Process() {}
    
    public static void RegisterMeshCombiner(GameObject meshCombiner) {
        meshCombiners.Add(meshCombiner);
    }
    
    private IEnumerator Combine() {
        for (int i = 0; i < meshCombiners.Count; i++) {
            Debug.Log("combining mesh " + i);
                
            meshCombiners[i].GetComponent<MeshCombiner>().CombineMeshes();
            yield return null;
        }
        SetComplete();
        yield return null;
    }
}