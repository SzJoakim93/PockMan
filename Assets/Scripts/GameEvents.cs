using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameEvents : MonoBehaviour {

	AudioSource bg_music;
	float respawn_delay = 1.0f;

	public float base_camera_speed;
    float camera_speed;
	float camera_speed_decreaser;
	float nextEnemyTime;

	public Transform pac_man;
	public GameObject comp_panel;
	public PopupText CompleteTxt;
	public Button nextButton;

	public GameObject [] enemy_pack;
	enemy_movement [] enemy;

	public GameObject double_score_signal;
	public GameObject warn_panel;
	public Text ready_to_go;
	public GameObject game_over_panel;
	public InGameTutorials Tutorials;
	public Sprite [] Sprites;
	public Image ExtraCardSignal;
	static LinkedList<int> soundTracks = new LinkedList<int>();

	// Use this for initialization
	void Start () {

		Global.enemies = new List<enemy_movement>();

		camera_speed_decreaser = 1.0f;
		camera_speed = base_camera_speed;
		Global.enemy_active = 0;
		
		Global.score = 0;

		
		Global.pause_game = true;

		if (Global.classic)
			Global.enemy_rise = 5.5f - Global.level * 0.1f;
		else
			Global.enemy_rise = 3.0f;

		Global.followEnemyAlive = false;
		Global.blockenemyAlive = false;

		nextEnemyTime = 1.0f;

		enemy = enemy_pack[Global.enemy_animation_offset].GetComponentsInChildren<enemy_movement> (true);

		if (Global.ac > -1) {
			if (Global.own_cards [Global.ac] == 1)
				camera_speed_decreaser = 0.9f;
			else if (Global.own_cards [Global.ac] == 6)
				camera_speed_decreaser = 0.85f;
			else if (Global.own_cards [Global.ac] == 11)
				camera_speed_decreaser = 0.8f;

			if (Global.own_cards [Global.ac] == 3)
				respawn_delay = 1.2f;
			else if (Global.own_cards [Global.ac] == 8)
				respawn_delay = 1.3f;
			else if (Global.own_cards [Global.ac] == 13)
				respawn_delay = 1.5f;

			//invoke card signal
			ExtraCardSignal.gameObject.SetActive(true);
			ExtraCardSignal.sprite = Sprites[Global.own_cards [Global.ac]];
		}

		Global.Invertibility.Reset();
		Global.PauseEnemy.Reset();
		Global.Magneton.Reset();
		Global.DoubleScore.Reset();
		Global.ClonePlayer.Reset();
		Global.LevelPause.Reset();
		Global.LevelRewind.Reset();

		if (Global.music_enabled)
        {
			if (soundTracks.Count == 0) {
				for (int i = 0; i < 10; i++)
					soundTracks.AddFirst(i);
			}
			bg_music = pac_man.GetComponent<AudioSource>();
			
			float x = Random.Range(0.0f, soundTracks.Count);
			int soundTrack = getTrackByIndex((int)x);
			soundTracks.Remove(soundTrack);
			switch (soundTrack)
			{
				case 0:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Sunset on the Bay (Electronic, Synthwave)");
					break;
				case 1:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Another World");
					break;
				case 2:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Cant Stop Me");
					break;
				case 3:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Zen Puzzle (World, Puzzle)");
					break;
				case 4:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Fatality Racer");
					break;
				case 5:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Solar Eclipse (Electronic, Space)");
					break;
				case 6:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Hard Rock");
					break;
				case 7:
					bg_music.clip = Resources.Load<AudioClip>("Loops/In a Rush (Rock, Action)");
					break;
				case 8:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Let's Rock");
					break;
				case 9:
					bg_music.clip = Resources.Load<AudioClip>("Loops/Underwater (Electronic, Action)");
					break;
			}

			bg_music.volume = PlayerPrefs.GetFloat("Volume", 0.5f);
			bg_music.Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (!Global.pause_game) {
			//level completed
			if ( !Global.classic && (int)(pac_man.position.x * 2) == Global.endcoord_x && (int)(pac_man.position.y * 2) == Global.endcoord_y || Global.classic && Global.remaining < 1 ) {
				Global.pause_game = true;
				CompleteTxt.Activate(2.0f);
			}

			
			//vertical moving of camera
			if (!Global.classic)
			{
				float relative_pos = (pac_man.position.y - transform.position.y) / 10.0f;
				camera_speed = (base_camera_speed + relative_pos)*camera_speed_decreaser;
				if (!Global.LevelRewind.IsActive() && !Global.LevelPause.IsActive())
					transform.Translate(0, camera_speed * Time.deltaTime, 0);
				else if (Global.LevelRewind.IsActive())
					transform.Translate(0, -1.0f * Time.deltaTime, 0);
			}


			
			//spawn enemy
			if ((Global.classic && Global.enemies.Count < Global.max_enemy && Time.timeSinceLevelLoad > nextEnemyTime) ||
				((transform.position.y+10.0f)*2.0f < Global.level_height && Time.timeSinceLevelLoad > nextEnemyTime))  {

				enemy_movement new_enemy;

				if (Global.classic && !Global.followEnemyAlive && Global.enemies.Count > 1) {
					Global.followEnemyAlive = true;
					new_enemy = (enemy_movement)Instantiate(enemy[0], spawn_enemy(), Quaternion.identity);
				} else if (Global.classic && !Global.blockenemyAlive && Global.enemies.Count > 3) {
					Global.blockenemyAlive = true;
					new_enemy = (enemy_movement)Instantiate(enemy[1], spawn_enemy(), Quaternion.identity);
				} else
					new_enemy = (enemy_movement)Instantiate(enemy[(int)Random.Range(2.0f, 4.9f)], spawn_enemy(), Quaternion.identity);

				new_enemy.gameObject.SetActive(true);
				Global.enemies.Add(new_enemy);

				nextEnemyTime += Global.enemy_rise * respawn_delay;

				if (!Global.classic && Global.enemy_rise > 1.25f)
					Global.enemy_rise -= 0.05f;
					
			}

			if (Global.LevelRewind.IsEnd() || Global.LevelPause.IsEnd())
				nextEnemyTime += 3.0f;

			if (!double_score_signal.activeInHierarchy && Global.DoubleScore.IsActive() )
				double_score_signal.SetActive(true);
			else if (Global.DoubleScore.IsEnd()) {
				double_score_signal.SetActive(false);
			}

		}

		if (Global.pause_game && Time.timeSinceLevelLoad > nextEnemyTime)
			nextEnemyTime += Global.enemy_rise;
	}

	public void OnStartGame() {
		if ((Global.tutorial & 1) != 1) {
			if (Global.controll_type == 0) {
				Tutorials.invokeTutorial(0);
				Global.tutorial = Global.tutorial | 1;
			}
				
		} else
			Global.pause_game = false;
	}

	public void OnGameOver() {
		game_over_panel.SetActive(true);
	}

	public void OnCompleteLevel() {

		foreach (var enemy in Global.enemies)
			Destroy(enemy.gameObject);
		Global.enemies.Clear();
		if (Global.classic) {
			Global.followEnemyAlive = false;
			Global.blockenemyAlive = false;
		}

		Global.global_points += Global.score / 10;

		int early_rate = 0;
		if (Global.classic)
			early_rate = PlayerPrefs.GetInt ("Classic_level_star" + Global.level, 0);
		else if (!Global.classic)
			early_rate = PlayerPrefs.GetInt ("Level_star" + Global.level, 0);

		int rate = 0;
		int treshold1 = (Global.classic ? Global.max_score : Global.max_score - Global.max_score / 3);
		int treshold2 = (Global.classic ? Global.max_score + Global.max_score * 2 : Global.max_score);
		bool noDead = pac_man.GetComponent<pac_movement>().NoDead();
        
		if (Global.score > treshold1)
			rate = 1;
        if (Global.score > treshold2)
			rate = 2;
        if (noDead) {
			rate++;
			Global.global_points += 200;
		}

		PlayerPrefs.SetInt ("Global_points", Global.global_points);

        //decrease usage number of dropping card
		if (!Global.classic || (Global.ac != -1 && Global.own_cards[Global.ac] != 0 && Global.own_cards[Global.ac] != 5 && Global.own_cards[Global.ac] != 10 &&
			Global.own_cards[Global.ac] != 1 && Global.own_cards[Global.ac] != 6 && Global.own_cards[Global.ac] != 11))
		{
			if (Global.ac > -1) {
				Global.card_remaining [Global.ac]--;
				PlayerPrefs.SetInt("Card_remaining" + Global.ac, Global.card_remaining[Global.ac]);

				if (Global.card_remaining [Global.ac] == 0) {
					Global.own_cards [Global.ac] = -1;
					PlayerPrefs.SetInt("Card_place" + Global.ac, -1);
					Global.ac = -1;
					PlayerPrefs.SetInt("Card_active", -1);
				}
			}
		}

		if (rate > early_rate) {
			Global.global_stars += rate - early_rate;
			PlayerPrefs.SetInt("Global_stars", Global.global_stars);

			if (Global.classic)
				PlayerPrefs.SetInt ("Classic_level_star" + (Global.level), rate);
			else
				PlayerPrefs.SetInt ("Level_star" + Global.level, rate);
		}

		if (!Global.classic && Global.level == Global.unlocked_levels-1 && Global.unlocked_levels % 5 != 0) {
			Global.unlocked_levels++;
			PlayerPrefs.SetInt("Unlocked_levels", Global.unlocked_levels);
		} else if (Global.classic && Global.level == Global.unlocked_clevels-1 && Global.unlocked_clevels % 5 != 0) {
			Global.unlocked_clevels++;
			PlayerPrefs.SetInt("Unlocked_levels_classic", Global.unlocked_clevels);
		}

		if (!Global.isPlayed) {
			Global.isPlayed = true;
			PlayerPrefs.SetInt("Played_at_day", 1);
		}

        if (Global.global_stars >= Global.next_card_stars && !Global.Free_slot_exist())
        {
            //case when the storage of dropping cards is full
            int selected = (int)Random.Range(0.0f, 3.9f);

            if (Global.own_cards[selected] < 5) //basic card
            {
                if (Global.next_card_stars % 400 == 0) //dropping gold card
                    Global.card_remaining[selected] += 10;
                else if (Global.next_card_stars % 100 == 0) //dropping silver card
                    Global.card_remaining[selected] += 7;
                else //dropping basic card
                    Global.card_remaining[selected] += 5;
            }
            else if (Global.own_cards[selected] < 10) //silver card
            {
                if (Global.next_card_stars % 400 == 0) //dropping gold card
                    Global.card_remaining[selected] += 7;
                else if (Global.next_card_stars % 100 == 0) //dropping silver card
                    Global.card_remaining[selected] += 5;
                else //dropping basic card
                    Global.card_remaining[selected] += 3;
            }
            else //gold card card
            {
                if (Global.next_card_stars % 400 == 0) //dropping gold card
                    Global.card_remaining[selected] += 5;
                else if (Global.next_card_stars % 100 == 0) //dropping silver card
                    Global.card_remaining[selected] += 3;
                else //dropping basic card
                    Global.card_remaining[selected] += 2;
            }

            warn_panel.SetActive(true);
        }
        else {
			if (!comp_panel.activeInHierarchy) {
				comp_panel.SetActive(true);
				comp_panel.GetComponent<PointAnimator>().StartAnimation(rate, treshold1, treshold2, noDead);
			}
			
		}
	}

	public Vector2 spawn_enemy() {

		if (Global.classic) //respaw enemy to the pre-defined position in classic mode
			return new Vector2(Global.endcoord_x/2.0f, Global.endcoord_y/2.0f);
        //respawn enemy to the camera position
		else {
			int side = (Random.value < 0.5f ? 0 : 1);
			int j = (int)(transform.position.y+7.0f)*2;
            bool no_safe = false;
			int k, l, m;
            for (k=0, l=9, m=9; k % 2 == 0 && Global.levelmatrix[j,l] == -1 || k % 2 == 1 && Global.levelmatrix[j,m] == -1  || !no_safe; k++)
            {
                no_safe = true;
				if (Global.safety_coords != null)
	                foreach (var pos in Global.safety_coords)
	                {
	                    if (Mathf.Abs(pos.y * 2.0f - j) < 1.0 && Mathf.Abs(pos.x * 2.0f - k) < 1.0)
	                        no_safe = false;
	                }

				if (k % 2 == side)
					l++;
				else
					m--;
            }

            if (k % 2 == 0)
				return new Vector2(l/2.0f, j/2.0f);
			else
				return new Vector2(m/2.0f, j/2.0f);
		}
	}

	int getTrackByIndex(int x) {
		int i = 0;
		foreach (var s in soundTracks) {
			if (i == x)
				return s;
			i++;
		}

		return -1;
	}
}
