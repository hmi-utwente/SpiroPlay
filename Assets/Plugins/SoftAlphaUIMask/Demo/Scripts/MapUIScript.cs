using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SoftAlphaUIMask/UI/Demo/MapUIScript")]
public class MapUIScript : MonoBehaviour
{
    public RectTransform maskPointer;
    public GameObject Map;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        maskPointer.position = Input.mousePosition;

        if (Input.GetKeyUp(KeyCode.M))
            Map.SetActive(!Map.gameObject.activeInHierarchy);
    }
}