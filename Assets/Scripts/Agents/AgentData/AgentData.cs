using UnityEngine;

public class AgentData : MonoBehaviour{

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
    }
}