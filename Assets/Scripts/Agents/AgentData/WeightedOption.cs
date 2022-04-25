public class WeightedOption {

    private readonly float weight;
    private readonly string optionName;
    private readonly int optionInt;

    public WeightedOption(float weight, string optionName) {
        this.weight = weight;
        this.optionName = optionName;
    }
    
    public WeightedOption(float weight, int optionInt) {
        this.weight = weight;
        this.optionName = optionInt + "";
        this.optionInt = optionInt;
    }

    public float GetValue() {
        return weight;
    }

    public string GetName() {
        return optionName;
    }

    public int GetOption() {
        return optionInt;
    }
}
