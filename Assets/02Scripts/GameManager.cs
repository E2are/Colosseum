using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    [Header("PlayerStats")]
    Player player = null;
    string UpText;
    public UpgradeData upgradeData;
    public BettingData bettingData;

    [Header("BossState")]
    public IBoss Boss = null;
    public Slider BossHPSlider;
    public Slider DelayedBossSlider;
    [Header("GameState")]
    public PlayableDirector PD;
    [SerializeField] string currentSceneName;
    public GameObject PauseMenuUI;
    public bool isPaused = false;

    public GameObject SettingMenu;
    [SerializeField]bool settingOpend = false;
    public AudioMixer audioMixer;
    public SoundVolumeData volumeData;

    public GameObject resultPage;
    public bool GameStarted = false;
    float time;
    string hour;
    string minute;
    string seconds;

    void CreateJsonFile(string createpath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createpath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fname)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fname), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        string JsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(JsonData);
    }

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

        if(resultPage != null)
        resultPage.SetActive(false);
        time = 0;

        PauseMenuUI.SetActive(false);

        SettingMenu.SetActive(false);

        upgradeData = LoadJsonFile<UpgradeData>(Application.dataPath, "UpgradeData");
    }

    private void Start()
    {
        SettingMenu.GetComponent<SettingMenu>().InitUI();
    }

    void Update()
    {
        if (!GameStarted)
        {
            if(PD!= null && Input.GetKeyDown(KeyCode.Return))
            {
                var timelineAsset = PD.playableAsset as TimelineAsset;
                var markers = timelineAsset.markerTrack.GetMarkers().ToArray();
                PD.Play();
                PD.time = markers[0].time;
            }
        }
        if (settingOpend&&Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingMenu();
            return;
        }
        if (!Boss.dead && !player.dead)
        {
            if (GameStarted && !isPaused)
            time += Time.unscaledDeltaTime;

            hour = ((int)time / 3600).ToString();
            minute = ((int)time / 60 % 60).ToString();
            seconds = ((int)time % 60).ToString();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    UnPause();
                }
                else
                {
                    Pause();
                }
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape)){
                LoadingScene.LoadScene("Ready Battle");
            }
            if(Input.GetKeyDown(KeyCode.Return)){
                LoadingScene.LoadScene(currentSceneName);
            }
        }

        if (BossHPSlider != null)
        {
            BossHPSlider.value = Boss.hp / Boss.maxhp;
            DelayedBossSlider.value = Mathf.Lerp(DelayedBossSlider.value, BossHPSlider.value, Time.deltaTime * 0.5f);
        }
    }

    private void PD_stopped(PlayableDirector obj)
    {
        throw new System.NotImplementedException();
    }

    public void win()
    {
        resultPage.SetActive(true);
        if (!player.dead)
        {
            resultPage.GetComponent<Text>().text = "Win\nClear Time : " + hour + " : " + minute + " : " + seconds;
            upgradeData.coin += (int)(bettingData.defaultEarns[bettingData.selectedBossIndex] * bettingData.BettingMultipliers[bettingData.CurrentSelectedMultiplierIndex]);
            UpText = JsonConvert.SerializeObject(upgradeData, Formatting.Indented);
            Debug.Log(UpText);
            CreateJsonFile(Application.dataPath, "UpgradeData", UpText);
        }
        else
        {
            resultPage.GetComponent<Text>().text = "Lose\nClear Time : " + hour + " : " + minute + " : " + seconds;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        PauseMenuUI.SetActive(isPaused);
    }
    
    public void UnPause()
    {
        Time.timeScale = 1;
        isPaused = false;
        PauseMenuUI?.SetActive(isPaused);
    }

    public void OpenSettingMenu()
    {
        SettingMenu.SetActive(true);
        settingOpend = true;
        SettingMenu.GetComponent<SettingMenu>().InitUI();
    }

    public void CloseSettingMenu()
    {
        SettingMenu.SetActive(false);
        settingOpend = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
