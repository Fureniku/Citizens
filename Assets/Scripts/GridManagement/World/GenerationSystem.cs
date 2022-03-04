using UnityEngine;

public abstract class GenerationSystem : MonoBehaviour {

    [ReadOnly, SerializeField] protected int percentage = 0;
    [ReadOnly, SerializeField] protected string message = "";

    public abstract int GetGenerationPercentage();
    public abstract string GetGenerationString();

}