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
	public GameObject paying;
	public GameObject payingClassic;

	int deactiveLockers = 0;
	int deactiveLockersClassic = 0;

	// Use this for initialization
	void Start () {

		gold = new Color (1.0f, 0.82f, 0.22f, 1.0f);
		

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

		Global.isStarted = true;
	}

	void Update() {
		if (deactiveLockers > 0) {
			deactiveLockers++;
			if (deactiveLockers > 150) {
				deactiveLockers = 0;
				locker.gameObject.SetActive(false);
			}

		}

		if (deactiveLockersClassic > 0) {
			deactiveLockersClassic++;
			if (deactiveLockersClassic > 150) {
				deactiveLockersClassic = 0;
				locker_classic.gameObject.SetActive(false);
			}

		}

	}
	
	void ReadUserData()
	{	
		Global.global_points = PlayerPrefs.GetInt("Global_points");
		Global.global_stars = PlayerPrefs.GetInt("Global_stars");

		if (!Global.isStarted) {
			Global.Ammo = new PowerUpPieceBased("Ammo", 1500, 3);
			Global.Mine = new PowerUpPieceBased("Mine", 1500, 3);
			Global.Thunder = new PowerUpPieceBased("Thunder", 1000, 2);
			Global.DiamondRush = new PowerUpPieceBased("DiamondRush", 500, 10);
			Global.ConvertEnemy = new PowerUpPieceBased("ConvertEnemy", 1000, 1);
			Global.SafeZone = new PowerUpPieceBased("SafeZone", 500, 50);
			Global.ClonePlayer = new PowerUpTimeBased("ClonePlayer", 500, 20.0f);
			Global.DoubleScore = new PowerUpTimeBased("DoubleScore", 500, 10.0f);
			Global.Magneton = new PowerUpTimeBased("Magneton", 500, 20.0f);
			Global.PauseEnemy = new PowerUpTimeBased("PauseEnemy", 500, 5.0f);
			Global.Invertibility = new PowerUpTimeBased("Invertibility", 0, 7.0f);
			Global.LevelPause = new PowerUpTimeBased("LevelPause", 0, 7.0f);
			Global.LevelRewind = new PowerUpTimeBased("LevelRewind", 0, 3.0f);

			Global.music_enabled = PlayerPrefs.GetInt("MusicEnabled", 0) == 0;
			Global.controll_type = PlayerPrefs.GetInt("ControlType", 0);

			if (PlayerPrefs.HasKey("Language"))
				Global.current_language = PlayerPrefs.GetString("Language", "ENG");
			else {
				if (Application.systemLanguage == SystemLanguage.Hungarian)
					Global.current_language = "HUN";
				else
					Global.current_language = "ENG";
			}
			
			Global.selectedCharacter = PlayerPrefs.GetInt("Character", 0);
		}

		Global.tutorial = PlayerPrefs.GetInt("Tutorial", 0);
			
		Global.unlocked_levels = PlayerPrefs.GetInt("Unlocked_levels", 1);
		Global.unlocked_clevels = PlayerPrefs.GetInt("Unlocked_levels_classic", 1);

		upgrade_levels[0].text = Global.PauseEnemy.Level.ToString();
		upgrade_levels[1].text = Global.Magneton.Level.ToString();
		upgrade_levels[2].text = Global.Thunder.Level.ToString();
		upgrade_levels[3].text = Global.Mine.Level.ToString();
		upgrade_levels[4].text = Global.ConvertEnemy.Level.ToString();
		upgrade_levels[5].text = Global.DoubleScore.Level.ToString();
		upgrade_levels[6].text = Global.SafeZone.Level.ToString();
		upgrade_levels[7].text = Global.DiamondRush.Level.ToString();
		upgrade_levels[8].text = Global.Ammo.Level.ToString();
		upgrade_levels[9].text = Global.ClonePlayer.Level.ToString();

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

		if (Global.unlocked_levels % 5 == 0 && Global.unlocked_levels != Global.max_level) {
			locker.gameObject.SetActive(true);
			locker.rectTransform.position = levels[Global.unlocked_levels].GetComponent<RawImage>().rectTransform.position;
			lock_cost.text = calcCostToUncock(Global.unlocked_levels).ToString();
		}

		if (Global.unlocked_clevels % 5 == 0 && Global.unlocked_clevels != Global.max_level) {
			locker_classic.gameObject.SetActive(true);
			locker_classic.rectTransform.position = classic_levels[Global.unlocked_clevels].GetComponent<RawImage>().rectTransform.position;
			lock_cost_classic.text = calcCostToUncock(Global.unlocked_clevels).ToString();
		}

		for (int i = 0; i < Global.own_cards.Length; i++) {
			Global.own_cards[i] = PlayerPrefs.GetInt("Card_place" + i, -1);
			Global.card_remaining[i] = PlayerPrefs.GetInt("Card_remaining" + i, 0);
		}

		Global.next_card_stars = PlayerPrefs.GetInt("Next_card_stars", 25);
		Global.ac = PlayerPrefs.GetInt("Card_active", -1);

	}

	public void unlock_level() {
		int costToUnclok = calcCostToUncock(Global.unlocked_levels);
		if (Global.global_points >= costToUnclok) {
			levels [Global.unlocked_levels].interactable = true;
			Global.global_points -= costToUnclok;
			point_txt.text = Global.global_points.ToString();
			Global.unlocked_levels++;
			PlayerPrefs.SetInt ("Unlocked_levels", Global.unlocked_levels);
			PlayerPrefs.SetInt("Global_points", Global.global_points);
			paying.SetActive(true);
			deactiveLockers = 1;

		} else
			not_enought.SetActive (true);
	}

	public void unlock_level_classic() {
		int costToUnclok = calcCostToUncock(Global.unlocked_clevels);
		if (Global.global_points >= costToUnclok) {
			classic_levels [Global.unlocked_clevels].interactable = true;
			Global.global_points -= costToUnclok;
			point_txt.text = Global.global_points.ToString();
			Global.unlocked_clevels++;
			PlayerPrefs.SetInt ("Unlocked_levels_classic", Global.unlocked_clevels);
			PlayerPrefs.SetInt("Global_points", Global.global_points);
			payingClassic.SetActive(true);
			deactiveLockersClassic = 1;

		} else
			not_enought.SetActive (true);
	}

	int calcCostToUncock(int unlockedLevels) {
		return 500 * (unlockedLevels / 5);
	}
}
