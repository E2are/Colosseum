using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static string nextscene;
    [SerializeField] Slider slider;
    public static void LoadScene(string wantedScene)
    {
        nextscene = wantedScene;
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        Time.timeScale = 1.0f;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextscene);
        op.allowSceneActivation = false;
        float timer = 0f;
        while(!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if(op.progress < 0.9f)
            {
                slider.value = Mathf.Lerp(slider.value,op.progress,timer);
                if (slider.value > op.progress)
                    timer = 0;
            }
            else
            {
                slider.value = Mathf.Lerp(slider.value, 1f, timer);
                if (slider.value == 1)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
