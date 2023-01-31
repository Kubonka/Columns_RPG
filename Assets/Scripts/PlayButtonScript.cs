using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonScript: MonoBehaviour
{
    public void Play()
    {

        GameObject.Find("Board").GetComponent<BoardManager>().enabled = true;
        GameObject.Find("Player").GetComponent<PlayerManager>().enabled = true;
        GameObject.Find("Enemy").GetComponent<EnemyManager>().enabled = true;
        GameObject.Find("CollectedGemsCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("MainMenu").gameObject.SetActive(false);
        GameObject.Find("Enemy").AddComponent<Snake>();
        GameObject.Find("Player").AddComponent<Hero>();


    }
}
