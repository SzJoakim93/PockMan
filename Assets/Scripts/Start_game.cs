using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Start_game : MonoBehaviour {

	public Text point_txt;
	public Text star_text;
	
	public GameObject level_pack;
	public GameObject classic_pack;
	Button [] levels;
	Button [] classic_levels;

	public Text [] upgrade_levels;

	public GameObject bonus_panel;
	Text [] day_text;

	DateTime last_played;

	Color gold;

	int play_index;

	public Image locker;
	public Image locker_classic;
	public Text lock_cost;
	public Text lock_cost_classic;

	public GameObject not_enought;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < Global.own_cards.Length; i++)
			Global.own_cards [i] = -1;

		gold = new Color (1.0f, 0.82f, 0.22f, 1.0f);
		string [] args = Environment.GetCommandLineArgs ();
		
		/*if (args.Length > 1) {
			Global.level_path = args[1];
			Application.LoadLevel ("ingame");
		}*/

		day_text = bonus_panel.GetComponentsInChildren<Text> (true);

		levels = level_pack.GetComponentsInChildren<Button> (true);
		classic_levels = classic_pack.GetComponentsInChildren<Button> (true);

		last_played = DateTime.Today;
		play_index = -1;

		ReadUserData();

		int dif = (int)(DateTime.Now - last_played).TotalHours;

		if (play_index == -1) { //first time running
			Global.global_points += 100;
			day_text [0].color = gold;
			play_index = 0;
            bonus_panel.SetActive(true);
            PlayerPrefs.SetInt("Global_points", Global.global_points);

		} else {
			if (dif >= 24 && dif < 60 && Global.isPlayed) {
				play_index++;
				Global.isPlayed = false;
				PlayerPrefs.SetInt("Played_at_day", 0);
			}
			else if (dif >= 60 || (dif >= 24 && !Global.isPlayed))
				play_index = 0;
			
			if (dif >= 24) {
                bonus_panel.SetActive(true);
				if (play_index == 0) {
					Global.global_points += 100;
					day_text [0].color = gold;
				} else if (play_index == 1) {
					Global.global_points += 200;
					day_text [1].color = gold;
				} else if (play_index == 2) {
					Global.global_points += 400;
					day_text [2].color = gold;
				} else if (play_index == 3) {
					Global.global_points += 700;
					day_text [3].color = gold;
				} else {
					Global.global_points += 1000;
					day_text [4].color = gold;
				}

                PlayerPrefs.SetInt("Global_points", Global.global_points);
			}
		}


		PlayerPrefs.SetInt ("Play_index", play_index);
		PlayerPrefs.SetString("Last_played", DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss,fff"));

		point_txt.text = Global.global_points.ToString();
		star_text.text = Global.global_stars.ToString ();
	}
	
	void ReadUserData()
	{	
		Global.global_points = PlayerPrefs.GetInt("Global_points");
		Global.global_stars = PlayerPrefs.GetInt("Global_stars");

        if (PlayerPrefs.HasKey("Ammo_level"))
		    Global.level_ammo = PlayerPrefs.GetInt("Ammo_level");
        if (PlayerPrefs.HasKey("Clone_level"))
		    Global.level_clone = PlayerPrefs.GetInt("Clone_level");
        if (PlayerPrefs.HasKey("Convert_level"))
		    Global.level_convert = PlayerPrefs.GetInt("Convert_level");
        if (PlayerPrefs.HasKey("Double_level"))
		    Global.level_double = PlayerPrefs.GetInt("Double_level");
        if (PlayerPrefs.HasKey("Fruits_level"))
		    Global.level_fruits = PlayerPrefs.GetInt("Fruits_level");
        if (PlayerPrefs.HasKey("Magneton_level"))
		    Global.level_magneton = PlayerPrefs.GetInt("Magneton_level");
        if (PlayerPrefs.HasKey("Mine_level"))
		    Global.level_mines = PlayerPrefs.GetInt("Mine_level");
        if (PlayerPrefs.HasKey("Pause_level"))
		    Global.level_pause = PlayerPrefs.GetInt("Pause_level");
        if (PlayerPrefs.HasKey("Safe_level"))
		    Global.level_safe = PlayerPrefs.GetInt("Safe_level");
        if (PlayerPrefs.HasKey("Thunder_level"))
		    Global.level_thunder = PlayerPrefs.GetInt("Thunder_level");
			
        if (PlayerPrefs.HasKey("Unlocked_levels"))
			Global.unlocked_levels = PlayerPrefs.GetInt("Unlocked_levels");

        if (PlayerPrefs.HasKey("Unlocked_levels_classic"))
			Global.unlocked_clevels = PlayerPrefs.GetInt("Unlocked_levels_classic");

		upgrade_levels[0].text = Global.level_pause.ToString();
		upgrade_levels[1].text = Global.level_magneton.ToString();
		upgrade_levels[2].text = Global.level_thunder.ToString();
		upgrade_levels[3].text = Global.level_mines.ToString();
		upgrade_levels[4].text = Global.level_convert.ToString();
		upgrade_levels[5].text = Global.level_double.ToString();
		upgrade_levels[6].text = Global.level_safe.ToString();
		upgrade_levels[7].text = Global.level_fruits.ToString();
		upgrade_levels[8].text = Global.level_ammo.ToString();
		upgrade_levels[9].text = Global.level_clone.ToString();

        if (PlayerPrefs.HasKey("Last_played"))
			 last_played = DateTime.ParseExact(PlayerPrefs.GetString("Last_played") , "yyyy-MM-dd HH:mm:ss,fff",
			                                  System.Globalization.CultureInfo.InvariantCulture);

        if (PlayerPrefs.HasKey("Play_index"))
			play_index = PlayerPrefs.GetInt("Play_index");

        if (PlayerPrefs.GetInt("Played_at_day") == 1)
            Global.isPlayed = true;
			
	    for (int i = 1; i < Global.unlocked_levels && i < levels.Length; i++)
			levels[i].interactable = true;

		for (int i = 1; i < Global.unlocked_clevels && i < classic_levels.Length; i++)
			classic_levels[i].interactable = true;	

		if (Global.unlocked_levels % 5 == 0) {
			locker.gameObject.SetActive(true);
			locker.rectTransform.position = levels[Global.unlocked_levels].GetComponent<RawImage>().rectTransform.position;
		}

		if (Global.unlocked_clevels % 5 == 0) {
			locker_classic.gameObject.SetActive(true);
			locker_classic.rectTransform.position = classic_levels[Global.unlocked_clevels].GetComponent<RawImage>().rectTransform.position;
		}

		for (int i = 0; i < Global.own_cards.Length; i++)
			if (PlayerPrefs.HasKey("Card_place" + i)) {
				Global.own_cards[i] = PlayerPrefs.GetInt("Card_place" + i);
				Global.card_remaining[i] = PlayerPrefs.GetInt("Card_remaining" + i);
			}

        if (PlayerPrefs.HasKey("Next_card_stars"))
		    Global.next_card_stars = PlayerPrefs.GetInt("Next_card_stars");

        if (PlayerPrefs.HasKey("Card_active"))
		Global.ac = PlayerPrefs.GetInt("Card_active");

        if (PlayerPrefs.HasKey("Language"))
            Global.current_language = PlayerPrefs.GetString("Language");
        if (PlayerPrefs.HasKey("Music"))
        {
            int mus_index = PlayerPrefs.GetInt("Music");
            if (mus_index == 0)
                Global.music_enabled = false;
            else
                Global.music_enabled = true;
        }

	}

	public void unlock_level() {
		if (Global.global_points >= 100 * (Global.unlocked_levels / 5)) {
			levels [Global.unlocked_levels].interactable = true;
			Global.global_points -= 100 * (Global.unlocked_levels / 5);
			point_txt.text = Global.global_points.ToString();
			Global.unlocked_levels++;
			PlayerPrefs.SetInt ("Unlocked_levels", Global.unlocked_levels);
			PlayerPrefs.SetInt("Global_points", Global.global_points);
			locker.gameObject.SetActive(false);

		} else
			not_enought.SetActive (true);
	}

	public void unlock_level_classic() {
		if (Global.global_points >= 100 * (Global.unlocked_clevels / 5)) {
			levels [Global.unlocked_clevels].interactable = true;
			Global.global_points -= 100 * (Global.unlocked_clevels / 5);
			point_txt.text = Global.global_points.ToString();
			Global.unlocked_clevels++;
			PlayerPrefs.SetInt ("Unlocked_levels_classic", Global.unlocked_clevels);
			PlayerPrefs.SetInt("Global_points", Global.global_points);
			locker_classic.gameObject.SetActive(false);

		} else
			not_enought.SetActive (true);
	}
}
