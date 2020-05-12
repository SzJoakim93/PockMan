using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
	public GameObject back_btn;
	public GameObject bonus_panel;
	public GameObject loadingPanel;

    Image[] card_images; //array of owned cards
	Image[][] card_getables; //matrix of getable cards
	public Sprite [] card_sprites; //sprites of card types

	public GameObject not_enought;
	public GameObject pop_up_dialog;

	public GameObject [] card_pack; //dropable cards array containing three elements (gold, silver, basic card pack)
	public GameObject own_card_pack; //pack of owned cards to get array

	public Text remaining_text;
	public Text point_txt;
	Scroll_fixer levelMenuS;
	Scroll_fixer classicMenuS;

	int delay = 0;

	int card_level=0;

	int [] card_costs = new int[] {3000, 10000, 15000};

	bool first_running_drop = true;

	// Use this for initialization
	void Start(){

		card_getables = new Image[3] [];

		for (int i = 0; i < 3; i ++)
			card_getables[i] = card_pack[i].GetComponentsInChildren<Image> (true);
		card_images = own_card_pack.GetComponentsInChildren<Image> (true);


		levelMenuS = LevelMenu.GetComponent<Scroll_fixer>();
		classicMenuS = ClassicMenu.GetComponent<Scroll_fixer>();

        //switch to the specified level menu
		if (Global.level_menu == 1) {
			isStart ();
			if (Global.level % 30 == 0)
				levelMenuS.rightClick();

		}
		else if (Global.level_menu == 2) {
			isStart_classic ();
			if (Global.level % 30 == 10)
				classicMenuS.rightClick();
		}
		else if (Global.level_menu == 3 || Global.level_menu == 4) {

            isDropCard();

			if (Global.next_card_stars % 400 == 0) //dropping gold card
				card_pack[2].SetActive(true);
			else if (Global.next_card_stars % 100 == 0) //dropping silver card
				card_pack[1].SetActive(true);
			else //dropping basic card
				card_pack[0].SetActive(true);
		}

		Button [] upgrade_buttons = Upgrade_menu.GetComponentsInChildren<Button>(true);


        //unlock upgradable items depending on game completion
		if (Global.unlocked_levels < 6 && Global.unlocked_clevels < 6) {
			upgrade_buttons [2].interactable = false;
			upgrade_buttons [3].interactable = false;
		}

		if (Global.unlocked_levels < 11 && Global.unlocked_clevels < 11)
			upgrade_buttons [4].interactable = false;

		if (Global.unlocked_levels < 21 && Global.unlocked_clevels < 21)
			upgrade_buttons [5].interactable = false;

		if (Global.unlocked_levels < 31 && Global.unlocked_clevels < 31)
			upgrade_buttons [6].interactable = false;

		if (Global.unlocked_levels < 41 && Global.unlocked_clevels < 41)
			upgrade_buttons [7].interactable = false;

		if (Global.unlocked_levels < 61 && Global.unlocked_clevels < 61)
			upgrade_buttons [8].interactable = false;

		if (Global.unlocked_levels < 71 && Global.unlocked_clevels < 71)
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
	public void isStart() {
		MainMenu.SetActive (false);
		LevelMenu.SetActive (true);
		back_btn.SetActive (true);
	}

    //back button clicked
	public void isBack() {

        if (Global.level_menu == 4)
            Application.LoadLevel("ingame");
		MainMenu.SetActive (true);
		LevelMenu.SetActive (false);
		ClassicMenu.SetActive (false);
		Upgrade_menu.SetActive (false);
		Dropping_menu.SetActive (false);
        Settings_Menu.SetActive(false);
		back_btn.SetActive (false);

        Global.level_menu = 0;
	}

    //classic mode clicked
	public void isStart_classic() {
		MainMenu.SetActive (false);
		ClassicMenu.SetActive (true);
		back_btn.SetActive (true);
	}

    //upgrade button clicked
	public void isUpgrade() {
		MainMenu.SetActive (false);
		Upgrade_menu.SetActive (true);
		//back_btn.SetActive (true);
		back_btn.SetActive (true);
	}

    //dropping card clicked
	public void isDropCard() {
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
					if (i == Global.ac) //set the activated card's text
						card_text[1].gameObject.SetActive(true);
				}

			refresh_next_star_text();

			first_running_drop = false;
		}
	}

    //cliclking setting menu
    public void settings()
    {
        MainMenu.SetActive(false);
        Settings_Menu.SetActive(true);
        back_btn.SetActive(true);
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
		Application.LoadLevel("ingame");
	}

	public void card_click(int y) {

		if (delay == 0) {
			int row = y / 5;
			int column = y % 5;
			int x = (int)Random.Range (row * 5 + 0.0f, row * 5 + 4.9f);

			int i;
			for (i = 0; i < 10 && Global.own_cards[i] != -1; i++)
				;

			if (i < 10) {
				Global.own_cards [i] = x;
                Global.card_remaining[i] = 5;
				PlayerPrefs.SetInt("Card_place" + i, x);
				PlayerPrefs.SetInt("Card_remaining" + i, 5);

				card_images [i+1].gameObject.SetActive (true);
				card_images [i+1].sprite = card_sprites [x];

				card_getables [row] [column + 1].sprite = card_sprites [x];

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
		} else
			not_enought.SetActive (true);
		pop_up_dialog.SetActive (false);

	}

	public void denie_drop_card() {
		pop_up_dialog.SetActive (false);
	}


	public void isQuit() {
			Application.Quit();
	}

	public void isOK() {
		not_enought.SetActive (false);
	}

}
