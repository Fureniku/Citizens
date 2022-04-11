using System.Collections;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {
    
    public void CombineMeshes() {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(); //Get all the filters from children of current object
        ArrayList combiners = new ArrayList();
        ArrayList materials = new ArrayList();
        int initialSize = meshFilters.Length;

        //Save current position/rotation
        Quaternion initialRot = transform.rotation;
        Vector3 initialPos = transform.position;
        //Reset to zero before processing
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        for (int i = 0; i < meshFilters.Length; i++) { //Iterate through all existing meshes
            MeshFilter filter = meshFilters[i];
            if (filter.transform == transform) { //Don't operate on the parent object (probably unneeded as we delete them anyway)
                //Debug.Log("Skipping parent");
                continue;
            }
            
            MeshRenderer subRenderer = filter.GetComponent<MeshRenderer>(); //Get the current selected child renderer

            for (int j = 0; j < filter.sharedMesh.subMeshCount; j++) {
                //Check if the current material is in our array, and if not, add it.
                int matArrayIndex = Contains(materials, subRenderer.sharedMaterials[j].name);
                if (matArrayIndex == -1) {
                    //Debug.Log("Added material " + subRenderer.sharedMaterials[j].name);
                    materials.Add(subRenderer.sharedMaterials[j]);
                    matArrayIndex = materials.Count - 1;
                }

                combiners.Add(new ArrayList());

                //Create a combine instance and set properties
                CombineInstance instance = new CombineInstance();
                instance.subMeshIndex = j;
                instance.mesh = filter.sharedMesh;
                instance.transform = filter.transform.localToWorldMatrix;
                //Add instance to arraylist
                ((ArrayList) combiners[matArrayIndex]).Add(instance);
            }
        }
        
        Debug.Log("Combiners has " + combiners.Count + " entries");

        MeshFilter combinedMeshFilter = GetComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = GetComponent<MeshRenderer>();

        //Create new arrays for all material/meshes
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int i = 0; i < materials.Count; i++) {
            //Set mesh data etc for each individual material into array
            CombineInstance[] combineInstanceArray = ((ArrayList) combiners[i]).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[i] = new Mesh();
            meshes[i].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[i].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[i] = new CombineInstance();
            combineInstances[i].mesh = meshes[i];
            combineInstances[i].subMeshIndex = 0;
        }

        //Combine all single-material meshes into one
        combinedMeshFilter.sharedMesh = new Mesh();
        combinedMeshFilter.sharedMesh.CombineMeshes(combineInstances, false, false);

        Material[] matArray = materials.ToArray(typeof(Material)) as Material[];

        combinedMeshRenderer.materials = matArray;
        
        //Destroy the original children for performance
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        //Move back to original location
        transform.rotation = initialRot;
        transform.position = initialPos;
        
        Debug.Log("Completed reducing meshes from " + initialSize + " to " + materials.Count);
    }

    //From https://answers.unity.com/questions/196649/combinemeshes-with-different-materials.html
    //Searches an array and returns the index of a requested object, or -1 if not found
    private int Contains(ArrayList searchList, string searchName) {
        for (int i = 0; i < searchList.Count; i++) {
            if (((Material)searchList[i]).name == searchName) {
                return i;
            }
        }
        return -1;
    }
    
    
    /* No longer used?
    Mesh CombineMeshes(MeshFilter[] meshes) {
        // Key: shared mesh instance ID, Value: arguments to combine meshes
        var helper = new Dictionary<int, List<CombineInstance>>();

        // Build combine instances for each type of mesh
        foreach (var m in meshes) {
            List<CombineInstance> tmp;
            if (!helper.TryGetValue(m.sharedMesh.GetInstanceID(), out tmp)) {
                tmp = new List<CombineInstance>();
                helper.Add(m.sharedMesh.GetInstanceID(), tmp);
            }

            var ci = new CombineInstance();
            ci.mesh = m.sharedMesh;
            ci.transform = m.transform.localToWorldMatrix;
            tmp.Add(ci);
        }

        // Combine meshes and build combine instance for combined meshes
        var list = new List<CombineInstance>();
        foreach (var e in helper) {
            var m = new Mesh();
            m.CombineMeshes(e.Value.ToArray());
            var ci = new CombineInstance();
            ci.mesh = m;
            list.Add(ci);
        }

        // And now combine everything
        var result = new Mesh();
        result.CombineMeshes(list.ToArray(), false, false);

        // It is a good idea to clean unused meshes now
        foreach (var m in list) {
            Destroy(m.mesh);
        }

        return result;
    } */
}
