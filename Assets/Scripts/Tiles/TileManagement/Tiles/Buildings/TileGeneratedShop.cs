using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TileGeneratedShop : TileData {

    [SerializeField] private GameObject[] shopBases;
    [SerializeField] private GameObject[] shopUppers;
    [SerializeField] private GameObject shopSignBack;
    [SerializeField] private GameObject shopSignFront;
    
    [SerializeField] private Brand brand;
    
    
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

    public override void CreateFromRegistry() {
        Debug.Log("Creating a " + brand.GetName() + " shop");
        if (shopBases.Length > 0) {
            GameObject shopBase = Instantiate(shopBases[Random.Range(0, shopBases.Length)], transform.position, Quaternion.Euler(0, 90, 0));
            shopBase.transform.parent = transform;
        }

        if (shopUppers.Length > 0) {
            GameObject shopUpper = Instantiate(shopUppers[Random.Range(0, shopUppers.Length)], transform.position, Quaternion.Euler(0, 90, 0));
            shopUpper.transform.parent = transform;
        }

        shopSignBack.GetComponent<MeshRenderer>().material = brand.GetColour();
        shopSignFront.GetComponent<MeshRenderer>().material = brand.GetLogo();
        
        CreateBase();
        //isRegistryEntry = false;
    }
    //21.40625
    //0.15625
}