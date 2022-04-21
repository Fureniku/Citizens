using System.Collections.Generic;
using UnityEngine;

public class BrandRegistry : MonoBehaviour {

    private static readonly GameObject[] registry = new GameObject[255];
    [SerializeField] private GameObject[] register = null;
    
    private static List<GameObject> clothesShops = new List<GameObject>();
    private static List<GameObject> groceryShops = new List<GameObject>();
    private static List<GameObject> jewelryShops = new List<GameObject>();
    private static List<GameObject> comicShops = new List<GameObject>();
    private static List<GameObject> restaurants = new List<GameObject>();
    private static List<GameObject> cafes = new List<GameObject>();
    private static List<GameObject> homewareShops = new List<GameObject>();
    private static List<GameObject> newsAgents = new List<GameObject>();
    private static List<GameObject> electronicsShops = new List<GameObject>();
    private static List<GameObject> musicShops = new List<GameObject>();
    private static List<GameObject> furnitureShops = new List<GameObject>();
    private static List<GameObject> chemists = new List<GameObject>();

    public void Initialize() {
        for (int i = 0; i < register.Length; i++) {
            GameObject reg = Instantiate(register[i], transform, true);
            reg.transform.position = new Vector3(0, -100, 0);
            reg.SetActive(false);
            registry[i] = reg;
        }
        
        PopulateRegistries();
    }
    
    private void PopulateRegistries() {
        for (int i = 0; i < register.Length; i++) {
            GameObject go = register[i];
            if (go != null) {
                Brand brand = go.GetComponent<Brand>();
                if (brand != null) {
                    switch (brand.GetShopType()) {
                        case ShopType.CLOTHES:
                            clothesShops.Add(go);
                            break;
                        case ShopType.GROCERY:
                            groceryShops.Add(go);
                            break;
                        case ShopType.JEWELRY:
                            jewelryShops.Add(go);
                            break;
                        case ShopType.COMIC:
                            comicShops.Add(go);
                            break;
                        case ShopType.RESTAURANT:
                            restaurants.Add(go);
                            break;
                        case ShopType.CAFE:
                            cafes.Add(go);
                            break;
                        case ShopType.HOMEWARE:
                            homewareShops.Add(go);
                            break;
                        case ShopType.NEWS_AGENTS:
                            newsAgents.Add(go);
                            break;
                        case ShopType.ELECTRONICS:
                            electronicsShops.Add(go);
                            break;
                        case ShopType.MUSIC:
                            musicShops.Add(go);
                            break;
                        case ShopType.FURNITURE:
                            furnitureShops.Add(go);
                            break;
                        case ShopType.CHEMISTS:
                            chemists.Add(go);
                            break;
                    }
                }
            }
        }
        
        Debug.Log("========================================================");
        Debug.Log("Registered " + clothesShops.Count + " clothes shops.");
        Debug.Log("Registered " + groceryShops.Count + " grocery shops.");
        Debug.Log("Registered " + jewelryShops.Count + " jewelry shops.");
        Debug.Log("Registered " + comicShops.Count + " comic shops.");
        Debug.Log("Registered " + restaurants.Count + " restaurant.");
        Debug.Log("Registered " + cafes.Count + " cafes.");
        Debug.Log("Registered " + homewareShops.Count + " homeware shops.");
        Debug.Log("Registered " + newsAgents.Count + " newsagents.");
        Debug.Log("Registered " + electronicsShops.Count + " electronics shops.");
        Debug.Log("Registered " + musicShops.Count + " music shops.");
        Debug.Log("Registered " + furnitureShops.Count + " furniture shops.");
        Debug.Log("Registered " + chemists.Count + " chemists shops.");
        Debug.Log("========================================================");
    }
}

public enum ShopType {
    CLOTHES,
    GROCERY,
    JEWELRY,
    COMIC,
    RESTAURANT,
    CAFE,
    HOMEWARE,
    NEWS_AGENTS,
    ELECTRONICS,
    MUSIC,
    FURNITURE,
    CHEMISTS
}