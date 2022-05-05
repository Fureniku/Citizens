using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewWorldBtn : MonoBehaviour
{

    //[SerializeField] private InputField worldName;
    [SerializeField] private InputField worldSize;
    [SerializeField] private InputField randomSeed;
    [SerializeField] private InputField initialVehicles;
    [SerializeField] private InputField maxVehicles;
    [SerializeField] private InputField initialPeds;
    [SerializeField] private InputField maxPeds;
    //[SerializeField] private GameObject saving;

    public void Submit() {
        //WorldData.Instance.SetWorldName(worldName.text);
        WorldData.Instance.SetWorldSize((Int32.Parse(worldSize.text)));
        WorldData.Instance.SetWorldSeed((Int32.Parse(randomSeed.text)));
        
        WorldData.Instance.SetInitVehicles((Int32.Parse(initialVehicles.text)));
        WorldData.Instance.SetMaxVehicles((Int32.Parse(maxVehicles.text)));
        WorldData.Instance.SetInitPeds((Int32.Parse(initialPeds.text)));
        WorldData.Instance.SetMaxPeds((Int32.Parse(maxPeds.text)));
        //WorldData.Instance.SetWorldSaving(saving.GetComponent<Toggle>().isOn);

        SceneManager.LoadScene("MainGameScene");
    }
}
