using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8_Discobowlen
{

    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// Camera_Behavior class:
    /// Controls the camera
    /// </summary>

    public class Camera_Behavior : MonoBehaviour
    {
        #pragma warning disable 649
        
        private Vector3 velocity;

        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private float cameraSpeed;
        
        #pragma warning restore 649
        private void Start()
        {
            transform.position = startPosition;
        }

        public void Move()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSpeed);
        }

        public void End()
        {
            transform.position = endPosition;
        }
    }
}