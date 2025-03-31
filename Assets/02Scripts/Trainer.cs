using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class Trainer : InteractableNPC
{
    public UpgradeData upgradeData;
    public GameObject TrainingCanvas;
    public UpgradeSelection[] UpgradeSelections;
    private void Start()
    {
        NPCCanvas.gameObject.SetActive(false);
        canvasActivated = false;
        texting = false;

        UpgradeSelections = NPCCanvas.GetComponentsInChildren<UpgradeSelection>();
        foreach (UpgradeSelection upgradeSelection in UpgradeSelections)
        {
            upgradeSelection.InitNPC(this);
        }
    }

    private void Update()
    {
        if (entered)
        {
            
            if (Input.GetKeyDown(KeyCode.UpArrow) && !texting)
            {
                upgradeData = GameManager.Instance.LoadJsonFile<UpgradeData>(Application.dataPath, "UpgradeData");
                
                foreach (UpgradeSelection upgradeSelection in UpgradeSelections)
                {
                    upgradeSelection.InitData(upgradeData);
                }
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
}
