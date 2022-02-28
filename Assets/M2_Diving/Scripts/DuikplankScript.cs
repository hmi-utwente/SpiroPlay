using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuikplankScript : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] Animator animator;
    #pragma warning restore 649
    
    public void PlayAnim(bool shouldPlay)
    {
        if (shouldPlay)
        {
            if (!animator.GetBool("needsMoving"))
            {
                animator.SetBool("needsMoving", true);
            }
        }
        else
        {
            if (animator.GetBool("needsMoving"))
            {
                animator.SetBool("needsMoving", false);
            }
        }
    }
}
