using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : Shape {

    public override Mesh Generate() {
        shapeType = ShapeTypes.Quad;

        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(Width, 0, 0);
        vertices[2] = new Vector3(0, Height, 0);
        vertices[3] = new Vector3(Width, Height, 0);

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

        return mesh;
    }
}
