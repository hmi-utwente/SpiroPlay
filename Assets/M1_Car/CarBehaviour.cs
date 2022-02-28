using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace M1_Auto
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// CarBehaviour class:
    /// The mechanics of the main car object in the car metaphor
    /// </summary>

    public class CarBehaviour : MonoBehaviour
    {
        #region variables
        [Header("References")]
        public Image[] CarStages;
        private Transform[] ExhaustPipes;
        public AudioSource RevAudio;
        public AudioSource DrivingAudio;
        private Camera cam;
        private ParticleSystem ExhaustionParticles;
        private ParticleSystem fireParticles;
        private ParticleSystem windParticles;
        private ParticleSystem.EmissionModule windEmission;
        private bool hasStartedDecreasing = false;

        [Header("Settings")]
        public float speedMultiplier = 1f;
        public float cameraSpeedMultiplier = 1f;
        public float AirResistance = 0.1f;
        public float IdleExhaustionRate = 3f;
        public float DrivingExhaustionRate
        {
            get
            {
                if (velocity / 20 < IdleExhaustionRate)
                    return IdleExhaustionRate;
                else
                    return velocity / 20;
            }
        }

        //movement
        [HideInInspector]
        public float acceleration = 0f;
        [HideInInspector]
        public float velocity = 0f;
        [HideInInspector]
        public bool transformationLocked = false;

        //parameters
        private int currentStage = 0;
        public float ExhaustionRate
        {
            get
            {
                var emission = ExhaustionParticles.emission;
                return emission.rateOverTime.constant;
            }
            set
            {
                var emission = ExhaustionParticles.emission;
                emission.rateOverTime = value;
            }
        }

        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            //references
            cam = GetComponentInChildren<Camera>();
            ExhaustionParticles = transform.Find("Exhaust Particles").GetComponentInChildren<ParticleSystem>();
            fireParticles = transform.Find("Fire Particles").GetComponentInChildren<ParticleSystem>();
            windParticles = transform.Find("Wind Particles").GetComponent<ParticleSystem>();
            windEmission = windParticles.emission;

            //setup stages and exhaust locations
            ExhaustPipes = new Transform[CarStages.Length];
            for (int i = 0; i < CarStages.Length; i++)
                if (CarStages[i].gameObject.transform.GetChild(0) != null)
                    ExhaustPipes[i] = CarStages[i].gameObject.transform.GetChild(0).transform;
            SetCarStage(0);
            fireParticles.Stop();
            fireParticles.Clear();
            windParticles.Stop();
            windParticles.Clear();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleCamera();
            HandleAudio();
        }

        private void HandleMovement()
        {
            //manage velocity
            velocity += acceleration;
            if (acceleration <= 0 && velocity > 0)
                acceleration -= AirResistance;

            //bring the car to a complete stop
            if (velocity < 0)
            {
                acceleration = 0;
                velocity = 0;
            }

            //actually move the car
            transform.Translate(new Vector3(velocity * speedMultiplier, 0, 0));
        }

        private void HandleCamera()
        {
            cam.transform.Translate(new Vector3(0, acceleration * cameraSpeedMultiplier * 0.6f, -acceleration * cameraSpeedMultiplier));
        }

        public void ActivateBoost(bool activate)
        {
            if (activate)
            {
                fireParticles.Play();
                windParticles.Play();
            }
            else
            {
                fireParticles.Stop();
                StartCoroutine(StartDecreasingWind());
            }
        }

        private IEnumerator StartDecreasingWind()
        {
            if (hasStartedDecreasing) yield break;
            hasStartedDecreasing = true;
            for (int i = 160; i > 0; i -= 3)
            {
                windEmission.rateOverTime = i;
                yield return new WaitForSeconds(0.01f);
            }
            windParticles.Stop();
        }

        public void SetCarStage(int stage)
        {
            //enable correct frame
            for (int i = 0; i < CarStages.Length; i++)
            {
                if (i == stage)
                {
                    currentStage = stage;
                    CarStages[i].enabled = true;
                }
                else
                    CarStages[i].enabled = false;
            }

            //reposition exhaust cloud
            ExhaustionParticles.transform.position = ExhaustPipes[currentStage].position;
            fireParticles.transform.position = ExhaustPipes[currentStage].position;

            if (currentStage >= 2)
                DrivingAudio.pitch -= (80 * PitchShiftAmount);
        }

        #region audio
        private const float PitchShiftAmount = 0.005f;

        public void StartAudio()
        {
            DrivingAudio.Play();
        }

        void HandleAudio()
        {
            if (DrivingAudio.isPlaying)
                DrivingAudio.pitch += PitchShiftAmount;
        }

        public IEnumerator AudioFadeOut()
        {
            /*
            while (DrivingAudio.volume >= 0)
            {
                DrivingAudio.volume -= 0.01f;
                DrivingAudio.pitch -= (100 * PitchShiftAmount);
                yield return new WaitForSeconds(0.01f);
            }
            DrivingAudio.Stop();
            yield return null;
            */


            DrivingAudio.Stop();
            RevAudio.Stop();
            yield return null;
        }

        #endregion
    }
}
