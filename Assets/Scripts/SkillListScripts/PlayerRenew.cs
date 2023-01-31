using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenew : Skill
{
        
    List<Transform> damageGemList;
    List<GameObject> newList;
    bool waiting = true;
    void Start()
    {
        damageGemList = new List<Transform>();
        newList = new List<GameObject>();
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
        boardRef.onReturnControl += Continue;
    }
    public override IEnumerator Cast()
    {
        playerRef.buffs.Add(this.gameObject);
        yield return null;
    }

    public override IEnumerator Proc()
    {
        waiting = true;
        damageGemList.Clear();
        newList.Clear();
        int totalChilds = boardRef.transform.childCount;
        for (int i = 0; i < totalChilds; i++)
        {
            if (boardRef.transform.GetChild(i).tag == "DamageGem")
            {
                damageGemList.Add(boardRef.transform.GetChild(i));
            }
        }
        while (damageGemList.Count > 0 && value > 0)
        {
            int rnd = UnityEngine.Random.Range(0, damageGemList.Count);            
            newList.Add(damageGemList[rnd].gameObject);
            damageGemList.RemoveAt(rnd);
            value--;
        }
        value = skillData.baseValue;
        boardRef.externalUse = true;
        boardRef.ResolveMatches(newList);
        while (waiting == true)
        {
            yield return null;
        }        
        boardRef.externalUse = false;
    }
    public override IEnumerator EndProc()
    {
        DestroyImmediate(this.gameObject);
        yield return null;
    }    

    public override void Create()
    {
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/PlayerRenew");
        skillData.SetGemCost();
    }

    private void Continue(bool status)
    {
        waiting = status;
    }

    private void OnDestroy()
    {
        boardRef.onReturnControl -= Continue;
    }
}
