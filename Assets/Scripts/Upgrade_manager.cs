using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Upgrade_manager : MonoBehaviour {

	public Text timetext;
	public Text thundertext;
	public Text magnetontext;
	public Text minetext;
	public Text converttext;
	public Text doubletext;
	public Text safetext;
	public Text fruittext;
	public Text ammotext;
	public Text clonetext;
	public Text score_txt;

	public Text cost;

	public GameObject panel;
	public GameObject panel2;

	public void upgrade(ref int level, ref int extra_value, ref Text txt, bool isPercent, string pref_value)
	{
		int req_point;

        if (isPercent)
            req_point = level * 250;
        else
            req_point = level * 2000;
		if (Global.global_points >= req_point) {
			level++;

			if (isPercent)
				extra_value += (int)(extra_value*0.2f);
			else
				extra_value++;
			Global.global_points -= req_point;
			txt.text = "" + level;
			score_txt.text = "" + Global.global_points;

			PlayerPrefs.SetInt("Global_points", Global.global_points);
			PlayerPrefs.SetInt(pref_value, level);
		} else {
			panel2.SetActive(true);
		}
	}

	public void upgrade_btn(int index) {
		Global.selected_upgrade = index;
		panel.SetActive (true);

		if (index == 0)
			cost.text = "" + Global.level_pause * 250;
		else if (index == 1)
			cost.text = "" + Global.level_thunder * 2000;
		else if (index == 2)
			cost.text = "" + Global.level_magneton * 250;
		else if (index == 3)
			cost.text = "" + Global.level_mines * 2000;
		else if (index == 4)
			cost.text = "" + Global.level_convert * 250;
		else if (index == 5)
			cost.text = "" + Global.level_double * 250;
		else if (index == 6)
			cost.text = "" + Global.level_safe * 250;
		else if (index == 7)
			cost.text = "" + Global.level_fruits * 250;
		else if (index == 8)
			cost.text = "" + Global.level_ammo * 2000;
		else if (index == 9)
			cost.text = "" + Global.level_clone * 250;
	}

	public void yes_btn() {
		if (Global.selected_upgrade == 0)
			upgrade (ref Global.level_pause, ref Global.max_pause, ref timetext, true, "Pause_level");
		else if (Global.selected_upgrade == 1)
			upgrade (ref Global.level_thunder, ref Global.max_thunder, ref thundertext, false, "Thunder_level");
		else if (Global.selected_upgrade == 2)
			upgrade (ref Global.level_magneton, ref Global.max_magneton, ref magnetontext, true, "Magneton_level");
		else if (Global.selected_upgrade == 3)
			upgrade (ref Global.level_mines, ref Global.max_mines, ref minetext, false, "Mine_level");
		else if (Global.selected_upgrade == 4)
			upgrade (ref Global.level_convert, ref Global.max_convert, ref converttext, true, "Convert_level");
		else if (Global.selected_upgrade == 5)
			upgrade (ref Global.level_double, ref Global.max_double, ref doubletext, true, "Double_level");
		else if (Global.selected_upgrade == 6)
			upgrade (ref Global.level_safe, ref Global.max_safe, ref safetext, true, "Safe_level");
		else if (Global.selected_upgrade == 7)
			upgrade (ref Global.level_fruits, ref Global.max_fruits, ref fruittext, true, "Fruits_level");
		else if (Global.selected_upgrade == 8)
			upgrade (ref Global.level_ammo, ref Global.max_ammo, ref ammotext, false, "Ammo_level");
		else if (Global.selected_upgrade == 9)
			upgrade (ref Global.level_clone, ref Global.max_clone, ref clonetext, true, "Clone_level");

		panel.SetActive (false);
	}

	public void no_btn() {
		panel.SetActive (false);
	}

	public void ok_btn() {
		panel2.SetActive (false);
	}
}
