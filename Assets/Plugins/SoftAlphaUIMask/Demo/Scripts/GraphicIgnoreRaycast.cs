using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SoftAlphaUIMask/UI/Demo/GraphicIgnoreRaycast")]
public class GraphicIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool AcceptMouse = false;

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return AcceptMouse;
    }
}