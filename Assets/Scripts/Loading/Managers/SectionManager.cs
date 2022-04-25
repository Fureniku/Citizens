using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class SectionManager : GenerationSystem {
    
    private List<Section> sections = new List<Section>();
    private int worldSize;

    private int progressZ;

    private bool scanComplete = false;
    private bool genStarted = false;
    private bool genComplete = false;

    private GenerateBuildingBase genLast = null;

    private bool gennedHospital = false;

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
        //First pass: large buildings
        GenerateHospital(TileRegistry.HOSPITAL_8x8);
        yield return null;
        
        //Rescan sections for next pass
        ClearSections();
        yield return Scan();
        
        //Second pass: subdivide remaining sections
        SubdivideSections();
        yield return null;
        
        //Rescan for next pass
        ClearSections();
        yield return Scan();
        Debug.Break();
        //Second pass: smaller buildings
        for (int i = 0; i < sections.Count+1; i++) {
            if (genLast != null) {
                genLast.CombineMeshes();
            }

            if (i < sections.Count) {
                Section s = sections[i];
                GenerateBuildingBase gen;
                
                int rng = Random.Range(0, 4);

                List<GenerateBuildingBase> viableBuildings = new List<GenerateBuildingBase>();

                viableBuildings.Add(new GenerateSmallBuilding(s.GetTilePos(), s.GetSizeX(), s.GetSizeZ(), TypeRegistries.SHOPS));
                viableBuildings.Add(new GenerateSmallBuilding(s.GetTilePos(), s.GetSizeX(), s.GetSizeZ(), TypeRegistries.HOUSES));
                
                if (s.CanFit(4, 3)) viableBuildings.Add(new GenerateCarPark(s.GetTilePos(), s.GetSizeX(), 3, 6, s.GetSizeZ()));
                if (s.CanFit(6, 3)) viableBuildings.Add(new GenerateOffice(s.GetTilePos(), s.GetSizeX(), 3, 6, s.GetSizeZ()));

                gen = viableBuildings[Random.Range(0, viableBuildings.Count - 1)];

                gen.Generate();
                genLast = gen;
            }
            
            yield return null;
        }
        genComplete = true;
        yield return null;
    }

    private void GenerateHospital(Tile hospital) {
        List<Section> viableSections = new List<Section>();
        int hospitalId = hospital.GetId();
        TileData hospitalTile = TileRegistry.GetTileFromID(hospitalId);

        if (!gennedHospital) {
            for (int i = 0; i < sections.Count; i++) {
                if (sections[i].CanFit(TileRegistry.GetTileFromID(hospitalId))) {
                    viableSections.Add(sections[i]);
                }
            }

            List<Section> bestSections = Attempt1(viableSections, hospitalTile.GetWidth(), hospitalTile.GetLength());

            if (bestSections == null) {
                bestSections = Attempt2(viableSections, hospitalTile.GetWidth(), hospitalTile.GetLength());
            }
            
            if (bestSections == null) {
                bestSections = Attempt3(viableSections, hospitalTile.GetWidth(), hospitalTile.GetLength());
            }

            if (bestSections == null) {
                bestSections = Attempt4(viableSections, hospitalTile.GetWidth(), hospitalTile.GetLength());
            }

            if (bestSections == null) {
                bestSections = Attempt5(viableSections, hospitalTile.GetWidth(), hospitalTile.GetLength());
            }

            if (bestSections != null) {
                Section chosenSection = bestSections[Random.Range(0, bestSections.Count - 1)];
                GenerateHospital gen = new GenerateHospital(chosenSection, hospitalId, EnumDirection.SOUTH, hospitalTile);
                gen.Generate();
                gen.PostGenerate();
                gennedHospital = true;
            }
            else {
                Debug.LogError("Hospital generation failed; no valid locations available.");
            }
        }
    }

    private void SubdivideSections() {
        for (int i = 0; i < sections.Count; i++) {
            Section s = sections[i];

            if (s.CanFit(10, 10)) {
                Debug.LogError("Subdividing!");
                SectionSubdivider gen = new SectionSubdivider(s, SubDividerType.HORI_STRIPED, 3);
                gen.Generate();
            }
        }
    }

    public void ClearSections() {
        for (int i = 0; i < sections.Count; i++) {
            sections[i].DeleteSection();
        }
        
        sections.Clear();
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
            } else {
                break;
            }
        }
        
        for (int col = 0; col < maxZ; col++) {
            TilePos tilePos = new TilePos(startPos.x, startPos.z + col);
            if (ValidTileForSection(tilePos)) {
                sizeZ++;
            } else {
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
    
    //SECTION ATTEMPTS for larger buildings to find ideal placement locations
#region LARGE_ATTEMPTS
    private List<Section> Attempt1(List<Section> viableSections, int width, int length) {
        //Attempt 1: A section of the exact size (8x8 = 8x8)
        List<Section> list = new List<Section>();
        for (int i = 0; i < viableSections.Count; i++) {
            Section s = viableSections[i];
            if (s.GetSizeX() == width && s.GetSizeZ() == length) {
                list.Add(s);
            }
        }

        if (list.Count >= 1) {
            Debug.Log("Attempt 1 was valid");
            return list;
        }
        return null;
    }
    
    private List<Section> Attempt2(List<Section> viableSections, int width, int length) {
        //Attempt 2: A section of the exact size on at least one side, with the larger side being +2 (8x8 = 8x10+ or 10+x8)
        List<Section> list = new List<Section>();
        for (int i = 0; i < viableSections.Count; i++) {
            Section s = viableSections[i];
            if ((s.GetSizeX() == width && s.GetSizeZ() > length+1) || (s.GetSizeX() > width+1 && s.GetSizeZ() == length)) {
                list.Add(s);
            }
        }
        
        if (list.Count >= 1) {
            Debug.Log("Attempt 2 was valid");
            return list;
        }
        return null;
    }
    
    private List<Section> Attempt3(List<Section> viableSections, int width, int length) {
        //Attempt 3: A section at least 2 bigger on both sides (8x8 = 10+x10+)
        List<Section> list = new List<Section>();
        for (int i = 0; i < viableSections.Count; i++) {
            Section s = viableSections[i];
            if (s.GetSizeX() > width+1 && s.GetSizeZ() > length+1) {
                list.Add(s);
            }
        }
        
        if (list.Count >= 1) {
            Debug.Log("Attempt 3 was valid");
            return list;
        }
        return null;
    }
    
    private List<Section> Attempt4(List<Section> viableSections, int width, int length) {
        //Attempt 4: A section at least 2 bigger on one side (8x8 = 9x10+ or 10+x9)
        List<Section> list = new List<Section>();
        for (int i = 0; i < viableSections.Count; i++) {
            Section s = viableSections[i];
            if ((s.GetSizeX() > width && s.GetSizeZ() > length+1) || (s.GetSizeX() > width+1 && s.GetSizeZ() > length)) {
                list.Add(s);
            }
        }
        
        if (list.Count >= 1) {
            Debug.Log("Attempt 4 was valid");
            return list;
        }
        return null;
    }
    
    private List<Section> Attempt5(List<Section> viableSections, int width, int length) {
        //Attempt 5: A section which is big enough (8x8 = 9x9)
        List<Section> list = new List<Section>();
        for (int i = 0; i < viableSections.Count; i++) {
            Section s = viableSections[i];
            if ((s.GetSizeX() > width && s.GetSizeZ() > length) || (s.GetSizeX() > width && s.GetSizeZ() > length)) {
                list.Add(s);
            }
        }
        
        if (list.Count >= 1) {
            Debug.Log("Attempt 5 was valid");
            return list;
        }
        return null;
    }
#endregion

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