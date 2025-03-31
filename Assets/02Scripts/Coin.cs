using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public UpgradeData upgradeData;
    public int cointAmount = 10;
    public  void initCoin(int coin)
    {
        cointAmount = coin;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            
    }
}
