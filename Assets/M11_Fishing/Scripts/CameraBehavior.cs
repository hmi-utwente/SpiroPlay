using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M11_Vissen
{

    public class CameraBehavior : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649
        
        private Vector3 nextPos;

        [SerializeField] private Transform target;

        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 zoomedOffset;

        [SerializeField] private float speed;
        [SerializeField] private float zoomSpeed;

        #pragma warning restore 649
        #endregion
        
        private void Start()
        {
            transform.position = target.position + offset;
        }

        public void Zoom()
        {
            if (offset != zoomedOffset)
            {
                offset = Vector3.Slerp(offset, zoomedOffset, zoomSpeed);
            }
        }

        private void Update()
        {
            nextPos = Vector3.Slerp(transform.position, target.position + offset, speed);
            transform.position = nextPos;
        }
    }

}

