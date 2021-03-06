﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManagement : MonoBehaviour {

	/*public GameObject [] main_menu_elements;
	public GameObject [] level_elements;
	public GameObject [] classic_level_elements;*/
	public GameObject MainMenu;
	public GameObject LevelMenu;
	public GameObject ClassicMenu;
	public GameObject Upgrade_menu;
	public GameObject Dropping_menu;
    public GameObject Settings_Menu;
	public GameObject AboutMenu;
	public GameObject CharacterMenu;
	public GameObject back_btn;
	public GameObject bonus_panel;
	public GameObject loadingPanel;
	public GameObject quitPanel;
	public Image [] CharacterImgs;
	public Text ApplyBtn;
	public GameObject BuyBtn;
	public Language_manager language_Manager;
	public AdManager adManager;
	public Purchaser purchaseManager;

    Image[] card_images; //array of owned cards
	Image[][] card_getables; //matrix of getable cards
	public Sprite [] card_sprites; //sprites of card types

	public GameObject not_enought;
	public GameObject pop_up_dialog;

	public GameObject [] card_pack; //dropable cards array containing three elements (gold, silver, basic card pack)
	public GameObject own_card_pack; //pack of owned cards to get array

	public Text remaining_text;
	public Text point_txt;
	public Scroll_fixer levelMenuS;
	public Scroll_fixer classicMenuS;
	public Text Description;
	public DialogOk dialogOk;
	Color white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	Color green = new Color(0.38f, 1.0f, 0.62f, 1.0f);

	int delay = 0;

	int card_level=0;

	int [] card_costs = new int[] {500, 1000, 2000};

	bool first_running_drop = true;
	int selectedCharacter = 0;

	// Use this for initialization
	void Start(){

		card_getables = new Image[3] [];

		for (int i = 0; i < 3; i ++)
			card_getables[i] = card_pack[i].GetComponentsInChildren<Image> (true);
		card_images = own_card_pack.GetComponentsInChildren<Image> (true);

        //switch to the specified level menu
		if (Global.level_menu == 1) {
			InvokeLevelsMenu();
			if (Global.level % 30 == 0)
				levelMenuS.rightClick();

		}
		else if (Global.level_menu == 2) {
			InvokeClassicMenu ();
			if (Global.level % 30 == 10)
				classicMenuS.rightClick();
		}
		else if (Global.level_menu == 3 || Global.level_menu == 4) {

            InvokeDropCard();

			if (Global.next_card_stars % 400 == 0) //dropping gold card
				card_pack[2].SetActive(true);
			else if (Global.next_card_stars % 100 == 0) //dropping silver card
				card_pack[1].SetActive(true);
			else //dropping basic card
				card_pack[0].SetActive(true);
		}

		Button [] upgrade_buttons = Upgrade_menu.GetComponentsInChildren<Button>(true);


        //lock upgradable items depending on game completion and in case of maxed out
		if (Global.Ammo.MaxedOut())
			upgrade_buttons[0].interactable = false;

		if (Global.Thunder.MaxedOut())
			upgrade_buttons[1].interactable = false;			
		
		if ((Global.unlocked_levels < 6 && Global.unlocked_clevels < 6) ||
			Global.Magneton.MaxedOut())
			upgrade_buttons [2].interactable = false;

		if ((Global.unlocked_levels < 6 && Global.unlocked_clevels < 6) ||
			Global.Mine.MaxedOut())
			upgrade_buttons [3].interactable = false;

		if ((Global.unlocked_levels < 11 && Global.unlocked_clevels < 11) ||
			Global.ConvertEnemy.MaxedOut())
			upgrade_buttons [4].interactable = false;

		if ((Global.unlocked_levels < 21 && Global.unlocked_clevels < 21) ||
			Global.DoubleScore.MaxedOut())
			upgrade_buttons [5].interactable = false;

		if ((Global.unlocked_levels < 31 && Global.unlocked_clevels < 31) ||
			Global.PauseEnemy.MaxedOut())
			upgrade_buttons [6].interactable = false;

		if ((Global.unlocked_levels < 41 && Global.unlocked_clevels < 41) ||
			Global.DiamondRush.MaxedOut())
			upgrade_buttons [7].interactable = false;

		if ((Global.unlocked_levels < 61 && Global.unlocked_clevels < 61) ||
			Global.SafeZone.MaxedOut())
			upgrade_buttons [8].interactable = false;

		if ((Global.unlocked_levels < 71 && Global.unlocked_clevels < 71) ||
			Global.ClonePlayer.MaxedOut())
			upgrade_buttons [9].interactable = false;

	}

	void Update() {

		if (delay > 0) {
			delay--;

			if (delay == 1) {


				for (int i = 0; i < 3; i++) {
					for (int j = 1; j < 6; j++)
						card_getables[i][j].sprite = card_sprites[15+i];
					card_pack[i].SetActive(false);
				}

				Description.gameObject.SetActive(false);

                back_btn.SetActive(true);
			}
		}
	}

	void increase(ref string str, int x) {
		char [] chr = str.ToCharArray();
		if (chr [x] <= '9')
			chr [x]++;
		else {
			chr [x] = '0';
			chr [x-1] ++;
		}
		str = new string(chr);
	}

    //rush mode button clicked
	public void InvokeLevelsMenu() {
		MainMenu.SetActive (false);
		LevelMenu.SetActive (true);
		back_btn.SetActive (true);
	}

    //back button clicked
	public void Back() {

        if (Global.level_menu == 4)
			SceneManager.LoadScene("ingame");
		MainMenu.SetActive (true);
		LevelMenu.SetActive (false);
		ClassicMenu.SetActive (false);
		Upgrade_menu.SetActive (false);
		Dropping_menu.SetActive (false);
        Settings_Menu.SetActive(false);
		AboutMenu.SetActive(false);
		CharacterMenu.SetActive(false);
		back_btn.SetActive (false);

        Global.level_menu = 0;
	}

    //classic mode clicked
	public void InvokeClassicMenu() {
		MainMenu.SetActive (false);
		ClassicMenu.SetActive (true);
		back_btn.SetActive (true);
	}

    //upgrade button clicked
	public void InvokeUpgrade() {
		MainMenu.SetActive (false);
		Upgrade_menu.SetActive (true);
		//back_btn.SetActive (true);
		back_btn.SetActive (true);
	}

    //dropping card clicked
	public void InvokeDropCard() {
		MainMenu.SetActive (false);
		Dropping_menu.SetActive (true);
		back_btn.SetActive (true);

        //initialize drop menu's UI graphics
		if (first_running_drop) {

			for (int i = 0; i < 4; i++)
				if (Global.own_cards[i] > -1) {
					card_images[i+1].gameObject.SetActive(true); //set as number of cards as many cards owned
					card_images[i+1].sprite = card_sprites[Global.own_cards[i]]; //set the sprite of cards to type of owned cards
					Text [] card_text = card_images [i+1].GetComponentsInChildren<Text>(true); //get text components of owned cards
                    card_text[0].text = "" + Global.card_remaining[i]; //set the number of usage rounds
					card_text[2].text = language_Manager.GetTextByValue("CardTitle" + Global.own_cards[i]);
					if (i == Global.ac) //set the activated card's text
						card_text[1].gameObject.SetActive(true);
				}

			refresh_next_star_text();

			first_running_drop = false;
		}
	}

    //cliclking setting menu
    public void InvokeSettings()
    {
        MainMenu.SetActive(false);
        Settings_Menu.SetActive(true);
        back_btn.SetActive(true);
    }

	public void InvokeAbout() {
		MainMenu.SetActive(false);
		AboutMenu.SetActive(true);
		back_btn.SetActive(true);
	}

	public void InvokeCharacter() {
		MainMenu.SetActive(false);
		CharacterMenu.SetActive(true);
		back_btn.SetActive(true);

		SelectCharacter(Global.selectedCharacter);

	}

	public void ApplySelectedCharacter() {
		Global.selectedCharacter = selectedCharacter;
		PlayerPrefs.SetInt("Character", selectedCharacter);

		Back();
	}

	public void SelectCharacter(int x) {

		CharacterImgs[selectedCharacter].color = white;
		CharacterImgs[x].color = green;
		selectedCharacter = x;

		int characterState = -1;

		if (x > 0)
			characterState = PlayerPrefs.GetInt("Character" + x.ToString(), 5);

		if (characterState  == -1) {
			ApplyBtn.gameObject.SetActive(true);
			BuyBtn.SetActive(false);
			ApplyBtn.text = language_Manager.GetTextByValue("SelectTxt");
		}
		else if (characterState > 0) {
			ApplyBtn.gameObject.SetActive(true);
			ApplyBtn.text =  language_Manager.GetTextByValue("TrialTxt");
			BuyBtn.SetActive(true);
		}
		else {
			ApplyBtn.gameObject.SetActive(false);
			BuyBtn.SetActive(true);
		}
	}

	public void refresh_next_star_text() {

		if (Global.next_card_stars % 400 == 0)
			remaining_text.text = "" + Global.next_card_stars + "\nfor free GOLD card";
		else if (Global.next_card_stars % 100 == 0)
			remaining_text.text = "" + Global.next_card_stars + "\nfor free SILVER card";
		else
			remaining_text.text = "" + Global.next_card_stars + "\nfor free BASIC card";
	}

	public void accept_daily_window() {
		bonus_panel.SetActive (false);
	}


	public void load(int level) {
		loadingPanel.SetActive(true);
		Global.level = level;
		SceneManager.LoadScene("ingame");
	}

	public void card_click(int y) {

		if (delay == 0) {
			int row = y / 5;
			int column = y % 5;
			int x = (int)Random.Range (row * 5 + 0.0f, row * 5 + 4.9f);

			int i;
			for (i = 0; i < Global.own_cards.Length && Global.own_cards[i] != -1; i++)
				;

			if (i < Global.own_cards.Length) {
				Global.own_cards [i] = x;
                Global.card_remaining[i] = 5;
				PlayerPrefs.SetInt("Card_place" + i, x);
				PlayerPrefs.SetInt("Card_remaining" + i, 5);

				card_images [i+1].gameObject.SetActive (true);
				card_images [i+1].sprite = card_sprites [x];
				card_images [i+1].GetComponentsInChildren<Text>(true)[2].text =
					language_Manager.GetTextByValue("CardTitle" + Global.own_cards[i]);

				card_getables [row] [column + 1].sprite = card_sprites [x];
				Description.gameObject.SetActive(true);
				Description.text = language_Manager.GetTextByValue("CardTitle" + x);
				Description.rectTransform.position =
					new Vector3(card_getables [row] [column + 1].rectTransform.position.x + 2,
						card_getables [row] [column + 1].rectTransform.position.y - 102,
						0);

				delay = 200;

				if (Global.global_stars >= Global.next_card_stars) {
					Global.next_card_stars += 25;
					PlayerPrefs.SetInt("Next_card_stars", Global.next_card_stars);
					refresh_next_star_text();
				}
			}
		}
	}

    //activate / deactivate our own cards
	public void ownCard_click(int x) {

		RectTransform [] card_text = card_images [x+1].GetComponentsInChildren<RectTransform>(true);

		if (x == Global.ac) {
			Global.ac = -1;
			card_text [2].gameObject.SetActive (false);
		} else {
			if (Global.ac > -1) {
				RectTransform [] prev_card_text = card_images [Global.ac+1].GetComponentsInChildren<RectTransform>();
				prev_card_text[2].gameObject.SetActive(false);
			}

			Global.ac = x;
			card_text [2].gameObject.SetActive (true);

		}

		PlayerPrefs.SetInt("Card_active", Global.ac);
	}

	int findCardIndex(int x, int [] array) {
		for (int i=0; i<array.Length; i++) {
			if (array[i] == x)
				return i;
		}

		return -1;
	}

	public void request_drop_card(int x) {
		card_level = x;
		pop_up_dialog.SetActive (true);
	}

	public void accept_drop_card() {

		if (Global.global_points >= card_costs [card_level] && Global.Free_slot_exist()) {
			card_pack [card_level].SetActive (true);
			Global.global_points -= card_costs [card_level];
			point_txt.text = Global.global_points.ToString();

			PlayerPrefs.SetInt("Global_points", Global.global_points);
            back_btn.SetActive(false);
		} else {
			dialogOk.InvokeDialog(language_Manager.GetTextByValue("NotEnought"));
		}
		pop_up_dialog.SetActive (false);

	}

	public void denie_drop_card() {
		pop_up_dialog.SetActive (false);
	}


	public void Quit() {
		Application.Quit();
	}

	public void OK() {
		not_enought.SetActive (false);
	}

	public void InvokeQuit() {
		quitPanel.SetActive(true);
	}

	public void NoQuit() {
		quitPanel.SetActive(false);
	}

	public void RateThisApp() {
		Application.OpenURL ("market://details?id=com.JokrGames.PockRunner");
	}

	public void ClickGainForAd() {
		adManager.UserChoseToWatchAd();
	}

	public void GainForAd() {
		Global.global_points += 200;
		point_txt.text = Global.global_points.ToString();
	}

	public void Purchase() {
		purchaseManager.BuyCharacter(selectedCharacter-1);

		if (PlayerPrefs.GetInt("Character" + selectedCharacter.ToString(), 0) == -1) {
			ApplyBtn.gameObject.SetActive(true);
			BuyBtn.SetActive(false);
			ApplyBtn.text = language_Manager.GetTextByValue("SelectTxt");
			dialogOk.InvokeDialog(language_Manager.GetTextByValue("ThanksForPurchase"));
		}
	}

}
