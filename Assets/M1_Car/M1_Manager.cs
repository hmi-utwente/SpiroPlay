using System.Collections;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;

namespace M1_Auto
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// M1_Manager class:
    /// Extends Spiro_Manager (look there for more info) and looks for events to manage this specific metaphor
    /// </summary>

    public class M1_Manager : SpiroManager
    {

        #region variables
#pragma warning disable 649

        [Header("M1: Auto")]
        [SerializeField] private CarBehaviour car;
        [SerializeField] private float SpeedModifier = 0f;
        [SerializeField] private SpeedometerBehaviour Speedometer;
        [SerializeField] private ProgressBar bar;
        [SerializeField] private GameObject finishLine;
        [SerializeField] private float spawnDistanceBeforeCar = 1000;

        private float _reachedPef;

        private bool hasSpawnedFinish = false;

#pragma warning restore 649
        #endregion

        private new void Start()
        {
            base.Start();
            Speedometer.SetSpeed(0);
            car.ExhaustionRate = car.IdleExhaustionRate;
        }

        private void FixedUpdate()
        {
            switch (gameState)
            {
                case State.Inhaling:
                    car.ExhaustionRate += 0.05f;
                    Speedometer.SetSpeed(inspirationProgressLong);
                    break;
                case State.Exhaling:
                    if (expirationProgressHard > _reachedPef)
                        _reachedPef = expirationProgressHard;
                    bar.progress = expirationProgressLong;
                    bar.UpdateBar();
                    SpawnFinishLine();
                    break;
                case State.Done:
                    car.ExhaustionRate = car.DrivingExhaustionRate;
                    break;
                case State.Prep:
                    break;
                default:
                    car.ExhaustionRate = 0;
                    break;
            }
        }

        private new void Update()
        {
            base.Update();
            if (gameState == State.Exhaling)
                car.acceleration = expirationProgressHard * SpeedModifier;
        }

        private void SpawnFinishLine()
        {
            if (!hasSpawnedFinish)
            {
                if (expirationProgressLong >= 100.0)
                {
                    hasSpawnedFinish = true;
                    float xPos = car.transform.position.x + spawnDistanceBeforeCar;
                    Instantiate(finishLine, new Vector3(xPos, finishLine.transform.position.y, finishLine.transform.position.z), finishLine.transform.rotation);

                }
            }
        }

        #region event overrides

        protected override void OnStartTest()
        {
            car.RevAudio.Play();
        }

        protected override void OnEndTest()
        {
            StartCoroutine(car.AudioFadeOut());
            car.ExhaustionRate = car.IdleExhaustionRate;
            car.transformationLocked = true;
            car.acceleration = -1;
            car.ActivateBoost(false);
        }

        private IEnumerator HandleCarTypes()
        {
            //after peak flow, determine maximum reached flow and chage to according car over time
            const float delayBetweenStages = 0.5f;
            yield return new WaitForSeconds(1f);
            if (_reachedPef > 40)
                car.SetCarStage(1);
            yield return new WaitForSeconds(delayBetweenStages);
            if (_reachedPef > 65)
                car.SetCarStage(2);
            yield return new WaitForSeconds(delayBetweenStages);
            if (_reachedPef > 85)
                car.SetCarStage(3);
            yield return new WaitForSeconds(delayBetweenStages);
            if (_reachedPef > 100)
                car.SetCarStage(4);
            yield return null;
        }

        protected override void OnSwitchToExpiration()
        {
            Speedometer.gameObject.SetActive(false);
            car.RevAudio.Stop();
            car.StartAudio();
            StartCoroutine(HandleCarTypes());
        }

        protected override void OnReachedProficientFlow()
        {
            car.ActivateBoost(true);
        }
        #endregion
    }
}
