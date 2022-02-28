using Spirometry.Statics;
using UnityEngine;

namespace M1_Auto
{
    public class SpeedometerBehaviour : MonoBehaviour
    {
        
        #region variables
        #pragma warning disable 649

        [SerializeField] private bool LimitRotation;
        [SerializeField] private float startRotation;
        [SerializeField] private float endRotation;
        [SerializeField] private RectTransform arrow;
        
        #pragma warning restore 649
        #endregion

        public void SetSpeed(float progress)
        {
            //limit position of the arrow to the end of the speedometer is configured
            var pos = LimitRotation && progress > 100 ? 100f : progress;
            pos = progress.Remap(0, 100, startRotation, endRotation);
            arrow.eulerAngles = new Vector3(0, 0, pos);
        }
    }
}
