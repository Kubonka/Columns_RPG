using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Skill
{
    void Start()
    {        
        enemyRef = GameObject.Find("Enemy").GetComponent<EnemyManager>();
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    public override IEnumerator Cast()
    {
        Animator animator = GameObject.Find("Player").GetComponent<PlayerManager>().avatar.GetComponent<Animator>();
        animator.SetBool("Attack", true);
        StartCoroutine(enemyRef.TakeDamage(value));               
        yield return new WaitForSeconds(1f);
        animator.SetBool("Attack", false);
        animator.SetBool("CastSkill", false);
    }
    public override IEnumerator Proc()
    {
        yield return null;
    }

    public override IEnumerator EndProc()
    {        
        yield return null;        
    }
    public override void Create()
    {
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/PlayerAttack");
        skillData.SetGemCost();
    }
}

