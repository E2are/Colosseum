using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [HideInInspector]
    public Vector3 dir;
    [HideInInspector]
    public float Damage;

    public bool Unblockable = false;

    public float speed;

    private void Start()
    {
        GetComponent<SpriteRenderer>().flipX = dir.x < 0;
    }

    private void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Animator>().enabled = false;
        if(collision.CompareTag("Player"))
        {
            if (Unblockable)
            {
                collision.GetComponent<Player>().OnDamaged(Damage, 0, true);
            }
            else
            {
                collision.GetComponent<Player>().OnDamaged(Damage, (int)dir.x);
            }
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(gameObject);
        }
        if(collision.CompareTag("Ground"))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(gameObject, 2f);
            speed = 0f;
        }
    }
}
