using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageAble
{
    [Header("References")]
    public GameObject PlayerUI;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    [Header("PlayerStat")]
    public float hp = 100;
    public int looking_dir;
    public float block_stamina = 10f;
    public float speed = 10f;
    float Max_Speed;
    public bool jumped = false;
    public float jumpPower = 10f;
    public bool dead = false;
    bool rolling = false;
    public bool talking = false;
    [Space(10f)]
    [Header("Player Atk1 Stat")]
    public float atk1_dmg;
    public float atk1_cooltime = 0.2f;
    int atk1cnt = 0;
    float nextatk1_combo_gap = 0.5f;
    float atk1_coltime = 0;
    public Vector2 atk1_range = new Vector2( 1, 1 );
    [Space(5f)]
    [Header("Spin ATK Stat")]
    public float spinATKdmg;
    public float spinCost = 20f;
    public bool spiningAtking = false;
    public Vector2 spinATK_Range = new Vector2(2, 2);
    [Header("SwordDance ATK Stat")]
    public float swordDanceATKdmg;
    public float finalswordDanceATKdmg;
    public float swordDanceCost = 20f;
    public bool swordDancing = false;
    public bool FinalSwordDancing = false;
    public Vector2 swordDanceATK_Range = new Vector2(2, 2);
    
    [Header("Player Sounds")]
    public AudioClip[] audioSources;
    public enum audios
    {
        attack = 0,
        block,
        jump,
        hit,
        dead
    }
    public AudioClip[] Rollingsounds;
    [Header("Effects")]
    public GameObject DustEffect;
    public GameObject atk1_hit_eft;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        Max_Speed = speed;

        PlayerUIManager Playerui = Instantiate(PlayerUI).GetComponent<PlayerUIManager>();
        Playerui.SetPlayer(this);
    }

    private void Update()
    {
        if (talking)
        {
            return;
        }
        if (!dead)
        {
            if (!anim.GetBool("def"))
            {
                if (!jumped && !swordDancing)
                {
                    Rolling();
                }
                if (!rolling)
                {
                    if (!spiningAtking)
                    {
                        Jump();
                        if(!swordDancing)
                        Attack1();
                        if(!jumped)
                        SwordDance();
                    }
                    if (anim.GetInteger("atkcnt") == 0)
                    {
                        if (!swordDancing)
                        {
                            Move();

                            spinAttack();
                        }
                    }
                }                

                if (block_stamina < 50f)
                {
                    block_stamina += Time.deltaTime * 5;
                }
            }
            if (!anim.GetBool("jumped") && !rolling && !spiningAtking)
            {
                Defence();
            }

            //Combo Attack Gap
            if (nextatk1_combo_gap > atk1_cooltime * 2.5f)
            {
                atk1cnt = 0;
                anim.SetInteger("atkcnt", atk1cnt);
            }
            else
            {
                nextatk1_combo_gap += Time.deltaTime;
            }
        }
    }

    public void Attack1Off()
    {
        atk1cnt = 0;
        anim.SetInteger("atkcnt", atk1cnt);
        anim.SetBool("atk", false);
        atk1_coltime = 0;
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");

        transform.Translate(Vector2.right * h * speed*Time.deltaTime);

        if (Mathf.Abs(h) > 0f)
        {
            anim.SetBool("running", true);
            spriteRenderer.flipX = h < 0;
            looking_dir = h > 0 ? 1 : -1;
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    void Jump()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1.5f,Color.red);
        
        if (Input.GetKeyDown("space") && !jumped && !Input.GetKey(KeyCode.DownArrow))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));

            PlaySound(audios.jump);

            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumped = true;
            anim.SetBool("jumped", jumped);
            if (hit.collider != null)
                Instantiate(DustEffect, hit.point, Quaternion.identity);
        }
        if (rigid.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));

            if(hit.distance < 1.4f && hit.collider != null && jumped)
            {
                Instantiate(DustEffect, hit.point, Quaternion.identity);
            }

            if (hit.distance < 1.4f && hit.collider != null)
            {
                jumped = false;
                anim.SetBool("jumped", jumped);
            }
        }
    }

    void Rolling()
    {
        if(Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space) && !rolling && block_stamina > 5f)
        {
            block_stamina -= 10f;
            anim.SetTrigger("Dash");
            rolling = true;
            gameObject.layer = LayerMask.NameToLayer("Dashing");

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
            if(hit.collider != null)
            Instantiate(DustEffect, hit.point, Quaternion.identity);
        }
        if (rolling)
        {
            transform.Translate(Vector2.right * looking_dir * speed * 1.2f * Time.deltaTime);
        }
    }

    public void startRoll()
    {
        audioSource.clip = Rollingsounds[0];
        audioSource.Play();
    }
    public void endRoll()
    {
        audioSource.clip = Rollingsounds[1];
        audioSource.Play();
    }

    public void RollingOff()
    {
        rolling = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(transform.position.x + looking_dir, transform.position.y-0.2f), atk1_range);
    }*/
    void spinAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !anim.GetBool("spinAttacking") && block_stamina >= spinCost)
        {
            anim.SetBool("spinAttacking", true);
            block_stamina -= spinCost;
            rigid.velocity = Vector3.zero;
            rigid.AddForce((Vector2.right * looking_dir * 3) + Vector2.up * jumpPower * 0.75f, ForceMode2D.Impulse);
            jumped = true;
            anim.SetBool("jumped", jumped);
        }
    }

    public void spiningAttack()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y - 0.2f), spinATK_Range, 0);
        foreach (Collider2D i in hit)
        {
            IDamageAble E = i.GetComponent<IDamageAble>();
            if (i.gameObject.tag == "Enemy" && E!=null)
            {
                E.OnDamaged(spinATKdmg);
                GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized, Quaternion.identity);
                hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                Debug.Log(spinATKdmg);
            }
        }
    }

    public void spiningAtkStart()
    {
        rigid.velocity = Vector3.zero;
        spiningAtking = true;
        rigid.gravityScale = 0;
    }

    public void spiningAtkEnd()
    {
        spiningAtking = false;
        anim.SetBool("spinAttacking", false);
        rigid.gravityScale = 1;
    }

    public void SwordDance()
    {
        if (Input.GetKeyDown(KeyCode.F) && swordDancing)
        {
            anim.SetFloat("SpeedMultiplier", 2f);
        }

        if (Input.GetKeyDown(KeyCode.F) && !swordDancing && block_stamina >= swordDanceCost)
        {
            block_stamina -= swordDanceCost;
            swordDancing = true;
            anim.SetTrigger("swordDanceTrigger");
            audioSource.pitch = 1.4f;
        }
        
        if (FinalSwordDancing)
        {
            transform.Translate(Vector2.right * looking_dir * speed * 2 * Time.deltaTime);
        }
    }

    public void SwordDanceATK()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.2f), swordDanceATK_Range, 0);
        foreach (Collider2D i in hit)
        {
            IDamageAble E = i.GetComponent<IDamageAble>();
            if (i.gameObject.tag == "Enemy" && E != null)
            {
                E.OnDamaged(swordDanceATKdmg);
                GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + (Vector3.up * Random.Range(-1f, 1f)), Quaternion.identity);
                hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                hiteft.transform.rotation = Quaternion.Euler(Random.Range(-80f, 80f), 0, 0);
            }
        }
    }

    public void FinaleSwordDancingATK()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.2f), swordDanceATK_Range, 0);
        foreach (Collider2D i in hit)
        {
            IDamageAble E = i.GetComponent<IDamageAble>();
            if (i.gameObject.tag == "Enemy" && E != null)
            {
                E.OnDamaged(finalswordDanceATKdmg);
                GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + (Vector3.up * Random.Range(-1f, 1f)), Quaternion.identity);
                hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                hiteft.transform.rotation = Quaternion.Euler(Random.Range(-80f, 80f), 0, 0);
            }
        }
    }

    public void FinaleSwordDancingStart()
    {
        FinalSwordDancing = true;
        anim.SetFloat("SpeedMultiplier",1);
    }

    public void FinaleSwordDancingEnd()
    {
        FinalSwordDancing = false;
        audioSource.pitch = 1;
    }

    public void EndSwordDancing()
    {
        swordDancing = false;
        anim.SetFloat("SpeedMultiplier", 1);
        audioSource.pitch = 1;
    }

    void Attack1()
    {
        if (Input.GetKeyDown(KeyCode.A) && atk1_cooltime < atk1_coltime)
        {
            if (!jumped)
            {
                nextatk1_combo_gap = 0;
                atk1cnt++;
                if (atk1cnt > 3)
                {
                    atk1_coltime = -atk1_cooltime;
                    atk1cnt = 0;
                }
                else
                {
                    audioSource.pitch = 1 + atk1cnt * 0.1f;
                    atk1_coltime = 0;
                }

                anim.SetInteger("atkcnt", atk1cnt);

                if (atk1cnt == 3)
                {
                    rigid.AddForce(Vector2.right * looking_dir * speed * 1 / 4, ForceMode2D.Impulse);
                }

            }
            else
            {
                atk1_coltime = atk1_cooltime - atk1_cooltime * 0.9f;
            }
            anim.SetTrigger("atk");
        }
        else
        {
            atk1_coltime += Time.deltaTime;
        }
    }

    public void Attack() {
        if (!jumped)
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.2f), atk1_range, 0);
            foreach (Collider2D i in hit)
            {
                IDamageAble E = i.GetComponent<IDamageAble>();
                if (i.gameObject.tag == "Enemy" && E != null)
                {
                    E.OnDamaged(spinATKdmg);
                    GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + (Vector3.up * Random.Range(-1f,1f)), Quaternion.identity);
                    hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                    hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                    hiteft.transform.rotation = Quaternion.Euler(Random.Range(-80f, 80f), 0, 0);
                    Debug.Log(atk1_dmg * (anim.GetInteger("atkcnt")));
                }
            }
        }
        else
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.2f), atk1_range, 0);
            foreach (Collider2D i in hit)
            {
                IDamageAble E = i.GetComponent<IDamageAble>();
                if (i.gameObject.tag == "Enemy" && E != null)
                {
                    E.OnDamaged(spinATKdmg);
                    Debug.Log(atk1_dmg);
                    GameObject hiteft = Instantiate(atk1_hit_eft, new Vector2(i.transform.position.x - (looking_dir / 2), transform.position.y + Random.Range(-1f, 1f)), Quaternion.Euler(Random.Range(-45f, 45f), 0, Random.Range(-45f, 45f)));
                    hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                }
            }
        }
    }

    void Defence()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            anim.SetBool("def", true);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            anim.SetBool("def", false);
            rigid.velocity = Vector3.zero;
        }
    }

    public void OnDamaged(float dmg, int dir)
    {
        if (anim.GetBool("def"))
        {
            if (dir != looking_dir)
            {
                if (block_stamina > dmg)
                {
                    block_stamina -= dmg/2;
                    rigid.AddForce(Vector2.right * dir * dmg / 10, ForceMode2D.Impulse);
                    PlaySound(audios.block);
                }
                else
                {
                    hp -= dmg;
                    anim.SetTrigger("hit");
                    rigid.AddForce((Vector2.up + (Vector2.right * dir)).normalized * dmg / 6, ForceMode2D.Impulse);
                    StartCoroutine(Hit_Seq());
                }
            }
            else
            {
                hp -= dmg;
                anim.SetTrigger("hit");
                rigid.AddForce((Vector2.up + (Vector2.right * dir)).normalized * dmg/6, ForceMode2D.Impulse);
                StartCoroutine(Hit_Seq());
            }
        }
        else
        {
            hp -= dmg;
            anim.SetTrigger("hit");
            rigid.AddForce((Vector2.up + (Vector2.right * dir)).normalized * dmg/6, ForceMode2D.Impulse);
            StartCoroutine(Hit_Seq());
        }
        if(hp <= 0)
        {
            dead = true;
            PlaySound(audios.dead);
            GameManager.Instance.win();
        }
    }

    IEnumerator Hit_Seq()
    {
        PlaySound(audios.hit);
        gameObject.layer = LayerMask.NameToLayer("hited");
        spriteRenderer.color = new Color(255, 255, 255, 0.5f);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = new Color(255, 255, 255, 1f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void PlaySound(audios audios)
    {
        switch(audios)
        {
            case audios.attack:
                audioSource.PlayOneShot(audioSources[(int)audios.attack]);
                break;
            case audios.block:
                audioSource.PlayOneShot(audioSources[(int)audios.block]);
                break;
            case audios.jump:
                audioSource.PlayOneShot(audioSources[(int)audios.jump]);
                break;
            case audios.hit:
                audioSource.PlayOneShot(audioSources[(int)audios.hit]);
                break;
            case audios.dead:
                audioSource.PlayOneShot(audioSources[(int)audios.dead]);
                break;
        }
        
    }
}
