using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RandomchaosLtd.SAUIM;

[AddComponentMenu("Scripts/SoftAlphaUIMask/UI/Demo/CutOffSlider")]
public class CutOffSlider : MonoBehaviour
{
    [Tooltip("The SoftMaskScript that you want to alter the CutOff value for")]
    public SoftMaskScript SoftMask;

    Scrollbar sb;

    // Use this for initialization
    void Start()
    {
        sb = GetComponent<Scrollbar>();
        sb.onValueChanged.AddListener(delegate { SoftMask.CutOff = sb.value; });
    }

}
