using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource source;
    [SerializeField] Image toggle;
    bool isPlaying;

    private void Start()
    {
        if (clip != null && source != null && toggle != null)
        {
            isPlaying = true;
            toggle.enabled = true;
            source.clip = clip;
            source.Play();
        }
    }

    public void Toggle()
    {
        if(isPlaying)
        {
            Mute();
        }
        else
        {
            UnMute();
        }
    }

    public void Mute()
    {
        source.mute = true;
        toggle.enabled = false;
        isPlaying = false;
    }

    public void UnMute()
    {
        source.mute = false;
        toggle.enabled = true;
        isPlaying = true;
    }
}
