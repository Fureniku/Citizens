using UnityEngine;

public static class MeshRendererExtension {

    public static void ApplyRandomMaterial(this MeshRenderer renderer, string shaderName, string goName) {
        
        Material randomMat = new Material(Shader.Find(shaderName));
        randomMat.name = $"{goName}_material";
        randomMat.color = GetRandomColour();

        randomMat.EnableKeyword("_EMISSION");
        randomMat.SetColor("_EmissionColor", randomMat.color);

        renderer.material = randomMat;
    }
    
    static Color GetRandomColour() => Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
}
