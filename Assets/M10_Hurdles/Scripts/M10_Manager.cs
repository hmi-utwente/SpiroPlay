using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spirometry.SpiroController;
using Spirometry.Statics;
using M9_Soccer;

public class M10_Manager : SpiroManager
{
    #pragma warning disable 649
    
    [Header("Preparation")]
    [SerializeField] private  Animator runnerAnimator = null;
    [SerializeField] private AnimationClip standUpClip = null;
    [SerializeField] private GameObject outlineFigure = null;
    [SerializeField] private Transform player = null;
    [SerializeField] private float lerpFactor = 0;
    [SerializeField] private float jumpAnimationSpeedFactor = 1.0f;
    [SerializeField] private Transform startPoint = null;
    [SerializeField] private Transform endPoint = null;
    [SerializeField] private AudienceBehaviour audience = null;
    [SerializeField] private CameraBehavior cameraController;
    [SerializeField] private Transform finishLine;
    
    public bool proficientExperiation = false;
    private bool canRun = false;

    #pragma warning restore 649
    
    private new void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        switch (gameState)
        {
            case State.Prep:

                break;
            case State.Inhaling:
                outlineFigure.SetActive(true);
                getReady();
                break;
            case State.Exhaling:
                KeepRunning();
                outlineFigure.SetActive(false);
                break;
            case State.Done:
                audience.IsCheering = true;
                break;
        }
    }

    private void getReady()
    {
        runnerAnimator.SetTrigger("getReady");
        if (runnerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= inspirationProgressLong / 100.0f)
        {
            runnerAnimator.speed = 0.0f;
        }
        else
        {
            runnerAnimator.speed = 1.0f;
        }
    }

    public void Jump() {
        runnerAnimator.SetTrigger("shouldJump");
        runnerAnimator.speed = (expirationProgressLong / 100.0f) * jumpAnimationSpeedFactor;
    }

    public void resetAnimationSpeed() {
        runnerAnimator.speed = 1.0f;
    }

    private IEnumerator StartRunning()
    {
        runnerAnimator.speed = 1.0f;
        runnerAnimator.SetTrigger("standUp");
        yield return new WaitForSeconds(standUpClip.length);
        canRun = true;
    }

    private void KeepRunning()
    {
        if (canRun)
        {
            player.position = Vector3.Lerp(player.position, new Vector3(expirationProgressLong.Remap(0, 100, startPoint.position.x, endPoint.position.x), player.position.y, player.position.z), lerpFactor * Time.fixedDeltaTime);
        }
    }

    private new void Update()
    {
        base.Update();
    }
    protected override void OnEndTest()
    {
        runnerAnimator.SetTrigger("doneRunning");
    }

    protected override void OnReachedProficientFlow()
    {
        proficientExperiation = true;
    }

    protected override void OnStartTest()
    {

    }

    protected override void OnSwitchToExpiration()
    {
        cameraController.targets.Add(finishLine);
        StartCoroutine(StartRunning());
    }
}
