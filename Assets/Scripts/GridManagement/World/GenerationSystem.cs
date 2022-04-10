using UnityEngine;

public abstract class GenerationSystem : MonoBehaviour {

    [ReadOnly, SerializeField] protected int percentage = 0;
    [ReadOnly, SerializeField] protected string message = "";
    [ReadOnly, SerializeField] private bool complete = false;

    public abstract int GetGenerationPercentage();
    public abstract string GetGenerationString();

    public abstract void Initialize();
    public abstract void Process();

    public bool IsComplete() { return complete; }
    public void SetComplete() => complete = true;

}