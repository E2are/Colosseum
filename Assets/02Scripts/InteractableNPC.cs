using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InteractableNPC : MonoBehaviour
{
    public AudioSource AS;
    public GameObject NPCCanvas;
    public Image NPCImage;
    public TMP_Text Text;
    public bool canvasActivated = false;

    public Sprite[] NPCSprites;

    public string[] InteractTexts;
    public bool texting = false;
    public bool entered = false;
    public GameObject EnteredSign;
    public int count = 0;

    private void Start()
    {
        EnteredSign.SetActive(false);
        AS = GetComponent<AudioSource>();
    }

    public void StartTexting(string[] texts)
    {
        StopAllCoroutines();
        StartCoroutine(Texting(texts));
        Text.text = "";
    }

    protected virtual IEnumerator Texting(string[] texts)
    {
        yield return null;
        while (texts.Length > count)
        {
            string[] txt = texts[count].Split(":");
            if (NPCImage != null)
            {
                NPCImage.sprite = NPCSprites[int.Parse(txt[1])];
            }
            for (int i = 0; i < txt[0].Length; i++)
            {
                Text.text += txt[0][i];
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
                if (!entered)
                {
                    break;
                }
                GetComponent<AudioSource>().Play();
            }
            if (!entered)
            {
                break;
            }
            count++;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow));
            if (count < texts.Length)
                Text.text = "";
        }
        count = 0;
    }
}
