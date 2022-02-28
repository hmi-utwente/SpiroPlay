using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using M8_Discobowlen;

namespace M8_Discobowlen
{

    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// Discoball_Behavior class:
    /// Controls the discoball
    /// </summary>
    
    public class Discoball_Behavior : MonoBehaviour
    {
        
        #region variables
        #pragma warning disable 649
        
        public bool party;
        [SerializeField] private float flashSpeed = 10f;
        private int state;
        private int timer;
        
        private SpriteRenderer sprite;
        private AudioSource disco;
        [SerializeField] private SpriteRenderer sprite2;
        [SerializeField] private Sprite[] balls;
        [SerializeField] private Sprite[] lights;
        
        #endregion
        #pragma warning restore 649
        
        void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            disco = GetComponent<AudioSource>();
            sprite.enabled = false;
        }
        
        private void Update()
        {
            if (party)
            {
                Disco();
            }
        }

        private void Disco()
        {
            sprite.enabled = true;
            disco.enabled = true;
            
            if (timer < flashSpeed)
            {
                state = 0;
            }
            else if(timer < 2 * flashSpeed)
            {
                state = 1;
            }
            else if (timer < 3 * flashSpeed)
            {
                state = 2;
            }
            else if (timer >= 3 * flashSpeed)
            {
                timer = 0;
            }

            timer += 1;
            sprite.sprite = balls[state];
            sprite2.sprite = lights[state];
        }
    }
}
