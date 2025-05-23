using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] Player player;
    UpgradeData upgradeData;
    float Maxhp;
    public Slider hpSlider;
    public Slider delayedHpSlider;
    public Slider BlockStaminaSlider;
    public Slider delayedblockStaminaSlider;
    public TMP_Text Money;
    float Max_BlockStamina;
    [Header("UIReferences")]
    public Image ui;
    public Sprite[] uis;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        Max_BlockStamina = player.block_stamina;
        Maxhp = player.hp;
    }

    void Update()
    {
        hpSlider.value = player.hp / Maxhp;
        delayedHpSlider.value = Mathf.Lerp(delayedHpSlider.value, hpSlider.value, Time.deltaTime / 0.5f);
        BlockStaminaSlider.value = player.block_stamina / Max_BlockStamina;
        delayedblockStaminaSlider.value = Mathf.Lerp(delayedblockStaminaSlider.value, BlockStaminaSlider.value, Time.deltaTime / 0.5f);
        upgradeData = GameManager.Instance.LoadJsonFile<UpgradeData>(Application.dataPath, "UpgradeData");
        Money.text = "coin : " + upgradeData.coin.ToString();

        if (player.hp <= 20)
        {
            ui.sprite = uis[uis.Length - 2];
        }
        else if (player.hp <= 50)
        {
            ui.sprite = uis[uis.Length - 3];
        }
        else
        {
            ui.sprite = uis[uis.Length - 4];
        }

        if (player.hp <= 0)
        {
            player.dead = true;
            ui.sprite = uis[uis.Length - 1];
            player.GetComponent<Animator>().SetBool("Dead", true);
        }
    }
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
