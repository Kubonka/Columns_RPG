using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlice : Skill
{
    private List<GameObject> list;    
    private bool waiting = true;

    private void Start()
    {
        list = new List<GameObject>();
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
        boardRef.onReturnControl += Continue;
    }

    public override IEnumerator Cast()
    {
        list.Clear();
        int topRow = boardRef.GetTopRow();
        int rnd = UnityEngine.Random.Range(0, topRow);
        list = boardRef.GetRow(rnd);
        boardRef.externalUse = true;
        boardRef.ResolveMatches(list);
        while (waiting == true)
        {
            yield return null;
        }
        boardRef.externalUse = false;        
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
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/PlayerSlice");
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
