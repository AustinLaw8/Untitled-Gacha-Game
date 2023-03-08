using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeGameplay : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField speedInput, offsetInput, transparentInput, brightInput;
    [SerializeField] Toggle showSkills;

    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("noteSpeed")) 
            PlayerPrefs.SetInt("noteSpeed", 5);

        if(!PlayerPrefs.HasKey("noteOffset")) 
            PlayerPrefs.SetInt("noteOffset", 5);

        if(!PlayerPrefs.HasKey("transparency")) 
            PlayerPrefs.SetFloat("transparency", 1f);
        
        if(!PlayerPrefs.HasKey("brightness")) 
            PlayerPrefs.SetFloat("brightness", 1f);

        if(!PlayerPrefs.HasKey("showSkill")) 
            PlayerPrefs.SetString("showSkill", "true");

        Load();
    }

    public void speedChange()
    {
        int value;
        if (int.TryParse(speedInput.text, out value)) 
        {
            PlayerPrefs.SetInt("noteSpeed", value);
            Load();
        }
    }

    public void offsetChange()
    {
        int value;
        if (int.TryParse(offsetInput.text, out value)) 
        {
            PlayerPrefs.SetInt("noteOffset", value);
            Load();
        }
    }
    
    //probably should add slider
    public void transparentChange()
    {
        int result;
        if (int.TryParse(transparentInput.text, out result)) 
        {
            if (result > 100)
                result = 100;
            float value = ((float) result) / 100;

            PlayerPrefs.SetFloat("transparency", value);
            Load();
        }
    }
    
    public void brightnessChange()
    {
        int result;
        if (int.TryParse(brightInput.text, out result)) 
        {
            if (result > 100)
                result = 100;
            float value = ((float) result) / 100;

            PlayerPrefs.SetFloat("brightness", value);
            Load();
        }
    }

    public void showSkill() 
    {
        if(showSkills.isOn)
            PlayerPrefs.SetString("showSkill", "true");
        else 
            PlayerPrefs.SetString("showSkill", "false");

        Load();
    }

    private void Load()
    {
        speedInput.text = PlayerPrefs.GetInt("noteSpeed").ToString();
        offsetInput.text = PlayerPrefs.GetInt("noteOffset").ToString();
        
        transparentInput.text = Mathf.Round(PlayerPrefs.GetFloat("transparency") * 100f).ToString();
        brightInput.text = Mathf.Round(PlayerPrefs.GetFloat("brightness") * 100f).ToString();
        
        bool prefOn = true;
        if (PlayerPrefs.GetString("showSkill") == "false")
            prefOn = false; 

        if (prefOn != showSkills.isOn)
            showSkills.isOn = prefOn;
    }
}
