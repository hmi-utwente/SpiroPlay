using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source = null;

    public void PlaySound(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlaySoundLoop(AudioClip clip) {
        source.loop = true;
        source.clip = clip;
        source.Play();
    }

    public void StopSound() {
        source.Stop();
    }
}
