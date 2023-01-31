using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Monster
{
    
    void Start()
    {
        prefabPath = "Prefabs/PlayerPrefabs/Hero";

        currentHealth = 100;
        
        skills = new GameObject[4];
        skills[0] = new GameObject();
        skills[0].AddComponent<PlayerAttack>();
        skills[0].name = "skill 1";
        skills[0].AddComponent<SpriteRenderer>();
        skills[0].GetComponent<Skill>().Create();
        skills[0].transform.parent = this.gameObject.transform;
        skills[1] = new GameObject();
        skills[1].AddComponent<PlayerSlice>();
        skills[1].name = "skill 2";
        skills[1].AddComponent<SpriteRenderer>();
        skills[1].GetComponent<Skill>().Create();
        skills[1].transform.parent = this.gameObject.transform;
        skills[2] = new GameObject();
        skills[2].AddComponent<PlayerRenew>();
        skills[2].name = "skill 3";
        skills[2].AddComponent<SpriteRenderer>();
        skills[2].GetComponent<Skill>().Create();
        skills[2].transform.parent = this.gameObject.transform;
        skills[3] = new GameObject();
        skills[3].AddComponent<PlayerFireBall>();
        skills[3].name = "skill 4";
        skills[3].AddComponent<SpriteRenderer>();
        skills[3].GetComponent<Skill>().Create();
        skills[3].transform.parent = this.gameObject.transform;        

        this.gameObject.GetComponent<Unit>().LoadCharacter();
    }

}
