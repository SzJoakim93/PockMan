using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class in_game_buttons : MonoBehaviour {

	public GameObject pause_panel;
    public GameObject warn_panel;
    public GameObject comp_panel;
    public GameObject arrows;
    public GameObject arrows_smooth;
    public GameObject CharacterPanel;
    int selectedButton = 0;

    void Start() {
        if (Global.controll_type == 0) {
            arrows.SetActive(false);
            arrows_smooth.SetActive(true);
        }
    }

	public void restart() {
        if (Global.classic)
            Global.level += 100;
		Application.LoadLevel("ingame");
	}

	public void next_level() {
        if (!isCharacterPlayable()) {
            CharacterPanel.SetActive(true);
            selectedButton = 1;
        } else
            nextLevel();
	}

	public void return_to_menu() {

        if (!isCharacterPlayable()) {
            CharacterPanel.SetActive(true);
            selectedButton = 0;
        } else
            returnToMenu();
	}

	public void pause_game() {
		Global.pause_game = true;
		pause_panel.SetActive (true);
	}

	public void resume_game() {
		Global.pause_game = false;
		pause_panel.SetActive (false);
	}

    public void warn_ok()
    {
        warn_panel.SetActive(false);
        comp_panel.SetActive(true);
    }

    public void CharacterWarnOk() {
        if (selectedButton == 0)
            returnToMenu();
        else
            nextLevel();
    }

    bool isCharacterPlayable() {
        if (Global.selectedCharacter == 0)
            return true;

        int characterStete = PlayerPrefs.GetInt("Character" + Global.selectedCharacter.ToString(), 5);

        if (characterStete > 0) {
            characterStete--;
            PlayerPrefs.SetInt("Character" + Global.selectedCharacter.ToString(), characterStete);
        }            

        if (characterStete == 0) {
            Global.selectedCharacter = 0;
            PlayerPrefs.SetInt("Character", characterStete);
            return false;
        }

        return true;

    }

    void nextLevel() {
        if (Global.classic) {
            Global.level += 101;

            if (Global.level-100 == Global.unlocked_clevels && Global.unlocked_clevels % 5 == 0) {
                Global.level_menu = 2;
                Application.LoadLevel("menu");
                return;
            }

        }  
        else {
            Global.level++;

            if (Global.level == Global.unlocked_levels && Global.unlocked_levels % 5 == 0) {
                Global.level_menu = 1;
                Application.LoadLevel("menu");
                return;
            }
        }
            

        if (Global.global_stars >= Global.next_card_stars && Global.Free_slot_exist())
        {
            Global.level_menu = 4;
            Application.LoadLevel("menu");
        }
        else
		    Application.LoadLevel ("ingame");
    }

    void returnToMenu() {
        if (Global.global_stars >= Global.next_card_stars && Global.Free_slot_exist())
            Global.level_menu = 3;
        else if (Global.classic)
            Global.level_menu = 2;
        else
            Global.level_menu = 1;
		Application.LoadLevel ("menu");
    }
}
