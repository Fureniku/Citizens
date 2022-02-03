using System.Collections;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {

    private bool combined = false;
    private SkyscraperGenerator generator = null;
    
    void Start() {
        generator = GetComponent<SkyscraperGenerator>();
    }

    void Update() {
        if (!combined && generator.IsGenerationComplete()) {
            CombineMeshes();
            combined = true;
        }
    }

    void CombineMeshes() {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        //Mesh finalMesh = new Mesh();
        ArrayList combiners = new ArrayList();
        ArrayList materials = new ArrayList();
        int initialSize = meshFilters.Length;
        
        Debug.Log("Preparing to combine " + initialSize + " meshes.");

        Quaternion initialRot = transform.rotation;
        Vector3 initialPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        for (int i = 0; i < meshFilters.Length; i++) {
            MeshFilter filter = meshFilters[i];
            if (filter.transform == transform) {
                continue;
            }
            
            MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
            for (int j = 0; j < filter.sharedMesh.subMeshCount; j++) {
                int matArrayIndex = Contains(materials, renderer.sharedMaterials[j].name);
                if (matArrayIndex == -1) {
                    materials.Add(renderer.sharedMaterials[j]);
                    matArrayIndex = materials.Count - 1;
                }

                combiners.Add(new ArrayList());

                CombineInstance instance = new CombineInstance();
                
                instance.subMeshIndex = j;
                instance.mesh = filter.sharedMesh;
                instance.transform = filter.transform.localToWorldMatrix;

                ((ArrayList) combiners[matArrayIndex]).Add(instance);
            }
        }
        
        //finalMesh.CombineMeshes(combiners);

        MeshFilter combinedMeshFilter = GetComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = GetComponent<MeshRenderer>();

        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int i = 0; i < materials.Count; i++) {
            CombineInstance[] combineInstanceArray = ((ArrayList) combiners[i]).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[i] = new Mesh();
            meshes[i].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[i] = new CombineInstance();
            combineInstances[i].mesh = meshes[i];
            combineInstances[i].subMeshIndex = 0;
        }

        combinedMeshFilter.sharedMesh = new Mesh();
        combinedMeshFilter.sharedMesh.CombineMeshes(combineInstances, false, false);

        Material[] matArray = materials.ToArray(typeof(Material)) as Material[];

        combinedMeshRenderer.materials = matArray;
        
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        transform.rotation = initialRot;
        transform.position = initialPos;
        
        Debug.Log("Completed reducing meshes from " + initialSize + " to " + materials.Count);
    }
    
    //From https://answers.unity.com/questions/196649/combinemeshes-with-different-materials.html
    private int Contains(ArrayList searchList, string searchName) {
        for (int i = 0; i < searchList.Count; i++) {
            if (((Material)searchList[i]).name == searchName) {
                return i;
            }
        }
        return -1;
    }
}
