using System.Collections;
using System.Collections.Generic;
using M8_Discobowlen;
using Spirometry.SpiroController;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;


namespace M8_Discobowlen
{

    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// M8_Manager class:
    /// Extends Spiro_Manager (look there for more info) and looks for events to manage this specific metaphor
    /// </summary>

    public class M8_Manager : SpiroManager
    {

        #region variables
        #pragma warning disable 649
        
        [Header("M8: Bowling")]

        [SerializeField] private Discoball_Behavior discoball;
        [SerializeField] private Bowlingball_Behavior bowlingball;
        [SerializeField] private Camera_Behavior mainCamera;
        [SerializeField] private Text score;
        [SerializeField] private Image scoreBackground;
        [SerializeField] private Animator cutscenes;
        
        [SerializeField] private float endScreenDelay;
        
        private bool l = true;
        private string count;
        private float scale;

        #pragma warning restore 649
        #endregion

        private new void Start()
        {
            score.enabled = false;
            scoreBackground.enabled = false;
            base.Start();
        }
        
        private new void Update()
        {
            base.Update();

            if (!(bowlingball.position.x > endScreenDelay) || !l) return;
            mainCamera.End();
            bowlingball.Switch();
            score.enabled = false;
            scoreBackground.enabled = false;
            // Play correct end cutscene
            cutscenes.Play("Cutscene" + count);
            l = false;

        }
        
        private void FixedUpdate()
        {
            switch (gameState)
            {
                case State.Inhaling:
                    if (inspirationProgressLong < 100f)
                    {
                        bowlingball.drawProgress = inspirationProgressLong / 100f;
                    }
                    break;
                case State.Exhaling:
                    bowlingball.thrown = true;
                    score.enabled = true;
                    scoreBackground.enabled = true;
                    mainCamera.Move();
                    CalculateScore(expirationProgressLong);
                    break;
                case State.Done:
                    bowlingball.EndLoop();
                    break;
                case State.Prep:
                    break;
            }
        }
        
        private void CalculateScore(float progress)
        {
            string bonus = "";
            
            if (progress >= 110f)
            {
                float extra = progress - 100f;
                int round = (int) (extra / 10f);
                bonus = new string('+', round);
            }
            else if (progress >= 100f) {count = "10";}
            else if (progress >= 90f) {count = "09";}
            else if (progress >= 80f) {count = "08";}
            else if (progress >= 70f) {count = "07";}
            else if (progress >= 60f) {count = "06";}
            else if (progress >= 50f) {count = "05";}
            else if (progress >= 40f) {count = "04";}
            else if (progress >= 30f) {count = "03";}
            else if (progress >= 20f) {count = "02";}
            else if (progress >= 10f) {count = "01";}
            else {count = "00";}

            score.text = count + bonus;
        }
        
        protected override void OnStartTest()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnEndTest()
        {
            bowlingball.End();
        }
        
        protected override void OnSwitchToExpiration()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnReachedProficientFlow()
        {
            discoball.party = true;
        }

    }
}

