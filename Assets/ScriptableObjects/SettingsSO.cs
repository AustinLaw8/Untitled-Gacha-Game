using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings", order = 1)]
public class SettingsSO : ScriptableObject
{
    /* Audio Settings */
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;

    /* Gameplay Settings */ 
    [SerializeField] private float _noteSpeed;
    [SerializeField] private float _noteOffset;
    [SerializeField] private float _transparency;
    [SerializeField] private float _brightness;
    [SerializeField] private bool _showSkill;

    void Awake()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        noteSpeed = PlayerPrefs.GetFloat("noteSpeed", 4f);
        noteOffset = PlayerPrefs.GetFloat("noteOffset", 0f);
        transparency = PlayerPrefs.GetFloat("transparency", .75f);
        brightness = PlayerPrefs.GetFloat("brightness", 1f);
        showSkill = PlayerPrefs.GetInt("showSkill", 1) == 1;
    }
    
    public void SetVolume()
    {
        if (masterVolume == 0)
            mixer.SetFloat ("master", -80f);
        else
            mixer.SetFloat ("master", Mathf.Log10(masterVolume) * 20f);

        if (musicVolume == 0)
            mixer.SetFloat ("music", -80f);
        else
            mixer.SetFloat ("music", Mathf.Log10(musicVolume) * 20f);

        if (sfxVolume == 0)
            mixer.SetFloat ("sfx", -80f);
        else
            mixer.SetFloat ("sfx", Mathf.Log10(sfxVolume) * 20f);
    }

    /* Audio Getter/Setters */
    public float masterVolume
    {
        get => _masterVolume;
        set => _masterVolume = Mathf.Clamp(value, 0f, 1f);
    }

    public float musicVolume
    {
        get => _musicVolume;
        set => _musicVolume = Mathf.Clamp(value, 0f, 1f);
    }

    public float sfxVolume
    {
        get => _sfxVolume;
        set => _sfxVolume = Mathf.Clamp(value, 0f, 1f);
    }

    public float noteSpeed
    {
        get => _noteSpeed;
        set => _noteSpeed = Mathf.Clamp(value, 1f, 12f);
    }

    public float noteOffset
    {
        get => _noteOffset;
        set => _noteOffset = value;
    }

    public float transparency
    {
        get => _transparency;
        set => _transparency = Mathf.Clamp(value, 0f, 1f);
    }

    public float brightness
    {
        get => _brightness;
        set => _brightness = Mathf.Clamp(value, 0f, 1f);
    }
    
    public bool showSkill
    {
        get => _showSkill;
        set => _showSkill = value;
    }
}
