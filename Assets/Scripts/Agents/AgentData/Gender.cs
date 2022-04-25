using System.Collections.Generic;

public class Gender {
    
    private static readonly Gender MALE = new Gender(EnumGender.MALE, "Male", 45.0f);
    private static readonly Gender FEMALE = new Gender(EnumGender.FEMALE, "Female", 45.0f);
    private static readonly Gender NON_BINARY = new Gender(EnumGender.NON_BINARY, "Non-Binary", 5.0f);
    private static readonly Gender GENDERFLUID = new Gender(EnumGender.GENDERFLUID, "Genderfluid", 2.0f);
    private static readonly Gender AGENDER = new Gender(EnumGender.AGENDER, "Agender", 1.0f);

    private readonly EnumGender gender;
    public readonly string name;
    private readonly float weight;

    private static List<Gender> list = new List<Gender>();
    
    public static void InitializeList() {
        list.Add(MALE);
        list.Add(FEMALE);
        list.Add(NON_BINARY);
        list.Add(GENDERFLUID);
        list.Add(AGENDER);
    }

    public Gender(EnumGender gender, string name, float weight) {
        this.gender = gender;
        this.name = name;
        this.weight = weight;
    }

    public EnumGender GetGender() {
        return gender;
    }

    public float GetWeight() {
        return weight;
    }

    public static Gender GetGender(string nameIn) {
        foreach (Gender g in list) {
            if (g.name == nameIn) {
                return g;
            }
        }

        return null;
    }
    
    public static Gender GetGender(EnumGender enumGenderIn) {
        foreach (Gender g in list) {
            if (g.gender == enumGenderIn) {
                return g;
            }
        }

        return null;
    }

    public static List<Gender> GetList() {
        return list;
    }
}

public enum EnumGender {
    MALE,
    FEMALE,
    NON_BINARY,
    GENDERFLUID,
    AGENDER
}