using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class pac_movement : MonoBehaviour {

	public float speed;

	public Transform camera;
	public GameObject fire;
	public Transform mine;
	public Transform ammo;
	GameObject ammo_obj;

	public Text life_text;
	public Text ready_to_go;
	public GameObject game_over_panel;
	public Text ghost_combo_txt;
	public Text mine_text;
	public Text ammo_text;

	public int ghost_combo;
	public int ghost_combo_countdown;
	public bool dead;

	public Animator anim;

	short req_direction;
	short pac_direction;
	short life=2;

	short fire_shield = 0;
	short max_fire_shield = 0;

	[HideInInspector]
	public int back_x;
	[HideInInspector]
	public int back_y;
	[HideInInspector]
	public int front_x;
	[HideInInspector]
	public int front_y;
	int prev_pos_x;
	int prev_pos_y;
	bool nodeReached = true;

	// Use this for initialization
	void Start () {

		req_direction = -1;
		pac_direction = 1;
		dead = false;
		
		anim = gameObject.GetComponent<Animator> ();
		anim.SetInteger("direction", pac_direction);

		ghost_combo = 50;
		ghost_combo_countdown = 0;
		ammo_obj = ammo.gameObject;

        //add dropping cards extras
		if (Global.ac > -1) {

			if (Global.own_cards [Global.ac] == 0)
				max_fire_shield = 50;
			else if (Global.own_cards [Global.ac] == 5)
				max_fire_shield = 100;
			else if (Global.own_cards [Global.ac] == 10)
				max_fire_shield = 150;

			if (Global.own_cards [Global.ac] == 4)
				speed *= 1.1f;
			else if (Global.own_cards [Global.ac] == 9)
				speed *= 1.15f;
			else if (Global.own_cards [Global.ac] == 14)
				speed *= 1.2f;
		}

		mine_text.text = "X " + Global.mines.ToString();
		ammo_text.text = "X " + Global.ammo.ToString();

		prev_pos_x = -1;
		prev_pos_y = -1;

		setCamera();
			
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
			if (Global.levelmatrix [matrix_y, matrix_x] == -1)
            	fix_pos(matrix_x, matrix_y);

			int current_pos = Global.levelmatrix [matrix_y, matrix_x];

			//set player's direction
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


				/*if (req_direction == 0)
					transform.localEulerAngles = new Vector3 (0, 180, 0);
				else if (req_direction == 1)
					transform.localEulerAngles = new Vector3 (0, 0, 0);
				else if (req_direction == 2)
					transform.localEulerAngles = new Vector3 (0, 0, 90);
				else if (req_direction == 3)
					transform.localEulerAngles = new Vector3 (0, 180, 270);*/

				pac_direction = req_direction;
				anim.SetInteger("direction", req_direction);
				req_direction = -1;

				
			}

            //determine the player's visible area
			if (prev_pos_x != matrix_x || prev_pos_y != matrix_y) {
				if (nodeReached) {
					front_y = back_y = matrix_y;
					front_x = back_x = matrix_x;

					if (pac_direction == 0) {
						for (; front_x > 0 && Global.levelmatrix[matrix_y, front_x] < 2 ; front_x--);
						for (; back_x < 20 && Global.levelmatrix[matrix_y, back_x] < 2 ; back_x++);
					} else if (pac_direction == 1) {
						for (; back_x > 0 && Global.levelmatrix[matrix_y, back_x] < 2 ; back_x--);
						for (; front_x < 20 && Global.levelmatrix[matrix_y, front_x] < 2; front_x++);
					} else if (pac_direction == 2) {
						for (; front_y < Global.level_height && Global.levelmatrix[front_y, matrix_x] < 2; front_y++);
						for (; back_y > 0 && Global.levelmatrix[back_y, matrix_x] < 2; back_y--);
					} else if (pac_direction == 3) {
						for (; back_y < Global.level_height && Global.levelmatrix[back_y, matrix_x] < 2; back_y++);
						for (; front_y > 0 && Global.levelmatrix[front_y, matrix_x] < 2; front_y--);
					}

					if (current_pos < 2)
						nodeReached = false;
						
				} else {
					if (current_pos > 1) {
						nodeReached = true;
						front_y = back_y = matrix_y;
						front_x = back_x = matrix_x;
					}
						
				}
			}

			prev_pos_x = matrix_x;
			prev_pos_y = matrix_y;


            //player's moving
			if (!dead && (pac_direction == 0 && (current_pos != 3 && current_pos != 4 && current_pos != 8 && current_pos != 1 || (int)(transform.position.x * 40) % 20 != 0) ||
				pac_direction == 1 && (current_pos != 2 && current_pos != 5 && current_pos != 9 && current_pos != 1 || (int)(transform.position.x * 40) % 20 != 0) ||
			    pac_direction == 2 && (current_pos != 4 && current_pos != 5 && current_pos != 6 && current_pos != 0 && transform.position.y < camera.transform.position.y + Global.view_range_top || (int)(transform.position.y * 40) % 20 != 0) ||
				pac_direction == 3 && (current_pos != 2 && current_pos != 3 && current_pos != 7 && current_pos != 0 && current_pos != 11 || (int)(transform.position.y * 40) % 20 != 0))) {
				
				switch (pac_direction)
				{
					case 0:
						transform.Translate (-speed*Time.deltaTime, 0, 0);
						break;
					case 1:
						transform.Translate (speed*Time.deltaTime, 0, 0);
						break;
					case 2:
						transform.Translate (0, speed*Time.deltaTime, 0);
						break;
					default:
						transform.Translate (0, -speed*Time.deltaTime, 0);
						break;
				}
					


                //horizontal moving of camera
				if (transform.position.x > 2.8f && transform.position.x < 5.4f)
				{
					if (pac_direction == 1)
						camera.Translate(speed*Time.deltaTime, 0, 0);
					else if (pac_direction == 0)
						camera.Translate(-speed*Time.deltaTime, 0, 0);
				}
				 

                //vertical moving of camera in classic mode
                if (Global.classic && transform.position.y > 2.0f && transform.position.y < Global.level_height - 5.4f)
                {
                    if (pac_direction == 2)
                        camera.Translate(0, speed * Time.deltaTime, 0);
                    else if (pac_direction == 3)
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
			else if (fire_shield < max_fire_shield && fire_shield > -1)
				fire_shield++;

            //reset ghost combo at the end of invertibility
			if (Global.inv_time == 1)
				ghost_combo = 50;
			

            //delay appearance of ghost combo title
			if (ghost_combo_countdown > 0) {
				ghost_combo_countdown--;
				
				if (ghost_combo_countdown == 1)
					ghost_combo_txt.gameObject.SetActive(false);		
			}
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

		setCamera();
		
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
		anim.SetInteger("direction", pac_direction);

		/*if (pac_direction == 0)
			transform.localEulerAngles = new Vector3(0,180,0);
		else if (pac_direction == 1)
			transform.localEulerAngles = new Vector3(0,0,0);
		else if (pac_direction == 2)
			transform.localEulerAngles = new Vector3(0,0,90);
		else if (pac_direction == 3)
			transform.localEulerAngles = new Vector3(0,180,270);*/
	}

	void new_life()
	{	
		if (life > 0) {
			life--;
			life_text.text = "X " + life.ToString ();

			Global.pause_game = false;
			Global.enemy_rise = 3.5f;

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
				anim.SetInteger("direction", pac_direction);

                int i, j = ((int)camera.transform.position.y) * 2, k;
                if (j < 0)
                    j = 0;

                if (Global.classic)
                    respawn_player(Global.startcoord_x / 2.0f, Global.startcoord_y / 2.0f);
                else
                {
                    for (k = 0; k < 17 && Global.levelmatrix[j, k] == -1; k++)
                        ;
                    respawn_player(k / 2.0f, j / 2.0f);
                }

                ready_to_go.gameObject.SetActive(true);
                Global.ready_to_go = 100;

                dead = false;
                anim.SetBool("dead", false);

				foreach (var enemy in Global.enemies)
					Destroy(enemy);
				Global.enemies.Clear();
				if (Global.classic) {
					Global.followEnemyAlive = false;
					Global.blockenemyAlive = false;
				}
            }
		}
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

	void fix_pos(int x, int y) {
		int i, j;
		if (pac_direction == 0) {
			for (i = x, j = y; i < 20 && Global.levelmatrix[j, i] == -1; i++);
			transform.position = new Vector3(i/2.0f+0.1f, j/2.0f, 0);
		}	
		else if (pac_direction == 1) {
			for (i = x, j = y; i > 0 && Global.levelmatrix[j, i] == -1; i--);
			transform.position = new Vector3(i/2.0f-0.1f, j/2.0f, 0);
		}	
		else if (pac_direction == 2) {
			for (i = x, j = y; j > 0 && Global.levelmatrix[j, i] == -1; j--);
			transform.position = new Vector3(i/2.0f, j/2.0f-0.1f, 0);
		}
		else {
			for (i = x, j = y; j < Global.level_height && Global.levelmatrix[j, i] == -1; j++);
			transform.position = new Vector3(i/2.0f, j/2.0f+0.1f, 0);
		}
			
	}

	public void drop_mine() {
		//drop mines
		if (Global.mines > 0) {
			Global.mines--;
			mine_text.text = "X " + Global.mines.ToString();
			Instantiate(mine, transform.position, Quaternion.identity);
		}
	}

	public void shot_ammo() {
		if (Global.ammo > 0 && !ammo_obj.activeInHierarchy) {
			Global.ammo--;
			ammo_text.text = "X " + Global.ammo.ToString();
			ammo.position = transform.position;

			switch(pac_direction) {
				case 0:
					ammo.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
					break;
				case 1:
					ammo.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
					break;
				case 2:
					ammo.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
					break;
				case 3:
					ammo.eulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
					break;
			}

			ammo_obj.SetActive(true);
		}
	}

	void setCamera() {
		if (transform.position.x < 2.8f)
			camera.position = new Vector3(2.8f, transform.position.y, camera.position.z);
		else if (transform.position.x > 5.2f)
			camera.position = new Vector3(5.2f, transform.position.y, camera.position.z);
		else
			camera.position = new Vector3(transform.position.x, transform.position.y, camera.position.z);
	}

}