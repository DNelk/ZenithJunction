using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    private AudioSource _audio;
    private Dictionary<string, AudioClip> _clips;
    public ClipsWithKeys[] Clips;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _clips = new Dictionary<string, AudioClip>();
        foreach (ClipsWithKeys c in Clips)
        {
            _clips.Add(c.Key, c.Clip);
        }

        _audio = GetComponent<AudioSource>();
    }

    public void PlayClip(string clipName, float pitchMod = 1, float soundMod = 0.4f)
    {
        if (_clips.ContainsKey(clipName))
        {
            _audio.pitch = pitchMod;
            _audio.PlayOneShot(_clips[clipName]);
        }
    }
    
}

[Serializable]
public struct ClipsWithKeys
{
    public string Key;
    public AudioClip Clip;
}