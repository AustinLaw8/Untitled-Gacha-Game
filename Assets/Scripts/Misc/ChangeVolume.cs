
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class ChangeVolume : MonoBehaviour
{
    [SerializeField] private SettingsSO settings;
    [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
    [SerializeField] private TMPro.TMP_InputField masterInput, musicInput, sfxInput;
    [SerializeField] private AudioMixer _MasterMixer;

    // Start is called before the first frame update
    void Start ()
    {
        if(!PlayerPrefs.HasKey("masterVolume")) 
            PlayerPrefs.SetFloat("masterVolume", 1);

        if(!PlayerPrefs.HasKey("musicVolume")) 
            PlayerPrefs.SetFloat("musicVolume", 1);

        if(!PlayerPrefs.HasKey("sfxVolume")) 
            PlayerPrefs.SetFloat("sfxVolume", 1);
        
        settings.masterVolume = PlayerPrefs.GetFloat("masterVolume");
        settings.musicVolume = PlayerPrefs.GetFloat("musicVolume");
        settings.sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        
        Load();
    }
    ///////////////////////////* Master Volume Functions *///////////////////////////
    public void masterSliderChange () 
    {
        settings.masterVolume = masterSlider.value;
        Load();
    }

    public void masterInputChange () 
    {
        int value;
        if (int.TryParse(masterInput.text, out value)) 
        {
            if (value > 100)
                value = 100;
            settings.masterVolume = value / 100f;
        }
        Load();
    }
    ////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////* Music Volume Functions */100f//////////////////////////
    public void musicSliderChange () 
    {
        settings.musicVolume = musicSlider.value;
        Load();
    }

    public void musicInputChange () 
    {
        int value;
        if (int.TryParse(musicInput.text, out value)) 
        {
            if (value > 100)
                value = 100;
            settings.musicVolume = value / 100f;
        }
        Load();
    }
    ////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////* SFX Volume Functions *///////////////////////////
    public void sfxSliderChange () 
    {   
        settings.sfxVolume = sfxSlider.value;
        Load();
    }
    
    public void sfxInputChange () 
    {
        int value;
        if (int.TryParse(sfxInput.text, out value)) 
        {
            if (value > 100)
                value = 100;
            settings.sfxVolume = value / 100f;
        }
        Load();
    }
    ////////////////////////////////////////////////////////////////////////////////


    // Reloads sliders and text
    private void Load ()
    {
        // Master load
        masterSlider.value = settings.masterVolume;
        masterInput.text = Mathf.Round(settings.masterVolume * 100.0f).ToString();

        if (settings.masterVolume == 0)
            _MasterMixer.SetFloat ("master", -80f);
        else
            _MasterMixer.SetFloat ("master", Mathf.Log10(settings.masterVolume) * 20f);


        // Music Load
        musicSlider.value = settings.musicVolume;
        musicInput.text = Mathf.Round(settings.musicVolume * 100.0f).ToString();

        if (settings.musicVolume == 0)
            _MasterMixer.SetFloat ("music", -80f);
        else
            _MasterMixer.SetFloat ("music", Mathf.Log10(settings.musicVolume) * 20f);


        // SFX Load
        sfxSlider.value = settings.sfxVolume;
        sfxInput.text = Mathf.Round(settings.sfxVolume * 100.0f).ToString();

        if (settings.sfxVolume == 0)
            _MasterMixer.SetFloat ("sfx", -80f);
        else
            _MasterMixer.SetFloat ("sfx", Mathf.Log10(settings.sfxVolume) * 20f);

        Save();
    }

    private void Save ()
    {
        PlayerPrefs.SetFloat("masterVolume", settings.masterVolume);
        PlayerPrefs.SetFloat("musicVolume", settings.musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", settings.sfxVolume);
    }
}
