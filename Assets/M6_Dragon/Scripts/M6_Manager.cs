using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spirometry.SpiroController;

public class M6_Manager : SpiroManager
{
    #region Variables
    #pragma warning disable 649
    
    [Header("Fire Particles & Objects")]
    [SerializeField] private Flammable[] burningObjects = null;
    //[SerializeField] private ParticleSystem fireBreathNonCombust = null;
    //[SerializeField] private ParticleSystem fireBreathCombust = null;
    //[SerializeField] private GameObject dragonMouth = null;
    [SerializeField] private Animator[] inspirationAnimators;
    [SerializeField] private Animator dragonFireAnim;

    [Header("Oil")]
    [SerializeField] private Oil oilScript = null;

    [Header("Looking Points & Rotation")]
    [SerializeField] private Transform startLookPoint = null;
    [SerializeField] private Transform endLookPoint = null;
    [SerializeField] private Transform dragonHead = null;
    [SerializeField] private float slerpFactor = 1.0f;

    [Header("Audio")]
    [SerializeField] private AudioManager manager = null;
    [SerializeField] private AudioClip fireSound = null;

    private ParticleSystem fireBreath = null;
    private float amountNeededToLight;
    private bool isBreathingFire = false;
    private bool fireSoundPlayed = false;
    private float angleFactor = 0.0f;

    #pragma warning restore 649
    #endregion
    
    private new void Start()
    {
        base.Start();
        amountNeededToLight = 100.0f / burningObjects.Length;

        /*if (fireBreathNonCombust != null)
        {
            fireBreath = fireBreathNonCombust;
            fireBreath.Stop();
            fireBreath.Clear();
        }*/

        if (dragonHead != null) angleFactor = calculateAngleFactor();
    }

    private new void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        switch (gameState)
        {
            case State.Prep:
                //if (dragonMouth != null) dragonMouth.SetActive(false);
                break;
            case State.Inhaling:
                //if (dragonMouth != null) dragonMouth.SetActive(true);
                if (inspirationProgressLong <= 100)
                    oilScript.DecreaseOilAmount(inspirationProgressLong);
                oilScript.PlaySlurpSound();
                
                //if animators are available, activate inspiration triggers
                foreach (var animator in inspirationAnimators)
                {
                    animator.SetFloat("inspirationProgress", inspirationProgressLong / 100);
                }
                break;
            case State.Exhaling:
                LightObjects();
                BreathFire();
                RotateHead();
                break;
            case State.Done:
                //if (dragonMouth != null) dragonMouth.SetActive(false);
                StopBreathingFire();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RotateHead()
    {
        if (dragonHead == null) return;
        dragonHead.rotation = Quaternion.Slerp(dragonHead.rotation, Quaternion.Euler(dragonHead.rotation.x, dragonHead.rotation.y, expirationProgressLong * angleFactor), slerpFactor);
    }

    private void LightObjects()
    {
        for (int i = 0; i < burningObjects.Length; i++)
        {
            if (expirationProgressLong >= (amountNeededToLight * (i + 1)))
            {
                burningObjects[i].Light();
                if (!fireSoundPlayed) {
                    fireSoundPlayed = true;
                    manager.PlaySoundLoop(fireSound);
                }
            }
        }
    }

    private void CompleteCombustion()
    {
        dragonFireAnim.SetTrigger("blue_fire");
        
        foreach (var t in burningObjects)
        {
            t.Combust();
        }

        if (isBreathingFire)
        {
            StopBreathingFire();
        }

        /*if (fireBreath == null || fireBreathCombust == null) return;
        fireBreath = fireBreathCombust;*/
    }

    protected override void OnStartTest()
    {

    }

    protected override void OnEndTest()
    {
    }

    protected override void OnSwitchToExpiration()
    {
        oilScript.StopSlurpSound();
    }

    protected override void OnReachedProficientFlow()
    {
        CompleteCombustion();
    }

    private void BreathFire()
    {
        if (fireBreath == null) return;
        if (!isBreathingFire)
        {
            fireBreath.Play();
            isBreathingFire = true;
        }
    }

    private void StopBreathingFire()
    {
        if (fireBreath != null) fireBreath.Stop();
        isBreathingFire = false;
    }


    private float calculateAngleFactor()
    {
        Vector2 dirStart = dragonHead.transform.position - startLookPoint.transform.position;
        Vector2 dirEnd = dragonHead.transform.position - endLookPoint.transform.position;

        return Vector2.Angle(dirStart.normalized, dirEnd.normalized) / 100.0f;
    }
}
