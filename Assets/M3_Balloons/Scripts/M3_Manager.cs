using System;
using System.Collections;
using System.Collections.Generic;
using M3_Ballonnen;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using TMPro;

namespace M3_Ballonnen
{

    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// Based on code by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// M3_Manager class:
    /// Extends Spiro_Manager (look there for more info) and looks for events to manage this specific metaphor
    /// </summary>

    public class M3_Manager : SpiroManager
    {

        #region variables

        #pragma warning disable 649

        [Header("M3: Ballonnen")] 
        
        [SerializeField] private ArrowBehavior arrow;
        [SerializeField] private ProgressBar bar;

        private bool shot;
        private bool drawn;

        #pragma warning restore 649

        #endregion

        private new void Start()
        {
            base.Start();
            shot = false;
            drawn = false;
        }


        private void FixedUpdate()
        {
            switch (gameState)
            {
                case State.Inhaling:
                    bar.UpdateBar();
                    arrow.Drawback();
                    break;
                case State.Exhaling:
                    if (shot == false)
                    {
                        arrow.ShootArrow();
                        shot = true;
                    }
                    break;
                case State.Done:
                    break;
                case State.Prep:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private new void Update()
        {
            base.Update();
            if (gameState == State.Inhaling)
            {
                bar.progress = inspirationProgressLong;
                arrow.arrowProgress = inspirationProgressLong;
                
                if (drawn == false)
                {
                    arrow.DrawSound();
                    drawn = true;
                }
            }

            if (gameState == State.Exhaling)
            {
                arrow.flyProgress = expirationProgressLong;
            }
        }

        #region event overrides

        protected override void OnStartTest()
        {
        }

        protected override void OnEndTest()
        {
            StartCoroutine(arrow.StopSound());
        }

        protected override void OnSwitchToExpiration()
        {
            bar.gameObject.SetActive(false);
        }

        protected override void OnReachedProficientFlow()
        {
            arrow.ReachedMaxFlow();
        }

        #endregion
    }
}
