using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] GameObject DestroyEffect;
    public void Destroy()
    {
        Destroy(gameObject);

        if(DestroyEffect != null)
            Instantiate(DestroyEffect);
    }
}
