using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
	string selectedUpgrade;

	public void upgrade(PowerUp powerUp, Text txt)
	{
		if (Global.global_points >= powerUp.Cost) {
			Global.global_points -= powerUp.Cost;
			PlayerPrefs.SetInt("Global_points", Global.global_points);
			score_txt.text = "" + Global.global_points;

			powerUp.Upgrade();
			txt.text = "" + powerUp.Level;

			
		} else {
			panel2.SetActive(true);
		}
	}

	public void upgrade_btn(string selectedUpgrade) {
		this.selectedUpgrade = selectedUpgrade;
		panel.SetActive (true);
		if (selectedUpgrade == "Pause")
			cost.text = "" + Global.PauseEnemy.Cost;
		else if (selectedUpgrade == "Thunder")
			cost.text = "" + Global.Thunder.Cost;
		else if (selectedUpgrade == "Magneton")
			cost.text = "" + Global.Magneton.Cost;
		else if (selectedUpgrade == "Mine")
			cost.text = "" + Global.Mine.Cost;
		else if (selectedUpgrade == "Convert")
			cost.text = "" + Global.ConvertEnemy.Cost;
		else if (selectedUpgrade == "Double")
			cost.text = "" + Global.DoubleScore.Cost;
		else if (selectedUpgrade == "Safe")
			cost.text = "" + Global.SafeZone.Cost;
		else if (selectedUpgrade == "DiamondRush")
			cost.text = "" + Global.DiamondRush.Cost;
		else if (selectedUpgrade == "Ammo")
			cost.text = "" + Global.Ammo.Cost;
		else if (selectedUpgrade == "Clone")
			cost.text = "" + Global.ClonePlayer.Cost;
	}

	public void yes_btn() {
		if (selectedUpgrade == "Pause")
			upgrade (Global.PauseEnemy, timetext);
		else if (selectedUpgrade == "Thunder")
			upgrade (Global.Thunder, thundertext);
		else if (selectedUpgrade == "Magneton")
			upgrade (Global.Magneton, magnetontext);
		else if (selectedUpgrade == "Mine")
			upgrade (Global.Mine, minetext);
		else if (selectedUpgrade == "Convert")
			upgrade (Global.ConvertEnemy, converttext);
		else if (selectedUpgrade == "Double")
			upgrade (Global.DoubleScore, doubletext);
		else if (selectedUpgrade == "Safe")
			upgrade (Global.SafeZone, safetext);
		else if (selectedUpgrade == "DiamondRush")
			upgrade (Global.DiamondRush, fruittext);
		else if (selectedUpgrade == "Ammo")
			upgrade (Global.Ammo, ammotext);
		else if (selectedUpgrade == "Clone")
			upgrade (Global.ClonePlayer,clonetext);

		panel.SetActive (false);
	}

	public void no_btn() {
		panel.SetActive (false);
	}

	public void ok_btn() {
		panel2.SetActive (false);
	}
}
