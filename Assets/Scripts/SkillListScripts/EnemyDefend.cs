using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefend : Skill
{
    
    void Start()
    {
        enemyRef = GameObject.Find("Enemy").GetComponent<EnemyManager>();        
    }    

    public override IEnumerator Cast()
    {
        enemyRef.buffs.Add(this.gameObject);
        yield return null;
    }
    public override IEnumerator Proc()
    {
        yield return null;
    }

    public override IEnumerator EndProc()
    {
        enemyRef.armor -= value;
        DestroyImmediate(this.gameObject);
        yield return null;
    }
    
    public override void Create()
    {
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/EnemyDefend");
        skillData.SetGemCost();
    }
}
