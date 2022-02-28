using System;
using System.Collections;
using System.Collections.Generic;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Serialization;

namespace M3_Ballonnen
{

    [RequireComponent(typeof(Camera))]
    public class CameraBehavior : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649

        [Header("References")]
        [SerializeField] private SpiroManager manager;
        [SerializeField] private ArrowBehavior arrow;
        [SerializeField] private Transform progressPosition;
        [SerializeField] private Transform firstBalloon;
        [SerializeField] private Transform lastBalloon;
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
        private Vector3 ArrowPos => arrow.transform.position;
        private CameraBehavior cambeh;
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

        #pragma warning restore 649
        #endregion

        private void Start()
        {
            cambeh = GetComponent<CameraBehavior>();
            _cam = GetComponent<Camera>();
            targets.Add(arrow.transform);
            targets.Add(progressPosition);

            _newPosition = transform.position;
            _newZoom = _cam.fieldOfView;
        }

        
        private void LateUpdate()
        {
            //calculate position of progress between balloons
            SetProgressPosition(manager.expirationProgressLong);

            switch (manager.gameState)
            {
                case SpiroManager.State.Inhaling:
                {
                    UpdateCameraPosition();
                    UpdateCameraZoom();
                    break;
                }
                case SpiroManager.State.Exhaling:
                {
                    //if arrow is far enough, AND out of range, stop camera movement
                    if (ArrowPos.x < lastBalloon.position.x)
                        if (ArrowPos.y < maxVertLimit && ArrowPos.y > minVertLimit)
                        {
                            UpdateCameraPosition();
                            UpdateCameraZoom();
                        }
                    break;
                }
            }

            //visualize position and zoom with smooth factor
            if (manager.gameState == SpiroManager.State.Inhaling || manager.gameState == SpiroManager.State.Exhaling)
            {
                transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_newPosition.x, _newPosition.y, _newPosition.z), ref _velocity, smoothTime);
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _newZoom, Time.deltaTime);
            }

            if (arrow.stopped)
            {
                cambeh.enabled = false;
            }
        }

        private void SetProgressPosition(float progress)
        {
            float pos = progress;
            var firstPos = firstBalloon.position;
            var lastPos = lastBalloon.position;
            pos = pos.Remap(0, 100, firstPos.x, lastPos.x);
            var y = (firstPos.y + lastPos.y) / 2;
            var position = progressPosition.position;
            position = new Vector3(pos, y, position.z);
            progressPosition.position = position;
        }
        
        private void UpdateCameraPosition()
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (var t in targets)
            {
                bounds.Encapsulate(t.position);
            }
            var centerPoint = bounds.center;
            _newPosition = centerPoint + offset;
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