using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerManager : MonoBehaviour
{
    public static ManagerManager Instance;

    public Canvas NPCCanvas;
    public Image NPCImage;
    public TMP_Text Text;
    bool canvasActivated = false;

    public Sprite[] NPCSprites;
    public int selectedBossIndex;
    public GameObject BossSets;
    BossSelection[] BossSelections;
    bool bossSelectionActivated = false;

    public string[] Texts;
    bool texting = false;
    public bool entered = false;
    int count = 0;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
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
                StartTexting();
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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            entered = false;
            canvasActivated = false;
            if(NPCCanvas != null)
            NPCCanvas.gameObject.SetActive(canvasActivated);
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
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)&&selectedBossIndex > 0)
        {
            BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = false;
            selectedBossIndex--;
            BossSelections[selectedBossIndex].GetComponent<Outline>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadingScene.LoadScene(BossSelections[selectedBossIndex].GetBossScene());
        }
    }

    public void LoadCurrentSelectedBoss()
    {
        LoadingScene.LoadScene(BossSelections[selectedBossIndex].GetBossScene());
    }

    public void StartTexting()
    {
        StartCoroutine(Texting());
        Text.text = "";
    }

    IEnumerator Texting()
    {
        yield return null;
        while (Texts.Length > count )
        {
            string[] txt = Texts[count].Split(":");
            if (NPCImage != null)
            {
                NPCImage.sprite = NPCSprites[int.Parse(txt[1])];
            }
            if (txt[2] !=null)
            {
                BossSets.SetActive(bool.Parse(txt[2]));
                bossSelectionActivated = bool.Parse(txt[2]);
            }
            for(int i = 0; i < txt[0].Length; i++)
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
            }
            if (!entered)
            {
                break;
            }
            yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.UpArrow));
            Text.text = "";
            count++;
        }
        count = 0;
        entered = false;
        canvasActivated = false;
        NPCCanvas.gameObject.SetActive(canvasActivated);
        texting = false;
    }
}
