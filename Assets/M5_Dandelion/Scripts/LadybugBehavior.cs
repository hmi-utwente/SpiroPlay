using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M5_Paardenbloem
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// LadybugBehaviour class:
    /// Controls the movement of the ladybug
    /// </summary>

    public class LadybugBehavior : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649
        
        [HideInInspector] public bool reachedFlow;

        private Animator animator;
        private SpriteRenderer sprite;
        [SerializeField] private Sprite[] ladybug; 
        
        private bool l;

        #pragma warning restore 649
        #endregion
        
        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.sprite = ladybug[0];
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (reachedFlow && l == false) 
            {
                sprite.sprite = ladybug[1];
                animator.Play("Fly");
                l = true;
            }
        }

    }

}
