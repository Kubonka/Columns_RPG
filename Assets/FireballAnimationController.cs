using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class FireballAnimationController : MonoBehaviour
{
    public Animator animator;    

    public void Travel()
    {
        animator.SetInteger("State", 2);
    }
}
