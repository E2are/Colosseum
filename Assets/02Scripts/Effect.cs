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

    public void CheckAttackHit(float dmg)
    {
        Collider2D[] PCD = new Collider2D[10];
        ContactFilter2D cf = new ContactFilter2D();
        GetComponent<PolygonCollider2D>().OverlapCollider(cf, PCD);
        foreach(Collider2D c in PCD)
        {
            if (c != null)
            {
                IDamageAble Chit = c.GetComponent<IDamageAble>();
                if (Chit != null && c.gameObject.CompareTag("Player"))
                {
                    Chit.OnDamaged(dmg);
                }
            }
        }
    }
}
