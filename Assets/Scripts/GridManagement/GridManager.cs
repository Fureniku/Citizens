using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {
    
    private static GridManager _instance;
    public static GridManager Instance {
        get { return _instance; }
    }
    
    [SerializeField, Range(5, 50)] private int width = 25;
    [SerializeField, Range(5, 50)] private int height = 25;

    private GameObject[,] grid = null;

    [SerializeField, Range(1, 200)] private float gridSlotSize = 50.0f;
    [SerializeField, Range(1, 200)] private int randomSeed = 10;
    private int prevSeed = -1;

    [SerializeField] private string randomShaderTypeName = "Transparent/Diffuse";

    [SerializeField] private GameObject defaultTilePrefab = null;

    private bool hasStarted = false;
    private bool hasCompletedInit = false;
    
    void Awake() {
        Debug.Log("Initialize grid manager");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }
    
    void Start() {
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
            GameObject rowGO = GetRowParent(row);
            for (int col = 0; col < width; col++) {
                FillGridCell(defaultTilePrefab, rowGO, row, col, 0);
                yield return null;
            }
            Debug.Log("Row complete");
        }
        Debug.Log("Grid generation complete");
        hasCompletedInit = true;
    }

    public bool IsValidLocation(TilePos pos) {
        return (pos.x <= width && pos.z <= height);
    }
    
    public void FillGridCell(GameObject go, GameObject parent, int row, int col, int rotation) {
        GameObject cell = null;
        if (grid[row, col] != null) { 
            DestroyImmediate(grid[row, col]);
        }

        cell = Instantiate(go, parent.transform, true);
        cell.name = $"cell_{row}_{col}";
        grid[row, col] = cell;
        cell.transform.position = new Vector3(gridSlotSize * row, 0, gridSlotSize * col) + transform.position;
        cell.transform.rotation = Quaternion.Euler(0,rotation,0);
        TilePos.GetGridPosFromLocation(cell.transform.position, this);
    }

    public void FillGridCell(GameObject go, int row, int col, int rotation) {
        FillGridCell(go, GetRowParent(row), row, col, rotation);
    }

    private GameObject GetRowParent(int row) {
        Transform rowTransform = transform.Find($"row_{row}");
        GameObject rowParent = null;
        if (rowTransform == null) {
            rowParent = new GameObject($"row_{row}");
            rowParent.transform.parent = transform;
        }
        else {
            rowParent = rowTransform.gameObject;
        }

        return rowParent;
    }
    
    public GameObject GetGridCellContents(int row, int col) {
        return grid[row, col];
    }

    public GameObject GetGridCellContents(TilePos pos) {
        return grid[pos.x, pos.z];
    }
    
    public int GetMaxGridX() {
        return width;
    }

    public int GetMaxGridY() {
        return height;
    }

    public float GetGridTileSize() {
        return gridSlotSize;
    }

    public bool IsInitialized() {
        return hasCompletedInit;
    }
}
