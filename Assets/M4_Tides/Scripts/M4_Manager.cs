using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;

namespace M4_Ebenvloed
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Based on code by Koen Vogel - k.a.vogel@student.utwente.nl
    /// Created by Sil Tijssen - s.b.tijssen@student.utwente.nl
    /// 
    /// M4_Manager class:
    /// Extends Spiro_Manager (look there for more info) and looks for events to manage this specific metaphor
    /// </summary>

    public class M4_Manager : SpiroManager
    {

        #region variables
        #pragma warning disable 649
        
        [Header("Components")]
        [Header("M4: Eb en vloed")]
        [SerializeField] private SeaScript sea;
        [SerializeField] private AudioSource source;

        [Header("Transforms")]
        [SerializeField] private Transform highInhaleTransform;
        [SerializeField] private Transform highExhaleTransform;
        [SerializeField] private Transform dolfijnRotateObject;
        [SerializeField] private Transform dolfijnMovingPoint;

        [Header("Variables")]
        [SerializeField] private AudioClip treasureFound;
        [SerializeField] private ParticleSystem treasurePs;
        [SerializeField] private float rotateSpeed;

        private Vector3 highInhalePos;
        private Vector3 highExhalePos;
        private Vector3 exhaleStartPoint;

        private float degreesRotated = 0.0f;

        private bool hasReachedPeak = false;
        private bool hasFoundTreasure = false;

#pragma warning restore 649
        #endregion

        private new void Start()
        {
            base.Start();
            highInhalePos = highInhaleTransform.position;
            highExhalePos = highExhaleTransform.position;

            exhaleStartPoint = sea.transform.position;
        }

        private void FixedUpdate()
        {
            switch (gameState)
            {
                case State.Inhaling:
                    sea.SmoothBetween(exhaleStartPoint, highInhalePos, inspirationProgressLong);
                    exhaleStartPoint = sea.transform.position;

                    if (!hasFoundTreasure && inspirationProgressLong >= 100) {
                        source.PlayOneShot(treasureFound);
                        treasurePs.Play();
                        hasFoundTreasure = true;
                    }

                    break;
                case State.Exhaling:
                    sea.SmoothBetween(exhaleStartPoint, highExhalePos, expirationProgressLong);

                    if (expirationProgressLong >= 100)
                    {
                        StartWalking();
                    }
                    break;
                case State.Done:
                    hasStartedWalking = true;
                    break;
                case State.Prep:
                    break;
                default:
                    break;
            }
        }

        private new void Update()
        {
            base.Update();
            if (!hasReachedPeak || !(degreesRotated < 360)) return;
            var toRotate = rotateSpeed * Time.deltaTime;
            dolfijnRotateObject.RotateAround(dolfijnMovingPoint.position, Vector3.forward, toRotate);
            degreesRotated += toRotate;
        }
        #region event overrides

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
            hasReachedPeak = true;
        }
        #endregion
        
        private static readonly int DogRunning = Animator.StringToHash("dog_running");
        public Animator dogAnimator;
        [SerializeField] private Vector3 walkOffset = new Vector3(0.0f, 0.61f, 0.0f);
        public bool hasStartedWalking = false;
        public void StartWalking()
        {
            if (hasStartedWalking) return;
            Debug.Log("full expiration reached, dog running away now...");
            dogAnimator.SetBool(DogRunning, true);
            dogAnimator.transform.localPosition = walkOffset;
            var localScale = dogAnimator.transform.localScale;
            localScale = new Vector3(localScale.x * -1.0f, localScale.y, localScale.z);
            dogAnimator.transform.localScale = localScale;
            Invoke("StopWalking", 4f);
            hasStartedWalking = true;
        }
        
        private void StopWalking()
        {
            dogAnimator.SetBool(DogRunning, false);
            dogAnimator.transform.localPosition = Vector3.zero;
        }
    }
}
