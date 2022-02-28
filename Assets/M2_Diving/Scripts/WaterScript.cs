using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] Animator animator;
    #pragma warning restore 649
    
    public void Splash(bool shouldPlay)
    {
        if (shouldPlay)
        {
            if (!animator.GetBool("waterShouldMove"))
            {
                animator.SetBool("waterShouldMove", true);
            }
        }
        else
        {
            if (animator.GetBool("waterShouldMove"))
            {
                animator.SetBool("waterShouldMove", false);
            }
        }
    }
}
