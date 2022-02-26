using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SaveLoadChunk : MonoBehaviour {
    
    public static void SerializeChunk(Chunk chunk) {
        JObject jObj = new JObject();
        for (int row = 0; row < 16; row++) {
            for (int col = 0; col < 16; col++) {
                TileData td = chunk.GetChunkCellContents(row, col).GetComponent<TileData>();
                jObj.Add(td.SerializeTile(td, row, col));
            }
        }
        
        JProperty json = new JProperty("chunk", jObj);
        if (DirectoryExists()) {
            using (StreamWriter file = File.CreateText(Application.persistentDataPath + $"/{World.Instance.GetWorldName()}/chunk_{chunk.GetPosition().x}_{chunk.GetPosition().z}.json"))
            using (JsonWriter writer = new JsonTextWriter(file)) {
                writer.Formatting = Formatting.Indented;
                JObject final = new JObject();
                final.Add(json);
                final.WriteTo(writer);

            }
        }
    }

    public static bool FileExists(ChunkPos pos) {
        if (DirectoryExists()) {
            return File.Exists(Application.persistentDataPath + $"/{World.Instance.GetWorldName()}/chunk_{pos.x}_{pos.z}.json");
        }

        return false;
    }

    public static bool DirectoryExists() {
        DirectoryInfo dir = Directory.CreateDirectory(Application.persistentDataPath + $"/{World.Instance.GetWorldName()}");
        return true;
    }
    
    public static GameObject[,] DeserializeChunk(ChunkPos origin) {
        GameObject[,] chunk = new GameObject[16, 16];
        if (DirectoryExists()) {
            using (StreamReader file = File.OpenText(Application.persistentDataPath + $"/{World.Instance.GetWorldName()}/chunk_{origin.x}_{origin.z}.json")) {
                JObject o = (JObject) JToken.ReadFrom(new JsonTextReader(file));
                JProperty json = o.Property("chunk");
                JObject prop = (JObject) json.Value;

                for (int row = 0; row < 16; row++) {
                    for (int col = 0; col < 16; col++) {
                        GameObject cell = DeserializeTile((JObject) prop.GetValue($"tile_{row}_{col}"), new TilePos((origin.x*Chunk.size) + row, (origin.z*Chunk.size) + col));
                        cell.name = $"cell_{row}_{col}";
                        chunk[row, col] = cell;
                    }
                }
            }
        }
        return chunk;
    }

    public static GameObject DeserializeTile(JObject json, TilePos pos) {
        int id = TileData.ParseInt(json.GetValue("id"));
        GameObject go = Instantiate(TileRegistry.GetGameObjectFromID(id));
        go.GetComponent<TileData>().DeserializeTile(json);
        go.transform.position = TilePos.GetWorldPosFromTilePos(pos);
        return go;
    }
}
