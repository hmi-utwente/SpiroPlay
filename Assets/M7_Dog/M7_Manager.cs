using System;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace M7_Hond
{
    public class M7_Manager : SpiroManager
    {
        
        #region variables
        #pragma warning disable 649

        [Header("References")]
        [SerializeField] private ShampooBottleBehaviour shampoo;
        [SerializeField] private DogBehaviour dog;
        [SerializeField] private GameObject[] activatedOnPeak;
        [SerializeField] private Animator furAnimator;
        [SerializeField] private ProgressBar expirationBar;
        private static readonly int FadeTime = Animator.StringToHash("Time");
        private static readonly int FadeStartGrow = Animator.StringToHash("StartGrow");

        [Header("V2 Animations")]
        [SerializeField] private UnityEvent onSwitchToExpiration;
        [SerializeField] private Animator bubblesAnimator;
        [SerializeField] private Animator furryDogAnimator;
        private static readonly int Progress = Animator.StringToHash("progress");
        private float BubbleProgress
        {
            get => bubblesAnimator.GetFloat(Progress);
            set => bubblesAnimator.SetFloat(Progress, value);
        }
        private float _bubbleProgressOnSwitch;

        #pragma warning restore 649
        #endregion
        
        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
            shampoo.Bottle.enabled = false;
            foreach (var go in activatedOnPeak)
            {
                go.SetActive(false);
            }
            expirationBar.gameObject.SetActive(false);
        }

        // Update is called once per frame
        private new void Update()
        {
            base.Update();
            switch (gameState)
            {
                case State.Prep:
                    break;
                case State.Inhaling:
                    shampoo.SetProgress(inspirationProgressLong);
                    if (dog != null) dog.SetFadeProgress(inspirationProgressLong);

                    //trigger v2 bubble animations
                    if (bubblesAnimator != null)
                        BubbleProgress = inspirationProgressLong.Remap(0f, 100f, 0f, 0.5f);
                    
                    break;
                case State.Exhaling:
                    //trigger fur animation on dog
                    if (expirationProgressLong > 100) return;
                    
                    //refresh the progressbar
                    if (expirationBar != null)
                    {
                        expirationBar.progress = expirationProgressLong;
                        expirationBar.UpdateBar();
                    }

                    //trigger v2 bubble animations
                    if (bubblesAnimator != null)
                    {
                        BubbleProgress = expirationProgressLong.Remap(0f, 50f, _bubbleProgressOnSwitch, 0f);
                        if (expirationProgressLong >= 50) bubblesAnimator.SetTrigger("stop");
                        
                    }

                    if (furryDogAnimator != null)
                    {
                        var progress = expirationProgressLong.Remap(0f, 100f, 0f, 1f);
                        furryDogAnimator.SetFloat("progress", progress);
                    }

                    if (furAnimator != null)
                    {
                        furAnimator.SetTrigger(FadeStartGrow);
                        var time = expirationProgressLong.Remap(0f, 100f, 0, 1);
                        furAnimator.SetFloat(FadeTime, time);
                    }

                    break;
                case State.Done:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnStartTest()
        {
            shampoo.Bottle.enabled = true;
            shampoo.shampooBubbles.Play();
            
            if (bubblesAnimator != null) bubblesAnimator.SetTrigger("start");
        }

        protected override void OnEndTest()
        {
            
        }

        protected override void OnSwitchToExpiration()
        {
            shampoo.Bottle.enabled = false;
            shampoo.shampooBubbles.Stop();
            expirationBar.gameObject.SetActive(true);
            
            //trigger functionality and animations for the updated graphics
            _bubbleProgressOnSwitch = BubbleProgress;
            onSwitchToExpiration.Invoke();
        }

        protected override void OnReachedProficientFlow()
        {
            foreach (var go in activatedOnPeak)
            {
                go.SetActive(true);
            }
        }
    }
}
