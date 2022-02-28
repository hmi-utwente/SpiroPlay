using System;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;

namespace M9_Soccer
{
    public class M9_Manager : SpiroManager
    {

        #pragma warning disable 649
        
        [Header("Metaphor References")]
        [SerializeField] private Animator soccerPlayer = null;
        [SerializeField] private AudienceBehaviour audience = null;
        [SerializeField] private Transform ball = null;
        [SerializeField] private Transform startPos = null;
        [SerializeField] private Transform endPos = null;
        [SerializeField] private ParticleSystem ballFlames = null;
        [SerializeField] private Animator inspirationMeter;
        [SerializeField] private TMPro.TextMeshProUGUI goalUi;

        [Header("Metaphor Config")]
        [SerializeField] private float ballTurnSpeedModifier = 5f;
        
        //private variables
        private static readonly int FadeRelease = Animator.StringToHash("Release");
        private static readonly int FadeDrawProgress = Animator.StringToHash("DrawProgress");
        private float _frac = 0f;
        private Vector3 _originalScale;
        private static readonly int FadeReleaseProgress = Animator.StringToHash("ReleaseProgress");
        private bool goalMade = false;
        
#pragma warning restore 649
        
        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
            
            //disable goal ui for appear animation
            var localScale = goalUi.transform.localScale;
            _originalScale = new Vector3(localScale.x, localScale.y, localScale.z);
            goalUi.transform.localScale = Vector3.zero;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            base.Update();
            switch (gameState)
            {
                case State.Prep:
                    break;
                case State.Inhaling:
                    soccerPlayer.SetFloat(FadeDrawProgress, inspirationProgressLong * 0.01f);
                    inspirationMeter.SetFloat(FadeDrawProgress, inspirationProgressLong.Remap(0, 100, 0, 1));
                    break;
                case State.Exhaling:
                    //only move ball if the soccer player has finished the kicking animation
                    var animationProgress = expirationProgressLong.Remap(0, 40, _legAnimationSwitchPoint, 1);
                    soccerPlayer.SetFloat(FadeReleaseProgress, animationProgress);

                    if (expirationProgressLong >= 100)
                    {
                        //popup feedback message for end test
                        goalUi.text = "Goal!";
                        ballFlames.Stop();
                        LeanTween.scale(goalUi.gameObject, _originalScale, 0.4f);
                        goalMade = true;
                    }
                    break;
                case State.Done:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //move the ball along the field
            if (expirationProgressLong < 20) return;
            var progress = expirationProgressLong < 107 ? expirationProgressLong : 107;
            var newPosX = progress.Remap(20, 100, startPos.position.x, endPos.position.x);
            var newPosition = new Vector3(newPosX, ball.position.y, ball.position.z);
            ball.position = Vector3.Lerp(ball.position, newPosition, Time.fixedDeltaTime * _frac);
            if (_frac < 1f) _frac += 4f;
            
            //rotate ball relative to flow
            var turnSpeed = expirationProgressHard.Remap(0, 100, 0, 1);
            ball.Rotate(new Vector3(0, 0, -turnSpeed * ballTurnSpeedModifier));
        }

        protected override void OnStartTest()
        {
        }

        protected override void OnEndTest()
        {
            if (goalMade) return;
            //popup feedback message for end test
            goalUi.text = "Bijna!";
            LeanTween.scale(goalUi.gameObject, _originalScale, 0.4f);
        }

        private float _legAnimationSwitchPoint = 0f;
        protected override void OnSwitchToExpiration()
        {
            //make sure the position of the leg is preserved when switching from draw to release animation
            var inspirationReached = 1- (inspirationProgressLong / 100);
            _legAnimationSwitchPoint = inspirationReached.Remap(0, 0.6f, 0, 0.4f);
            soccerPlayer.SetTrigger(FadeRelease);
        }

        protected override void OnReachedProficientFlow()
        {
            ballFlames.Play();
            audience.IsCheering = true;
        }
    }
}
