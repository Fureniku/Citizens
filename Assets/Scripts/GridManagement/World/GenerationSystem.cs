using UnityEngine;

public abstract class GenerationSystem : MonoBehaviour {

    [SerializeField] protected int percentage = 0;
    [SerializeField] protected string message = "";
    [SerializeField] private bool complete = false;

    public abstract int GetGenerationPercentage();
    public abstract string GetGenerationString();

    public abstract void Initialize();
    public abstract void Process();

    public bool IsComplete() { return complete; }
    public void SetComplete() => complete = true;

}