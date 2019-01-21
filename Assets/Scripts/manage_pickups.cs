﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Audio;

public class manage_pickups : MonoBehaviour {

	public Text score_text;
	public Text Combo_text;

	BoxCollider2D boxCollider;
	public GameObject [] enemy;
	public bool isEnemy;
	public short enemy_index;
	public Transform enemy_dead;
	public Transform clone_man;
	public Transform thunder;
	public Transform dropper;
	
	public Text rate_text;

	public Sound [] sounds;
		
	int rate_count_down; //determine the rating text's appearance time

	int score_count_down; //countdown if you don't pick up any item
	int combo_count; //counting the points that picked up from the beginning of new combo
	int combo; //combo points that adding to the scores

	float card_bonus = 1.0f;

	enemy_movement [] enemy_scripts;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		enemy_scripts = new enemy_movement[enemy.Length];
		for (int i = 0; i < 5; i++)
			enemy_scripts[i] = enemy[i].GetComponent<enemy_movement>();
		combo_count = 0;
		combo = 0;

		rate_count_down = 0;

		if (Global.ac > -1) {
			if (Global.own_cards [Global.ac] == 2)
				card_bonus = 1.2f;
			else if (Global.own_cards [Global.ac] == 7)
				card_bonus = 1.3f;
			else if (Global.own_cards [Global.ac] == 12)
				card_bonus = 1.4f;
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.volume = s.volume;
			s.source.loop = s.loop;
			//s.source.outputAudioMixerGroup = mixerGroup;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.magneton > 0) {
			Global.magneton--;
			if (Global.magneton == 1)
				boxCollider.size = new Vector2 (0.2f, 0.2f);
		}

		if (score_count_down > 0) {
			score_count_down--;

            //End of combo points and adding to the scores
			if (score_count_down == 1) {

				if (combo >= 250)
					activate_rate_text("Marvelous combo\n" + combo);
				else if (combo >= 200)
					activate_rate_text("Wonderful combo\n" + combo);
				else if (combo >= 150)
					activate_rate_text("Perfect combo\n" + combo);
				else if (combo >= 100)
					activate_rate_text("Nice combo\n" + combo);
				else if (combo >= 50)
					activate_rate_text("Good combo\n" + combo);

				Global.score += combo;
				score_text.text = Global.score.ToString ();
				combo_count = 0;
				combo = 0;
				Combo_text.text = combo.ToString ();
			}
		}
		
		//hide rate text
		if (rate_count_down > 0) {
			rate_count_down--;
			
			if (rate_count_down == 1)
				rate_text.gameObject.SetActive(false);		
		}
	}

    //appearing rate text
	void activate_rate_text(string title) {
		rate_text.text = title;
		rate_text.gameObject.SetActive(true);
		rate_count_down = 100;
	}

	void add_point(int value) {
		if (Global.double_score != 0) {
			Global.score += value*2;
			combo_count += value*2;
		} else {
			Global.score += value;
			combo_count += value;
		}

		/*combo += 25;
		Combo_text.text = combo.ToString ();*/


		score_text.text = Global.score.ToString ();
		score_count_down = 40;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		 if (coll.gameObject.tag == "pickup") {

			Destroy (coll.gameObject);
			if (Global.double_score != 0) {
				Global.score += 2;
				combo_count += 2;
			}
			else {
				Global.score++;
				combo_count++;
			}

			if (combo_count % 10 == 0) {

                if (combo < 30)
                    combo += 10;
                else if (combo < 75)
                    combo += 15;
                else if (combo < 200)
                    combo += 25;
                else
                    combo += 50;
				Combo_text.text = combo.ToString();
			}

			score_text.text = Global.score.ToString ();
			Global.remaining--;
			score_count_down = 40;
			sounds[0].source.Play();

		} else if (coll.gameObject.tag == "invertibility") {
			Destroy (coll.gameObject);
			add_point(10);
			Global.inv_time = (int)(500.0f * card_bonus);
            sounds[1].source.Play();

		} else if (coll.gameObject.tag == "bananna") {
			Destroy (coll.gameObject);
			add_point(100);
            sounds[2].source.Play();

		} else if (coll.gameObject.tag == "pineapple") {
			Destroy (coll.gameObject);
			add_point(100);
            sounds[2].source.Play();

		} else if (coll.gameObject.tag == "melone") {
			Destroy (coll.gameObject);
			add_point(100);
            sounds[2].source.Play();

		} else if (coll.gameObject.tag == "apple") {
			Destroy (coll.gameObject);
			add_point(100);
            sounds[2].source.Play();

		} else if (coll.gameObject.tag == "rewind") {
			Destroy (coll.gameObject);
			Global.rewind = 200;
            score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "pause") {
			Destroy (coll.gameObject);
			Global.pause = (int)(Global.max_pause * card_bonus);
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "pause_enemy") {
			Destroy (coll.gameObject);
			Global.pause_enemy = (int)(300.0f * card_bonus);
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "double_score") {
			Destroy (coll.gameObject);
			Global.double_score = (int)(Global.max_double * card_bonus);
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "magneton") {
			Destroy (coll.gameObject);
			Global.magneton = (int)(Global.max_magneton * card_bonus);
			score_count_down = 40;
			boxCollider.size = new Vector2 (2.0f, 2.0f);
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "mine") {
			Destroy (coll.gameObject);
			Global.mines = Global.max_mines;
			score_count_down = 40;
            sounds[4].source.Play();
		} else if (coll.gameObject.tag == "ammo") {
			Destroy (coll.gameObject);
			Global.ammo = Global.max_ammo;
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "convert_enemy") {
			Destroy (coll.gameObject);
			for (int i = 0; i < Global.max_convert; i++)
				enemy_scripts[i].isAlly = 1000;
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "clone") {
			Global.clone_reamining = (int)(Global.max_clone * card_bonus);
			Destroy (coll.gameObject);
			clone_man.gameObject.SetActive (true);
			clone_man.position = transform.parent.position;
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "extend_points") {
			Global.dropping_mode = 0;
			Destroy (coll.gameObject);
			dropper.position = new Vector2(8.0f, Mathf.Round(transform.parent.position.y) + 4.0f);
			dropper.gameObject.SetActive(true);
			score_count_down = 40;
            sounds[4].source.Play();

		} else if (coll.gameObject.tag == "extend_fruits") {
			Global.dropping_mode = 1;
			Destroy (coll.gameObject);
			dropper.position = new Vector2(8.0f, Mathf.Round(transform.parent.position.y) + 10.0f);
			dropper.gameObject.SetActive(true);
			score_count_down = 40;
            sounds[4].source.Play();
			
		} else if (coll.gameObject.tag == "safety_road") {
			Global.dropping_mode = 2;
			Destroy (coll.gameObject);
			Global.safe_counter = (int)(Global.max_safe * card_bonus);
			dropper.position = new Vector2( Mathf.Round(transform.parent.position.x), Mathf.Round(transform.parent.position.y));
			dropper.gameObject.SetActive(true);
			score_count_down = 40;
            Global.safety_coords.Clear();
            sounds[4].source.Play();
			
		} else if (coll.gameObject.tag == "thunder") {
			Destroy (coll.gameObject);
            sounds[3].source.Play();

			short [] index = new short[Global.max_thunder];

			for (short i = 0; i< Global.max_thunder && i < Global.enemy_active; i++) {

				float distance = 999.0f;
				for (short j = 0; j < 5; j++) {
					float curr_distance = Vector3.Distance (transform.position, enemy[j].transform.position);
					if ((i == 0 || j != index[i-1]) && curr_distance < distance && enemy[j].activeInHierarchy) {
						distance = curr_distance;
						index[i] = j;
					}
				}

				enemy[index[i]].gameObject.SetActive (false);

				Transform new_thunder = (Transform)Instantiate(thunder, transform.position, Quaternion.identity);
				new_thunder.position = enemy[index[i]].transform.position;
				new_thunder.gameObject.SetActive (true);
				Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
				new_dead.position = enemy[index[i]].transform.position;
				new_dead.gameObject.SetActive (true);

				Global.enemy_active--;
				Global.score += 100;
				score_count_down = 40;

			}

			
		} else if (isEnemy && coll.gameObject.tag == "enemy" && coll.gameObject != transform.parent.gameObject && (enemy_index == -1 || enemy_scripts[enemy_index].isAlly > 0)) {
				coll.gameObject.SetActive (false);
				Global.enemy_active--;
				Global.score += 10;
				
				Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
				new_dead.position = coll.transform.position;
				new_dead.gameObject.SetActive (true);
                sounds[3].source.Play();
		} 
	
	}
}
