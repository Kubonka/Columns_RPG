using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.UIElements;

public class Gem : MonoBehaviour
{
    public int fallCount = 0;
    private float fallSpeed = 8f;
    public bool isFalling = false;
    private Vector3 moveAmount;
    private float target;
    private GameObject boardRef;
    private Animator animator;    

    void Start()
    {
        boardRef = GameObject.Find("Board");
        moveAmount = new Vector3(0, -1, 0) * fallSpeed;
        animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isFalling)
        {
            if (fallCount != 0)
            {
                target = transform.position.y - fallCount;
                fallCount = 0;
            }
            if (transform.position.y > target)
            {
                transform.position += moveAmount * Time.deltaTime;
            }
            else
            {
                isFalling = false;
                transform.position = new Vector3(transform.position.x, target, 0);
                boardRef.GetComponent<BoardManager>().fallingGemsCount -= 1;
            }
        }        
    }    
}
