using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [Header("Oil & Positioning")]
    [SerializeField] private Transform oilLiquid = null;
    [SerializeField] private float positionScaleFactor = 7.5f;
    [SerializeField] private ParticleSystem oilStream = null;

    [Header("Audio")]
    [SerializeField] private AudioManager manager = null;
    [SerializeField] private AudioClip slurpSound = null;

    private float initPosY;
    private float previousAmount;
    private bool isSlurping = false;
    private void Start()
    {
        if (oilLiquid != null) initPosY = oilLiquid.localPosition.y;
        previousAmount = 100.0f;
    }

    public void DecreaseOilAmount(float totalAmount)
    {
        float remaining = 100.0f - totalAmount;
        float scale = remaining / 100.0f;

        if (oilLiquid == null) return;
        if (remaining != previousAmount)
        {
            if (remaining > 0.0f)
            {
                oilLiquid.localScale = new Vector3(oilLiquid.localScale.x, scale, oilLiquid.localScale.z);
            }
            else
            {
                oilLiquid.localScale = new Vector3(oilLiquid.localScale.x, 0.01f, oilLiquid.localScale.z);
            }

            oilLiquid.localPosition = new Vector3(oilLiquid.localPosition.x, initPosY - (positionScaleFactor * (1 - scale)), oilLiquid.localPosition.z);
            emitOil(Mathf.CeilToInt(remaining));
            previousAmount = remaining;
        }
    }

    public void PlaySlurpSound()
    {
        if (!isSlurping)
        {
            manager.PlaySoundLoop(slurpSound);
            isSlurping = true;
        }
    }

    public void StopSlurpSound()
    {
        if (isSlurping)
        {
            manager.StopSound();
            isSlurping = false;
        }
    }

    public void emitOil(int amount)
    {
        int amountToEmit = Mathf.CeilToInt(previousAmount - amount);
        if (amountToEmit > 0)
        {
            oilStream.Emit(new ParticleSystem.EmitParams(), amountToEmit);
        }
    }
}
