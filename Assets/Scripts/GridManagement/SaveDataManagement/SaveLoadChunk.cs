using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GridManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class SaveLoadChunk : MonoBehaviour {

    public void SaveFile() {
        string dest = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(dest)) {
            file = File.OpenWrite(dest);
        }
        else {
            file = File.Create(dest);
        }

        ChunkData data = new ChunkData(17, null);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile() {
        string dest = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(dest)) {
            file = File.OpenRead(dest);
        }
        else {
            Debug.Log("No save exists");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        ChunkData data = (ChunkData) bf.Deserialize(file);
        file.Close();
    }

    public static void SerializeChunk(Chunk chunk, int x, int z) {
        JObject jObj = new JObject();
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                TileData td = chunk.GetChunkCellContents(j, i).GetComponent<TileData>();
                jObj.Add(td.SerializeTile(td, j, i));
            }
        }
        
        JProperty json = new JProperty("chunk", jObj);
        if (DirectoryExists()) {
            using (StreamWriter file = File.CreateText(Application.persistentDataPath + $"/{GridManager.Instance.GetWorldName()}/chunk_{x}_{z}.json"))
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
            return File.Exists(Application.persistentDataPath + $"/{GridManager.Instance.GetWorldName()}/chunk_{pos.x}_{pos.z}.json");
        }

        return false;
    }

    public static bool DirectoryExists() {
        DirectoryInfo dir = Directory.CreateDirectory(Application.persistentDataPath + $"/{GridManager.Instance.GetWorldName()}");
        return true;
    }
    
    public static GameObject[,] DeserializeChunk(ChunkPos origin) {
        GameObject[,] chunk = new GameObject[16, 16];
        if (DirectoryExists()) {
            using (StreamReader file = File.OpenText(Application.persistentDataPath + $"/{GridManager.Instance.GetWorldName()}/chunk_{origin.x}_{origin.z}.json")) {
                JObject o = (JObject) JToken.ReadFrom(new JsonTextReader(file));
                JProperty json = o.Property("chunk");
                JObject prop = (JObject) json.Value;

                for (int col = 0; col < 16; col++) {
                    for (int row = 0; row < 16; row++) {
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
