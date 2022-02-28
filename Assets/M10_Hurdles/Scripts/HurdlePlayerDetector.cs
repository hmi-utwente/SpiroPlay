using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdlePlayerDetector : MonoBehaviour
{
    [SerializeField] private M10_Manager manager = null;
    [SerializeField] private Animator anim = null;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            manager.Jump();
            if (!manager.proficientExperiation) {
                anim.SetTrigger("Tumble");
            }
        }
    }

}
