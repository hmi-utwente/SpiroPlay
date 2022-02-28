using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InfoButton : MonoBehaviour
{

    #pragma warning disable 649
    [SerializeField] private GameObject overlay;
    private Button Button => GetComponent<Button>();
    #pragma warning restore 649
    
    // Start is called before the first frame update
    private void Start()
    {
        Button.onClick.AddListener(ToggleOverlay);
    }

    private void ToggleOverlay()
    {
        overlay.SetActive(!overlay.activeSelf);
    }
}
