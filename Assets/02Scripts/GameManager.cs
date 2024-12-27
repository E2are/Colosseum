using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    [Header("PlayerStats")]

    Player player = null;
    public IBoss Boss = null;
    public Slider BossHPSlider;
    public Slider DelayedBossSlider;

    public GameObject resultPage;
    float time;
    string hour;
    string minute;
    string seconds;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        player = GameObject.Find("Player").GetComponent<Player>();

        resultPage.SetActive(false);
        time = 0;
    }

    
    void Update()
    {
        
        if (!Boss.dead && !player.dead)
        {
            time += Time.unscaledDeltaTime;
            hour = ((int)time/3600).ToString();
            minute = ((int)time / 60%60).ToString();
            seconds = ((int)time % 60).ToString();
        }

        BossHPSlider.value = Boss.hp / Boss.maxhp;
        DelayedBossSlider.value = Mathf.Lerp(DelayedBossSlider.value,BossHPSlider.value, Time.deltaTime * 0.5f);
    }

    public void win()
    {
        resultPage.SetActive(true);
        if (!player.dead)
        {
            resultPage.GetComponent<Text>().text = "Win\nClear Time : " + hour + " : " + minute + " : " + seconds;
        }
        else
        {
            resultPage.GetComponent<Text>().text = "Lose\nClear Time : " + hour + " : " + minute + " : " + seconds;
        }
    }
}
