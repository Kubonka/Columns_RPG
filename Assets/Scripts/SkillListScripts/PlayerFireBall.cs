using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PlayerFireBall : Skill
{
    private List<GameObject> list;
    private GameObject fireGem;
    private GameObject go;
    private bool waiting = true;
    public Animator animator;
    private float speed;
    private bool traveling = true;
    private Vector3 target;
    private float distance;
    private float angle;
    private Vector3 direction;

    private void Start()
    {
        list = new List<GameObject>();
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
        enemyRef = GameObject.Find("Enemy").GetComponent<EnemyManager>();
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
        boardRef.onReturnControl += Continue;
        speed = 20f;
    }
    
    private IEnumerator AnimateFireball()
    {
        while (traveling)
        {
            //mover go hacia target
            go.transform.position += direction.normalized * Time.deltaTime * speed;
            distance = Vector3.Distance(go.transform.position, target);
            // Si llego al target traveling = false
            if (distance < 0.1f)
                traveling = false;
            else
                yield return null;            
        }
        animator.SetInteger("State", 2);
        boardRef.ResolveMatches(list);
    }

    public override IEnumerator Cast()
    {
        list.Clear();
        enemyRef.TakeDamage(skillData.baseValue);
        foreach (Transform child in boardRef.transform)
        {
            if (child.tag == "FireGem")
            {
                list.Add(child.gameObject);
            }
        }
        if (list.Count > 0)
        {
            int rnd = UnityEngine.Random.Range(0, list.Count);
            fireGem = list[rnd];
            list.Clear();
            list = boardRef.Get3by3((int)fireGem.transform.position.y, (int)fireGem.transform.position.x);
            boardRef.externalUse = true;

            //COMIENZA LA ANIMACION
            target = fireGem.transform.position;
            go = Instantiate(Resources.Load("Prefabs/SkillsPrefabs/PlayerFireball"), new Vector3(playerRef.gameObject.transform.position.x, playerRef.transform.position.y, 0), Quaternion.identity) as GameObject;
            go.transform.localScale = new Vector3(2, 2, 2);
            angle = CalculateAngle();
            go.transform.eulerAngles = Vector3.forward * angle;
            animator = go.GetComponent<Animator>();
            animator.SetInteger("State", 1);
            target = new Vector3(fireGem.transform.position.x, fireGem.transform.position.y, 0);
            Debug.Log(target.x);
            Debug.Log(target.y);
            yield return StartCoroutine(AnimateFireball());
            //boardRef.ResolveMatches(list);
            while (waiting == true)
            {
                yield return null;
            }
            boardRef.externalUse = false;
        }
    }

    private float CalculateAngle()
    {
        direction = target - go.transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90+145;
        return angle;
    }
    public override IEnumerator EndProc()
    {
        yield return null;
    }

    public override IEnumerator Proc()
    {
        yield return null;
    }

    public override void Create()
    {
        skillData = Resources.Load<SkillData>("ScriptableObjects/Skills/PlayerFireBall");
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
