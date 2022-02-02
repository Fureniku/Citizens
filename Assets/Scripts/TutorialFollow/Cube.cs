using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Shape {
    
    public override Mesh Generate() {
        shapeType = ShapeTypes.Cube;
        Mesh mesh = new Mesh();
        
        int faces = 6;
        float x0 = 0;
        float y0 = 0;
        float z0 = 0;
        float x1 = Width;
        float y1 = Random.Range(10, 50);
        float z1 = Depth;

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

        
        normals[0] = Vector3.forward; //front
        normals[1] = Vector3.forward; //front
        normals[2] = Vector3.forward; //front
        normals[3] = Vector3.forward; //front
        
        normals[4] = Vector3.up; //top
        normals[5] = Vector3.up; //top
        normals[6] = Vector3.up; //top
        normals[7] = Vector3.up; //top
        
        normals[8]  = Vector3.right; //right
        normals[9]  = Vector3.right; //right
        normals[10] = Vector3.right; //right
        normals[11] = Vector3.right; //right
        
        normals[12] = Vector3.back; //back
        normals[13] = Vector3.back; //back
        normals[14] = Vector3.back; //back
        normals[15] = Vector3.back; //back
        
        normals[16] = Vector3.down; //bottom
        normals[17] = Vector3.down; //bottom
        normals[18] = Vector3.down; //bottom
        normals[19] = Vector3.down; //bottom
        
        normals[20] = Vector3.left; //left
        normals[21] = Vector3.left; //left
        normals[22] = Vector3.left; //left
        normals[23] = Vector3.left; //left

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        
        mesh.Optimize();

        return mesh;
    }
}
