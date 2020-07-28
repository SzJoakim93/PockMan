using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Xml;

public class Manage_settings : MonoBehaviour {

    public Dropdown music;
    public Dropdown lang;
    public Dropdown controll;
    public Language_manager lang_manager;
    public Slider Volume;

	// Use this for initialization
	void Start () {
        music.value = PlayerPrefs.GetInt("MusicEnabled", 0);
        controll.value = PlayerPrefs.GetInt("ControlType", 0);
        lang.value =  LangStrToInt(PlayerPrefs.GetString("Language", "ENG"));
        Volume.value = PlayerPrefs.GetFloat("Volume", 0.5f);

        if (PlayerPrefs.HasKey("Language"))
            lang.value =  LangStrToInt(PlayerPrefs.GetString("Language", "ENG"));
        else {
            if (Application.systemLanguage == SystemLanguage.Hungarian)
                lang.value =  LangStrToInt("HUN");
            else
                lang.value =  LangStrToInt("ENG");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void set_language(int value)
    {
        switch (value)
        {
            case 0:
                Global.current_language = "ENG";
                break;
            case 1:
                Global.current_language = "HUN";
                break;
        }

        lang_manager.Set_Language();
        PlayerPrefs.SetString("Language", Global.current_language);
    }

    public void set_music(int value)
    {
        Global.music_enabled = value == 0;
        PlayerPrefs.SetInt("MusicEnabled", value);
    }

    public void setMusicVolume(float value) {
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void setControllType(int value) {
        Global.controll_type = value;
        PlayerPrefs.SetInt("ControlType", value);
    }

    int LangStrToInt(string langStr) {
        if (langStr == "HUN")
            return 1;
        return 0;
    }
}
