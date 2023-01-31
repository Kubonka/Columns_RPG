using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Skill
{
    void Start()
    {        
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    public override IEnumerator Cast()
    {
        yield return StartCoroutine(playerRef.TakeDamage(value));        
        yield return null;
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
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/EnemyAttack");
        skillData.SetGemCost();        
    }
}
