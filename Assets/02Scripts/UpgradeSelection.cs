using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System;

public class UpgradeSelection : MonoBehaviour
{
    InteractableNPC InteractableNPC;
    UpgradeData upgradeData;
    string UpText;
    public TMP_Text UpgradeCostText;
    public TMP_Text UpgradeCntText;
    public int upgradeIndex= 0;
    public int cost = 0;
    public int costRaiseAmount;
    public AudioClip UpgradeClip;
    public string[] upgradeTexts;
    public string[] Descriptions;
    public AudioClip FailedClip;
    public string[] notEnoughTexts;

    public void Start()
    {
        
    }

    void CreateJsonFile(string createpath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createpath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public void InitData(UpgradeData upData)
    {
        upgradeData = upData;
        cost = cost + (costRaiseAmount * upgradeData.UpgradedCnt[upgradeIndex]);
        UpgradeCostText = GetComponentsInChildren<TMP_Text>()[0];
        UpgradeCntText = GetComponentsInChildren<TMP_Text>()[1];
        UpgradeCostText.text = cost.ToString();
        UpgradeCntText.text = upgradeData.UpgradedCnt[upgradeIndex].ToString();
    }

    public void InitNPC(InteractableNPC NPC)
    {
        InteractableNPC = NPC;
    }

    public void OnMouseOver()
    {
        InteractableNPC.StartTexting(Descriptions);
    }


    public void Upgrade()
    {
        if(upgradeData.coin >= cost)
        {
            upgradeData.coin -= cost;
            cost = cost + costRaiseAmount;
            upgradeData.UpgradedCnt[upgradeIndex]++;
            UpgradeCostText.text = cost.ToString();
            UpgradeCntText.text = upgradeData.UpgradedCnt[upgradeIndex].ToString();
            InteractableNPC.StartTexting(upgradeTexts);
            InteractableNPC.AS.PlayOneShot(UpgradeClip);
            UpText = JsonConvert.SerializeObject(upgradeData, Formatting.Indented);
            Debug.Log(UpText);
            CreateJsonFile(Application.dataPath, "UpgradeData", UpText);
        }
        else
        {
            InteractableNPC.StartTexting(notEnoughTexts);
            InteractableNPC.AS.PlayOneShot(FailedClip);
        }
    }
}
