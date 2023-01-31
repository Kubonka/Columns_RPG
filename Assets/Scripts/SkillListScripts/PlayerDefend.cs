using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefend : Skill
{    
    void Start()
    {        
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
        
    }
    public override IEnumerator Cast()
    {        
        playerRef.buffs.Add(this.gameObject);             
        yield return null;
    }
    public override IEnumerator Proc()
    {
        yield return null;
    }

    public override IEnumerator EndProc()
    {
        playerRef.armor -= value;        
        DestroyImmediate(this.gameObject);
        yield return null;
    }

    public override void Create()
    {
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/PlayerDefend");
        skillData.SetGemCost();
    }
    
}
