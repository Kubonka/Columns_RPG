using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    private EnemyManager enemyRef;
    public Text enemyHealthText;
    private void Start()
    {
        enemyRef = GameObject.Find("Enemy").GetComponent<EnemyManager>();        
    }
    void Update()
    {
        if (enemyRef.enabled == true)
            enemyHealthText.text = enemyRef.currentHealth.ToString();
    }
}
