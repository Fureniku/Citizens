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
                jObj.Add(SerializeTile(j, i));
                
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

    public static JProperty SerializeTile(int row, int col) {
        TileData data = GridManager.Instance.GetGridTile(row, col);
            
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetGridPos().x));
        jObj.Add(new JProperty("col", data.GetGridPos().z));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }


    public static void DeserializeChunk() {
        using (StreamReader file = File.OpenText(Application.persistentDataPath + "/chunk.json")) {
            JObject o = (JObject) JToken.ReadFrom(new JsonTextReader(file));
            JProperty json = o.Property("chunk");
            JObject prop = (JObject) json.Value;

            for (int col = 0; col < 16; col++) {
                for (int row = 0; row < 16; row++) {
                    Debug.Log("Deserialize " + row + ", " + col);
                    DeserializeTile((JObject) prop.GetValue($"tile_{row}_{col}"));
                }
            }
            
            
            /*JsonTextReader reader = new JsonTextReader(file);

            while (reader.Read()) {
                Debug.Log("Read!");
                if (reader.Value != null) {
                    if (reader.Value.ToString().Contains("tile_")) {
                        Debug.Log("Starting a tile!");
                    }
                    Debug.Log("Token type: " + reader.TokenType + ", value: " + reader.Value);
                }
                else {
                    Debug.Log("Token type: " + reader.TokenType);
                }
            }*/
        }
    }

    public static void DeserializeTile(JObject json) {
        int id = ParseInt(json.GetValue("id"));
        int rotation = ParseInt(json.GetValue("rotation"));
        int row = ParseInt(json.GetValue("row"));
        int col = ParseInt(json.GetValue("col"));
        
        Debug.Log("Deserialized tile!");
        Debug.Log("ID is " + id + ", which is " + TileRegistry.GetTileFromID(id));
        Debug.Log("Rotation is " + rotation + ", which is " + Direction.GetDirection(rotation));
        Debug.Log("And the position is " + row + ", " + col);
    }

    public static int ParseInt(JToken token) {
        int result = 0;

        if (token != null) {
            try {
                result = Int32.Parse(token.ToString());
            }
            catch (FormatException) {
                Debug.Log("SaveLoadChunk failed to parse int from string: " + token);
            }
        }
        else {
            Debug.Log("SaveLoadChunk: Token was null!");
        }
        
        

        return result;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

    /*public static void DeserializeChunkOld() {
        FileStream fileStream = new FileStream(Application.persistentDataPath + "/chunk.xml", FileMode.Open);
        XmlTextReader xml = new XmlTextReader(new StreamReader(fileStream));
        Debug.Log("Attempting file deserialization");
        DeserializeTileOld(xml);
        
        xml.Close();
    }

    public static void SerializeChunkOld() {
        FileStream fileStream = new FileStream(Application.persistentDataPath + "/chunk.xml", FileMode.Create);
        XmlTextWriter xml = new XmlTextWriter(new StreamWriter(fileStream));
        xml.Formatting = Formatting.Indented;
        xml.Indentation = 4;
        xml.WriteStartElement("chunk");
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                TileData data = GridManager.Instance.GetGridTile(j, i);
                SerializeTileOld(data, ref xml);
            }
        }
        xml.WriteEndElement();
        xml.Flush();
        xml.Close();
        Debug.Log("XML created at " + Application.persistentDataPath + "/chunk.xml");
    }

    public static void SerializeTileOld(TileData tile, ref XmlTextWriter xml) {
        xml.WriteStartElement("Tile");
        xml.WriteAttributeString("row", tile.GetGridPos().x.ToString());
        xml.WriteAttributeString("col", tile.GetGridPos().z.ToString());
        xml.WriteElementString("id", tile.GetId().ToString());
        xml.WriteElementString("rotation", tile.GetRotation().GetRotation().ToString());
            xml.WriteStartElement("building_data");
            xml.WriteElementString("segments", "3");
            xml.WriteElementString("height", "91");
            xml.WriteEndElement();
        xml.WriteEndElement();
    }

    public static void DeserializeTileOld(XmlTextReader xml) {
        //int row = -1;
        //int col = -1;
        //int id = TileRegistry.maxId;
        //int rotation = 0;
        
        while (xml.Read()) {
            if (xml.Name.Equals("Tile") && xml.NodeType == XmlNodeType.Element) {
                Debug.Log("Reading tile...");
                string row = xml.GetAttribute("row");
                string col = xml.GetAttribute("col");
                string id = xml.ReadElementString("id");//GetAttribute("id");
                string rot = xml.ReadElementString("rotation");
                
                Debug.Log("Got tile with data. Row: " + row + ", col: " + col + ", ID: " + id + ", rot: " + rot);
                //Debug.Log("The ID suggests its a " + TileRegistry.GetTileFromID(0));
            }
        }
    }*/
}
