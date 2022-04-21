using UnityEngine;

public class Brand : MonoBehaviour {

    [SerializeField] private string name;
    [SerializeField] private Material colour;
    [SerializeField] private Material logo;
    [SerializeField] private GameObject[] shopFillings;
    [SerializeField] private ShopType shopType;

    public Material GetColour() {
        return colour;
    }

    public Material GetLogo() {
        return logo;
    }

    public GameObject[] GetShopFillings() {
        return shopFillings;
    }

    public GameObject GetShopFilling(int id) {
        return shopFillings[id];
    }

    public ShopType GetShopType() {
        return shopType;
    }

    public string GetName() {
        return name;
    }
}