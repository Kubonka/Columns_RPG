using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerThunderStrike : Skill
{

    private List<GameObject> list;
    private bool waiting = true;
    public GameObject prefab;
    private GameObject go;

    private void Start()
    {
        list = new List<GameObject>();
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
        boardRef.onReturnControl += Continue;
    }

    private IEnumerator Resolve()
    {
        yield return new WaitForSeconds(0.3f);
        boardRef.ResolveMatches(list);
    }
    public override IEnumerator Cast()
    {
        list.Clear();
        int rnd = UnityEngine.Random.Range(0, boardRef.gridWidth);
        while (boardRef.IsPopulatedColumn(rnd) == false)
        {
            rnd = UnityEngine.Random.Range(0, boardRef.gridWidth);
        }
        list = boardRef.GetColumn(rnd);
        boardRef.externalUse = true;
                
        go = Instantiate(Resources.Load("Prefabs/SkillsPrefabs/PlayerThunderStrike"), new Vector2((float)rnd,6), Quaternion.identity) as GameObject;        
        go.GetComponent<Animator>().SetTrigger("Cast");
        StartCoroutine(Resolve());
        //Invoke(boardRef.ResolveMatches(list), 0.3f);
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
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/PlayerThunderStrike");
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
