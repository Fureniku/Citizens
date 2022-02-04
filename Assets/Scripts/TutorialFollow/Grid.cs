using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    [SerializeField, Range(5, 50)] private int width = 25;
    [SerializeField, Range(5, 50)] private int height = 25;

    private int prevWidth = 0;
    private int prevHeight = 0;

    private GameObject[,] grid = null;

    [SerializeField, Range(1, 200)] private float shapeWidth = 2.0f;
    [SerializeField, Range(1, 200)] private float shapeHeight = 2.0f;
    [SerializeField, Range(1, 200)] private float shapeDepth = 2.0f;
    
    [SerializeField, Range(1, 200)] private float maxRandomHeight = 10.0f;
    
    [SerializeField, Range(1, 200)] private int randomSeed = 10;
    private int prevSeed = -1;

    [SerializeField] private string randomShaderTypeName = "Transparent/Diffuse";

    [SerializeField] private GameObject buildingPrefab;

    private bool hasStarted = false;

    void Start() {
        prevWidth = width;
        prevHeight = height;

        Random.InitState(randomSeed);
        prevSeed = randomSeed;

        grid = new GameObject[height, width];
    }

    void Update() {
        if (!hasStarted) {
            Debug.Log("Grid starting...");
            StartCoroutine(BuildGrid());
            hasStarted = true;
        }
    }

    IEnumerator BuildGrid() {
        Debug.Log("Starting build grid of total size " + height*width);
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                FillGridCell(buildingPrefab, row, col);
                yield return null;
            }
            Debug.Log("Row complete");
        }
        Debug.Log("Grid generation complete");
    }

    private void FillGridCell(GameObject go, int row, int col) {
        GameObject cell = null;
        if (grid[row, col] != null) { 
            DestroyImmediate(grid[row, col]);
        }

        cell = Instantiate(go, gameObject.transform, true);
        cell.name = $"cell_{row}_{col}";
        grid[row, col] = cell;

        cell.transform.position = new Vector3(shapeWidth * row, 0, shapeDepth * col);
    }

    GameObject GetGridCellContents(int row, int col) {
        return grid[row, col];
    }
}
