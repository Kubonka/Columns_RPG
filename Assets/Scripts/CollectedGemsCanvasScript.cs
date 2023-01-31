using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedGemsCanvasScript : MonoBehaviour
{
    private BoardManager boardRef;
    private void Start()
    {
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
    }
    void Update()
    {
        StartCoroutine(DrawCollectedGems());
    }

    private IEnumerator DrawCollectedGems()
    {
        yield return new WaitForSeconds(0.4f);
    }
}
