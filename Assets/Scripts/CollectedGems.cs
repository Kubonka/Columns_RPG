using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollectedGems
{
    public int attackGem;
    public int defendGem;
    public int fireGem;
    public int airGem;
    public int earthGem;
    public int waterGem;
    public CollectedGems(int a, int b, int c, int d, int e, int f)
    {
        attackGem = a;
        defendGem = b;
        fireGem = c;
        airGem = d;
        earthGem = e;
        waterGem = f;
    }

    public void Add(GameObject go)
    {
        if (go.tag != "DamageGem")
            switch (go.tag)
            {
                case "AttackGem":
                    attackGem++;
                    break;

                case "DefendGem":
                    defendGem++;
                    break;

                case "FireGem":
                    fireGem++;
                    break;

                case "AirGem":
                    airGem++;
                    break;

                case "EarthGem":
                    earthGem++;
                    break;
                case "WaterGem":
                    waterGem++;
                    break;
            }
    }
    public void Remove(CollectedGems gemCost)
    {
        attackGem -= gemCost.attackGem;
        defendGem -= gemCost.defendGem;
        fireGem -= gemCost.fireGem;
        airGem -= gemCost.airGem;
        earthGem -= gemCost.earthGem;
        waterGem -= gemCost.waterGem;          
    }

    /*public void Remove(CollectedGems cg)
    {        
        for (int i = 0; i < cg.attackGem; i++)
        {
            attackGem--;
        }
        for (int i = 0; i < cg.defendGem; i++)
        {
            defendGem--;
        }
        for (int i = 0; i < cg.fireGem; i++)
        {
            fireGem--;
        }
        for (int i = 0; i < cg.airGem; i++)
        {
            airGem--;
        }
        for (int i = 0; i < cg.earthGem; i++)
        {
            earthGem--;
        }
        for (int i = 0; i < cg.waterGem; i++)
        {
            waterGem--;
        }
    }*/
    /*public void Add(List<string> tags)
    {
        foreach (string tag in tags)
        switch (tag)
        {
            case "AttackGem":
                attackGem++;
            break;

            case "DefendGem":
                defendGem++;
                break;

            case "FireGem":
                fireGem++;
                break;

            case "AirGem":
                airGem++;
                break;

            case "EarthGem":
                earthGem++;
                break;
            case "WaterGem":
                waterGem++;
                break;
        }
    }*/
    public void Remove(List<CollectedGems> list)
    {
        
    }

    /*public void Remove(List<string> tags)
    {
        foreach (string tag in tags)
        {
            switch (tag)
            {
                case "AttackGem":
                    attackGem--;
                    break;

                case "DefenseGem":
                    defendGem--;
                    break;

                case "FireGem":
                    fireGem--;
                    break;

                case "AirGem":
                    airGem--;
                    break;

                case "EarthGem":
                    earthGem--;
                    break;
                case "WatergemGem":
                    waterGem--;
                    break;
            }
        }
    }*/
}
