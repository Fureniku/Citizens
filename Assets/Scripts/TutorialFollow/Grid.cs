using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    [SerializeField, Range(1, 200)] private int width = 1;
    [SerializeField, Range(1, 200)] private int height = 1;

    private int prevWidth = 0;
    private int prevHeight = 0;

    private GameObject[,] grid = null;

    [SerializeField, Range(1, 200)] private float shapeWidth = 2.0f;
    [SerializeField, Range(1, 200)] private float shapeHeight = 2.0f;
    [SerializeField, Range(1, 200)] private float shapeDepth = 2.0f;
    
    [SerializeField, Range(1, 200)] private float maxRandomHeight = 10.0f;
    
    [SerializeField, Range(1, 200)] private int randomSeed = 10;
    private int prevSeed = -1;

    [SerializeField] private ShapeTypes shapeType = ShapeTypes.Cube;
    [SerializeField] private string randomShaderTypeName = "Transparent/Diffuse";

    void Start() {
        prevWidth = width;
        prevHeight = height;

        Random.InitState(randomSeed);
        prevSeed = randomSeed;

        grid = new GameObject[height, width];

        BuildGrid();
    }

    void BuildGrid() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                GameObject cell = null;
                if (grid[row, col] != null) { 
                    DestroyImmediate(grid[row, col]);
                }
                cell = new GameObject($"cell_{row}_{col}");
                cell.transform.parent = gameObject.transform;
                grid[row, col] = cell;

                cell.transform.position = new Vector3(shapeWidth * row, 0, shapeDepth * col);

                MeshFilter meshFilter = cell.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = cell.AddComponent<MeshRenderer>();

                meshFilter.mesh = new Cube {
                    Width = shapeWidth,
                    Height = shapeHeight,
                    Depth = shapeDepth
                }.Generate();

                meshRenderer.ApplyRandomMaterial(randomShaderTypeName, cell.name);
                
                //meshRenderer.material.EnableKeyword("_BaseMap");
                //meshRenderer.material.SetTexture("_BaseMap", buildingTexture);
            }
        }
    }
    
    void Update() {
        if (prevSeed != randomSeed) {
            BuildGrid();
            prevSeed = randomSeed;
        }

        if (prevWidth != width || prevHeight != height) {
            ClearAll();

            prevWidth = width;
            prevHeight = height;
            grid = new GameObject[height, width];
        }
    }

    private void ClearAll() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                if (grid[row, col] != null) {
                    DestroyImmediate(grid[row, col]);
                }
            }
        }
    }
}
