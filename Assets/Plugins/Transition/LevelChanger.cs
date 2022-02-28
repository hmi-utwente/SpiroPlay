using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spiro-Play project by University of Twente and MST
/// Made by Koen Vogel - k.a.vogel@student.utwente.nl
/// 
/// LevelChanger class:
/// Manages smooth scene transitions
/// </summary>

public class LevelChanger : MonoBehaviour
{
    #region variables
    #pragma warning disable 649
    
    //singleton pattern
    private static LevelChanger _instance;
    public static LevelChanger Instance { get { return _instance; } }

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float TransitionDuration = 2f;

    private static int levelToLoadIndex = 99;
    private static string levelToLoadName = "";
    private static bool IsSwitching = false;
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");
    private static readonly int FadeIn = Animator.StringToHash("FadeIn");
    private static readonly int Speed = Animator.StringToHash("Speed");

#pragma warning restore 649
    #endregion

    private void Awake()
    {
        //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        //make sure the levelchanger resets on scene switch
        DontDestroyOnLoad(this.gameObject);
    }
    
    //overloading methods for activating scene switches
    public static void FadeToLevel(int levelIndex)
    {
        if (IsSwitching) return;
        
        //fallback; load scene anyway if fade is not possible
        if (_instance == null)
        {
            SceneManager.LoadScene(levelIndex);
            return;
        }

        levelToLoadIndex = levelIndex;
        levelToLoadName = "";
        Instance.StartCoroutine(Instance.StartFade());
    }

    public static void FadeToLevel(string levelName)
    {
        if (IsSwitching) return;
        
        //fallback; load scene anyway if fade is not possible
        if (_instance == null)
        {
            SceneManager.LoadScene(levelName);
            return;
        }

        levelToLoadName = levelName;
        levelToLoadIndex = 99;
        Instance.StartCoroutine(Instance.StartFade());
    }

    private IEnumerator StartFade()
    {
        IsSwitching = true;
        animator.SetFloat(Speed, 1/TransitionDuration);
        animator.SetTrigger(FadeOut);
        yield return new WaitForSeconds(TransitionDuration);
        
        if (levelToLoadIndex != 99)
            SceneManager.LoadScene(levelToLoadIndex);
        else
            SceneManager.LoadScene(levelToLoadName);
        
        
        animator.SetTrigger(FadeIn);
        IsSwitching = false;
        yield return null;
    }
}
