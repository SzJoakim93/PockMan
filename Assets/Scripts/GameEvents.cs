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
	public Text comp_text;
	public Button nextButton;

	public GameObject [] enemy_pack;
	GameObject [] enemy;

	public GameObject double_score_signal;
	public GameObject warn_panel;
	public Text ready_to_go;
	public GameObject game_over_panel;
	public InGameTutorials Tutorials;

	// Use this for initialization
	void Start () {

		Global.enemies = new List<GameObject>();

		camera_speed_decreaser = 1.0f;
		camera_speed = base_camera_speed;
		Global.ready_to_go = 100;
		Global.enemy_active = 0;
		
		Global.score = 0;

		
		Global.pause_game = false;

		if (Global.classic)
			Global.enemy_rise = 6.0f;
		else
			Global.enemy_rise = 3.5f;

		Global.followEnemyAlive = false;
		Global.blockenemyAlive = false;

		nextEnemyTime = 3.25f;

		enemy_movement [] enemy_temp;
		enemy_temp = enemy_pack[Global.enemy_animation_offset].GetComponentsInChildren<enemy_movement> (true);

		enemy = new GameObject[enemy_temp.Length];
		for (int i = 0; i < enemy_temp.Length; i++)
			enemy[i] = enemy_temp[i].gameObject;

		if (Global.ac > -1) {
			if (Global.own_cards [Global.ac] == 1)
				camera_speed_decreaser = 0.9f;
			else if (Global.own_cards [Global.ac] == 6)
				camera_speed_decreaser = 0.85f;
			else if (Global.own_cards [Global.ac] == 11)
				camera_speed_decreaser = 0.8f;

			if (Global.own_cards [Global.ac] == 3)
				respawn_delay = 0.5f;
			else if (Global.own_cards [Global.ac] == 8)
				respawn_delay = 0.3f;
			else if (Global.own_cards [Global.ac] == 13)
				respawn_delay = 0.1f;
		}



		if (Global.music_enabled)
        {
			bg_music = pac_man.GetComponent<AudioSource>();
			
			float x = Random.Range(0.0f, 10.0f);
			if (x >= 0.0f && x < 1.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Sunset on the Bay (Electronic, Synthwave)");
			else if (x >= 1.0f && x < 2.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Another World");
			else if (x >= 2.0f && x < 3.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Cant Stop Me");
			else if (x >= 3.0f && x < 4.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Zen Puzzle (World, Puzzle)");
			else if (x >= 4.0f && x < 5.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Fatality Racer");
			else if (x >= 5.0f && x < 6.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Solar Eclipse (Electronic, Space)");
			else if (x >= 6.0f && x < 7.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Hard Rock");
			else if (x >= 7.0f && x < 8.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/In a Rush (Rock, Action)");
			else if (x >= 8.0f && x < 9.0f)
				bg_music.clip = Resources.Load<AudioClip>("Loops/Let's Rock (ver.1)");
			else
				bg_music.clip = Resources.Load<AudioClip>("Loops/Underwater (Electronic, Action)");


			bg_music.Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.ready_to_go == 0 && !Global.pause_game) {
			//level completed
			if ( !Global.classic && (int)(pac_man.position.x * 2) == Global.endcoord_x && (int)(pac_man.position.y * 2) == Global.endcoord_y || Global.classic && Global.remaining < 1 ) {
				Global.ready_to_go = 200;
				comp_text.gameObject.SetActive(true);
			}

			
			//vertical moving of camera
			if (!Global.classic)
			{
				float relative_pos = (pac_man.position.y - transform.position.y) / 10.0f;
				camera_speed = (base_camera_speed + relative_pos)*camera_speed_decreaser;
				if (Global.rewind == 0 && Global.pause == 0)
					transform.Translate(0, camera_speed * Time.deltaTime, 0);
				else if (Global.rewind != 0)
					transform.Translate(0, -1.0f * Time.deltaTime, 0);
			}


			
			//spawn enemy
			if ((Global.classic && Global.enemies.Count < Global.max_enemy && Time.timeSinceLevelLoad > nextEnemyTime) ||
				((transform.position.y+10.0f)*2.0f < Global.level_height && Time.timeSinceLevelLoad > nextEnemyTime))  {

				GameObject new_enemy;

				if (Global.classic && !Global.followEnemyAlive) {
					Global.followEnemyAlive = true;
					new_enemy = (GameObject)Instantiate(enemy[0], spawn_enemy(), Quaternion.identity);
				} else if (Global.classic && !Global.blockenemyAlive) {
					Global.blockenemyAlive = true;
					new_enemy = (GameObject)Instantiate(enemy[1], spawn_enemy(), Quaternion.identity);
				} else
					new_enemy = (GameObject)Instantiate(enemy[(int)Random.Range(2.0f, 4.9f)], spawn_enemy(), Quaternion.identity);

				new_enemy.SetActive(true);
				Global.enemies.Add(new_enemy);

				nextEnemyTime += Global.enemy_rise;

				if (!Global.classic && Global.enemy_rise > 1.25f)
					Global.enemy_rise -= 0.05f * respawn_delay;
					
			}


			//decrease bonus time
			if (Global.inv_time > 0)
				Global.inv_time--;

			if (Global.pause > 0) {
				Global.pause--;
				if (Global.pause == 1)
					nextEnemyTime += 3.0f;
			}
				

			if (Global.pause_enemy > 0)
				Global.pause_enemy--;

			if (Global.rewind > 0) {
				Global.rewind--;
				if (Global.rewind == 1)
					nextEnemyTime += 3.0f;
			}
				

			if (Global.double_score > 0) {
				if (Global.double_score == Global.max_double-1)
					double_score_signal.SetActive(true);
				if (Global.double_score == 1)
					double_score_signal.SetActive(false);

				Global.double_score--;
			}
		} else if (Global.ready_to_go > 0 && (Global.ready_to_go < 50 || Global.ready_to_go % 100 != 1)) {
			Global.ready_to_go--;
			if (Global.ready_to_go == 1) {
				ready_to_go.gameObject.SetActive(false);
				if ((Global.tutorial & 1) != 1) {
					if (Global.controll_type == 0) {
						Tutorials.invokeTutorial(0);
						Global.tutorial = Global.tutorial | 1;
					}
						
				}
					
			}
			else if (Global.ready_to_go == 102)
				//Application.LoadLevel ("menu");
				end_level();
			else if (Global.ready_to_go == 202)
				//Application.LoadLevel ("menu");
				game_over_panel.SetActive(true);
		}

		if (Global.pause_game && Time.timeSinceLevelLoad > nextEnemyTime)
			nextEnemyTime += Global.enemy_rise;
	}

	void end_level() {

		foreach (var enemy in Global.enemies)
			Destroy(enemy);
		Global.enemies.Clear();
		if (Global.classic) {
			Global.followEnemyAlive = false;
			Global.blockenemyAlive = false;
		}

		Global.global_points += Global.score / 10;

		int early_rate = 0;
		if (Global.classic && PlayerPrefs.HasKey ("Classic_level_star" + Global.level))
			early_rate = PlayerPrefs.GetInt ("Classic_level_star" + Global.level);
		else if (!Global.classic && PlayerPrefs.HasKey ("Level_star" + Global.level))
			early_rate = PlayerPrefs.GetInt ("Level_star" + Global.level);

		int rate = 0;

        if (!Global.classic && Global.score > Global.max_score - Global.max_score / 3 || Global.classic && Global.score > Global.max_score + Global.max_score * 2)
			rate = 1;

        if (!Global.classic && Global.score > Global.max_score || Global.classic && Global.score > Global.max_score + Global.max_score * 4)
			rate = 2;

        if (!Global.classic && Global.score > Global.max_score + Global.max_score / 3 || Global.classic && Global.score > Global.max_score + Global.max_score * 6)
			rate = 3;


		PlayerPrefs.SetInt ("Global_points", Global.global_points);


        //decrease usage number of dropping card
		if (!Global.classic || (Global.ac != 0 && Global.ac != 5 && Global.ac != 10 &&
			Global.ac != 1 && Global.ac != 6 && Global.ac != 11))
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
				comp_panel.GetComponent<PointAnimator>().StartAnimation(rate,
				Global.max_score - Global.max_score / 3, Global.max_score, Global.max_score + Global.max_score / 3);
			}
			
		}


		if ((!Global.classic && Global.level == 39) || (!Global.classic && Global.level == 19))
				nextButton.interactable = false;

        //end of in-game actions
		if (Global.ready_to_go > 0 && (Global.ready_to_go < 50 || Global.ready_to_go % 100 != 1)) {
			Global.ready_to_go--;
			if (Global.ready_to_go == 1)
				ready_to_go.gameObject.SetActive(false);
			else if (Global.ready_to_go == 102)
				//Application.LoadLevel ("menu");
				end_level();
			else if (Global.ready_to_go == 202)
				//Application.LoadLevel ("menu");
				game_over_panel.SetActive(true);
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
            for (k=0, l=7, m=7; k % 2 == 0 && Global.levelmatrix[j,l] == -1 || k % 2 == 1 && Global.levelmatrix[j,m] == -1  || !no_safe; k++)
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
}
