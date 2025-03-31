using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScareCrow : IBoss, IDamageAble
{
    public GameObject dmgIndicater;
    public void OnDamaged(float dmg, int dir, bool isKnockbackable = false)
    {
        TMP_Text Indicater = Instantiate(dmgIndicater, transform.position + Vector3.up + Vector3.right * Random.Range(-1f,1f),Quaternion.identity).GetComponentInChildren<TMP_Text>();
        Indicater.text = dmg.ToString();
    }
}
