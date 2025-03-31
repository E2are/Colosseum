using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerManager : InteractableNPC
{
    public int selectedBossIndex;
    public GameObject BossSets;
    BossSelection[] BossSelections;
    public Scrollbar Scrollbar;
    bool bossSelectionActivated = false;
    private void Start()
    {
        NPCCanvas.gameObject.SetActive(false);
        canvasActivated = false;
        texting = false;

        BossSelections = BossSets.GetComponentsInChildren<BossSelection>();
        foreach(BossSelection boss in BossSelections)
        {
            boss.GetComponent<Outline>().enabled = false;
        }
        BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = true;
    }

    private void Update()
    {
        if (entered)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && !texting)
            {
                texting = true;
                StartTexting(InteractTexts);
                canvasActivated = !canvasActivated;
                NPCCanvas.gameObject.SetActive(canvasActivated);
            }
            if (bossSelectionActivated)
            {
                SelectBoss();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            entered = true;
            if(EnteredSign != null)
            {
                EnteredSign.gameObject.SetActive(entered);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            entered = false;
            if (EnteredSign != null)
            {
                EnteredSign.gameObject.SetActive(entered);
            }
            canvasActivated = false;
            if(NPCCanvas != null)
            NPCCanvas.gameObject.SetActive(canvasActivated);
            StopAllCoroutines();
            texting = false;
        }
    }

    void SelectBoss()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && selectedBossIndex < BossSelections.Length-1)
        {
            BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = false;
            selectedBossIndex++;
            BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = true;
            Scrollbar.value = (selectedBossIndex + 1 / BossSelections.Length);
            Debug.Log((selectedBossIndex + 1 / BossSelections.Length));
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)&&selectedBossIndex > 0)
        {
            BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = false;
            selectedBossIndex--;
            BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = true;
            Scrollbar.value = (selectedBossIndex + 1 / BossSelections.Length);
            Debug.Log((selectedBossIndex + 1 / BossSelections.Length));
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadingScene.LoadScene(BossSelections[selectedBossIndex].GetBossScene());
        }
    }

    public void LoadCurrentSelectedBoss()
    {
        GameManager.Instance.bettingData.selectedBossIndex = selectedBossIndex;
        LoadingScene.LoadScene(BossSelections[selectedBossIndex].GetBossScene());
    }

    protected override IEnumerator Texting(string[] texts)
    {
        yield return null;
        while (texts.Length > count)
        {
            string[] txt = texts[count].Split(":");
            if (NPCImage != null)
            {
                NPCImage.sprite = NPCSprites[int.Parse(txt[1])];
            }
            if (txt[2] != null)
            {
                bossSelectionActivated = bool.Parse(txt[2]);
                BossSets.SetActive(bossSelectionActivated);
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
