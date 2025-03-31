using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Data", menuName = "BettingData")]
public class BettingData : ScriptableObject
{
    public int CurrentSelectedMultiplierIndex;

    public int selectedBossIndex;

    public float[] BettingMultipliers;

    public int[] defaultEarns;
}
