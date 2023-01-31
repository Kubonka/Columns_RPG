using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditorInternal;
using UnityEngine;

public class PlayerManager : Unit
{
      
    private GameObject enemyRef;
    private GameObject boardRef;

    void Start()
    {
        //skills = new GameObject[5];
        boardRef = GameObject.Find("Board");
        enemyRef = GameObject.Find("Enemy");
        
        //skills[0] = new GameObject();
        //skills[0].AddComponent<PlayerThunderStrike>();
        //skills[0].name = "skill 1";
        //skills[0].AddComponent<SpriteRenderer>();
        //skills[0].GetComponent<Skill>().Create();
        //skills[0].transform.parent = this.gameObject.transform;
        //skills[1] = new GameObject();
        //skills[1].AddComponent<PlayerSlice>();
        //skills[1].name = "skill 2";
        //skills[1].AddComponent<SpriteRenderer>();
        //skills[1].GetComponent<Skill>().Create();
        //skills[1].transform.parent = this.gameObject.transform;
        //skills[2] = new GameObject();
        //skills[2].AddComponent<PlayerRenew>();
        //skills[2].name = "skill 3";
        //skills[2].AddComponent<SpriteRenderer>();
        //skills[2].GetComponent<Skill>().Create();
        //skills[2].transform.parent = this.gameObject.transform;
        //skills[3] = new GameObject();
        //skills[3].AddComponent<PlayerFireBall>();
        //skills[3].name = "skill 4";
        //skills[3].AddComponent<SpriteRenderer>();
        //skills[3].GetComponent<Skill>().Create();
        //skills[3].transform.parent = this.gameObject.transform;
    }

    public IEnumerator PhaseOne()
    {
        yield return ResolveTurn();
        boardRef.GetComponent<BoardManager>().currentCombatState = BoardManager.CombatState.PhaseThree;        
    }    

    private void PrepareSkill(GameObject skill)
    {        
        if (skill != null)
        {
            if (currentSkill == null)
            {
                if (skill.GetComponent<Skill>().cooldown == 0)
                {
                    if (CanAfford(skill))
                    {
                        currentSkill = Instantiate(skill) as GameObject;
                        currentSkill.name = "Clone";
                        currentSkill.transform.parent = skill.transform;
                        boardRef.GetComponent<BoardManager>().RemoveFromCollectedGemsAndUpdateCanvas(currentSkill.GetComponent<Skill>().skillData.gemCost);
                        skill.GetComponent<Skill>().cooldown = skill.GetComponent<Skill>().skillData.baseCooldown;

                        if (currentSkill != null)                                               // TEST
                            Debug.Log(currentSkill.GetComponent<Skill>().skillData.skillName); //TEST

                        //ANIMATION
                        animator.SetBool("CastSkill", true);
                    }
                    else
                        Debug.Log("NOT ENOUGH MANA");
                }
                else
                    Debug.Log("SKILL NOT READY");
            }
            else
                Debug.Log("ALREADY CASTING A SKILL");
        }
    }
    public IEnumerator CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrepareSkill(skills[0]);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            PrepareSkill(skills[1]);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PrepareSkill(skills[2]);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            PrepareSkill(skills[3]);
        }
        yield return null;
    }

    private bool CanAfford(GameObject skill)
    {        
        if (boardRef.GetComponent<BoardManager>().collectedGems.attackGem >= skill.GetComponent<Skill>().skillData.gemCost.attackGem &&
            boardRef.GetComponent<BoardManager>().collectedGems.defendGem >= skill.GetComponent<Skill>().skillData.gemCost.defendGem &&
            boardRef.GetComponent<BoardManager>().collectedGems.fireGem >= skill.GetComponent<Skill>().skillData.gemCost.fireGem &&
            boardRef.GetComponent<BoardManager>().collectedGems.airGem >= skill.GetComponent<Skill>().skillData.gemCost.airGem &&
            boardRef.GetComponent<BoardManager>().collectedGems.earthGem >= skill.GetComponent<Skill>().skillData.gemCost.earthGem &&
            boardRef.GetComponent<BoardManager>().collectedGems.waterGem >= skill.GetComponent<Skill>().skillData.gemCost.waterGem)
            return true;
        else
            return false;
    }

    public IEnumerator TakeDamage(int damage)
    {
        int damageRemainder = ReduceArmor(damage);
        if (damageRemainder > 0)
            yield return StartCoroutine(boardRef.GetComponent<BoardManager>().GenerateDamage(damageRemainder));
        animator.SetBool("Hurt", true);
        yield return null;
    }
}
