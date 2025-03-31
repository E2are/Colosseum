using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleFOr : MonoBehaviour
{
    public void LoadScene(string SceneName)
    {
        LoadingScene.LoadScene(SceneName);
    }
}
