using UnityEngine;

public class MeshTile : MonoBehaviour {
    public Mesh mesh;
    public Material[] materials;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGrid : MonoBehaviour {
    public MeshTile[] meshTiles;
    SmartMeshData[] meshData;

    void Awake() {
        meshData = new SmartMeshData[meshTiles.Length];
        for (int i = 0; i < meshTiles.Length; i++) {
            meshData[i] = new SmartMeshData(meshTiles[i].mesh, meshTiles[i].materials, meshTiles[i].transform.localPosition, meshTiles[i].transform.localRotation);
        }

        Mesh combinedMesh = GetComponent<MeshFilter>().mesh;
        combinedMesh.name = "Combined Mesh";
        Material[] combinedMaterials;

        combinedMesh.CombineMeshesSmart(meshData, out combinedMaterials);

        GetComponent<MeshRenderer>().sharedMaterials = combinedMaterials;
    }
}