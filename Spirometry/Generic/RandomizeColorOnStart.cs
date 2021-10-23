using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeColorOnStart : MonoBehaviour
{
    
    //variables
    public Color[] colors;
    private Color RandomColor => colors[Random.Range(0, colors.Length)];

    // Start is called before the first frame update
    private void Start()
    {
        
        if (GetComponent<SpriteRenderer>() != null) ChangeSpriteColor();
        if (GetComponent<Image>() != null) ChangeImageColor();
    }

    private void ChangeSpriteColor()
    {
        GetComponent<SpriteRenderer>().color = RandomColor;
    }

    private void ChangeImageColor()
    {
        GetComponent<Image>().color = RandomColor;
    }
}
