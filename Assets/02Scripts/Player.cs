using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class Player : MonoBehaviour, IDamageAble
{
    [Header("References")]
    public GameObject PlayerUI;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    CharacterGhostTrail CGT;

    [Header("PlayerStat")]
    public float hp = 100;
    public int looking_dir;
    public float block_stamina = 10f;
    public float speed = 10f;
    float Max_Speed;
    public bool jumped = false;
    public float jumpPower = 10f;
    public bool Knocked = false;
    public bool dead = false;
    bool rolling = false;
    public bool talking = false;
    [Space(10f)]
    [Header("Player Atk1 Stat")]
    public UpgradeData UD;
    public float atk1_dmg;
    public float atk1_cooltime = 0.2f;
    int atk1cnt = 0;
    float nextatk1_combo_gap = 0.5f;
    float atk1_timer = 0;
    public Vector2 atk1_range = new Vector2( 1, 1 );
    [Space(5f)]
    [Header("Pierce ATK Stat")]
    public float Patk_dmg;
    public float Patk_cooltime = 0.2f;
    float Patk_timer = 0;
    public Vector2 Patk_range = new Vector2(2, 0.3f);
    [Space(5f)]
    [Header("Ranged ATK Stat")]
    public GameObject rangeAtkPrefab;
    public float RangeAtk_dmg;
    public float RangeAtk_cooltime = 2f;
    float Ratk_timer = 0;
    public float AtkLifeTime = 3f;
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
    public AudioClip[] Runningsounds;
    [Header("Effects")]
    public GameObject DustEffect;
    public GameObject RunDustEffect;
    public GameObject atk1_hit_eft;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        CGT = GetComponentInChildren<CharacterGhostTrail>();

        Max_Speed = speed;

        PlayerUIManager Playerui = Instantiate(PlayerUI).GetComponent<PlayerUIManager>();
        Playerui.SetPlayer(this);
    }

    private void Update()
    {
        UD = GameManager.Instance.LoadJsonFile<UpgradeData>(Application.dataPath, "UpgradeData");
        Checkgrounded();
        if (!GameManager.Instance.GameStarted)
        {
            return;
        }
        if (dead || GameManager.Instance.isPaused || Knocked)
        {
            return;
        }
        if (!anim.GetBool("def"))
        {
            if (!jumped && !swordDancing)
                Rolling();

            if(!swordDancing&&!spiningAtking&&!rolling)
                Jump();

            if(!swordDancing && !spiningAtking && !rolling)
                Attack();

            if (!swordDancing && !spiningAtking && !rolling && anim.GetInteger("atkcnt") == 0 && !jumped)
                PierceAttack();

            if (!swordDancing && !spiningAtking && !rolling && anim.GetInteger("atkcnt") == 0 && !jumped)
                RangedAttack();

            if (anim.GetInteger("atkcnt") == 0 && !rolling)
            {
                if (!swordDancing)
                {
                    Move();
                    if(UD.UpgradedCnt[(int)UpgradeData.Upgrades.SpinAttack] > 0)
                    spinAttack();
                }
            }

            if (!jumped && !spiningAtking && !rolling && UD.UpgradedCnt[(int)UpgradeData.Upgrades.SwordDance] > 0)
                SwordDance();

            if (block_stamina < 50f)
            {
                block_stamina += Time.deltaTime * 5;
            }
        }
        if (!jumped && !rolling && !spiningAtking&& !swordDancing)
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

    public void Attack1Off()
    {
        atk1cnt = 0;
        anim.SetInteger("atkcnt", atk1cnt);
        anim.SetBool("atk", false);
        atk1_timer = 0;
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

    public void GenerateRunDust()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            SpriteRenderer RunDustSR =Instantiate(RunDustEffect, hit.point, Quaternion.identity).GetComponent<SpriteRenderer>();
            RunDustSR.flipX = Input.GetAxis("Horizontal") < 0;
        }
    }

    void Jump()
    {        
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
    }

    void Checkgrounded()
    {
        if (rigid.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
            if (hit.distance < 1.4f && hit.collider != null)
            {
                if (jumped) Instantiate(DustEffect, hit.point, Quaternion.identity);
                jumped = false;
                anim.SetBool("jumped", jumped);
                anim.SetBool("Knocked", false);
                Knocked = false;
                rigid.velocity = new Vector2(0, rigid.velocity.y);
            }
        }
    }

    void Rolling()
    {
        if((Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.D))&& !rolling && block_stamina > 5f)
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
            StartGenerateGhost();
        }
        else
        {
            StopGenerateGhost();
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
        Gizmos.DrawCube(new Vector2(transform.position.x, transform.position.y - 0.7f), new Vector2(atk1_range.x, atk1_range.y * 0.3f));
    }*/
    
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.A) && atk1_cooltime < atk1_timer)
        {
            if (!jumped)
            {
                nextatk1_combo_gap = 0;
                atk1cnt++;
                if (atk1cnt > 3)
                {
                    atk1_timer = -atk1_cooltime*1.5f;
                    atk1cnt = 0;
                }
                else
                {
                    audioSource.pitch = 1 + atk1cnt * 0.1f;
                    atk1_timer = 0;
                }

                anim.SetInteger("atkcnt", atk1cnt);

                if (atk1cnt == 3)
                {
                    rigid.AddForce(Vector2.right * looking_dir * speed * 1 / 4, ForceMode2D.Impulse);
                }

            }
            else
            {
                atk1_timer = atk1_cooltime - atk1_cooltime * 0.9f;
            }
            anim.SetTrigger("atk");
        }
        else
        {
            atk1_timer += Time.deltaTime;
        }
    }
    public void AttackCheck()
    {
        if (!jumped)
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.2f), atk1_range, 0);
            foreach (Collider2D i in hit)
            {
                IDamageAble E = i.GetComponent<IDamageAble>();
                if (i.gameObject.tag == "Enemy" && E != null)
                {
                    E.OnDamaged(atk1_dmg * anim.GetInteger("atkcnt") + (anim.GetInteger("atkcnt") * UD.UpgradedCnt[(int)UpgradeData.Upgrades.Attack] /5));
                    GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + (Vector3.up * Random.Range(-1f, 1f)), Quaternion.identity);
                    hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                    hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                    hiteft.transform.rotation = Quaternion.Euler(Random.Range(-80f, 80f), 0, 0);
                    Debug.Log(atk1_dmg * anim.GetInteger("atkcnt") + (anim.GetInteger("atkcnt") * UD.UpgradedCnt[(int)UpgradeData.Upgrades.Attack] / 5));
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
                    E.OnDamaged(atk1_dmg + (UD.UpgradedCnt[(int)UpgradeData.Upgrades.Attack] / 5));
                    Debug.Log(atk1_dmg + (UD.UpgradedCnt[(int)UpgradeData.Upgrades.Attack] / 5));
                    GameObject hiteft = Instantiate(atk1_hit_eft, new Vector2(i.transform.position.x - (looking_dir / 2), transform.position.y + Random.Range(-1f, 1f)), Quaternion.Euler(Random.Range(-45f, 45f), 0, Random.Range(-45f, 45f)));
                    hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                }
            }
        }
    }

    void PierceAttack()
    {
        if (Input.GetKeyDown(KeyCode.S) && block_stamina >= 5 && Patk_timer > Patk_cooltime)
        {
            Attack1Off();
            nextatk1_combo_gap = 0;
            atk1_timer = -atk1_cooltime * 0.4f;
            atk1cnt = 2;
            Patk_cooltime = 0;
            anim.ResetTrigger("atk");
            anim.ResetTrigger("PierceAtk");
            anim.SetInteger("atkcnt", 0);
            anim.SetInteger("atkcnt", 2);
            anim.SetTrigger("PierceAtk");

            block_stamina -= 5;
        }
        else
        {
            Patk_timer += Time.deltaTime;
        }
    }
    public void PierceAttackCheck() {
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.3f), Patk_range,0);
        foreach (Collider2D i in hit)
        {
            IDamageAble E = i.GetComponent<IDamageAble>();
            if (i.gameObject.tag == "Enemy" && E != null)
            {
                E.OnDamaged(Patk_dmg + (Patk_dmg * UD.UpgradedCnt[(int)UpgradeData.Upgrades.PierceAttack] / 10));
                GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + Vector3.down * 0.7f, Quaternion.identity);
                hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                hiteft.transform.rotation = Quaternion.Euler(80, 0, 0);
            }
        }
    }

    void RangedAttack()
    {
        if(Input.GetKeyDown(KeyCode.E) && Ratk_timer >   RangeAtk_cooltime)
        {
            anim.SetTrigger("RangeAtk");
            Ratk_timer = 0;
        }
        else
        {
            Ratk_timer += Time.deltaTime;
        }
    }

    public void RangeShoot()
    {
        GameObject ball = Instantiate(rangeAtkPrefab,transform.position,Quaternion.identity  );
    }

    private void spinAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !anim.GetBool("spinAttacking") && block_stamina >= spinCost)
        {
            anim.SetBool("spinAttacking", true);
            block_stamina -= spinCost;
            rigid.velocity = Vector3.zero;
            rigid.AddForce((Vector2.right * looking_dir * 3) + Vector2.up * jumpPower * 0.75f, ForceMode2D.Impulse);
            jumped = true;
            anim.SetBool("jumped", jumped);
            StartGenerateGhost();
        }
    }
    public void spiningAttackCheck()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y - 0.2f), spinATK_Range, 0);
        foreach (Collider2D i in hit)
        {
            IDamageAble E = i.GetComponent<IDamageAble>();
            if (i.gameObject.tag == "Enemy" && E!=null)
            {
                E.OnDamaged(spinATKdmg + (spinATKdmg * (UD.UpgradedCnt[(int)UpgradeData.Upgrades.SpinAttack] - 1) / 8));
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
    
    private void SwordDance()
    {
        if (Input.GetKeyDown(KeyCode.F)&& swordDancing && anim.GetFloat("SpeedMultiplier") < 2)
        {
            anim.SetFloat("SpeedMultiplier", anim.GetFloat("SpeedMultiplier") + 2/3f);
        }

        if (Input.GetKeyDown(KeyCode.F) && !swordDancing && block_stamina >= swordDanceCost)
        {
            block_stamina -= swordDanceCost;
            swordDancing = true;
            anim.SetTrigger("swordDanceTrigger");
            audioSource.pitch = 1.4f;
            StartGenerateGhost();
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
                E.OnDamaged(swordDanceATKdmg + (swordDanceATKdmg * (UD.UpgradedCnt[(int)UpgradeData.Upgrades.SwordDance] - 1) / 10));
                GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + (Vector3.up * Random.Range(-1f, 1f)), Quaternion.identity);
                hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                hiteft.transform.rotation = Quaternion.Euler(Random.Range(-80f, 80f), 0, 0);
            }
        }
    }
    public void EndSwordDancing()
    {
        swordDancing = false;
        anim.SetFloat("SpeedMultiplier", 1);
        audioSource.pitch = 1;
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
    public void FinaleSwordDancingATK()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + looking_dir, transform.position.y - 0.2f), swordDanceATK_Range, 0);
        foreach (Collider2D i in hit)
        {
            IDamageAble E = i.GetComponent<IDamageAble>();
            if (i.gameObject.tag == "Enemy" && E != null)
            {
                E.OnDamaged(finalswordDanceATKdmg + (swordDanceATKdmg * (UD.UpgradedCnt[(int)UpgradeData.Upgrades.SwordDance] -1) / 4));
                GameObject hiteft = Instantiate(atk1_hit_eft, transform.position + (i.transform.position - transform.position).normalized + (Vector3.up * Random.Range(-1f, 1f)), Quaternion.identity);
                hiteft.transform.right = ((i.transform.position - transform.position).normalized);
                hiteft.GetComponent<SpriteRenderer>().flipX = looking_dir < 0;
                hiteft.transform.rotation = Quaternion.Euler(Random.Range(-80f, 80f), 0, 0);
            }
        }
    }

    void Defence()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rigid.velocity = Vector3.zero;
        }
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

    public void OnDamaged(float dmg, int dir = 0, bool isKnockbackable = false)
    {
        if (anim.GetBool("def"))
        {
            if (dir != looking_dir)
            {
                if (block_stamina > dmg/2)
                {
                    anim.SetTrigger("shieldHit");
                    block_stamina -= dmg/2;
                    rigid.AddForce(Vector2.right * dir * dmg / 10, ForceMode2D.Impulse);
                    PlaySound(audios.block);
                }
                else
                {
                    hp -= dmg;
                    anim.SetTrigger("hit");
                    rigid.AddForce((Vector2.up + (Vector2.right * dir)).normalized * dmg / 6, ForceMode2D.Impulse);
                    StartCoroutine(Hit_Seq(isKnockbackable));
                }
            }
            else
            {
                hp -= dmg;
                anim.SetTrigger("hit");
                rigid.AddForce((Vector2.up + (Vector2.right * dir)).normalized * dmg/6, ForceMode2D.Impulse);
                StartCoroutine(Hit_Seq(isKnockbackable));
            }
        }
        else
        {
            hp -= dmg;
            anim.SetTrigger("hit");
            rigid.AddForce((Vector2.up + (Vector2.right * dir)).normalized * dmg/6, ForceMode2D.Impulse);
            StartCoroutine(Hit_Seq(isKnockbackable));
        }
        if(hp <= 0)
        {
            dead = true;
            PlaySound(audios.dead);
            GameManager.Instance.win();
        }
    }

    IEnumerator Hit_Seq(bool isKnockbackable)
    {
        PlaySound(audios.hit);
        if (isKnockbackable)
        {
            anim.SetBool("Knocked", true);
            Knocked = true;
        }
        gameObject.layer = LayerMask.NameToLayer("hited");
        spriteRenderer.color = new Color(255, 255, 255, 0.5f);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = new Color(255, 255, 255, 1f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void StartGenerateGhost()
    {
        CGT.doGenerate = true;
    }
    public void StopGenerateGhost()
    {
        CGT.doGenerate = false;
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

    public void PlayFootStepSound()
    {
        audioSource.PlayOneShot(Runningsounds[Random.Range(0,Runningsounds.Length)]);
    }
}
