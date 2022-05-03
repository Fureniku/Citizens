using System;
using System.Collections.Generic;
using UnityEngine;

public class Profession {
    
    private static readonly Profession DOCTOR = new Profession(EnumProfession.DOCTOR, "Doctor", 10.0f);
    private static readonly Profession LAWYER = new Profession(EnumProfession.LAWYER, "Lawyer", 10.0f);
    private static readonly Profession FAST_FOOD_WORKER = new Profession(EnumProfession.FAST_FOOD_WORKER, "Fast Food Worker", 75.0f);
    private static readonly Profession WAREHOUSE_OPERATIVE = new Profession(EnumProfession.WAREHOUSE_OPERATIVE, "Warehouse Operative", 50.0f);
    private static readonly Profession GAME_DESIGNER = new Profession(EnumProfession.GAME_DESIGNER, "Game Designer", 15.0f);
    private static readonly Profession STUDENT = new Profession(EnumProfession.STUDENT, "Student", 25.0f);
    private static readonly Profession RETIRED = new Profession(EnumProfession.RETIRED, "Retired", 20.0f);
    private static readonly Profession UNEMPLOYED = new Profession(EnumProfession.UNEMPLOYED, "Unemployed", 20.0f);

    private readonly EnumProfession profession;
    public readonly string name;
    private readonly float weight;

    private static List<Profession> list = new List<Profession>();
    
    public static void InitializeList() {
        list.Add(DOCTOR);
        list.Add(LAWYER);
        list.Add(FAST_FOOD_WORKER);
        list.Add(WAREHOUSE_OPERATIVE);
        list.Add(GAME_DESIGNER);
        list.Add(UNEMPLOYED);
    }

    public Profession(EnumProfession profession, string name, float weight) {
        this.profession = profession;
        this.name = name;
        this.weight = weight;
    }
    
    public float GetWeight() {
        return weight;
    }

    public EnumProfession GetProfession() {
        return profession;
    }

    public static Profession GetProfession(string nameIn) {
        foreach (Profession p in list) {
            if (p.name == nameIn) {
                return p;
            }
        }

        return null;
    }
    
    public static Profession GetProfession(EnumProfession enumProfession) {
        foreach (Profession p in list) {
            if (p.profession == enumProfession) {
                return p;
            }
        }

        return null;
    }

    public static List<Profession> GetList() {
        return list;
    }
}

public enum EnumProfession {
    DOCTOR,
    LAWYER,
    FAST_FOOD_WORKER,
    WAREHOUSE_OPERATIVE,
    GAME_DESIGNER,
    STUDENT,
    RETIRED,
    UNEMPLOYED,
}