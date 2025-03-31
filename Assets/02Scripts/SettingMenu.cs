using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingMenu : MonoBehaviour
{
    FullScreenMode screenMode;
    List<Resolution> resolutions = new List<Resolution>();
    public TMP_Dropdown resolutionDropDown;
    public TMP_Dropdown FrameRateDropDown;
    public Toggle FullScreenBtn;
    public int selectedindex = 0;
    public int currentFrameRate = 0;

    public SoundVolumeData volumeData;
    public AudioMixer audioMixer;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider BGMSlider;

    private void Awake()
    {
        InitUI();
    }

    public void InitUI()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (int.Parse(Screen.resolutions[i].refreshRateRatio.ToString().Split(".")[0]) == 60 || int.Parse(Screen.resolutions[i].refreshRateRatio.ToString().Split(".")[0]) == 240)
                resolutions.Add(Screen.resolutions[i]);
        }
        resolutionDropDown.options.Clear();
        FrameRateDropDown.options.Clear();

        int optionnum = 0;
        foreach (Resolution resolution in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = resolution.width + " x " + resolution.height + " " + resolution.refreshRateRatio.ToString().Split(".")[0] + "h2";
            resolutionDropDown.options.Add(option);

            if (resolution.width == Screen.width && resolution.height == Screen.height)
                resolutionDropDown.value = optionnum;
            optionnum++;
        }

        int frameRate = 30;
        int frameoptionnum = 0;
        while (frameRate <= 240)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = frameRate.ToString() + " fps";
            FrameRateDropDown.options.Add(option);
            if (frameRate == Application.targetFrameRate)
                FrameRateDropDown.value = frameoptionnum;
            frameRate *= 2;
            frameoptionnum++;
        }
        resolutionDropDown.RefreshShownValue();
        FrameRateDropDown.RefreshShownValue();

        FullScreenBtn.isOn = Screen.fullScreenMode == (FullScreenMode.FullScreenWindow) ? true : false;

        MasterSlider.value = volumeData.MasterVolume;
        BGMSlider.value = volumeData.BGMVolume;
        SFXSlider.value = volumeData.SFXVolume;

        MasterSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        BGMSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        SFXSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });

        audioMixer.SetFloat("Master", Mathf.Log(volumeData.MasterVolume) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log(volumeData.SFXVolume) * 20);
        audioMixer.SetFloat("BGM", Mathf.Log(volumeData.BGMVolume) * 20);
    }

    //다이나믹 항목의 함수 적용
    public void DropDownBoxOptionChange(int num)
    {
        selectedindex = num;
    }

    //다이나믹 항목의 함수 적용
    public void FullScreenSet(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OkBtnCheck()
    {
        Screen.SetResolution(resolutions[selectedindex].width, resolutions[selectedindex].height,screenMode);
        Application.targetFrameRate = currentFrameRate;
    }

    public void SetFrameRate(int num)
    {
        currentFrameRate = int.Parse(FrameRateDropDown.options[num].text.Split(" ")[0]);
    }

    public void OnVolumeChange()
    {
        volumeData.MasterVolume = MasterSlider.value;
        volumeData.SFXVolume = SFXSlider.value;
        volumeData.BGMVolume = BGMSlider.value;

        audioMixer.SetFloat("Master",Mathf.Log(volumeData.MasterVolume) * 20);
        audioMixer.SetFloat("SFX",Mathf.Log(volumeData.SFXVolume) * 20);
        audioMixer.SetFloat("BGM",Mathf.Log(volumeData.BGMVolume) * 20);
    }
}
