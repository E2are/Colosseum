using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CharacterGhostTrail : MonoBehaviour
{
    public GameObject GhostPrefab;
    public GameObject Character;
    public bool doGenerate = false;
    public float generateDelay = 0.5f;
    float genTimer = 0;
    public float DelTime = 1f;

    public void Start()
    {
        genTimer = generateDelay;
    }

    public void Update()
    {
        if (doGenerate)
        {
            if(genTimer < generateDelay) {
                genTimer += Time.deltaTime;
            }
            else
            {
                genTimer = 0;
                GameObject Ghost = Instantiate(GhostPrefab, Character.transform.position, Quaternion.identity);

                Ghost.GetComponent<SpriteRenderer>().sprite = Character.GetComponent<SpriteRenderer>().sprite;
                Ghost.transform.localScale = Character.transform.localScale;
                Ghost.GetComponent<SpriteRenderer>().flipX = Character.GetComponent<SpriteRenderer>().flipX;
                Destroy(Ghost, DelTime);
            }
        }
    }
}
