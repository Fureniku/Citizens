using Newtonsoft.Json.Linq;
using UnityEngine;

public class TileGeneratedShop : TileData {
    
    [SerializeField] 
    
    
    
    
    
    
    
    
    public override JProperty SerializeTile(TileData td, int row, int col) {
        throw new System.NotImplementedException();
    }

    public override void DeserializeTile(JObject json) {
        throw new System.NotImplementedException();
    }

    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
        //isRegistryEntry = true;
    }

    public override void Create() {
        CreateBase();
        //isRegistryEntry = false;
    }
}