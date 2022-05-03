public class AgentDataObject {
    
    public readonly string firstName;
    public readonly string lastName;
    public readonly int age;
    public readonly Gender gender;
    public readonly Profession profession;

    public AgentDataObject(string firstName, string lastName, int age, Gender gender, Profession profession) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.age = age;
        this.gender = gender;
        this.profession = profession;
    }
}
