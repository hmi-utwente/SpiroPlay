using System.Collections;
using System.Collections.Generic;
using M3_Ballonnen;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;

namespace M5_Paardenbloem
{

    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// M5_Manager class:
    /// Extends Spiro_Manager (look there for more info) and looks for events to manage this specific metaphor
    /// </summary>

    public class M5_Manager : SpiroManager
    {

        #region variables
        #pragma warning disable 649

        [Header("M5: Paardenbloem")]
        public List<GameObject> seeds;
        public GameObject ladybug;
        [SerializeField] private float fadeSpeed;

        private float lastThreshold;
        private float interval;
        private bool hasThreshold;

        #pragma warning restore 649
        #endregion

        private void Awake()
        {
            seeds = new List<GameObject>();
        }

        private new void Update()
        {
            base.Update();
            
            // Setting the threshold for all seeds
            int length = seeds.Count;

            if (length > 0f && hasThreshold == false)
            {
                interval = 100f / length; 
                
                foreach (var seed in seeds)
                {
                    var obj = seed.gameObject.GetComponent<SeedBehavior>();
                    obj.threshold = lastThreshold + interval;
                    lastThreshold = obj.threshold;
                }
                hasThreshold = true;
            }
            else
            {
                foreach (var seed in seeds)
                {
                    var obj = seed.gameObject.GetComponent<SeedBehavior>();
                    Image img = seed.gameObject.GetComponent<Image>();
                    ChangeOpacity(img, obj.seedOpacity);

                    Image [] images;
                    images = seed.GetComponentsInChildren<Image>();

                    foreach (Image flowerImg in images)
                    {
                        if (flowerImg != img)
                        {
                            ChangeOpacity(flowerImg, obj.flowerOpacity);
                        }
                    }
                }
            }
            
            if (gameState == State.Inhaling)
            {
                foreach (var seed in seeds)
                {
                    var obj = seed.gameObject.GetComponent<SeedBehavior>();
                    if (obj.change == false && inspirationProgressLong >= obj.threshold)
                    {
                        obj.seedOpacity = 1f;
                        obj.flowerOpacity = 0f;
                        obj.change = true;
                    }
                }
            }

            // Making the seeds fly
            if (gameState != State.Exhaling) {return;}
            
            foreach (var seed in seeds)
            {
                var obj = seed.gameObject.GetComponent<SeedBehavior>();
                if (obj.flown == false && expirationProgressLong >= obj.threshold)
                {
                    obj.Fly();
                    obj.flown = true;
                }
            }
        }
        
        private void FixedUpdate()
        {
            switch (gameState)
            {
                case State.Inhaling:
                    break;
                case State.Exhaling:
                    break;
                case State.Done:
                    break;
                case State.Prep:
                    break;
            }
        }

        private void ChangeOpacity(Image image, float targetOpacity)
        {
            var currentColor = image.color;
            var opacityDifference = Mathf.Abs(currentColor.a - targetOpacity);

            if (opacityDifference > 0f)
            {
                currentColor.a = Mathf.Lerp(currentColor.a, targetOpacity, fadeSpeed * Time.deltaTime);
                image.color = currentColor;
            }
        }
        
        protected override void OnStartTest()
        {
        }

        protected override void OnEndTest()
        {
        }
        
        protected override void OnSwitchToExpiration()
        {
        }

        protected override void OnReachedProficientFlow()
        {
            ladybug.GetComponent<LadybugBehavior>().reachedFlow = true;
        }

    }
}

