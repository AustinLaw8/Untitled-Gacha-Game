using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings", order = 1)]
public class SettingsSO : ScriptableObject
{
    /* Audio Settings */
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;

    /* Gameplay Settings */ 
    [SerializeField] private float _noteSpeed;
    [SerializeField] private float _noteOffset;
    [SerializeField] private float _transparency;
    [SerializeField] private float _brightness;
    [SerializeField] private bool _showSkill;

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
        set => _noteSpeed = value;
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
