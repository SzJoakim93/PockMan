using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class in_game_buttons : MonoBehaviour {

	public GameObject pause_panel;
    public GameObject warn_panel;
    public GameObject comp_panel;
    public GameObject arrows;
    public GameObject arrows_smooth;
    public GameObject CharacterPanel;
    public AdManager adManager;
    enum SelectedButton
    {
        QuitToMenu,
        NextLevel,
        RestartLevel
    }
    SelectedButton selectedButton;
    static bool secondLevel = false;
    bool enableAds;

    void Start() {
        if (Global.controll_type == 0) {
            arrows.SetActive(false);
            arrows_smooth.SetActive(true);
        }

        enableAds = PlayerPrefs.GetInt("EnableAds", 1) == 1;
    }

	public void RestartBtn() {
        selectedButton = SelectedButton.RestartLevel;

        #if !UNITY_EDITOR
        if (!isCharacterPlayable()) {
            CharacterPanel.SetActive(true);
            return;
        }

        if (enableAds && secondLevel && adManager.ShowInterstitialAd()) {
            secondLevel = false;
            return;
        } else
            secondLevel = true;
        #endif

        restart();

	}

	public void next_level() {
        selectedButton = SelectedButton.NextLevel;

        #if !UNITY_EDITOR
        if (!isCharacterPlayable()) {
            CharacterPanel.SetActive(true);
            return;
        }
 
        if (enableAds && secondLevel && adManager.ShowInterstitialAd()) {
            secondLevel = false;
            return;
        } else
            secondLevel = true;
        #endif

        nextLevel();
	}

	public void return_to_menu() {
        selectedButton = SelectedButton.QuitToMenu;

        #if !UNITY_EDITOR
        if (!isCharacterPlayable()) {
            CharacterPanel.SetActive(true);
            return;
        }            

        if (adManager.ShowInterstitialAd())
            return;
        #endif

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
        if (adManager.ShowInterstitialAd())
            return;

        invokeSelectedEvent();
    }

    public void OnAdClosedEvent() {
        if (!isCharacterPlayable()) {
            CharacterPanel.SetActive(true);
            return;
        }
        
        invokeSelectedEvent();
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

            if (Global.level == 29) {
                SceneManager.LoadScene("outro");
                return;
            }

            Global.level += 101;

            if (Global.level-100 == Global.unlocked_clevels && Global.unlocked_clevels % 5 == 0) {
                Global.level_menu = 2;
                SceneManager.LoadScene("menu");
                return;
            }

        }  
        else {
            if (Global.level == 29) {
                SceneManager.LoadScene("outro");
                return;
            }

            Global.level++;

            if (Global.level == Global.unlocked_levels && Global.unlocked_levels % 5 == 0) {
                Global.level_menu = 1;
                SceneManager.LoadScene("menu");
                return;
            }
        }
            

        if (Global.global_stars >= Global.next_card_stars && Global.Free_slot_exist())
        {
            Global.level_menu = 4;
            SceneManager.LoadScene("menu");
        }
        else
		    SceneManager.LoadScene("ingame");
    }

    void returnToMenu() {
        if (Global.global_stars >= Global.next_card_stars && Global.Free_slot_exist())
            Global.level_menu = 3;
        else if (Global.classic)
            Global.level_menu = 2;
        else
            Global.level_menu = 1;
		SceneManager.LoadScene("menu");
    }

    void restart() {
        if (Global.classic)
            Global.level += 100;
		SceneManager.LoadScene("ingame");
    }

    void invokeSelectedEvent() {
        switch (selectedButton)
        {
            case SelectedButton.QuitToMenu:
                returnToMenu();
                break;
            case SelectedButton.NextLevel:
                nextLevel();
                break;
            case SelectedButton.RestartLevel:
                restart();
                break;
        }
    }
}
