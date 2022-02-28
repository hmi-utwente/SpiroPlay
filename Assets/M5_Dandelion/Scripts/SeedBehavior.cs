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
    /// SeedBehaviour class:
    /// Controls all the seeds in the scene
    /// </summary>
    
    public class SeedBehavior : MonoBehaviour
    {
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

        [HideInInspector] public float threshold = 0f;
        [HideInInspector] public float seedOpacity = 0f;
        [HideInInspector] public float flowerOpacity = 0f;
        [HideInInspector] public bool change;
        [HideInInspector] public bool flown;

        private void Start()
        {
            flowerOpacity = 1f;
            manager.seeds.Add(gameObject);
            
            var col = GetComponent<Image>().color;
            col.a = 0f;
            GetComponent<Image>().color = col;
            
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

        public void Fly()
        {
            rb.AddForce(speed * hor, speed * ver, 0f);
            cj.angularZMotion = ConfigurableJointMotion.Free;
        }
    }
}
