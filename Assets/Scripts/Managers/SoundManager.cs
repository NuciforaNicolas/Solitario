using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource source;
    [SerializeField] Sprite mute, unMute;
    [SerializeField] Button soundButton;
    bool isPlaying;

    private void Start()
    {
        if (clip != null && source != null && soundButton != null)
        {
            isPlaying = true;
            soundButton.image.sprite = unMute;
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
        soundButton.image.sprite = mute;
        isPlaying = false;
    }

    public void UnMute()
    {
        source.mute = false;
        soundButton.image.sprite = unMute;
        isPlaying = true;
    }
}
