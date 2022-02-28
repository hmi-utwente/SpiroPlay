using System;
using System.Collections;
using System.Collections.Generic;
using M3_Ballonnen;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spiro-Play project by University of Twente and MST
/// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
/// 
/// BalloonBehavior class:
/// The mechanics of the balloon objects in M3
/// </summary>

public class BalloonBehavior : MonoBehaviour
{

    #region variables
    #pragma warning disable 649
    
    public Image[] BalloonExplosion;
    public AudioSource pop;
    private int currentState = 0;
    private int delay;
    private bool hit;
    [HideInInspector] public bool popped;
    [SerializeField] private ArrowBehavior arrow;
    [SerializeField] private float interval = 5f;
    [SerializeField] private float offset = 180f;

    #pragma warning restore 649
    #endregion
    
    private void Start()
    {
        SetState(0);
        delay = 0;
        hit = false;
        popped = false;
    }

    private void FixedUpdate()
    {

        if (arrow.transform.position.x > transform.position.x - offset)
        {
            if (arrow.flyProgress > interval)
            {
                if (popped == false)
                {
                    hit = true;
                }
            }
            else
            {
                 arrow.anim.enabled = false;
                 arrow.stopped = true;
            }
        }

        if (currentState == 6 && delay > 11)
        {
            Destroy(gameObject);
        }
        if (currentState == 5 && delay > 9)
        {
            SetState(6);
        }
        if (currentState == 4 && delay > 7)
        {
            SetState(5);
        }
        if (currentState == 3 && delay > 5)
        {
            SetState(4);
        }
        if (currentState == 2 && delay > 3)
        {
            SetState(3);
        }
        if (currentState == 1 && delay > 1)
        {
            SetState(2);
        }
        if (hit)
        {
            delay++;
            
            if (popped == false)
            { 
                SetState(1);
                pop.Play();
                popped = true;
            }
        }
    }
    
    private void SetState(int state){
        
        for (int i = 0; i < BalloonExplosion.Length; i++)
        {
            if (i == state)
            {
                currentState = state;
                BalloonExplosion[i].enabled = true;
            }
            else
                BalloonExplosion[i].enabled = false;
        }
    }
}
