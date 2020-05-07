using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class in_game_buttons : MonoBehaviour {

	public GameObject pause_panel;
    public GameObject warn_panel;
    public GameObject comp_panel;
    public GameObject arrows;
    public GameObject arrows_smooth;

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
        if (Global.classic) {
            Global.level += 101;

            if (Global.level == Global.unlocked_levels-1 && Global.unlocked_levels % 5 == 0) {
                Global.level_menu = 1;
                Application.LoadLevel("menu");
            }

        }  
        else {
            Global.level++;

            if (Global.level == Global.unlocked_clevels-1 && Global.unlocked_clevels % 5 == 0) {
                Global.level_menu = 2;
                Application.LoadLevel("menu");
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

	public void return_to_menu() {

        if (Global.global_stars >= Global.next_card_stars && Global.Free_slot_exist())
            Global.level_menu = 3;
        else if (Global.classic)
            Global.level_menu = 2;
        else
            Global.level_menu = 1;
		Application.LoadLevel ("menu");
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
}
