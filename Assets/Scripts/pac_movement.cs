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

	public int ghost_combo;
	public int ghost_combo_countdown;
	public int rate_countdown;

	public bool dead;

	public Animator anim;

	//static int[,] levelmatrix = new int[500, 500];
	short req_direction;
	short pac_direction;
	short life=3;

	short mine_delay=0;

	short fire_shield = 0;

	/*List<Transform> pickups;
	List<Transform> invertibility;*/

	// Use this for initialization
	void Start () {
		/*pickups = new List<Transform> ();
		invertibility = new List<Transform> ();*/

		req_direction = -1;
		pac_direction = 1;
		dead = false;
		
		anim = gameObject.GetComponent<Animator> ();

		ghost_combo = 50;
		ghost_combo_countdown = 0;
		rate_countdown = -1;

		ammo_obj = ammo.gameObject;

		/*enemy_trans = new Transform [enemy.Length];

		for (int i = 0; i < Global.max_enemy; i++) {
			enemy_trans[i] = enemy[i].transform;
			//enemy [i].SetActive (false);
			enemy_scripts[i] = enemy[i].GetComponent<enemy_movement>();
		}*/


        //add dropping cards extras
		if (Global.ac > -1) {

			if (Global.own_cards [Global.ac] == 0)
				fire_shield = 50;
			else if (Global.own_cards [Global.ac] == 5)
				fire_shield = 100;
			else if (Global.own_cards [Global.ac] == 10)
				fire_shield = 150;

			if (Global.own_cards [Global.ac] == 4)
				speed *= 1.1f;
			else if (Global.own_cards [Global.ac] == 9)
				speed *= 1.15f;
			else if (Global.own_cards [Global.ac] == 14)
				speed *= 1.2f;
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

                //drop mines under turning
				if (Global.mines > 0 && mine_delay == 0) {
					Global.mines--;
					Instantiate(mine, transform.position, Quaternion.identity);
					mine_delay=100;
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
			else if (fire_shield > -1 && fire_shield < 50)
				fire_shield++;


            //delay of dropping out mine
			if (mine_delay > 0)
				mine_delay--;


            //moving ammo
			if (ammo.gameObject.activeInHierarchy)
				ammo.Translate(2*speed*Time.deltaTime, 0, 0);

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

                /*for (i = 0; i < Global.max_enemy; i++)
                    enemy[i].SetActive(false);
                Global.enemy_active = 0;*/

                ready_to_go.gameObject.SetActive(true);
                Global.ready_to_go = 100;

                dead = false;
                anim.SetBool("dead", false);

				foreach (var enemy in Global.enemies)
					Destroy(enemy);
				Global.enemies.Clear();
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

}