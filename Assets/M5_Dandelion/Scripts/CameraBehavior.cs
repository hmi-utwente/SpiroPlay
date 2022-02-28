using System;
using System.Collections;
using System.Collections.Generic;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Serialization;

namespace M5_Paardenbloem
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// Based on code by Koen Vogel
    /// 
    /// CameraBehaviour class:
    /// Controls the movement of the camera
    /// </summary>
    
    [RequireComponent(typeof(Camera))]
    public class CameraBehavior : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649

        [Header("References")]
        [SerializeField] private SpiroManager manager;
        [SerializeField] private M5_Manager m5Manager;
        [SerializeField] private Transform centerPosition;
        private List<GameObject> seeds;
        private Camera _cam;
        private Vector3 CamPos
        {
            get => transform.position;
            set
            {
                var vector3 = new Vector3(value.x, value.y, value.z);
            }
        }
        private Vector3 _newPosition;
        private float _newZoom;

        [SerializeField] private List<Transform> targets = new List<Transform>();

        //damping
        private Vector3 _velocity;
        private bool hasStopped;

        [Header("Inspiration Settings")]
        public float zoomSpeed;
        public float zLimit;
        
        [Header("Expiration Settings")]
        [SerializeField] private float smoothTime = 0.5f;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float minZoom;
        [SerializeField] private float maxZoom;
        [SerializeField] private float zoomLimiter;
        [SerializeField] private float minVertLimit;
        [SerializeField] private float maxVertLimit;
        //[SerializeField] private float fadeSpeed = 0.01f;

        #pragma warning restore 649
        #endregion

        private void Start()
        {
            seeds = m5Manager.seeds;
            
            _cam = GetComponent<Camera>();
            targets.Add(centerPosition);

            foreach (var seed in seeds)
            {
                var position = seed.transform;
                targets.Add(position);
            }

            _newPosition = transform.position;
            _newZoom = _cam.fieldOfView;
        }

        
        private void LateUpdate()
        {
            //calculate position of progress between balloons
            
            switch (manager.gameState)
            {
                case SpiroManager.State.Inhaling:
                {
                    minZoom = 17;
                    UpdateCameraPosition();
                    UpdateCameraZoom();
                    break;
                }
                case SpiroManager.State.Exhaling:
                {
                    minZoom = 23;
                    UpdateCameraPosition();
                    UpdateCameraZoom();
                    break;
                }
                case SpiroManager.State.Done:
                {
                    UpdateCameraPosition();
                    UpdateCameraZoom();
                    break;
                }
            }
            
            //visualize position and zoom with smooth factor
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_newPosition.x, _newPosition.y, _newPosition.z), ref _velocity, smoothTime);
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _newZoom, Time.deltaTime);
            
        }

        private void UpdateCameraPosition()
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (var t in targets)
            {
                bounds.Encapsulate(t.position);
            }
            var centerPoint = bounds.center;
            if (centerPoint.y > minVertLimit && centerPoint.y < maxVertLimit)
            {
                _newPosition = centerPoint + offset;
            }
        }

        private void UpdateCameraZoom()
        {
            if (targets.Count <= 1) return;
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (var t in targets)
            {
                bounds.Encapsulate(t.position);
            }
            var greatestDistance = bounds.size.x;
            _newZoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter);
        }
    }
}