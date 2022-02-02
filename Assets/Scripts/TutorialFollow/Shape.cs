using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shape {
    
    public ShapeTypes shapeType { get; set; }
    
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }

    public abstract Mesh Generate();

}
