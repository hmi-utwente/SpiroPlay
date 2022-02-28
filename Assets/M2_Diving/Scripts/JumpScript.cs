using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScript : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] Schoonspringer playerScript;
    #pragma warning restore 649
    
    void HasPerformedJump() {
        playerScript.SetJumpBool();
    }
}
