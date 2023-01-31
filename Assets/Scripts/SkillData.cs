using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="SkillData",menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public enum Type
    {
        Armor,
        Heal,
        Damage,
    }
    public string skillName;
    public string description;
    public Sprite icon;
    public int baseCooldown;    
    public int baseBuffTime;    
    public CollectedGems gemCost;
    public Type type;
    public int baseValue;    

    [Header("GEM COST")]
    public int attack;
    public int defend;
    public int fire;
    public int air;
    public int earth;
    public int water;

    public void SetGemCost()
    {
        gemCost.attackGem = attack;
        gemCost.defendGem = defend;
        gemCost.fireGem = fire;
        gemCost.airGem = air;
        gemCost.earthGem = earth;
        gemCost.waterGem = water;
    }
}
