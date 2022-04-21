using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : GenerationSystem {
    
    private List<Section> sections = new List<Section>();
    private int worldSize;

    private int progressZ;

    private bool scanComplete = false;
    private bool genStarted = false;
    private bool genComplete = false;

    private GenerateBuildingBase genCPLast = null;
    
    public override void Initialize() {
        worldSize = World.Instance.GetChunkManager().GetSize() * Chunk.size;
        StartCoroutine(Scan());
    }
    
    public override void Process() {
        if (scanComplete && genComplete) {
            SetComplete();
        }
        
        if (scanComplete && !genStarted) {
            Debug.Log("Generating buildings");
            StartCoroutine(Populate());
            genStarted = true;
        }
    }

    private IEnumerator Populate() {
        for (int i = 0; i < sections.Count+1; i++) {
            if (genCPLast != null) {
                genCPLast.CombineMeshes();
            }

            if (i < sections.Count) {
                Section s = sections[i];
                GenerateBuildingBase genCP;

                int rng = Random.Range(0, 4);

                switch (rng) {
                    case 0:
                        genCP = new GenerateSmallBuilding(s.GetTilePos(), s.GetSizeX(), s.GetSizeZ(), TypeRegistries.SHOPS);
                        break;
                    case 1:
                        genCP = new GenerateCarPark(s.GetTilePos(), s.GetSizeX(), 3, 6, s.GetSizeZ());
                        break;
                    case 2:
                        genCP = new GenerateOffice(s.GetTilePos(), s.GetSizeX(), 3, 6, s.GetSizeZ());
                        break;
                    case 3:
                        genCP = new GenerateSmallBuilding(s.GetTilePos(), s.GetSizeX(), s.GetSizeZ(), TypeRegistries.HOUSES);
                        break;
                    default:
                        genCP = new GenerateSmallBuilding(s.GetTilePos(), s.GetSizeX(), s.GetSizeZ(), TypeRegistries.SHOPS);
                        break;
                }
                
                genCP.Generate();
                genCPLast = genCP;
            }
            
            yield return null;
        }
        genComplete = true;
        yield return null;
    }

    public IEnumerator Scan() {
        for (int col = 0; col < worldSize; col++) {
            for (int row = 0; row < worldSize; row++) {
                TilePos tilePos = new TilePos(row, col);
                TileData tileData = World.Instance.GetChunkManager().GetTile(tilePos);
                if (tileData != null && tileData.GetTile() == TileRegistry.GRASS) {
                    TileGrass grass = (TileGrass) tileData;

                    if (!grass.IsInSection()) {
                        sections.Add(NewSection(tilePos));
                    }
                }
            }
            progressZ = col;
            yield return null;
        }
        Debug.Log("section scanning complete. Created " + sections.Count + " sections.");
        scanComplete = true;
        yield return null;
    }
    
    Section NewSection(TilePos startPos) {
        
        int maxX = worldSize - startPos.x;
        int maxZ = worldSize - startPos.z;
        int sizeX = 0;
        int sizeZ = 0;

        for (int row = 0; row < maxX; row++) {
            TilePos tilePos = new TilePos(startPos.x + row, startPos.z);
            if (ValidTileForSection(tilePos)) {
                sizeX++;
            }
            else {
                break;
            }
        }
        
        for (int col = 0; col < maxZ; col++) {
            TilePos tilePos = new TilePos(startPos.x, startPos.z + col);
            if (ValidTileForSection(tilePos)) {
                sizeZ++;
            }
            else {
                break;
            }
        }

        for (int col = 0; col < sizeZ; col++) {
            for (int row = 0; row < sizeX; row++) {
                TilePos tilePos = new TilePos(startPos.x + row, startPos.z + col);
                TileData tileData = World.Instance.GetChunkManager().GetTile(tilePos);
                if (tileData.GetTile() == TileRegistry.GRASS) {
                    TileGrass grass = (TileGrass) tileData;
                    grass.AddToSection();
                }
                else {
                    Debug.LogError("Tile " + tileData.name + " @ " + tilePos +  "was not grass; section is wrong!");
                }
            }
        }
        
        return new Section(startPos, sizeX, sizeZ);
    }

    private bool ValidTileForSection(TilePos pos) {
        TileData tileData = World.Instance.GetChunkManager().GetTile(pos);
        
        if (tileData.GetTile() == TileRegistry.GRASS) {
            TileGrass grass = (TileGrass) tileData;
            if (!grass.IsInSection()) {
                return true;
            }
        }

        return false;
    }

    public override int GetGenerationPercentage() {
        return (int)(progressZ / (float)worldSize * 100);
    }

    public override string GetGenerationString() {
        return "Generating building sections";
    }

    /*private void OnDrawGizmos() {
        for (int i = 0; i < sections.Count; i++) {
            Gizmos.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            Section s = sections[i];
            Vector3 pos = s.GetTilePos().GetWorldPos();
            float tileSize = World.Instance.GetChunkManager().GetGridTileSize();
            Vector3 drawPos = new Vector3(pos.x + (s.GetSizeX()*tileSize/2), 10.0f, pos.z + (s.GetSizeZ()*tileSize/2));
            Gizmos.DrawCube(drawPos, new Vector3(s.GetSizeX()*tileSize, 0.1f, s.GetSizeZ()*tileSize));
        }
    }*/
}