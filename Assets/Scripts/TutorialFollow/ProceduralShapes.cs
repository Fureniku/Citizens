using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class ProceduralShapes : MonoBehaviour {

    [SerializeField] private float width = 10.0f;
    [SerializeField] private float height = 10.0f;
    [SerializeField] private float depth = 10.0f;
    [SerializeField] private string randomShaderTypeName = "Transparent/Diffuse";
    [SerializeField] private ShapeType shapeType = ShapeType.Quad;
    
    private Mesh mesh;

    private enum ShapeType {
        Quad,
        Cube
    }
    
    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        ApplyRandomMaterial();
    }

    // Update is called once per frame
    void Update() {
        if (shapeType == ShapeType.Quad) {
            GenerateQuad(width, height);
        } else if (shapeType == ShapeType.Cube) {
            GenerateCube(width, height, width);
        }
        
    }

    void GenerateQuad(float newWidth, float newHeight) {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(newWidth, 0, 0);
        vertices[2] = new Vector3(0, newHeight, 0);
        vertices[3] = new Vector3(newWidth, newHeight, 0);

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;

        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        Vector2[] UVs = new Vector2[4];
        UVs[0] = new Vector2(0, 0);
        UVs[1] = new Vector2(1, 0);
        UVs[2] = new Vector2(0, 1);
        UVs[3] = new Vector2(1, 1);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = UVs;

        mesh.Optimize();
    }

    void GenerateCube(float x1, float y1, float z1) {
        int faces = 6;
        float x0 = 0;
        float y0 = 0;
        float z0 = 0;
        
        Vector3[] vertices = new Vector3[faces*4];
        Vector3 vNBL = new Vector3(x0, y0, z0);
        Vector3 vNBR = new Vector3(x1, y0, z0);
        Vector3 vNTL = new Vector3(x0, y1, z0);
        Vector3 vNTR = new Vector3(x1, y1, z0);
        
        Vector3 vFBL = new Vector3(x0, y0, z1);
        Vector3 vFBR = new Vector3(x1, y0, z1);
        Vector3 vFTL = new Vector3(x0, y1, z1);
        Vector3 vFTR = new Vector3(x1, y1, z1);
        
        //Front
        vertices[0] = vNBL;
        vertices[1] = vNBR;
        vertices[2] = vNTL;
        vertices[3] = vNTR;
        //Top
        vertices[4] = vNTL;
        vertices[5] = vNTR;
        vertices[6] = vFTL;
        vertices[7] = vFTR;
        //right
        vertices[8]  = vNBR;
        vertices[9]  = vFBR;
        vertices[10] = vNTR;
        vertices[11] = vFTR;
        //back
        vertices[12] = vFBR;
        vertices[13] = vFBL;
        vertices[14] = vFTR;
        vertices[15] = vFTL;
        //bottom
        vertices[16] = vFBL;
        vertices[17] = vFBR;
        vertices[18] = vNBL;
        vertices[19] = vNBR;
        //left
        vertices[20] = vFBL;
        vertices[21] = vNBL;
        vertices[22] = vFTL;
        vertices[23] = vNTL;
        

        int[] triangles = new int[faces*6];

        for (int i = 0; i < faces; i++) {
            triangles[0 + (i * 6)] = 0 + (i * 4);
            triangles[1 + (i * 6)] = 2 + (i * 4);
            triangles[2 + (i * 6)] = 1 + (i * 4);
            triangles[3 + (i * 6)] = 2 + (i * 4);
            triangles[4 + (i * 6)] = 3 + (i * 4);
            triangles[5 + (i * 6)] = 1 + (i * 4);
        }

        Vector3[] normals = new Vector3[faces*4];
        for (int i = 0; i < faces * 4; i++) {
            if (i <= 3) normals[i]  = Vector3.forward; //front
            if (i <= 7) normals[i]  = Vector3.up; //top
            if (i <= 11) normals[i] = Vector3.right; //right
            if (i <= 15) normals[i] = Vector3.back; //back
            if (i <= 19) normals[i] = Vector3.down; //bottom
            if (i <= 23) normals[i] = Vector3.left; //left
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        
        mesh.Optimize();
    }
    
    void ApplyRandomMaterial() {
        Material randomMat = new Material(Shader.Find(randomShaderTypeName));
        randomMat.name = $"{gameObject.name}_material";
        randomMat.color = GetRandomColour();

        randomMat.EnableKeyword("_EMISSION");
        randomMat.SetColor("_EmissionColor", randomMat.color);

        GetComponent<Renderer>().material = randomMat;
    }

    Color GetRandomColour() => Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

}
