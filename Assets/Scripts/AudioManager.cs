using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Sound[] sfxSound;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("SFXMute") == 1)
        {
            sfxSource.mute = true;
        }
        else
        {
            sfxSource.mute = false;
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound SFX not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        if (sfxSource.mute)
        {
            PlayerPrefs.SetInt("SFXMute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SFXMute", 0);
        }
    }
}
