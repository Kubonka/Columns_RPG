using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoison : Skill
{
    
    void Start()
    {
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    public override IEnumerator Cast()
    {
        playerRef.debuffs.Add(this.gameObject);
        yield return null;        
    }    
    public override IEnumerator Proc()
    {
        yield return StartCoroutine(playerRef.TakeDamage(value));
    }

    public override IEnumerator EndProc()
    {
        DestroyImmediate(this.gameObject);
        yield return null;
    }    

    public override void Create()
    {
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/EnemyPoison");
        skillData.SetGemCost();
    }

}
