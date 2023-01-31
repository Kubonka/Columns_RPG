using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Skill : MonoBehaviour
{
    public SkillData skillData;
    public int value;
    public int cooldown;
    public int buffTime;
    public PlayerManager playerRef;
    public BoardManager boardRef;
    public EnemyManager enemyRef;    

    public abstract IEnumerator Cast();
    public abstract IEnumerator Proc();
    public abstract IEnumerator EndProc();
    public abstract void Create();


}

