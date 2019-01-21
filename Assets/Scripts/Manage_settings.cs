using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Xml;

public class Manage_settings : MonoBehaviour {

    public Text lang_text;
    public Text music_text;
    public Language_manager lang_manager;

	// Use this for initialization
	void Start () {
        fix_titles();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void fix_titles()
    {
        if (Global.music_enabled)
            music_text.text = Global.GetTextByValue("name", "/Canvas/Settings/music_txt") + ": " + Global.GetTextByValue("name", "SWITCH ON");
        else
            music_text.text = Global.GetTextByValue("name", "/Canvas/Settings/music_txt") + ": " + Global.GetTextByValue("name", "SWITCH OFF");
    }

    public void set_language()
    {
        if (Global.current_language == "ENG")
            Global.current_language = "HUN";
        else
            Global.current_language = "ENG";
        lang_manager.Set_Language();

        //change_settings_text(lang_text, Global.current_language);
        PlayerPrefs.SetString("Language", Global.current_language);

        fix_titles();
    }

    public void set_music()
    {
        if (Global.music_enabled)
        {
            Global.music_enabled = false;
            music_text.text = Global.GetTextByValue("name", "/Canvas/Settings/music_txt") + ": " + Global.GetTextByValue("name", "SWITCH OFF");
        }
        else
        {
            Global.music_enabled = true;
            music_text.text = Global.GetTextByValue("name", "/Canvas/Settings/music_txt") + ": " + Global.GetTextByValue("name", "SWITCH ON");
        }
    }

    void change_settings_text(Text set_text, string option)
    {
        string[] splited = set_text.text.Split(':');
        set_text.text = splited[0] + ": " + option;
    }
}
