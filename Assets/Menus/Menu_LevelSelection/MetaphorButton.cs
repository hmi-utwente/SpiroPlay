using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MetaphorButton : MonoBehaviour
{
    
    #region variables
    #pragma warning disable 649

    [Header("Button fields")]
    public Button button;
    public Button gifButton;
    public VideoClip gifClip;
    public Image lockIcon;
    public bool isTest;

    public bool Unlocked
    {
        get => _unlocked;
        set
        {
            lockIcon.enabled = !value;
            _unlocked = value;
            
            //change button colors
            switch (value)
            {
                case true:
                    button.GetComponent<Image>().color = defaultButtonColor;
                    break;
                default:
                    button.GetComponent<Image>().color = lockedButtonColor;
                    break;
            }
        }
    }
    private bool _unlocked = false;
    
    private Vector3 _originalScale;

    [Header("Config")]
    public int index;
    public int cost;
    //public int daysToUnlock;
    public Color defaultButtonColor;
    public Color lockedButtonColor;
    
    #pragma warning restore 649
    #endregion
    
    // Start is called before the first frame update
    private void Start()
    {
        //disable button for appear animation
        var localScale = transform.localScale;
        _originalScale = new Vector3(localScale.x, localScale.y, localScale.z);
        transform.localScale = Vector3.zero;
        
        //show correct interactable state for gif button and lock icon
        gifButton.interactable = gifClip != null;
        //GET USER SAVE HERE FOR UNLOCKED METAPHORS
        lockIcon.enabled = !_unlocked;
    }

    public void Appear()
    {
        LeanTween.scale(this.gameObject, _originalScale, 0.4f);
    }
}
