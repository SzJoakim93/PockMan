using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class pac_movement : MonoBehaviour {

	public float speed;
	public float base_camera_speed;
    float camera_speed;

	public Transform camera;
	public Transform enemy_dead;
	public GameObject fire;
	public Transform mine;
	public Transform ammo;
	public GameObject double_score_signal;
	GameObject ammo_obj;

	public Text score_text;
	public Text life_text;
	public Text ready_to_go;
	//public Text game_over_text;
	public GameObject game_over_panel;
	public GameObject comp_panel;
	public Text comp_text;
	public Text ghost_combo_txt;
	public Text rate_text;


	public Text gain_points;
	public Text global_points;
	public Image star1;
	public Image star2;
	public Image star3;
	public Sprite active_star;

	public GameObject [] enemy;
	public Transform [] enemy_trans;
	enemy_movement [] enemy_scripts = new enemy_movement[Global.max_enemy];


	int ghost_combo;
	int ghost_combo_countdown;
	int rate_countdown;

	bool dead;

	Animator anim;

	//static int[,] levelmatrix = new int[500, 500];
	short req_direction;
	short pac_direction;
	short life=3;

	short mine_delay=0;

	short fire_shield = 0;

	int respawn_delay = 0;

    public Sound[] bg_music;

    public GameObject warn_panel;

	/*List<Transform> pickups;
	List<Transform> invertibility;*/

	// Use this for initialization
	void Start () {
		/*pickups = new List<Transform> ();
		invertibility = new List<Transform> ();*/

        camera_speed = base_camera_speed;
		req_direction = -1;
		pac_direction = 1;
		dead = false;
		Global.ready_to_go = 100;
		Global.enemy_active = 0;
		
		Global.score = 0;
		anim = gameObject.GetComponent<Animator> ();

		ghost_combo = 50;
		ghost_combo_countdown = 0;
		rate_countdown = -1;

		ammo_obj = ammo.gameObject;


		//ammo_obj.SetActive (false);

		//game_over_text.gameObject.SetActive (false);
		//comp_text.gameObject.SetActive (false);

		enemy_trans = new Transform [enemy.Length];

		for (int i = 0; i < Global.max_enemy; i++) {
			enemy_trans[i] = enemy[i].transform;
			//enemy [i].SetActive (false);
			enemy_scripts[i] = enemy[i].GetComponent<enemy_movement>();
		}

		Global.pause_game = false;


        //add dropping cards extras
		if (Global.ac > -1) {

			if (Global.own_cards [Global.ac] == 0)
				fire_shield = 50;
			else if (Global.own_cards [Global.ac] == 5)
				fire_shield = 100;
			else if (Global.own_cards [Global.ac] == 10)
				fire_shield = 150;

			if (Global.own_cards [Global.ac] == 1)
				camera_speed *= 0.9f;
			else if (Global.own_cards [Global.ac] == 6)
				camera_speed *= 0.85f;
			else if (Global.own_cards [Global.ac] == 11)
				camera_speed *= 0.8f;

			if (Global.own_cards [Global.ac] == 3)
				respawn_delay = 15;
			else if (Global.own_cards [Global.ac] == 8)
				respawn_delay = 20;
			else if (Global.own_cards [Global.ac] == 13)
				respawn_delay = 35;

			if (Global.own_cards [Global.ac] == 4)
				speed *= 1.1f;
			else if (Global.own_cards [Global.ac] == 9)
				speed *= 1.15f;
			else if (Global.own_cards [Global.ac] == 14)
				speed *= 1.2f;

		}

        if (Global.music_enabled)
        {
            foreach (Sound s in bg_music)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
                //s.source.outputAudioMixerGroup = mixerGroup;
            }

            bg_music[Global.level % 10].source.Play();
        }
			
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.ready_to_go == 0 && !Global.pause_game) {

			if (Input.GetKey (KeyCode.LeftArrow))
				req_direction = 0;
			else if (Input.GetKey (KeyCode.RightArrow))
				req_direction = 1;
			else if (Input.GetKey (KeyCode.UpArrow))
				req_direction = 2;
			else if (Input.GetKey (KeyCode.DownArrow))
				req_direction = 3;

			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				// Get movement of the finger since last frame
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				Debug.Log("Touched");
				if (touchDeltaPosition.x < -0.01)
					req_direction = 0;
				else if (touchDeltaPosition.x > 0.01)
					req_direction = 1;
				else if (touchDeltaPosition.y < -0.01)
					req_direction = 2;
				else if (touchDeltaPosition.y > 0.01)
					req_direction = 3;
			}


			int matrix_x = (int)(transform.position.x * 2);
			int matrix_y = (int)(transform.position.y * 2);

            //fix position when go out of playground
            if (matrix_x < 0)
            {
                for (int i = 0; Global.levelmatrix[matrix_y, i] == -1; i++ )
                    matrix_x = 0;
                transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
            }
            else if (matrix_x > 19)
            {
                matrix_x = 19;
                transform.position = new Vector3(10.5f, transform.position.y, transform.position.z);
            }

			int current_pos = Global.levelmatrix [matrix_y, matrix_x];


			if (req_direction != -1 && ((int)(transform.position.x * 20) % 10 == 0 && (int)(transform.position.y * 20) % 10 == 0 || pac_direction == 0 && req_direction == 1 || pac_direction == 1 && req_direction == 0 || pac_direction == 2 && req_direction == 3 || pac_direction == 3 && req_direction == 2) &&
				(current_pos == 0 && (req_direction == 0 || req_direction == 1) ||
				current_pos == 1 && (req_direction == 2 || req_direction == 3) ||
				current_pos == 2 && (req_direction == 0 || req_direction == 2) ||
				current_pos == 3 && (req_direction == 1 || req_direction == 2) ||
				current_pos == 4 && (req_direction == 1 || req_direction == 3) ||
				current_pos == 5 && (req_direction == 0 || req_direction == 3) ||
				current_pos == 6 && req_direction != 2 ||
				(current_pos == 7 || current_pos == 11) && req_direction != 3 ||
				current_pos == 8 && req_direction != 0 ||
				current_pos == 9 && req_direction != 1 ||
				current_pos == 10)) {


				if (req_direction == 0)
					transform.localEulerAngles = new Vector3 (0, 180, 0);
				else if (req_direction == 1)
					transform.localEulerAngles = new Vector3 (0, 0, 0);
				else if (req_direction == 2)
					transform.localEulerAngles = new Vector3 (0, 0, 90);
				else if (req_direction == 3)
					transform.localEulerAngles = new Vector3 (0, 180, 270);

				pac_direction = req_direction;
				req_direction = -1;

                //drop mines under turning
				if (Global.mines > 0 && mine_delay == 0) {
					Global.mines--;
					Instantiate(mine, transform.position, Quaternion.identity);
					mine_delay=100;
				}

                //managing the ammo bonus, shooting
				if (Global.ammo > 0 && !ammo_obj.activeInHierarchy) {
					float x = transform.position.x;
					float y = transform.position.y;
					int xi = (int)(x * 2);
					int yi = (int)(y * 2);
					bool isCollision = false;
					while (!isCollision && (int)(y * 2) > -1 && (int)(x * 2) > -1 && Global.levelmatrix [(int)(y * 2), (int)(x * 2)] != -1) {
						for (int i = 0; i < 5; i++)
							if (x > enemy_trans[i].position.x - 0.5 && x < enemy_trans[i].position.x + 0.5 && y > enemy_trans[i].position.y - 0.5 && y < enemy_trans[i].position.y + 0.5) {
								ammo_obj.SetActive(true);	
								ammo.position = transform.position;
								ammo.eulerAngles = transform.eulerAngles;
								Global.ammo--;
								isCollision = true;
								break;
							}

						if (pac_direction == 0)
							x--;
						else if (pac_direction == 1)
							x++;
						else if (pac_direction == 2)
							y++;
						else
							y--; 
					}
					
				}
			}

            //determine the player's visible area
			if (current_pos > 1) {
				int i = matrix_y, j = matrix_x;

				if (pac_direction == 0)
					for (j=matrix_x-1, i=matrix_y; j > 0 && Global.levelmatrix[i, j] < 2 ; j--);
				else if (pac_direction == 1)
					for (j=matrix_x+1, i=matrix_y; j < 20 && Global.levelmatrix[i, j] < 2; j++);
				else if (pac_direction == 2)
					for (j=matrix_x, i=matrix_y+1; i < Global.level_height && Global.levelmatrix[i, j] < 2; i++);
				else if (pac_direction == 3)
					for (j=matrix_x, i=matrix_y-1; i > 0 && Global.levelmatrix[i, j] < 2; i--);

				Global.pock_front.x = j * 0.5f;
				Global.pock_front.y = i * 0.5f;
			}

            //level completed
			if ( !Global.classic && matrix_x == Global.endcoord_x && matrix_y == Global.endcoord_y || Global.classic && Global.remaining < 1 ) {
				Global.ready_to_go = 200;
				comp_text.gameObject.SetActive(true);
			}
				

			/*if (pac_direction == 0 && transform.position.x > 2.0f)
			    camera.transform.Translate (-0.002f, 0, 0);
		    else if (pac_direction == 1 && transform.position.x < 8.0f)
			    camera.transform.Translate (0.002f, 0, 0);*/

            
            //vertical moving of camera
            if (!Global.classic)
            {
                float relative_pos = (transform.position.y - camera.position.y) / 15.0f;
                camera_speed = base_camera_speed + relative_pos;
                if (Global.rewind == 0 && Global.pause == 0)
                    camera.Translate(0, camera_speed * Time.deltaTime, 0);
                else if (Global.rewind != 0)
                    camera.Translate(0, -1.0f * Time.deltaTime, 0);
            }


			
            //spawn enemy
			if (Global.classic || matrix_y < Global.level_height - 10)
				for (int i = 0; i < Global.max_enemy; i++) {
					if (!enemy [i].active && Time.frameCount % ((150 + respawn_delay)*(Global.enemy_active+1)) == 0) {
						enemy [i].SetActive (true);
						Global.enemy_active++;
						//enemy_scripts [i].count_down = 50;
						enemy_scripts [i].respawn_enemy ();
						break;
					}
				}


            //decrease bonus time
			if (Global.inv_time > 0)
				Global.inv_time--;
			if (Global.pause > 0)
				Global.pause--;
			if (Global.pause_enemy > 0)
				Global.pause_enemy--;
			if (Global.rewind > 0)
				Global.rewind--;
			if (Global.double_score > 0) {
				if (Global.double_score == Global.max_double-1)
					double_score_signal.SetActive(true);
				if (Global.double_score == 1)
					double_score_signal.SetActive(false);

				Global.double_score--;
			}


            //player's moving
			if (!dead && (pac_direction == 0 && (current_pos != 3 && current_pos != 4 && current_pos != 8 && current_pos != 1 || (int)(transform.position.x * 40) % 20 != 0) ||
				pac_direction == 1 && (current_pos != 2 && current_pos != 5 && current_pos != 9 && current_pos != 1 || (int)(transform.position.x * 40) % 20 != 0) ||
			    pac_direction == 2 && (current_pos != 4 && current_pos != 5 && current_pos != 6 && current_pos != 0 && transform.position.y < camera.transform.position.y + Global.view_range_top || (int)(transform.position.y * 40) % 20 != 0) ||
				pac_direction == 3 && (current_pos != 2 && current_pos != 3 && current_pos != 7 && current_pos != 0 && current_pos != 11 || (int)(transform.position.y * 40) % 20 != 0))) {
				transform.Translate (speed*Time.deltaTime, 0, 0);

                //horizontal moving of camera
				if (transform.position.x > 2.8f && transform.position.x < 5.4f && transform.eulerAngles.z == 0) {
					if (transform.eulerAngles.y == 0)
						camera.Translate(speed*Time.deltaTime, 0, 0);
					else
						camera.Translate(-speed*Time.deltaTime, 0, 0);
				}

                //vertical moving of camera in classic mode
                if (Global.classic && transform.position.y > 2.0f && transform.position.y < Global.level_height - 5.4f && transform.eulerAngles.z != 0)
                {
                    if (transform.eulerAngles.y == 0)
                        camera.Translate(0, speed * Time.deltaTime, 0);
                    else
                        camera.Translate(0, -speed * Time.deltaTime, 0);
                }


			}

            //inside the fire
			if (!Global.classic && transform.position.y < camera.transform.position.y + Global.view_range_bottom && !dead) {

				if (fire_shield > 0)
					fire_shield--;
				else {
					anim.SetBool ("dead", true);
					dead = true;
				}
			}
			else if (fire_shield > -1 && fire_shield < 50)
				fire_shield++;


            //delay of dropping out mine
			if (mine_delay > 0)
				mine_delay--;


            //moving ammo
			if (ammo.gameObject.activeInHierarchy)
				ammo.Translate(2*speed*Time.deltaTime, 0, 0);


            //collision of enemy
			for (int i=0; i<Global.max_enemy; i++)
			if (enemy_scripts[i].isAlly == 0 && Global.pause_enemy == 0 && !dead && enemy[i].active && enemy_scripts[i].count_down == 0 && Vector2.Distance(transform.position, enemy_trans[i].position) < 0.25f) {
				
                //enemy kills the player
                if (Global.inv_time == 0) {
					anim.SetBool ("dead", true);
					dead = true;
					Global.pause_game = true;

                //invertibility enabled
				} else {

					enemy[i].SetActive (false);
					Global.enemy_active--;
					
					Global.score += ghost_combo;
					
                    //show total ghost combo
					if (ghost_combo == 250) {
						rate_text.text = "Total ghost combo!";
						rate_text.gameObject.SetActive(true);
						rate_countdown = 100;
					}
					
                    //set and show ghost combo title
					ghost_combo_countdown = 100;
					ghost_combo_txt.gameObject.SetActive(true);
					ghost_combo_txt.text = "+" + ghost_combo.ToString();

					Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
					new_dead.position = enemy_trans[i].position;
					new_dead.gameObject.SetActive (true);

                    ghost_combo += 50;
				}
			}

            //reset ghost combo at the end of invertibility
			if (Global.inv_time == 1)
				ghost_combo = 50;
			

            //delay appearance of ghost combo title
			if (ghost_combo_countdown > 0) {
				ghost_combo_countdown--;
				
				if (ghost_combo_countdown == 1)
					ghost_combo_txt.gameObject.SetActive(false);		
			}
        //end of in-game actions
		} else if (Global.ready_to_go > 0 && (Global.ready_to_go < 50 || Global.ready_to_go % 100 != 1)) {
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

	public void tap_left() {
		req_direction = 0;
	}

	public void tap_right() {
		req_direction = 1;
	}

	public void tap_up() {
		req_direction = 2;
	}

	public void tap_down() {
		req_direction = 3;
	}

	void respawn_player(float x, float y)
	{
		transform.position = new Vector2 (x, y);

		if (x < 2.8f)
			camera.position = new Vector3(2.8f, y, camera.position.z);
		else if (x > 5.2f)
			camera.position = new Vector3(5.2f, y, camera.position.z);
		else
			camera.position = new Vector3(x, y, camera.position.z);
		
		/*if (x>100 && x<550)
			camera_offset.x = 20-(x-100)/2;
		else if (x<101)
			camera_offset.x=20;
		else
			camera_offset.x=-205;*/
		
		int pos = Global.levelmatrix[(int)transform.position.y*2,(int)transform.position.x*2];
		if (pos == 0 || pos == 3 || pos == 7 || pos == 11)
			pac_direction = 1;
		else if  (pos == 1 || pos == 4 || pos == 5 || pos == 8 || pos == 9 || pos == 10)
			pac_direction = 2;
		else if (pos == 2)
			pac_direction = 0;

		if (pac_direction == 0)
			transform.localEulerAngles = new Vector3(0,180,0);
		else if (pac_direction == 1)
			transform.localEulerAngles = new Vector3(0,0,0);
		else if (pac_direction == 2)
			transform.localEulerAngles = new Vector3(0,0,90);
		else if (pac_direction == 3)
			transform.localEulerAngles = new Vector3(0,180,270);
	}

	void new_life()
	{	
		if (life > 0) {
			life--;
			life_text.text = "X " + life.ToString ();

			Global.pause_game = false;

			if (life == 0) {
				Global.ready_to_go = 300;
				//game_over_text.gameObject.SetActive(true);
				//game_over_panel.SetActive(true);
            }
            else if (!Global.classic && transform.position.y * 2 > Global.level_height - 10)
            {

                Global.ready_to_go = 103;
            }
            else
            {
                pac_direction = 1;

                int i, j = ((int)camera.transform.position.y) * 2, k;
                if (j < 0)
                    j = 0;

                //set_camera_y(camera_offset.y-240);
                if (Global.classic)
                    respawn_player(Global.startcoord_x / 2.0f, Global.startcoord_y / 2.0f);
                else
                {
                    for (k = 0; k < 17 && Global.levelmatrix[j, k] == -1; k++)
                        ;
                    respawn_player(k / 2.0f, j / 2.0f);
                }

                for (i = 0; i < Global.max_enemy; i++)
                    enemy[i].SetActive(false);
                Global.enemy_active = 0;
                /*if (enemy [i].gameObject.active)
                    enemy_scripts [i].respawn_enemy ();*/



                ready_to_go.gameObject.SetActive(true);
                Global.ready_to_go = 100;

                dead = false;
                anim.SetBool("dead", false);
            }
		}
	}

	void end_level() {

		Global.global_points += Global.score / 10;

		gain_points.text = "Points gained:\n" + (Global.score/10).ToString ();
		global_points.text = "All points:\n" + Global.global_points.ToString();

		int early_rate = 0;
		if (Global.classic && PlayerPrefs.HasKey ("Classic_level_star" + Global.level))
			early_rate = PlayerPrefs.GetInt ("Classic_level_star" + Global.level);
		else if (!Global.classic && PlayerPrefs.HasKey ("Level_star" + Global.level))
			early_rate = PlayerPrefs.GetInt ("Level_star" + Global.level);

		int rate = 0;
        if (!Global.classic && Global.score > Global.max_score - Global.max_score / 3 || Global.classic && Global.score > Global.max_score + Global.max_score * 2)
        {
			star1.sprite = active_star;
			rate = 1;
		}
        if (!Global.classic && Global.score > Global.max_score || Global.classic && Global.score > Global.max_score + Global.max_score * 4)
        {
			star2.sprite = active_star;
			rate = 2;
		}
        if (!Global.classic && Global.score > Global.max_score + Global.max_score / 3 || Global.classic && Global.score > Global.max_score + Global.max_score * 6)
        {
			star3.sprite = active_star;
			rate = 3;
		}


		PlayerPrefs.SetInt ("Global_points", Global.global_points);


        //decrease usage number of dropping card
		if (Global.ac > -1) {
			Global.card_remaining [Global.ac]--;
            PlayerPrefs.SetInt("Card_remaining" + Global.ac, Global.card_remaining[Global.ac]);

			if (Global.card_remaining [Global.ac] == 0) {
				Global.own_cards [Global.ac] = -1;
                PlayerPrefs.SetInt("Card_place" + Global.ac, -1);
				Global.ac = -1;
			}
		}

		if (rate > early_rate) {
			Global.global_stars += rate - early_rate;
			PlayerPrefs.SetInt("Global_stars", Global.global_stars);

			if (Global.classic)
				PlayerPrefs.SetInt ("Classic_level_star" + (Global.level-100), rate);
			else
				PlayerPrefs.SetInt ("Level_star" + Global.level, rate);
		}

		if (!Global.classic && Global.level == Global.unlocked_levels-1 && Global.unlocked_levels % 5 != 0) {
			Global.unlocked_levels++;
			PlayerPrefs.SetInt("Unlocked_levels", Global.unlocked_levels);
		} else if (Global.classic && Global.level-100 == Global.unlocked_clevels-1 && Global.unlocked_clevels % 5 != 0) {
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
        else
            comp_panel.SetActive(true);

	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "enemy_ammo") {
			anim.SetBool ("dead", true);
			dead = true;
			Global.pause_game = true;
		} else if (coll.gameObject.tag == "speed_zone") {
			speed = 2.0f;
		} else if (coll.gameObject.tag == "slow_zone") {
			speed = 1.0f;
		}

	}

	void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.tag == "speed_zone" || coll.gameObject.tag == "slow_zone") {
			speed = 1.5f;
		}
	}

}