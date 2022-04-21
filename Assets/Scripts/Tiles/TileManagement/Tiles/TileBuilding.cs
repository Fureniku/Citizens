using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class TileBuilding : TileData {

    [SerializeField] protected int minWidth = 4;
    [SerializeField] protected int minLength = 3;
    [SerializeField] protected int minHeight = 30;
    
    [SerializeField] protected int maxWidth = 10;
    [SerializeField] protected int maxLength = 10;
    [SerializeField] protected int maxHeight = 60;

    [SerializeField] protected float minScale = 1.0f;
    [SerializeField] protected float maxScale = 1.0f;
    
    [ReadOnly, SerializeField] protected int height = 30;
    
    protected bool generationComplete = false;

    protected int segmentHeight;
    protected float scale;

    protected bool skyscraperCreated;

    void Start() {
        Debug.Log("Tile building start called");
        width = Random.Range(minWidth, maxWidth);
        length = Random.Range(minLength, maxLength);
        height = Random.Range(minHeight, maxHeight);
        scale = Random.Range(minScale, maxScale);

        Initialize();
        SetInitialPos();
    }

    public bool IsGenerationComplete() {
        return generationComplete;
    }

    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
    }

    public override void CreateFromRegistry() {
        CreateBase();
    }
}
