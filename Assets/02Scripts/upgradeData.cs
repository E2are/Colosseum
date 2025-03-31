using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Json : MonoBehaviour
{
    private void Start()
    {
        if (LoadJsonFile<UpgradeData>(Application.dataPath, "UpgradeData") == null)
        {
            UpgradeData UD = new UpgradeData();
            string UpgradeJson = ObjectToJson(UD);
            Debug.Log(UpgradeJson);
            CreateJsonFile(Application.dataPath, "UpgradeData", UpgradeJson);
        }
    }
    void CreateJsonFile(string createpath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createpath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    string ObjectToJson(UpgradeData GB)
    {
        return JsonConvert.SerializeObject(GB, Formatting.Indented);
    }

    T JsonToObject<T>(string JsonData)
    {
        return JsonConvert.DeserializeObject<T>(JsonData);
    }

    T LoadJsonFile<T>(string loadPath, string fname)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fname), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        string JsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(JsonData);
    }
}

public class UpgradeData
{
    public int[] UpgradedCnt = { 0, 0, 0, 0};
    public enum Upgrades
    {
        Attack,
        PierceAttack,
        SpinAttack,
        SwordDance
    }

    public int coin = 0;

    public UpgradeData()
    {

    }

    public UpgradeData(bool isSet)
    {
        if (isSet)
        {
            for (int i = 0; i < UpgradedCnt.Length; i++)
            {
                UpgradedCnt[i] = 0;
            }

            coin = 0;
        }
    }

    public void SkillUpgrade(Upgrades upgrade)
    {
        UpgradedCnt[(int)upgrade]++;
    }
}
