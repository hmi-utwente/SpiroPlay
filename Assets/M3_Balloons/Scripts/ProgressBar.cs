using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spiro-Play project by University of Twente and MST
/// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
/// 
/// Inhalation class:
/// The mechanics of the progress bar object in M3
/// </summary>

public class ProgressBar : MonoBehaviour
{
    private Image ProgressImage;
    private float FixedProgress;
    private float ReverseProgress;
    [HideInInspector] public float progress;

    private void Start()
    {
        ProgressImage = transform.Find("Progress").GetComponent<Image>();
        ProgressImage.fillAmount = 0;
        progress = 0;
        FixedProgress = 0;
        ReverseProgress = 0;
        ProgressImage.color = new Color(1,0,0,1);
    }

    public void UpdateBar()
    {
        FixedProgress = progress / 100;
        ReverseProgress = 1 - FixedProgress;
        ProgressImage.fillAmount = FixedProgress;
        ProgressImage.color = new Color(ReverseProgress, FixedProgress, 0, 1);
    }
}
