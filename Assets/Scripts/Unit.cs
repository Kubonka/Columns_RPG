using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{    
    public int armor = 0;
    public Animator animator;
    public GameObject avatar;
    public GameObject[] skills;
    public List<GameObject> buffs;
    public List<GameObject> debuffs;
    public GameObject currentSkill;    
    private UnitUIManager ui;
    
    public event Action<bool> onDestroy;

    private void Start()
    {
        
        buffs = new List<GameObject>();
        debuffs = new List<GameObject>();
        
    }

    public IEnumerator TickBuffsAndDebuffs()
    {
        ui = this.gameObject.GetComponent<UnitUIManager>();
        Debug.Log(ui.canvas.name);

        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            Skill buff = buffs[i].GetComponent<Skill>();
            if (buff.buffTime > 1)
            {
                buff.buffTime--;
                yield return StartCoroutine(buff.Proc());
            }
            else
            {
                yield return StartCoroutine(buff.EndProc());                 
                buffs.RemoveAt(i);                
                onDestroy?.Invoke(true); // Avisa que se destruye un SKILL
            }
        }        
    }
    public int ReduceArmor(int damage)
    {
        for (int i = buffs.Count-1; i >= 0 ; i--)
        {
            if (buffs[i].GetComponent<Skill>().skillData.type == SkillData.Type.Armor)
            {
                if (damage >= buffs[i].GetComponent<Skill>().value)
                {
                    armor -= buffs[i].GetComponent<Skill>().value;
                    damage -= buffs[i].GetComponent<Skill>().value;
                    buffs.RemoveAt(i);
                    onDestroy?.Invoke(true); // Avisa que se destruye un SKILL
                }
                else
                {
                    buffs[i].GetComponent<Skill>().value -= damage;                    
                    armor -= damage;
                    damage = 0;
                }               
            }
        }        
        return damage;
    }

    public void ResetSkill()
    {
        currentSkill.GetComponent<Skill>().cooldown = currentSkill.GetComponent<Skill>().skillData.baseCooldown;
        currentSkill.GetComponent<Skill>().buffTime = currentSkill.GetComponent<Skill>().skillData.baseBuffTime;
        currentSkill.GetComponent<Skill>().value = currentSkill.GetComponent<Skill>().skillData.baseValue;
    }

    public IEnumerator ResolveTurn()
    {
        foreach (GameObject skill in skills)    //REDUCE COOLDOWNS
        {
            if (skill != null)
            {
                if (skill.GetComponent<Skill>().cooldown > 0)
                    skill.GetComponent<Skill>().cooldown--;
            }
        }
        //PROC BUFFS AND DEBUFFS IN UNIT

        yield return StartCoroutine(TickBuffsAndDebuffs());

        //CAST SKILL
        if (currentSkill != null)
        {
            ResetSkill();
            Skill _skill = currentSkill.GetComponent<Skill>();
            //ANIMATION
            animator.SetBool("CastSkill", false);
            yield return StartCoroutine(_skill.Cast());
            yield return StartCoroutine(_skill.Proc());
            if (currentSkill.GetComponent<Skill>().skillData.baseBuffTime > 0)
                this.gameObject.GetComponent<UnitUIManager>().CreateElement(true);
            else
                DestroyImmediate(currentSkill);
            //Destroy(currentSkill);
            currentSkill = null;
        }
        this.gameObject.GetComponent<UnitUIManager>().Refresh(buffs, debuffs);
    }

    public void LoadCharacter()
    {        
        avatar = Instantiate(Resources.Load(this.gameObject.GetComponent<Monster>().prefabPath), transform.position, Quaternion.identity) as GameObject;
        avatar.transform.parent = this.transform;
        avatar.transform.localPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0);
        //skills[0] = this.gameObject.GetComponent<Monster>().skills[0];
        //skills[1] = this.gameObject.GetComponent<Monster>().skills[1];
        //skills[2] = this.gameObject.GetComponent<Monster>().skills[2];        
        skills = this.gameObject.GetComponent<Monster>().skills;
        animator = avatar.GetComponent<Animator>();
    }
}
        
    

    
