using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGemScript : MonoBehaviour
{
    BoardManager boardRef;
    private void Start()
    {
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
    }
    public void DestroyGem()
    {        
        boardRef.gemsToDestroy--;
    }
}
