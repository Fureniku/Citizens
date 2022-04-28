using UnityEngine;

public class AgentData : MonoBehaviour {

    [SerializeField] private string firstName;
    [SerializeField] private string lastName;
    [SerializeField] private int age;
    [SerializeField] private string gender;
    [SerializeField] private string profession;
    
    [SerializeField] private Gender genderObj;
    [SerializeField] private Profession professionObj;
    
    //Visual stuff TBC

    void Awake() {
        AgentDataObject data = AgentDataRegistry.CreateAgentData();

        firstName = data.firstName;
        lastName = data.lastName;
        age = data.age;
        genderObj = data.gender;
        professionObj = data.profession;

        gender = genderObj.name;
        profession = professionObj.name;

        gameObject.name = "PA_" + firstName + "_" + lastName;
    }

    public string GetFirstName() { return firstName; }
    public string GetLastName() { return lastName; }
    public string GetFullName() { return firstName + " " +  lastName; }
    public int GetAge() { return age; }

    public string GetGenderName() { return gender; }
    public string GetProfessionName() { return profession; }
    
    public Gender GetGender() { return genderObj; }
    public Profession GetProfession() { return professionObj; }
}