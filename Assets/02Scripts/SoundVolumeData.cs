using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Data",menuName = "SoundVolumeData")]
public class SoundVolumeData : ScriptableObject
{
    public float MasterVolume = 1;
    public float BGMVolume = 1;
    public float SFXVolume = 1;
}