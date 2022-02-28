using System;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.UI;

namespace M7_Hond
{
    public class ShampooBottleBehaviour : MonoBehaviour
    {

        #region variables
        #pragma warning disable 649

        [Header("References")]
        [SerializeField] public ParticleSystem shampooBubbles;
        [SerializeField] private M7_Manager manager;
        public SpriteRenderer Bottle => GetComponent<SpriteRenderer>();

        [Header("Settings")]
        [SerializeField] private Transform startPosition;
        [SerializeField] private Transform endPosition;

        private float _frac = 0f;
        
        #pragma warning restore 649
        #endregion

        
        // Update is called once per frame
        public void SetProgress(float progress)
        {
            var posX = progress.Remap(0, 100, startPosition.position.x, endPosition.position.x);
            var newPosition = new Vector3(posX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _frac);
            if (_frac < 1f) _frac += 5f;
        }
    }
}
