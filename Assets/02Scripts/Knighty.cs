using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knighty : IBoss, IDamageAble
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
    public bool ShoulderBashing = false;
    public int currentAttack1Index = 0;
    bool attacking;
    float atk1delayTimer = 0;
    public bool turnAttackinghit = false;
    bool earthQuakeHit = false;
    public Vector2 Attack1Zone = new Vector2(2, 2);
    public Vector2 Attack2Zone = new Vector2(5, 1);
    public bool targetpulled = false;
    public GameObject zone1show;

    [Header("Boss Patern stats")]
    public Vector2 Patern1Zone = new Vector2(8, 5);
    public GameObject PaternAtkPrefab;
    public Patern nowPatern;

    public CharacterGhostTrail CGT;
    [SerializeField] bool jumepd;
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
                    Knighty_skill();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (rigid.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, LayerMask.GetMask("Ground"));
            if (hit.distance < 2.6f && hit.collider != null)
            {
                anim.SetBool("jumped", false);
                gameObject.layer = 8;
            }
        }

        anim.SetFloat("yvelocity", rigid.velocity.y);
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
            nowPatern = Patern.attack;
        }
    }

    private void Move()
    {
        if (Vector2.Distance(transform.position, target.transform.position) > Attack1Zone.x*4)
        {
            move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
            gameObject.GetComponent<SpriteRenderer>().flipX = move_dir < 0;
            transform.Translate(Vector2.right * move_dir * move_Speed * Time.deltaTime);
            anim.SetBool("running", true);
        }
        else
        {
            nowPatern = Patern.attack;
            if (currentAttack1Index == 0)
            {
                atk1delayTimer = attack1_Delay;
            }
            else
            {
                atk1delayTimer = 0;
            }
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
        if (target.GetComponent<Player>().dead)
        {
            nowPatern = Patern.idle;
        }
        else
        {
            if (!attacking)
            {
                StopAllCoroutines();
                switch (currentAttack1Index)
                {
                    case 0:
                        if (Vector2.Distance(transform.position, target.transform.position) < Attack1Zone.x * 3)
                        {
                            StartCoroutine(Atk1Seq());
                            anim.SetBool("running", false);
                        }
                        else
                        {
                            transform.Translate(Vector2.right * move_dir * move_Speed * Time.deltaTime);
                            Turn();
                            anim.SetBool("running", true);
                        }
                        break;
                    case 1:
                        if (Vector2.Distance(transform.position, target.transform.position) < Attack1Zone.x * 2)
                        {
                            StartCoroutine(Atk2Seq());
                            anim.SetBool("running", false);
                        }
                        else
                        {
                            transform.Translate(Vector2.right * move_dir * move_Speed * Time.deltaTime);
                            Turn();
                            anim.SetBool("running", true);
                        }
                        break;
                }
            }
        }
        if (ShoulderBashing)
        {
            transform.Translate(Vector2.right * move_Speed * 2f * move_dir * Time.deltaTime);
        }
    }

    IEnumerator Atk1Seq()
    {
        audioSource.pitch = 1f;
        PlaySound(audios.attack1);
        attacking = true;
        anim.SetTrigger("Attack1");
        anim.SetBool("ShoulderBashing", true);
        ShoulderBashing = true;
        CGT.doGenerate = true;
        yield return new WaitForSeconds(1f);
        CGT.doGenerate = false;
        ShoulderBashing = false;
        if (target.transform.position.x < transform.position.x && move_dir == 1 || target.transform.position.x > transform.position.x && move_dir == -1)
        {
            anim.SetTrigger("TurnAtk");
            audioSource.pitch = 1.2f;
            yield return new WaitForSeconds(1.1f);
            if (!turnAttackinghit && earthQuakeHit || Vector2.Distance(transform.position, target.transform.position) < Attack1Zone.x*2.4f)
            {
                anim.SetTrigger("Attack2");
                rigid.AddForce(Vector2.right * move_dir * move_Speed + Vector2.up * 2,ForceMode2D.Impulse);
                yield return new WaitForSeconds(1.4f);
            }
        }

        anim.SetBool("ShoulderBashing", false);
        currentAttack1Index = 1;
        attacking = false;
        Turn();
        audioSource.pitch = 1;
        nowPatern = Patern.idle;
    }

    IEnumerator Atk2Seq()
    {
        attacking = true;
        audioSource.pitch = 1f;
        PlaySound(audios.attack1);
        Turn();
        rigid.gravityScale = 1;
        gameObject.layer = 9;
        rigid.AddForce(Vector2.right * -move_dir * move_Speed * 2 + Vector2.up * move_Speed, ForceMode2D.Impulse);
        anim.SetBool("jumped", true);
        CGT.doGenerate = true;
        yield return new WaitUntil(() => !anim.GetBool("jumped"));
        yield return new WaitForSeconds(0.2f);

        gameObject.layer = 9;
        rigid.AddForce(Vector2.right * move_dir * Mathf.Abs(transform.position.x - target.transform.position.x) + Vector2.up * move_Speed * 2, ForceMode2D.Impulse);
        anim.SetTrigger("Attack1");
        anim.SetBool("jumped", true);
        anim.SetBool("attacking", true);
        yield return new WaitForSeconds(0.3f);

        rigid.gravityScale = 4;
        yield return new WaitUntil(()=>!anim.GetBool("jumped"));
        CGT.doGenerate = false;

        rigid.gravityScale = 1;
        gameObject.layer = 8;
        yield return new WaitForSeconds(0.4f);

        anim.SetBool("attacking", false);
        currentAttack1Index = 0;
        if (Vector2.Distance(target.transform.position,transform.position)<Attack1Zone.x * 2 && currentAttack1Index == 0)
        {
            Turn();
            anim.SetBool("jumped", true);
            CGT.doGenerate = true;
            gameObject.layer = 9;
            rigid.AddForce(Vector2.right * -move_dir * move_Speed * 2 + Vector2.up * move_Speed, ForceMode2D.Impulse);
            yield return new WaitUntil(() => !anim.GetBool("jumped"));
            CGT.doGenerate = false;
            gameObject.layer = 8;
        }
        
        attacking = false;
        Turn();
        audioSource.pitch = 1;
        nowPatern = Patern.idle;
    }

    public void EndShoulderBash()
    {
        ShoulderBashing = false;
    }

    public void CheckAtk1()
    {
        Collider2D[] collider = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + move_dir * 1.5f, transform.position.y - 1f), Attack1Zone, 0);
        foreach (Collider2D i in collider)
        {
            if (i.gameObject.tag == "Player")
            {
                i.GetComponent<Player>().OnDamaged(atk1_dmg, move_dir);
                i.GetComponent<Rigidbody2D>().AddForce(Vector2.right*move_dir * atk1_dmg, ForceMode2D.Impulse);
            }
        }
    }

    public void TurnAtk(int timethedir = -1)
    {
        turnAttackinghit = false;
        earthQuakeHit = false;
        move_dir *= timethedir;
        Camera.main.GetComponent<CameraFollow>().DoImppulse(1f);
        Collider2D[] collider = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + move_dir * 2f, transform.position.y - 1f), Attack2Zone, 0);
        foreach (Collider2D i in collider)
        {
            Player player = i.GetComponent<Player>();
            if (i.gameObject.tag == "Player" && player != null)
            {
                i.GetComponent<Player>().OnDamaged(atk1_dmg, move_dir);
                i.GetComponent<Rigidbody2D>().AddForce(Vector2.right * move_dir * atk1_dmg, ForceMode2D.Impulse);
                turnAttackinghit = true;
                player.GetComponent<Animator>().SetBool("jumped", true);
                player.GetComponent<Animator>().SetBool("def", false);
            }
        }
        if (target != null && !turnAttackinghit)
        {
            Player player = target.GetComponent<Player>();
            if (player != null && !player.jumped)
            {
                earthQuakeHit = true;
                player.OnDamaged(0, 0, true);
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.right * -move_dir * Vector2.Distance(transform.position, target.transform.position) * 1.4f + Vector2.up * Vector2.Distance(transform.position, target.transform.position) * 0.5f, ForceMode2D.Impulse);
                player.jumped = true;
                player.GetComponent<Animator>().SetBool("jumped", true);
                player.GetComponent<Animator>().SetBool("def", false);
            }
        }
    }

    public void Turn()
    {
        move_dir = target.transform.position.x > transform.position.x ? 1 : -1;
        gameObject.GetComponent<SpriteRenderer>().flipX = move_dir < 0;
    }

    public void CheckAtk2()
    {
        Camera.main.GetComponent<CameraFollow>().DoImppulse(1.8f);
        Collider2D[] collider = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + move_dir * 1.5f, transform.position.y - 1f), Attack2Zone, 0);
        foreach (Collider2D i in collider)
        {
            if (i.gameObject.tag == "Player")
            {
                i.GetComponent<Player>().OnDamaged(atk1_dmg * 3, move_dir, true);
                i.GetComponent<Rigidbody2D>().AddForce(Vector2.right * move_dir * atk1_dmg, ForceMode2D.Impulse);
                Player player = i.GetComponent<Player>();
                if(player != null)
                {
                    player.GetComponent<Animator>().SetBool("jumped", true);
                    player.GetComponent<Animator>().SetBool("def", false);
                }
            }
        }
    }

    void atkoff()
    {
        attacking = false;
        anim.SetBool("ShoulderBashing", false);
        nowPatern = Patern.idle;
    }

    IEnumerator Patern1_Seq()
    {
        yield return new WaitUntil(() => !anim.GetBool("jumped"));
        skilling = true;
        anim.SetTrigger("skilled");
        anim.SetBool("skilling", true);
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        //preShowAttackZone
        List<GameObject> zoneShow = new List<GameObject>();
        for (int i = 3; i < Patern1Zone.x; i += 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * i, Vector2.down, 20, LayerMask.GetMask("Ground"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.right * -i, Vector2.down, 20, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit2.collider != null)
            {
                GameObject patern1show = Instantiate(zone1show, hit.point + Vector2.up / 2, Quaternion.identity);
                patern1show.SetActive(true);
                patern1show.transform.localScale = Vector3.one + Vector3.up;
                zoneShow.Add(patern1show);
                GameObject patern1show2 = Instantiate(zone1show, hit2.point + Vector2.up / 2, Quaternion.identity);
                patern1show2.SetActive(true);
                patern1show2.transform.localScale = Vector3.one + Vector3.up;
                zoneShow.Add(patern1show2);
            }
            yield return null;
        }
        yield return new WaitForSeconds(1.4f);
        for (int i = zoneShow.Count-1; i >= 0; i--)
        {
            GameObject destroy = zoneShow[i];
            zoneShow.RemoveAt(i);
            Destroy(destroy);
        }
        //atk
        for (int i = 3;i < Patern1Zone.x;i += 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * i, Vector2.down,20,LayerMask.GetMask("Ground"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.right * -i, Vector2.down, 20, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit2.collider != null) {
                GameObject atkPrefab = Instantiate(PaternAtkPrefab);
                atkPrefab.transform.position = hit.point;
                GameObject atkPrefab2 = Instantiate(PaternAtkPrefab);
                atkPrefab2.transform.position = hit2.point;
                atkPrefab2.GetComponent<SpriteRenderer>().flipX = true;
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }
        //preShowAttackZone
        for (int i = 1; i < Patern1Zone.x; i += 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * i, Vector2.down, 20, LayerMask.GetMask("Ground"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.right * -i, Vector2.down, 20, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit2.collider != null)
            {
                GameObject patern1show = Instantiate(zone1show, hit.point + Vector2.up / 2, Quaternion.identity);
                patern1show.SetActive(true);
                patern1show.transform.localScale = Vector3.one + Vector3.up;
                zoneShow.Add(patern1show);
                GameObject patern1show2 = Instantiate(zone1show, hit2.point + Vector2.up / 2, Quaternion.identity);
                patern1show2.SetActive(true);
                patern1show2.transform.localScale = Vector3.one + Vector3.up;
                zoneShow.Add(patern1show2);
            }
            yield return null;
        }
        yield return new WaitForSeconds(1.4f);
        for (int i = zoneShow.Count-1; i >= 0; i--)
        {
            GameObject destroy = zoneShow[i];
            zoneShow.RemoveAt(i);
            Destroy(destroy);
        }
        //atk
        for (int i = 1;i < Patern1Zone.x;i += 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * i, Vector2.down,20,LayerMask.GetMask("Ground"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.right * -i, Vector2.down, 20, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit2.collider != null) {
                GameObject atkPrefab = Instantiate(PaternAtkPrefab);
                atkPrefab.transform.position = hit.point;
                GameObject atkPrefab2 = Instantiate(PaternAtkPrefab);
                atkPrefab2.transform.position = hit2.point;
                atkPrefab2.GetComponent<SpriteRenderer>().flipX = true;
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        anim.SetBool("skilling",false);
        yield return new WaitForSeconds(0.5f);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        skilling = false;
        nowPatern = Patern.idle;
    }

    void Dead()
    {
        dead = true;
        StopAllCoroutines();
        anim.SetTrigger("dead");
        anim.SetBool("Dead", true);
        PlaySound(audios.dead);
        zone1show.SetActive(false);

        GameManager.Instance.win();
    }

    public void OnDamaged(float dmg, int dir, bool isKnockbackable = false)
    {
        if (skilling) return;
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
    void Knighty_skill()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (ShoulderBashing)
        {
            IDamageAble damageAble = collision.gameObject.GetComponent<IDamageAble>();
            if(damageAble != null)
            {
                damageAble.OnDamaged(atk1_dmg,move_dir);
            }
        }
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
