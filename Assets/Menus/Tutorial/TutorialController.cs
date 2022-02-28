using System.Collections;
using System.Collections.Generic;
using Spirometry.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
public class TutorialController : MonoBehaviour
{

    //determine if watching tutorial from options menu by presense of spirocontroller
    private static bool FromMenu => GameObject.FindWithTag("GameController") != null;
    public Settings settings;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        //wach tutorial videoPlayer if not done before or started from options menu
        var watched = PlayerPrefs.GetInt("watched", 0);
        if (watched != 1 || FromMenu)
            StartCoroutine(WatchTutorial());
        else
            SceneManager.LoadScene("UserSelection");
    }

    private IEnumerator WatchTutorial()
    {
        //play videoPlayer
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = settings.Feedback.video;
        videoPlayer.Prepare();
        videoPlayer.EnableAudioTrack (0, true);
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        audioSource.Play();
        videoPlayer.Play();

        yield return new WaitForSeconds(2f);
        //skip once done
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        
        //yield return new WaitForSeconds(5f);
        Skip();
        yield return null;
    }

    public void Skip()
    {
        PlayerPrefs.SetInt("watched", 1);
        LevelChanger.FadeToLevel(FromMenu ? "MainMenu" : "UserSelection");
    }
}
