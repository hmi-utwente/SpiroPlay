using System;
using System.Collections;
using System.Collections.Generic;
using M5_Paardenbloem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace M5_Paardenbloem
{
    
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// LeafBehaviour class:
    /// Controls all the extra leafs in the scene
    /// </summary>
    
    public class LeafBehaviour : MonoBehaviour
    {
        #region variables
#pragma warning disable 649
        
        public M5_Manager manager;
        private Rigidbody rb;
        private ConfigurableJoint cj;

        private float angle;
        [SerializeField] private float minAngle = 50f;
        [SerializeField] private float maxAngle = 130f;
        private float speed;
        [SerializeField] private float minSpeed = 400f;
        [SerializeField] private float maxSpeed = 600f;

        private float hor;
        private float ver;

        [SerializeField] private float threshold;
        private bool flown;

#pragma warning restore 649
        #endregion
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            cj = GetComponent<ConfigurableJoint>();
            cj.angularZMotion = ConfigurableJointMotion.Locked;
            cj.targetRotation = transform.rotation;
            
            angle = Random.Range(minAngle, maxAngle);
            speed = Random.Range(minSpeed, maxSpeed);

            if (angle < 90f)
            {
                ver = angle / 90f;
                hor = -1f +  ver;
            }
            else if (angle > 90f)
            {
                hor = (angle - 90f) / 90f;
                ver = 1f - hor;
            }
            else
            {
                ver = 1f;
            }
        }

        private void Update()
        {
            if (flown == false && manager.expirationProgressLong > threshold)
            {
                flown = true;
                Fly();
            }
        }

        private void Fly()
        {
            rb.AddForce(speed * hor, speed * ver, 0f);
            cj.angularZMotion = ConfigurableJointMotion.Free;
        }
    }
}
