using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLifeDrain : Skill
{
    private void Start()
    {
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
        enemyRef = GameObject.Find("Enemy").GetComponent<EnemyManager>();
    }
    public override IEnumerator Cast()
    {
        yield return StartCoroutine(playerRef.TakeDamage(value));
        enemyRef.currentHealth += value;
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
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/EnemyLifeDrain");
        skillData.SetGemCost();
    }
}