using System;
using Spirometry.Statics;
using UnityEngine;

namespace M7_Hond
{
    public class DogBehaviour : MonoBehaviour
    {
        
        #region variables
        #pragma warning disable 649

        [Header("References")]
        [SerializeField] private M7_Manager manager;
        [SerializeField] private RectTransform maskParent;
        [SerializeField] private RectTransform imageChild;

        [Header("Config")]
        [SerializeField] private float scaleFactor;
        private Vector3 _startPositionMask;
        private Vector3 _startPositionChild;

#pragma warning restore 649
        #endregion

        private void Start()
        {
            _startPositionMask = maskParent.position;
            _startPositionChild = imageChild.position;
        }

        public void SetFadeProgress(float progress)
        {
            if (progress > 106) return;
            var shift = progress.Remap(0, 100, 0, scaleFactor);
            maskParent.position = new Vector3(_startPositionMask.x + shift, _startPositionMask.y, _startPositionMask.z);
            imageChild.position = new Vector3(_startPositionChild.x, _startPositionChild.y, _startPositionChild.z);
        }
    }
}
