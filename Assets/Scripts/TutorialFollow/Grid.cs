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

    [SerializeField] private GameObject buildingPrefab;

    void Start() {
        prevWidth = width;
        prevHeight = height;

        Random.InitState(randomSeed);
        prevSeed = randomSeed;

        grid = new GameObject[height, width];
        
        Debug.Log("Grid starting...");

        BuildGrid();
    }

    void BuildGrid() {
        Debug.Log("Starting build grid of total size " + height*width);
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                GenerateGridCell(row, col);
            }
            Debug.Log("Row complete");
        }
        Debug.Log("Grid generation complete");
        
    }

    private void GenerateGridCell(int row, int col) {
        GameObject cell = null;
        if (grid[row, col] != null) { 
            DestroyImmediate(grid[row, col]);
        }

        cell = Instantiate(buildingPrefab);
        cell.name = $"cell_{row}_{col}";
        cell.transform.parent = gameObject.transform;
        grid[row, col] = cell;

        cell.transform.position = new Vector3(shapeWidth * row, 0, shapeDepth * col);
    }
}
