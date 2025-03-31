using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BettingManager : InteractableNPC
{
    UpgradeData UD;
    public BettingData BettingData;
    public GameObject BettingSelection;
    Dropdown BettingSelections;
    int currentSelectedItemIndex = 0;
    private void Start()
    {
        NPCCanvas.gameObject.SetActive(false);
        canvasActivated = false;
        texting = false;
        BettingSelections = BettingSelection.GetComponentInChildren<Dropdown>();
        UD = GameManager.Instance.LoadJsonFile<UpgradeData>(Application.dataPath,"UpgradeData");
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            entered = true;
            if (EnteredSign != null)
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
            if (NPCCanvas != null)
                NPCCanvas.gameObject.SetActive(canvasActivated);
            StopAllCoroutines();
            texting = false;
        }
    }

    public void DropDownBoxOptionChange(int num)
    {
        currentSelectedItemIndex = num;
    }

    public void confirmOption()
    {
        BettingData.CurrentSelectedMultiplierIndex = currentSelectedItemIndex;
    }
}

