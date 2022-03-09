using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewWorldBtn : MonoBehaviour
{

    [SerializeField] private GameObject worldName;
    [SerializeField] private GameObject worldSize;
    [SerializeField] private GameObject saving;

    public void Submit() {
        WorldData.Instance.SetWorldName(worldName.GetComponent<Text>().text);
        WorldData.Instance.SetWorldSize((Int32.Parse(worldSize.GetComponent<Text>().text)));
        WorldData.Instance.SetWorldSaving(saving.GetComponent<Toggle>().isOn);

        SceneManager.LoadScene("MainGameScene");
    }
}
