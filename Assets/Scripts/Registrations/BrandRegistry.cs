using System.Collections.Generic;
using UnityEngine;

public class BrandRegistry : MonoBehaviour {

    private static readonly GameObject[] registry = new GameObject[255];
    [SerializeField] private GameObject[] register = null;
    
    private static List<GameObject> clothesShops = new List<GameObject>();
    private static List<GameObject> groceryShops = new List<GameObject>();
    private static List<GameObject> jewelryShops = new List<GameObject>();
    private static List<GameObject> charityShops = new List<GameObject>();
    private static List<GameObject> comicShops = new List<GameObject>();
    private static List<GameObject> restaurants = new List<GameObject>();
    private static List<GameObject> cafes = new List<GameObject>();
    private static List<GameObject> homewareShops = new List<GameObject>();
    private static List<GameObject> estateAgents = new List<GameObject>();
    private static List<GameObject> newsAgents = new List<GameObject>();

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
                        case ShopType.CHARITY:
                            charityShops.Add(go);
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
                        case ShopType.ESTATE_AGENT:
                            estateAgents.Add(go);
                            break;
                        case ShopType.NEWS_AGENTS:
                            newsAgents.Add(go);
                            break;
                    }
                }
            }
        }
        
        Debug.Log("Registered " + clothesShops.Count + " clothes shops.");
        Debug.Log("Registered " + groceryShops.Count + " grocery shops.");
        Debug.Log("Registered " + jewelryShops.Count + " jewelry shops.");
        Debug.Log("Registered " + charityShops.Count + " charity shops.");
        Debug.Log("Registered " + comicShops.Count + " comic shops.");
        Debug.Log("Registered " + restaurants.Count + " restaurant.");
        Debug.Log("Registered " + cafes.Count + " cafes.");
        Debug.Log("Registered " + homewareShops.Count + " homeware shops.");
        Debug.Log("Registered " + estateAgents.Count + " estate agents.");
        Debug.Log("Registered " + newsAgents.Count + " newsagents.");
    }
}

public enum ShopType {
    CLOTHES,
    GROCERY,
    JEWELRY,
    CHARITY,
    COMIC,
    RESTAURANT,
    CAFE,
    HOMEWARE,
    ESTATE_AGENT,
    NEWS_AGENTS
}