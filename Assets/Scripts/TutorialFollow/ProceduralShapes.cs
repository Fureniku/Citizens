using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class ProceduralShapes : MonoBehaviour {

    [SerializeField] private float width = 10.0f;
    [SerializeField] private float height = 10.0f;
    [SerializeField] private float depth = 10.0f;
    [SerializeField] private string randomShaderTypeName = "Transparent/Diffuse";
    [SerializeField] private ShapeTypes shapeType = ShapeTypes.Quad;
    
    private Mesh mesh;
    
#region RNG Implementation
    [SerializeField] private bool shouldGenRandomSizes = false;
    [SerializeField, Range(1, 100)] int randomSeed = 10;
    [SerializeField, Range(1.0f, 30.0f)] private float maxRandSize = 10.0f;
    private int previousRandomSeed = -1;
    private bool matApplied = false;
#endregion
    
    void Start() {
        Random.InitState(randomSeed);
    }

    // Update is called once per frame
    void Update() {
        if (shapeType == ShapeTypes.Quad || shapeType == ShapeTypes.Cube) {
            Generate();
        } else if (previousRandomSeed != randomSeed && shouldGenRandomSizes) {
            previousRandomSeed = randomSeed;
            GenRandomSizes();
            Generate();
        }
    }

    void Generate() {
        if (shapeType == ShapeTypes.Quad) {
            mesh = new Quad {
                Width = width,
                Height = height
            }.Generate();
        } else if (shapeType == ShapeTypes.Cube) {
            mesh = new Cube {
                Width = width,
                Height = height,
                Depth = depth
            }.Generate();
        }

        GetComponent<MeshFilter>().mesh = mesh;
        
        if (!matApplied) {
            GetComponent<MeshRenderer>().ApplyRandomMaterial(randomShaderTypeName, gameObject.name);
            matApplied = true;
        }
    }

    void GenRandomSizes() {
        width = Random.Range(1.0f, maxRandSize);
        height = Random.Range(1.0f, maxRandSize);
        depth = Random.Range(1.0f, maxRandSize);
    }
}
