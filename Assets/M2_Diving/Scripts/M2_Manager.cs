using System;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using System.Collections;
using M9_Soccer;

namespace M2_Schoonspringen
{
    /*
     M3 Metaphore created by Koen Vogel,  k.a.vogel@student.utwente.nl and Sil Tijssen, s.b.tijsen@student.utwente.nl
     SpiroPlay Project
    */
    public class M2_Manager : SpiroManager
    {

        #region variables
#pragma warning disable 649

        [Header("Metaphor specific fields")]
        public DuikplankScript duikplank;
        public Schoonspringer Player = null;
        public AudioSource source = null;
        public AudioClip waterClip = null;
        public AudioClip applauseClip = null;
        public Transform PlayerGFX;
        public GameObject cape;
        public SpriteRenderer backgroundSprite;
        public ParticleSystem waterPs;
        public AudienceBehaviour audience;
        //public WaterScript water;

        [Header("Transforms")]
        public Transform StartOfDuikplank = null;
        public Transform EndOfDuikplank = null;
        public Transform MaxJumpLocation = null;
        public Transform PoolEndLocation = null;
        public Transform AppearPoint = null;
        public Transform cameraTransform = null;
        public Transform LookAtJumpPosition = null;

        [SerializeField] Transform waterFollowTransform = null;
        [SerializeField] Transform playerTouchWaterLocation = null;
        [SerializeField] Transform playerStopLocation = null;

        [Header("Animation Offsets")]
        [SerializeField] Vector3 OffsetJumpAnim = Vector3.zero;
        [SerializeField] Vector3 OffsetWalkAnim = Vector3.zero;
        [SerializeField] Vector3 OffsetTricksAnim = Vector3.zero;

        [Header("Variables")]
        [SerializeField] float slerpFactor = 1.0f;
        [SerializeField] float walkSlerpFactor = 1.0f;
        [SerializeField] float cameraWidth = 300;
        [SerializeField] private bool useCustomWidth = false;

        private Transform playerTransform;
        private Vector3 StartOfJump;
        private Vector3 camOffset;
        private Vector3 previousPlayerLocation;

        private bool hasStartPoint = false;
        private bool hasPlayedDuikplankAnim = false;
        private bool hasReachedEnd = false;
        private bool isWalking = false;
        private bool hasPlayedWaterClip = false;
        private bool _reachedFvc = false;

#pragma warning restore 649
        #endregion

        // Start is called before the first frame update

        private new void Start()
        {
            base.Start();
            playerTransform = Player.transform;
            var position = playerTransform.position;
            StartOfDuikplank.position = new Vector3(position.x, position.y, position.z);
            Player.SetKinematic();
            camOffset = cameraTransform.position - position;

            float camSize = cameraWidth;

            if (!useCustomWidth)
            {
                camSize = backgroundSprite.bounds.size.x * Screen.height / Screen.width * 0.5f;
            }

            if (cameraTransform.GetComponent<Camera>().orthographic)
            {
                cameraTransform.gameObject.GetComponent<Camera>().orthographicSize = camSize;
                if (!useCustomWidth)
                {
                    cameraTransform.position = new Vector3(backgroundSprite.bounds.center.x, cameraTransform.position.y, cameraTransform.position.z);
                }
            }

            waterPs.Stop();
            waterPs.Clear();
            cape.SetActive(false);
        }

        protected new void Update()
        {
            base.Update();

            switch (gameState)
            {
                case State.Inhaling:
                    hasStartPoint = true;
                    StartWalking();
                    break;

                case State.Exhaling:
                    StopWalking();
                    //detect if the progress has reached 100 for realtime metaphor events
                    if (gameState == State.Exhaling && expirationProgressLong >= 100 && !_reachedFvc)
                    {
                        Debug.Log("user has reached a expiration progress of 100");
                        audience.IsCheering = true;
                        _reachedFvc = true;
                    }
                    Player.Jump();


                    //PlayerGFX.localPosition = OffsetJumpAnim;
                    //if (Player.Jumped)
                    //{
                    Jumping();
                    //StartCoroutine(playDuikplankAnim());
                    //}
                    break;

                case State.Prep:
                    break;

                case State.Done:
                    StopWalking();
                    Tricks();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (playerTransform.position.y <= playerTouchWaterLocation.position.y)
            {
                //water.Splash(true);
                if (!hasPlayedWaterClip)
                {
                    hasPlayedWaterClip = true;
                    StartCoroutine(OnDive());
                }
            }
            else
            {
                cameraTransform.position = new Vector3(cameraTransform.position.x, (playerTransform.position + camOffset).y, cameraTransform.position.z);
                waterFollowTransform.position = new Vector3(playerTransform.position.x, waterFollowTransform.position.y, waterFollowTransform.position.z);
            }

            if (playerTransform.position.y <= playerStopLocation.position.y)
            {
                Player.SetKinematic();
                Player.StopMoving();
                hasReachedEnd = true;
            }
        }

        private IEnumerator OnDive()
        {
            source.PlayOneShot(waterClip);
            source.PlayOneShot(applauseClip);
            waterPs.Play();
            yield return new WaitForSeconds(1.0f);
            waterPs.Stop();
            yield return null;
        }

        protected override void OnStartTest()
        {
        }
        protected override void OnEndTest()
        {
            if (cape.activeSelf)
            {
                cape.SetActive(false);
            }
        }
        protected override void OnReachedProficientFlow()
        {
            cape.SetActive(true);
        }
        protected override void OnSwitchToExpiration() { }

        private void Walk()
        {
            playerTransform.position = new Vector3(inspirationProgressLong.Remap(0, 100, StartOfDuikplank.position.x, EndOfDuikplank.transform.position.x), playerTransform.position.y, playerTransform.position.z);

            if (playerTransform.position.x != previousPlayerLocation.x)
            {
                Player.WalkStartAnim();
                PlayerGFX.localPosition = OffsetWalkAnim;
            }
            else
            {
                Player.WalkStopAnim();
                PlayerGFX.localPosition = Vector3.zero;
            }

            previousPlayerLocation = playerTransform.position;
            StartOfJump = playerTransform.position;
        }

        private void StartWalking()
        {
            if (!isWalking)
            {
                isWalking = true;

                Player.WalkStartAnim();
                PlayerGFX.localPosition = OffsetWalkAnim;
            }

            playerTransform.position = Vector3.Slerp(playerTransform.position, new Vector3(inspirationProgressLong.Remap(0, 100, StartOfDuikplank.position.x, EndOfDuikplank.transform.position.x), playerTransform.position.y, playerTransform.position.z), Time.deltaTime * walkSlerpFactor);
            StartOfJump = playerTransform.position;
        }

        private void StopWalking()
        {
            if (isWalking)
            {
                isWalking = false;
                Player.WalkStopAnim();
                PlayerGFX.localPosition = Vector3.zero;
            }
        }

        private void Jumping()
        {
            if (!hasStartPoint)
            {
                StartOfJump = playerTransform.position;
                hasStartPoint = true;
            }
            PlayerGFX.localPosition = OffsetJumpAnim;
            playerTransform.position = Vector3.Slerp(playerTransform.position, new Vector3(expirationProgressLong.Remap(0, 100, StartOfJump.x, MaxJumpLocation.position.x), expirationProgressLong.Remap(0, 100, StartOfJump.y, MaxJumpLocation.position.y), playerTransform.position.z), Time.deltaTime * slerpFactor);
            Player.RotateTowardsPointSlerp(LookAtJumpPosition.position);
        }

        private void Tricks()
        {
            if (!hasReachedEnd)
            {
                //if (Player.Jumped)
                //{
                PlayerGFX.localPosition = OffsetTricksAnim;
                //}
                Player.RotateTowardsPointSlerp(PoolEndLocation.position);
                Player.SetDynamic();
                Player.DoTricks();
            }
            else
            {
                PlayerGFX.localPosition = Vector3.zero;
                Player.SlerpToPoint(AppearPoint);
            }

        }

        private IEnumerator playDuikplankAnim()
        {
            if (!hasPlayedDuikplankAnim)
            {
                hasPlayedDuikplankAnim = true;
                duikplank.PlayAnim(true);
                yield return new WaitForSeconds(0.5f);
                duikplank.PlayAnim(false);
            }
        }
    }
}
