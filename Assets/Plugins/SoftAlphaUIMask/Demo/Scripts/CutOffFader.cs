using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RandomchaosLtd.SAUIM;

[AddComponentMenu("Scripts/SoftAlphaUIMask/UI/Demo/CutOffFader")]
public class CutOffFader : MonoBehaviour
{
    SoftMaskScript sms;
    public float FadeSpeed = .1f;

    public bool HardBlend = true;

    public bool InTransistion = false;

    GraphicIgnoreRaycast gir;

	// Use this for initialization
	void Start ()
    {
        sms = GetComponent<SoftMaskScript>();
        
        gir = GetComponent<GraphicIgnoreRaycast>();

        FadeOut();
    }

    void Update()
    {
        if (!InTransistion && Input.GetKeyUp(KeyCode.F1))
            FadeOut();

        if (!InTransistion && Input.GetKeyUp(KeyCode.F2))
            FadeIn();

        gir.AcceptMouse = sms.CutOff == 0;

        sms.HardBlend = HardBlend;
    }

    public void FadeOut()
    {
        StartCoroutine("fadeOut");
    }

    public void FadeIn()
    {
        StartCoroutine("fadeIn");
    }

    IEnumerator fadeOut()
    {
        InTransistion = true;

        while (sms.CutOff < 1)
        {
            sms.CutOff += FadeSpeed;
            yield return new WaitForEndOfFrame();
        }

        if (sms.CutOff > 1)
            sms.CutOff = 1;

        InTransistion = false;
    }

    IEnumerator fadeIn()
    {
        InTransistion = true;

        while (sms.CutOff > 0)
        {
            sms.CutOff -= FadeSpeed;
            yield return new WaitForEndOfFrame();
        }

        if (sms.CutOff < 0)
            sms.CutOff = 0;

        InTransistion = false;
    }
}
