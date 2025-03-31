using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSelection : MonoBehaviour
{
    public ManagerManager Manager;

    [SerializeField] int Index;
    [SerializeField] string SelectedBossScene;

    public string GetBossScene()
    {
        return SelectedBossScene;
    }

    public void SelectThisBoss()
    {
        BossSelection[] Boss = Manager.BossSets.GetComponentsInChildren<BossSelection>();
        Boss[Manager.selectedBossIndex].GetComponent<Outline>().enabled = false;
        Manager.selectedBossIndex = Index;
        Boss[Manager.selectedBossIndex].GetComponent<Outline>().enabled = true;
    }
}
