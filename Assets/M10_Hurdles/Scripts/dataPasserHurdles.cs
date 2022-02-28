using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataPasserHurdles : MonoBehaviour
{
    [SerializeField] private M10_Manager manager = null;

    public void Reset()
    {
        manager.resetAnimationSpeed();
    }

}
