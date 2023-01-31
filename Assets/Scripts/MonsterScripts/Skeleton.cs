using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Monster
{    
    void Start()
    {
        currentHealth = 100;

        skills = new GameObject[3];
        skills[0] = new GameObject();
        skills[0].AddComponent<EnemyDefend>();
        skills[0].name = "skill 1";
        skills[0].AddComponent<SpriteRenderer>();
        skills[0].GetComponent<Skill>().Create();
        skills[0].transform.parent = this.gameObject.transform;

        skills[1] = new GameObject();
        skills[1].AddComponent<EnemyDefend>();
        skills[1].name = "skill 2";
        skills[1].AddComponent<SpriteRenderer>();
        skills[1].GetComponent<Skill>().Create();
        skills[1].transform.parent = this.gameObject.transform;

        skills[2] = new GameObject();
        skills[2].AddComponent<EnemyDefend>();
        skills[2].name = "skill 3";
        skills[2].AddComponent<SpriteRenderer>();
        skills[2].GetComponent<Skill>().Create();
        skills[2].transform.parent = this.gameObject.transform;
    }
}
