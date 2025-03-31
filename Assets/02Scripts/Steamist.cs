using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Steamist : IBoss, IDamageAble
{
    Animator anim;
    Rigidbody2D rigid;
    AudioSource audioSource;

    GameObject target;

    public bool IsCutScene = true;

    public float move_Speed = 5f;
    public int move_dir;

    bool waken = false;

    [Header("Boss atack stats")]
    public float atk1_dmg;
    public float attack1_Delay = 2f;
    bool attacking;
    float atk1delay = 0;
    public Vector2 Attack1Zone = new Vector2(2, 2);
    public Vector2 Attack2Zone = new Vector2(5, 1);
    public GameObject zone1show;

    [Header("Boss Patern stats")]
    public Vector2 Patern1Zone = new Vector2(8, 5);

    public Patern nowPatern;
    public enum Patern
    {
        idle,
        move,
        attack,
        patern1,
        dead
    }


    bool skilling = false;
    public int availableSkillcnt = 1;
    int maxavailableSkillcnt;

    public AudioClip[] audioSources;
    public enum audios
    {
        attack1 = 0,
        attack2,
        patern1,
        dead
    }
    Material material;

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        target = GameObject.Find("Player");
        maxhp = hp;
        nowPatern = Patern.idle;

        GameManager.Instance.Boss = this;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        maxavailableSkillcnt = availableSkillcnt;
    }


    public void wake()
    {
        waken = true;
        rigid.velocity = Vector2.zero;
        GameManager.Instance.GameStarted = true;
    }

    void Update()
    {
        if (waken)
        {
            if (!dead)
            {
                if (!skilling)
                {
                    switch (nowPatern)
                    {
                        case Patern.idle:
                            Idle();
                            break;
                        case Patern.move:
                            Move();
                            break;
                        case Patern.attack:
                            Attack1();
                            break;
                        case Patern.dead:
                            Dead();
                            break;
                    }
                }
                if (skilling)
                {
                    Steamist_skill();
                }
            }
        }
    }

    void FixedUpdate()
    {

    }

    IEnumerator delayedPaterntomove()
    {
        yield return new WaitForSeconds(0.5f);
        nowPatern = Patern.move;
    }

    void Idle()
    {
        if (!target.GetComponent<Player>().dead)
        {
            nowPatern = Patern.move;
        }
    }

    private void Move()
    {
        if (Mathf.Abs(transform.position.x - target.transform.position.x) > Attack1Zone.x)
        {
            move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
            gameObject.GetComponent<SpriteRenderer>().flipX = move_dir < 0;
            transform.Translate(Vector2.right * move_dir * move_Speed * Time.deltaTime);
            anim.SetBool("running", true);
        }
        else
        {
            nowPatern = Patern.attack;
            atk1delay = attack1_Delay / 2;
            anim.SetBool("running", false);
        }
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(transform.position.x + move_dir * 1.4f, transform.position.y - 0.5f), Attack1Zone);
    }*/

    void Attack1()
    {
        if (Mathf.Abs(transform.position.x - target.transform.position.x) > Attack1Zone.x * 2 && !attacking)
        {
            StartCoroutine(delayedPaterntomove());
        }
        else if (target.GetComponent<Player>().dead)
        {
            nowPatern = Patern.idle;
        }
        else
        {
            atk1delay += Time.deltaTime;
            if (atk1delay > attack1_Delay && !attacking)
            {
                StartCoroutine(Atk1Seq());
                atk1delay = 0;
            }
        }
    }

    IEnumerator Atk1Seq()
    {
        audioSource.pitch = 1f;
        PlaySound(audios.attack1);
        attacking = true;
        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(0.1f);
        rigid.AddForce(Vector2.right * move_dir * move_Speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.7f);
        if (target.transform.position.x < transform.position.x && move_dir == 1 || target.transform.position.x > transform.position.x && move_dir == -1)
        {
            if (target.GetComponent<Rigidbody2D>().velocity.y <= 0)
            {
                anim.SetBool("attacking", true);
                anim.SetTrigger("Crushing");
                audioSource.pitch = 1.2f;
                PlaySound(audios.attack2);
                zone1show.SetActive(true);
                zone1show.transform.position = new Vector2(transform.position.x, transform.position.y - 1.7f);
                zone1show.transform.localScale = Attack2Zone;
                yield return new WaitForSeconds(1f);
                zone1show.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
            move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
            gameObject.GetComponent<SpriteRenderer>().flipX = move_dir < 0;
        }
        turn();
        attacking = false;
        anim.SetBool("attacking", false);
        audioSource.pitch = 0;
        nowPatern = Patern.idle;
    }

    public void CheckAtk1()
    {
        Collider2D[] collider = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + move_dir*1.4f, transform.position.y - 0.5f), Attack1Zone, 0);
        foreach (Collider2D i in collider)
        {
            if (i.gameObject.tag == "Player")
            {
                i.GetComponent<Player>().OnDamaged(atk1_dmg, move_dir, true);
            }
        }
    }

    public void CheckAtk2()
    {
        Collider2D[] coll = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y - 1.7f), Attack2Zone, 0);
        foreach (Collider2D i in coll)
        {
            if (i.gameObject.tag == "Player")
            {
                i.GetComponent<Player>().OnDamaged(atk1_dmg / 2, i.GetComponent<Player>().looking_dir, true);
            }
        }
        Camera.main.GetComponent<CameraFollow>().DoImppulse(0.2f);
        PlaySound(audios.dead);
    }

    public void turn()
    {
        move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
        gameObject.GetComponent<SpriteRenderer>().flipX = move_dir < 0;
    }

void atkoff()
    {
        attacking = false;
        turn();
        nowPatern = Patern.idle;
    }

    IEnumerator Patern1_Seq()
    {
        material.SetFloat("_CamouflageFade", 1);
        material.SetFloat("_PixelOutlineFade", 1);
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        skilling = true;
        anim.SetBool(nameof(skilling), true);
        anim.SetTrigger("skilled");
        PlaySound(audios.patern1);
        yield return new WaitForSeconds(5f);
        Collider2D[] collider = Physics2D.OverlapBoxAll(transform.position, Patern1Zone, 0);
        foreach (Collider2D i in collider)
        {
            if (i.gameObject.tag == "Player")
            {
                i.GetComponent<Player>().OnDamaged(atk1_dmg * 3, i.GetComponent<Player>().looking_dir);
            }
        }
        Camera.main.GetComponent<CameraFollow>().DoImppulse(1f);
        PlaySound(audios.attack1);
        zone1show.gameObject.SetActive(false);
        attacking = false;
        anim.SetBool(nameof(skilling), false);
        skilling = false;
        yield return new WaitForSeconds(0.5f);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        material.SetFloat("_CamouflageFade", 0);
        material.SetFloat("_PixelOutlineFade", 0);
        StartCoroutine(delayedPaterntomove());
        
    }

    void Dead()
    {
        dead = true;
        StopAllCoroutines();
        anim.SetTrigger("dead");
        PlaySound(audios.dead);
        zone1show.SetActive(false);
        
        GameManager.Instance.win();
    }

    public void OnDamaged(float dmg, int dir, bool isKnockbackable = false)
    {
            hp -= dmg;
            if (hp <= maxhp / (availableSkillcnt + 1) && availableSkillcnt > 0)
            {
                audioSource.pitch = 1f;
                StopAllCoroutines();
                StartCoroutine(Patern1_Seq());
                availableSkillcnt--;
            }
            if (hp <= 0)
            {
                nowPatern = Patern.dead;
            }
        
    }
    void Steamist_skill()
    {
        zone1show.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
        zone1show.transform.localScale = Patern1Zone;
        zone1show.gameObject.SetActive(true);
        target.transform.Translate(new Vector2(transform.position.x - target.transform.position.x, transform.position.y + 0.2f).normalized * (target.GetComponent<Player>().speed - 0.3f) * Time.deltaTime);
    }

    public void PlaySound(audios audios)
    {
        audioSource.Stop();
        switch (audios)
        {
            case audios.attack1:
                audioSource.clip = audioSources[(int)audios.attack1];
                audioSource.Play();
                break;
            case audios.attack2:
                audioSource.clip = audioSources[(int)audios.attack2];
                audioSource.Play();
                break;
            case audios.patern1:
                audioSource.clip = audioSources[(int)audios.patern1];
                audioSource.Play();
                break;
            case audios.dead:
                audioSource.clip = audioSources[(int)audios.dead];
                audioSource.Play();
                break;
        }
    }
}
