using UnityEngine;

[System.Serializable]
public class Position {
    [SerializeField] public int x;
    [SerializeField] public int z;
        
    public Position(int x, int z) {
        this.x = x;
        this.z = z;
    }
    
    public override string ToString() {
        return "[" + x + "," + z + "]";
    }
}