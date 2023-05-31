using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeGameplay : MonoBehaviour
{
    [SerializeField] private SettingsSO settings;

    [SerializeField] private TMPro.TMP_InputField speedInput, offsetInput, transparentInput, brightnessInput;
    [SerializeField] private Slider transparentSlider, brightnessSlider;
    [SerializeField] private Toggle showSkills;

    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("noteSpeed")) 
            PlayerPrefs.SetFloat("noteSpeed", 4f);

        if(!PlayerPrefs.HasKey("noteOffset")) 
            PlayerPrefs.SetFloat("noteOffset", 0f);

        if(!PlayerPrefs.HasKey("transparency")) 
            PlayerPrefs.SetFloat("transparency", .75f);
        
        if(!PlayerPrefs.HasKey("brightness")) 
            PlayerPrefs.SetFloat("brightness", 1f);

        if(!PlayerPrefs.HasKey("showSkill")) 
            PlayerPrefs.SetInt("showSkill", 1);

        settings.noteSpeed = PlayerPrefs.GetFloat("noteSpeed");
        settings.noteOffset = PlayerPrefs.GetFloat("noteOffset");
        settings.transparency = PlayerPrefs.GetFloat("transparency");
        settings.brightness = PlayerPrefs.GetFloat("brightness");
        settings.showSkill = PlayerPrefs.GetInt("showSkill") == 1;

        Load();
    }

    public void speedChange()
    {
        float value;
        if (float.TryParse(speedInput.text, out value)) 
        {
            settings.noteSpeed = value;
        }
        Load();
    }

    public void offsetChange()
    {
        float value;
        if (float.TryParse(offsetInput.text, out value)) 
        {
            settings.noteOffset = value;
        }
        Load();
    }

    public void transparentSliderChange() 
    {
        settings.transparency = transparentSlider.value;
        Load();
    }

    public void transparentInputChange()
    {
        float value;
        if (float.TryParse(transparentInput.text, out value)) 
        {
            value = Mathf.Clamp01(value);
            settings.transparency = value;
        }
        Load();
    }
    
    public void brightnessSliderChange() 
    {
        settings.brightness = brightnessSlider.value;
        Load();
    }

    public void brightnessInputChange()
    {
        float value;
        if (float.TryParse(brightnessInput.text, out value)) 
        {
            value = Mathf.Clamp01(value);
            settings.brightness = value;
        }
        Load();
    }

    public void showSkill() 
    {
        settings.showSkill = showSkills.isOn;
        Load();
    }

    private void Load()
    {
        speedInput.text = settings.noteSpeed.ToString("0.0");
        offsetInput.text = settings.noteOffset.ToString("0.0");

        transparentSlider.value = settings.transparency;
        brightnessSlider.value = settings.brightness;
        
        transparentInput.text = (Mathf.Round(settings.transparency * 100) / 100).ToString();
        brightnessInput.text = (Mathf.Round(settings.brightness * 100) / 100).ToString();

        showSkills.isOn = settings.showSkill;

        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("noteSpeed", settings.noteSpeed);
        PlayerPrefs.SetFloat("noteOffset", settings.noteOffset);
        PlayerPrefs.SetFloat("transparency", settings.transparency);
        PlayerPrefs.SetFloat("brightness", settings.brightness);
        PlayerPrefs.SetInt("showSkill", settings.showSkill ? 1 : 0);
    }
}
