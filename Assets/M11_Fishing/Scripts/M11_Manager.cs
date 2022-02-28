using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spirometry.SpiroController;
using UnityEngine.Serialization;


namespace M11_Vissen
{


    public class M11_Manager : SpiroManager
    {
        #region variables
        #pragma warning disable 649
        
        [Header("M11_Fishing")]
        [SerializeField] private NetBehaviour net;
        [SerializeField] private EnvironmentSpawner spawner;
        [FormerlySerializedAs("camera")]
        [SerializeField] private CameraBehavior cameraBehaviour;
        [SerializeField] private AudioManager audioManager;

        [SerializeField] private AudioClip clipWater;
        [SerializeField] private AudioClip splashClip;
        [SerializeField] private string netTag;

        private bool isPlayingWaterSound = false;

        #pragma warning restore 649
        #endregion
        
        private void FixedUpdate()
        {
            switch (gameState)
            {
                case State.Inhaling:
                    net.Inhaling(inspirationProgressLong);
                    cameraBehaviour.Zoom();
                    break;
                case State.Exhaling:
                    cameraBehaviour.Zoom();
                    net.Exhaling();
                    net.SpawnFish(expirationProgressLong);
                    if (!isPlayingWaterSound)
                    {
                        audioManager.PlaySoundLoop(clipWater);
                        isPlayingWaterSound = true;
                    }
                    spawner.SetSpawning(true);
                    break;
                case State.Done:
                    spawner.SetSpawning(false);
                    audioManager.StopSound();
                    net.Finished();
                    break;
                case State.Prep:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(netTag))
            {
                audioManager.PlaySound(splashClip);
            }
        }

        protected override void OnEndTest()
        {

        }

        protected override void OnStartTest()
        {

        }

        protected override void OnSwitchToExpiration()
        {

        }

        protected override void OnReachedProficientFlow()
        {
            net.SpawnTropicalFish();
        }
    }
}