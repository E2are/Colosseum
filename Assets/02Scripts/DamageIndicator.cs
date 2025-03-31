using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    Color color;
    float h = 1;

    private void Start()
    {
        color = GetComponent<TMP_Text>().color;
    }

    private void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime);

        if(h > 0)
        {
            GetComponent<TMP_Text>().color = new Color(255, 255, 255, h);
            h -= Time.deltaTime;
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
