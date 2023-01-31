using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMultiAttack : Skill
{    
    void Start()
    {
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    public override IEnumerator Cast()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(playerRef.GetComponent<PlayerManager>().TakeDamage(value));
        }        
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
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/EnemyMultiAttack");
        skillData.SetGemCost();
    }
}
