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

    public static void SerializeChunk() {
        JObject jObj = new JObject();
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                TileData td = GridManager.Instance.GetGridCellContents(j, i).GetComponent<TileData>();
                jObj.Add(td.SerializeTile(j, i));
            }
        }
        
        JProperty json = new JProperty("chunk", jObj);
        
        Debug.Log(json);

        using (StreamWriter file = File.CreateText(Application.persistentDataPath + "/chunk.json"))
        using (JsonWriter writer = new JsonTextWriter(file)) {
            writer.Formatting = Formatting.Indented;
            JObject final = new JObject();
            final.Add(json);
            final.WriteTo(writer);
            
        }
    }
    
    public static GameObject[,] DeserializeChunk(TilePos origin) {
        GameObject[,] chunk = new GameObject[16, 16];
        using (StreamReader file = File.OpenText(Application.persistentDataPath + "/chunk.json")) {
            JObject o = (JObject) JToken.ReadFrom(new JsonTextReader(file));
            JProperty json = o.Property("chunk");
            JObject prop = (JObject) json.Value;

            for (int col = 0; col < 16; col++) {
                for (int row = 0; row < 16; row++) {
                    GameObject cell = DeserializeTile((JObject) prop.GetValue($"tile_{row}_{col}"), new TilePos(origin.x + row, origin.z + col));
                    cell.name = $"cell_{row}_{col}";
                    chunk[row, col] = cell;
                }
            }
        }

        return chunk;
    }

    public static GameObject DeserializeTile(JObject json, TilePos pos) {
        int id = TileData.ParseInt(json.GetValue("id"));
        GameObject go = Instantiate(TileRegistry.GetGameObjectFromID(id));
        Debug.Log("Created " + go.name);
        if (go == null) {
            Debug.Log("error loading gameobject with id " + id);
        }
        go.GetComponent<TileData>().DeserializeTile(json);
        go.transform.position = TilePos.GetWorldPosFromTilePos(pos);
        return go;
    }
}
