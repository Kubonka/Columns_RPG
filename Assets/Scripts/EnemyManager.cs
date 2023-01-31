using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Unit
{
    public int maxHealth = 0;
    public int currentHealth = 100;   

    
    private PlayerManager playerRef;
    private BoardManager boardRef;
    void Start()
    {
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();
        skills = new GameObject[4];
        //skills = GetComponent<Monster>().skills;
        //currentHealth = GetComponent<Monster>().currentHealth;
        //skills = new GameObject[3];
        //skills[0] = new GameObject();
        //skills[0].AddComponent<EnemyAttack>();
        //skills[0].name = "skill 1";
        //skills[0].AddComponent<SpriteRenderer>();
        //skills[0].GetComponent<Skill>().Create();
        //skills[0].transform.parent = this.gameObject.transform;

        //skills[1] = new GameObject();
        //skills[1].AddComponent<EnemyAttack>();
        //skills[1].name = "skill 2";
        //skills[1].AddComponent<SpriteRenderer>();
        //skills[1].GetComponent<Skill>().Create();
        //skills[1].transform.parent = this.gameObject.transform;

        //skills[2] = new GameObject();
        //skills[2].AddComponent<EnemyAttack>();
        //skills[2].name = "skill 3";
        //skills[2].AddComponent<SpriteRenderer>();
        //skills[2].GetComponent<Skill>().Create();
        //skills[2].transform.parent = this.gameObject.transform;
        //foreach (GameObject skill in skills)
        //{
        //    skill.transform.parent = this.gameObject.transform;
        //}
    }
    
    public IEnumerator PhaseThree()
    {
        
        yield return ResolveTurn();     // <---- ACA PASA TODO !
                          
        boardRef.currentCombatState = BoardManager.CombatState.PhaseFour;      
    }
    

    public void EnemySkillPicker()
    {
        int result = -1;
        int rnd = UnityEngine.Random.Range(1, 101);
        if (rnd <= 50)
        {
            result = 0;            
        }
        else if (rnd > 50 && rnd <= 85)
        {
            result = 1;            
        }
        else // rnd 85 to 100 
        {
            result = 2;            
        }
        SetCurrentSkill(result);
    }
    private void SetCurrentSkill(int i)
    {
        if (skills[i].GetComponent<Skill>().cooldown == 0)
        {
            currentSkill = Instantiate(skills[i]) as GameObject;
            currentSkill.name = "Clone";
            currentSkill.transform.parent = skills[i].transform;            
            skills[i].GetComponent<Skill>().cooldown = skills[i].GetComponent<Skill>().skillData.baseCooldown;
        }
        else
            currentSkill = null;   // CON ESTE MODO A VECES NO CASTEA NADA EL ENEMIGO, SINO PUEDO INTENTAR CASTEAR OTRO SKILL        
    }
    public IEnumerator TakeDamage(int damage)
    {
        int damageRemainder = ReduceArmor(damage);
        if (damage > 0)
            currentHealth -= damageRemainder;
        animator.SetBool("Hurt", true);
        yield return null;
    }
}

    
