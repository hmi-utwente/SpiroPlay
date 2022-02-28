using System;
using System.Collections;
using UnityEngine;
using Spirometry.Statics;
using Spirometry.SpiroController;
// ReSharper disable All

namespace M3_Ballonnen
{

    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// ArrowBehavior class:
    /// The mechanics of the arrow object in M3
    /// </summary>

    public class ArrowBehavior : MonoBehaviour
    {

        #pragma warning disable 649

        private float animationProgress;
        public Animator anim;
        
        [Header("References")]
        public ParticleSystem fire;
        public ParticleSystem smoke;
        public Animator bowAnimation;
        public AudioSource source;
        [SerializeField] private AudioClip drawAudio;
        [SerializeField] private AudioClip shootAudio;
        
        [HideInInspector] public float arrowProgress;
        [HideInInspector] public float flyProgress;
        [HideInInspector] public bool stopped;

        [SerializeField] private float accel;
        [SerializeField] private float rotation;
        private float speed;

        private static readonly int Release = Animator.StringToHash("Release");

        #pragma warning restore 649

        private void Update()
        {
            animationProgress = arrowProgress / 100f;
            if (animationProgress > 1f)
            {
                animationProgress = 1f;
            }

            if (stopped && transform.position.y > 50f)
            {
                ArrowControl();
            }
        }
        
        public void Drawback()
        {
            bowAnimation.SetFloat("Drawback", animationProgress);
            anim.SetFloat("Drawback", animationProgress);
        }
        
        public void ShootArrow()
        {
            float bowStart = (1 - animationProgress) / 2f;
            float arrowStart = (1 - animationProgress) / 30f;
            
            bowAnimation.Play("Bow_Release", 0, bowStart);
            anim.Play("Arrow_Shoot", 0, arrowStart);
            
            //play audio
            source.Stop();
            source.clip = shootAudio;
            source.Play();
        }

        public void ArrowControl()
        {
            source.Stop();
            Vector3 pos = transform.position;
            speed += accel;
            pos.y -= speed;
            transform.position = pos;
            
            var rot = transform.rotation;
            rot.z -= rotation;
            transform.rotation = rot;
        }

        public void DrawSound()
        {
            source.clip = drawAudio;
            source.Play();
        }
        
        public void ReachedMaxFlow()
        {
            smoke.Play();
            fire.Play();
        }

        public IEnumerator StopSound()
        {
            //gradually decrease volume over time
            while (source.volume > 0)
            {
                source.volume -= 0.1f;
                yield return new WaitForSeconds(0.2f);
            }
            
            //stop source playback when volume passes zero
            source.Stop();
            yield return null;
        }
    }
}
    