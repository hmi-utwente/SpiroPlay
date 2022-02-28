using UnityEngine;
using Spirometry.Statics;

public class SeaScript : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] Transform seaTransform = null;

    [Header("Variables")]
    [SerializeField] float smoothFactor = 1.0f;

    private Vector3 SmoothToPosition(Vector3 newPos)
    {
        return Vector3.LerpUnclamped(seaTransform.position, newPos, Time.fixedDeltaTime * smoothFactor);
    }

    public void SmoothBetween(Vector3 lowerPos, Vector3 higherPos, float progress)
    {
        seaTransform.position = SmoothToPosition(new Vector3(progress.Remap(0, 110, lowerPos.x, higherPos.x), progress.Remap(0, 110, lowerPos.y, higherPos.y), seaTransform.position.z));
    }


}
