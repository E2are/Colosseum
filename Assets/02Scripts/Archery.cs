using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Archery : IBoss, IDamageAble
{
    Animator anim;
    Rigidbody2D rigid;
    AudioSource audioSource;

    GameObject target;

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

    [Space(10f)]
    [Header("ArcherStateMent")]
    public GameObject arrow;
    [SerializeField] bool jumped = false;
    public AudioClip jumpSound;
    public float skillArrowtimes = 15f;
    public CharacterGhostTrail CGT;

    Material material;

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        target = GameObject.Find("Player");
        maxhp = hp;
        nowPatern = Patern.idle;

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
                    CGT.doGenerate = jumped;
                }
                if (skilling)
                {
                    Archer_skill();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(rigid.velocity.x) < 0.2f)
        {
            Debug.DrawRay(transform.position, Vector2.right * -move_dir);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * -move_dir, 1f, LayerMask.GetMask("Ground"));
            if (hit.distance < 1f && hit.collider != null && rigid.velocity.y == 0)
            {
                rigid.AddForce(Vector2.up * 3 + Vector2.right * move_dir * move_Speed * 0.75f, ForceMode2D.Impulse);
                anim.SetBool("jumped", true);
                jumped = true;
                gameObject.layer = 9;
            }
        }

        if (rigid.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
            if (hit.distance < 1.4f && hit.collider != null)
            {
                if(anim.GetBool("jumped"))Turn();
                anim.SetBool("jumped", false);
                jumped = false;
                gameObject.layer = 8;
            }
        }
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

        if (Vector2.Distance(transform.position, target.transform.position) > Attack1Zone.x)
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
        Gizmos.DrawCube(new Vector2(transform.position.x + move_dir, transform.position.y - 0.5f), Attack1Zone*1.5f);
    }*/

    void Attack1()
    {
        if (Vector2.Distance(transform.position, target.transform.position) > Attack1Zone.x && !attacking)
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
        attacking = true;
        Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y), Attack2Zone, 0);
        foreach (Collider2D i in col)
        {
            if(Random.Range(0,3) == 0)
            {
                Collider2D[] co = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + move_dir, transform.position.y), Attack2Zone - Vector2.right * 4, 0);
                foreach(Collider2D i2 in co)
                {
                    if (i2.GetComponent<IDamageAble>() != null && i2.gameObject.CompareTag("Player"))
                    {
                        anim.SetTrigger("pushatk");

                        i2.GetComponent<IDamageAble>().OnDamaged(0, move_dir, true);

                        i2.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 3 + Vector2.right * move_dir * move_Speed, ForceMode2D.Impulse);
                    }
                    yield return new WaitForSeconds(0.3f);
                }
            }
            if (i.gameObject.tag == "Player" && Random.Range(0, 4) != 1)
            {
                if (i.GetComponent<Rigidbody2D>().velocity.y < 0 || rigid.velocity.x == 0 && Random.Range(0, 2) != 1)
                {
                    move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
                    anim.SetBool("jumped", true);
                }
                rigid.AddForce(Vector2.up * 5 + Vector2.right * -move_dir * move_Speed * 1.5f, ForceMode2D.Impulse);
                jumped = true;
                audioSource.PlayOneShot(jumpSound);
                gameObject.layer = 9;
            }
        }
        anim.SetTrigger("atk");
        yield return null;
    }

    void ArrowAtk()
    {
        GameObject Arrow = Instantiate(arrow, new Vector2(transform.position.x + move_dir * 0.2f, transform.position.y - 0.3f), Quaternion.identity);
        audioSource.pitch = 1.4f;
        PlaySound(audios.attack1);
        audioSource.pitch = 1f;
        Arrow.GetComponent<Arrow>().dir = new Vector3(move_dir, 0, 0);
        Arrow.GetComponent<Arrow>().Damage = atk1_dmg;
    }

    void atkoff()
    {
        attacking = false;
        anim.ResetTrigger("atk");
        Turn();
        nowPatern = Patern.idle;
    }

    public void Turn()
    {
        move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
        gameObject.GetComponent<SpriteRenderer>().flipX = move_dir < 0;
    }

    IEnumerator Patern1_Seq()
    {
        skilling = true;
        anim.SetBool(nameof(skilling), true);
        anim.SetTrigger("skilled");
        GameObject[] arrows = new GameObject[((int)skillArrowtimes)];
        GameObject[] paths = new GameObject[((int)skillArrowtimes)];
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < skillArrowtimes; i++)
        {
            arrows[i] = Instantiate(arrow, new Vector2(Random.Range(-Patern1Zone.x, Patern1Zone.x), Patern1Zone.y), Quaternion.Euler(0, 0, Random.Range(-80,-100)));
            arrows[i].GetComponent<Arrow>().dir = Vector2.right;
            arrows[i].GetComponent<Arrow>().speed = 0;
            arrows[i].GetComponent<Arrow>().Damage = atk1_dmg * 2;
            arrows[i].GetComponent<Arrow>().Unblockable = true;

            paths[i] = Instantiate(zone1show, arrows[i].transform.position, arrows[i].transform.rotation);

            RaycastHit2D hit = Physics2D.Raycast(arrows[i].transform.position + arrows[i].transform.right, arrows[i].transform.right, 100f, LayerMask.GetMask("Ground"));

            paths[i].gameObject.SetActive(true);
            paths[i].GetComponent<LineRenderer>().SetPosition(0, arrows[i].transform.position);
            paths[i].GetComponent<LineRenderer>().SetPosition(1, hit.point);

            yield return new WaitForSeconds(0.0001f);
        }
        foreach (GameObject arrow in arrows)
        {
            arrow.GetComponent<Arrow>().speed = 20f;
        }
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject path in paths)
        {
            Destroy(path);
        }
        skilling = false;
        attacking = false;
        anim.SetBool(nameof(skilling), false);
        nowPatern = Patern.move;
    }

    public void BowSound()
    {
        audioSource.pitch = 1.4f;
        PlaySound(audios.attack2);
        audioSource.pitch = 1f;
    }

    public void ShotSound()
    {
        audioSource.pitch = Random.Range(1, 1.4f);
        PlaySound(audios.attack1);       
        audioSource.pitch = 1f;
    }

    void Dead()
    {
        dead = true;
        StopAllCoroutines();
        anim.SetBool("dead",true);
        anim.SetBool("jumped", true);
        PlaySound(audios.dead);
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.right * -move_dir * move_Speed/2 + Vector2.up * move_Speed,ForceMode2D.Impulse);
        gameObject.layer = 9;
        GameManager.Instance.win();
    }

    public void OnDamaged(float dmg, int dir, bool isKnockbackable = false)
    {
        hp -= dmg;
        if (hp < maxhp * (availableSkillcnt) / (maxavailableSkillcnt + 1) && availableSkillcnt > 0)
        {
            audioSource.pitch = 1f;
            availableSkillcnt--;
            StopAllCoroutines();
            StartCoroutine(Patern1_Seq());
        }
        if (hp <= 0)
        {
            nowPatern = Patern.dead;
            PlaySound(audios.dead);
        }
        
    }
    void Archer_skill()
    {
        anim.SetBool("jumped", false);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 && anim.GetBool("jumped") && !dead)
        {
            anim.SetBool("jumped", false);
            rigid.AddForce(Vector2.up * 10 + Vector2.right * move_dir * move_Speed * 1.5f, ForceMode2D.Impulse);
            anim.SetBool("jumped", true);
        }
    }

}
