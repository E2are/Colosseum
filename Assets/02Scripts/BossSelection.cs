using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSelection : MonoBehaviour
{
    [SerializeField] int Index;
    [SerializeField] string SelectedBossScene;

    public string GetBossScene()
    {
        return SelectedBossScene;
    }

    public void SelectThisBoss()
    {
        BossSelection[] Boss = ManagerManager.Instance.BossSets.GetComponentsInChildren<BossSelection>();
        Boss[ManagerManager.Instance.selectedBossIndex].GetComponent<Outline>().enabled = false;
        ManagerManager.Instance.selectedBossIndex = Index;
        Boss[ManagerManager.Instance.selectedBossIndex].GetComponent<Outline>().enabled = true;
    }
}
