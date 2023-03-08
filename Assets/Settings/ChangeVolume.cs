
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class ChangeVolume : MonoBehaviour
{
    [SerializeField] Slider masterSlider, musicSlider, fxSlider;
    [SerializeField] TMPro.TMP_InputField masterInput, musicInput, fxInput;
    public AudioMixer _MasterMixer;


    // Start is called before the first frame update
    void Start ()
    {
        if(!PlayerPrefs.HasKey("masterVolume")) 
            PlayerPrefs.SetFloat("masterVolume", 1);

        if(!PlayerPrefs.HasKey("musicVolume")) 
            PlayerPrefs.SetFloat("musicVolume", 1);

        if(!PlayerPrefs.HasKey("fxVolume")) 
            PlayerPrefs.SetFloat("fxVolume", 1);
        
        Load();
    }


    // Master Volume Functions
    public void masterSliderChange () 
    {
        masterInput.text = Mathf.Round(masterSlider.value * 100.0f).ToString(); 
        
        Save();
        Load();
    }

    public void masterInputChange () 
    {
        int value;
        if (int.TryParse(masterInput.text, out value)) 
        {
            if (value > 100)
                value = 100;
            masterSlider.value = ((float) value) / 100;
            
            Save();
            Load();
        }
    }



    // Music Volume Functions
    public void musicSliderChange () 
    {
        musicInput.text = Mathf.Round(musicSlider.value * 100.0f).ToString(); 
        
        Save();
        Load();
    }

    public void musicInputChange () 
    {
        int value;
        if (int.TryParse(musicInput.text, out value)) 
        {
            if (value > 100)
                value = 100;
            musicSlider.value = ((float) value) / 100;
            
            Save();
            Load();
        }
    }


    // Fx Volume Functions
    public void fxSliderChange () 
    {
        fxInput.text = Mathf.Round(fxSlider.value * 100.0f).ToString(); 
        
        Save();
        Load();
    }
    
    public void fxInputChange () 
    {
        int value;
        if (int.TryParse(fxInput.text, out value)) 
        {
            if (value > 100)
                value = 100;
            fxSlider.value = ((float) value) / 100;

            Save();
            Load();
        }
    }


    // Functions for Saving 
    private void Load ()
    {
        // Master load
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        masterInput.text = Mathf.Round(masterSlider.value * 100.0f).ToString();

        if (masterSlider.value == 0)
            _MasterMixer.SetFloat ("master", -80f);
        else
            _MasterMixer.SetFloat ("master", Mathf.Log10(masterSlider.value) * 20f);


        // Music Load
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        musicInput.text = Mathf.Round(musicSlider.value * 100.0f).ToString();

        if (musicSlider.value == 0)
            _MasterMixer.SetFloat ("music", -80f);
        else
            _MasterMixer.SetFloat ("music", Mathf.Log10(musicSlider.value) * 20f);


        // FX Load
        fxSlider.value = PlayerPrefs.GetFloat("fxVolume");
        fxInput.text = Mathf.Round(fxSlider.value * 100.0f).ToString();

        if (fxSlider.value == 0)
            _MasterMixer.SetFloat ("fx", -80f);
        else
            _MasterMixer.SetFloat ("fx", Mathf.Log10(fxSlider.value) * 20f);
    }

    private void Save ()
    {
        PlayerPrefs.SetFloat("masterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("fxVolume", fxSlider.value);
    }
}
