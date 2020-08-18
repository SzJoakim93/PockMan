using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class enemy_movement : MonoBehaviour {

	int direction=0; //enemy's moving direction (0: <- , 1: -> , 2: ^ , 3: ˇ
	public static float speed = Global.enemy_speed; //determine enemy's speed from a global const
	int enemy_pos; //value of enemy's position on levelmatrix
	public int count_down; //use for enemy respawning and deactivating at the end of level

	public int enemy_type; //determine an AI sample

	public Transform camera; //camera position
	public Transform pock_man; //players's position
	public Transform enemy_dead; //a sample object to clne to the enemy psition in case of dead
	public PopupText GhostComboPopup;
	public Sprite [] sprites; //an arry containing all enym sprites

	SpriteRenderer current_sprite; //the enym's current sprite

	GameObject Collider; //the collider object that using for ally mode

	bool isAlly = false;
	float allyTime = -10.0f;
	int allyAnim;
	bool invertibility = false;
	short invTime = 0;
	short ally_sprite = 0; //offset to ally sprite in sprites array

	BoxCollider2D bc; //the collider that disabled in allymode to not kill the player

	public int animation_type;
	pac_movement pac_script;

	BalancedSearch searchAI;
	int matrix_x;
	int matrix_y;
	float roundedPosX;
	float roundedPosY;
	float eatTime = -10.0f;
	bool eating = false;
	bool isSpawned = false;
	float spawnTime;

	//bool selectable=true;

	// Use this for initialization
	void Start () {

		speed = Global.enemy_speed;

		current_sprite = this.GetComponent<SpriteRenderer> ();
		Transform [] temp = this.GetComponentsInChildren<Transform> (true);
		Collider = temp [1].gameObject;
		Collider.SetActive (false);

		bc = this.GetComponent<BoxCollider2D> ();

        if (!Global.classic)
			enemy_type = 0;

        Global.safety_coords = new System.Collections.Generic.List<Vector2>();
		pac_script = pock_man.GetComponent<pac_movement>();

		int enemy_pos = Global.levelmatrix[(int)transform.position.y*2,(int)transform.position.x*2];
		if (enemy_pos == 0 || enemy_pos == 3 || enemy_pos == 7 || enemy_pos == 11)
			direction = 1;
		else if  (enemy_pos == 1 || enemy_pos == 4 || enemy_pos == 5 || enemy_pos == 8 || enemy_pos == 9 || enemy_pos == 10)
			direction = 3;
		else if (enemy_pos == 2)
			direction = 0;
		setRotation();

		matrix_x = -1;
		matrix_y = -1;
		//enemy_pos = Global.levelmatrix [matrix_y, matrix_x];

		count_down = 200;
		spawnTime = Time.timeSinceLevelLoad;

		searchAI = new BalancedSearch();
	}
	
	// Update is called once per frame
	void Update () {

		if (!Global.PauseEnemy.IsActive() && !Global.pause_game && !eating) {
			if (isSpawned) {

				roundedPosX = Mathf.Round(transform.position.x * 2.0f) / 2.0f;
				roundedPosY = Mathf.Round(transform.position.y * 2.0f) / 2.0f;

				if ((roundedPosX * 2 != matrix_x || roundedPosY * 2 != matrix_y) &&
						((direction == 0 && transform.position.x < roundedPosX) ||
						(direction == 1 && transform.position.x > roundedPosX) ||
						(direction == 2 && transform.position.y > roundedPosY) ||
						(direction == 3 && transform.position.y < roundedPosY))) {

					//get level matrix coordinates
					matrix_x = Mathf.RoundToInt(roundedPosX * 2.0f);
					matrix_y = Mathf.RoundToInt(roundedPosY * 2.0f);

                    //get enemy pos from coordinates
					enemy_pos = Global.levelmatrix [matrix_y, matrix_x];

					//fix enemy position when go out of playground
					/*if (((enemy_pos == 2 || enemy_pos == 5 || enemy_pos == 9) && transform.position.x > roundedPosX) ||
						((enemy_pos == 3 || enemy_pos == 4 || enemy_pos == 8) && transform.position.x < roundedPosX) ||
						((enemy_pos == 4 || enemy_pos == 5 || enemy_pos == 6) && transform.position.y > roundedPosY) ||
						((enemy_pos == 2 || enemy_pos == 3 || enemy_pos == 7) && transform.position.y < roundedPosY)) {
							Debug.Log("" + transform.position.x + " " + roundedPosX + " " + transform.position.y + " " + roundedPosY);
							transform.position = new Vector3(roundedPosX, roundedPosY, 0);
					}*/

					if (enemy_pos > 1)
						transform.position = new Vector3(roundedPosX, roundedPosY, 0);

                    //fix enemy position when go out of playground
					if (enemy_pos == -1)
						fix_enemy_pos(matrix_x, matrix_y);

				    //fix direction
					if (enemy_pos == 0 && (direction == 2 || direction == 3)) {
						direction = 0;
						if (animation_type == 1)
							transform.localEulerAngles = new Vector3 (0, 0, 90);
					}
					else if (enemy_pos == 1 && (direction == 0 || direction == 1)) {
						direction = 2;
						if (animation_type == 1)
							transform.localEulerAngles = new Vector3 (0, 0, 0);
					}

                    //move back if the enemy go out of camera view
					if (transform.position.y > camera.transform.position.y + Global.view_range_top && enemy_pos != 0 && enemy_pos != 2 && enemy_pos != 3 && enemy_pos != 7 && enemy_pos != 11) {
						direction = 3;
						if (animation_type == 1)
							transform.localEulerAngles = new Vector3 (0, 0, 180);
					}
					else if (isAlly && transform.position.y < camera.transform.position.y + Global.view_range_bottom + 1.0f && enemy_pos != 0 && enemy_pos != 6 && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 11) {
						direction = 2;
						if (animation_type == 1)
							transform.localEulerAngles = new Vector3 (0, 0, 0);
					}
                    //determine enemy's moving direction
					else
					{
						if (enemy_pos > 1)
						{
							if (Global.classic && (
									enemy_type < 2 ||
									enemy_type == 2 && Vector3.Distance(transform.position, pock_man.position) > 2.85f ||
									enemy_type == 3 && Vector3.Distance(transform.position, pock_man.position) > 5.55f) ||
								(enemy_type == 5 && Vector3.Distance(transform.position, new Vector3(camera.position.x, camera.position.y+2.5f, camera.position.z)) > 3.55f))
							{
							
								switch (enemy_type) {
									case 0:
										searchAI.search(matrix_x, matrix_y,
										(Math.Abs(pac_script.back_x - pock_man.position.x * 2) < Math.Abs(pac_script.front_x - pock_man.position.x * 2) ? pac_script.back_x : pac_script.front_x),
										((Math.Abs(pac_script.back_y - pock_man.position.y * 2) < Math.Abs(pac_script.front_y - pock_man.position.y * 2) ? pac_script.back_y : pac_script.front_y)));
										break;
									case 1:
										searchAI.search(matrix_x, matrix_y, pac_script.front_x, pac_script.front_y);
										break;
									case 2:
										searchAI.search(matrix_x, matrix_y, pac_script.front_x, pac_script.front_y, 4);
										break;
									case 3:
										searchAI.search(matrix_x, matrix_y, 4, pac_script.front_y, 8);
										break;
									case 5:
										searchAI.search(matrix_x, matrix_y, (int)(camera.position.x * 2.0f), (int)(camera.position.y * 2.0f)+5, 5);
										break;
								}
								

								if (searchAI.NextWayX() > matrix_x)
									direction = 1;
								else if (searchAI.NextWayX() < matrix_x)
									direction = 0;
								else if (searchAI.NextWayY() > matrix_y)
									direction = 2;
								else if (searchAI.NextWayY() < matrix_y)
									direction = 3;
								else {
									if (direction < 2) {
										if ((int)(pock_man.position.y * 2.0f) > matrix_y)
											direction = 2;
										else if ((int)(pock_man.position.y * 2.0f) < matrix_y)
											direction = 3;
									}
									else {
										if ((int)(pock_man.position.x * 2.0f) > matrix_x)
											direction = 1;
										else if ((int)(pock_man.position.x * 2.0f) < matrix_x)
											direction = 0;
									}
								}

								setRotation();

							}
							else
								determine_direction ();
						}
					}

                    //change enemy's sprite
					if (!Global.Invertibility.IsActive()) {
						if (animation_type == 0)
							current_sprite.sprite = sprites [direction+ally_sprite];
					}

					//deactivate enemy at top of level
					if (!Global.classic && matrix_y > Global.level_height - 8) {
						Global.enemies.Remove(this);
						Destroy(gameObject);
                    }
				}

				if (Global.controll_type == 0 && !isAlly && !Global.Invertibility.IsActive() &&
					Vector3.Distance(transform.position, pock_man.position) < 2.0f)
					if (matrix_y == (int)(pock_man.position.y * 2.0f)) {
						if ((pac_script.getDirection == 0 && transform.position.x > pock_man.position.x) ||
							(pac_script.getDirection == 1 && transform.position.x < pock_man.position.x)) {
							pac_script.setBehindEnemy = true;
							Debug.Log("Enemy behind horizontal");
						}
					} else if (matrix_x == (int)(pock_man.position.x * 2.0f)) {
						if ((pac_script.getDirection == 2 && transform.position.y < pock_man.position.y) ||
							(pac_script.getDirection == 3 && transform.position.y > pock_man.position.y)) {
							pac_script.setBehindEnemy = true;
							Debug.Log("Enemy behind vertical");
						}
					}


                //invertibility time section
				if (Global.Invertibility.IsActive()) {
					if (!invertibility) {
						invertibility = true;
						invTime = 100;
					}

                    //enemy sprite will be blue if invertibility is active and change lower speed
					if (Global.Invertibility.TimeRemaining > 2.0f) {
						if (animation_type == 0)
							current_sprite.sprite = sprites [direction+4];
						else
							current_sprite.sprite = sprites [1];
						speed = 0.8f;
					} else {
						if (invTime / 20 % 2 == 0) {
							if (animation_type == 0)
								current_sprite.sprite = sprites [direction+4];
							else
								current_sprite.sprite = sprites [1];
							
						} else {
							if (animation_type == 0)
								current_sprite.sprite = sprites [direction+8];
							else
								current_sprite.sprite = sprites [2];
						}

						invTime--;
					}
				} else if (invertibility) {
					speed = (Global.classic ? Global.enemy_speed : Global.enemy_speed * 0.8f);
					if (animation_type < 5)
						current_sprite.sprite = sprites [ally_sprite];
					invertibility = false;
				}

                //ally activation section
				if (isAlly) {
					if (Time.timeSinceLevelLoad - allyTime > 10.0f) { //end of ally
						deconvertFromAlly();
					} else if (Time.timeSinceLevelLoad - allyTime > 7.0f) {
						if (allyAnim / 20 % 2 == 0)
							setAllySprite(true);
						else
							setAllySprite(false);
						allyAnim--;

					}
				}

				//die enemy in case of going out of camera view (in rush mode only)
				if (!Global.classic && transform.position.y < camera.transform.position.y + Global.view_range_bottom) {
					Global.enemies.Remove(this);			
					Destroy(gameObject);
				}

                //move of enemy
				if (animation_type == 0 || animation_type == 2) {
					if (direction == 0)
						transform.Translate (-speed*Time.deltaTime, 0, 0);
					else if (direction == 1)
						transform.Translate (speed*Time.deltaTime, 0, 0);
					else if (direction == 2)
						transform.Translate (0, speed*Time.deltaTime, 0);
					else if (direction == 3)
						transform.Translate (0, -speed*Time.deltaTime, 0);
				}
				else
					transform.Translate (0, speed*Time.deltaTime, 0);

				//collision of enemy
				if (!isAlly && !Global.PauseEnemy.IsActive() && !pac_script.dead && isSpawned && Vector2.Distance(transform.position, pock_man.position) < 0.25f) {
						
						//enemy kills the player
						if (!Global.Invertibility.IsActive()) {
							pac_script.anim.SetBool ("dead", true);
							pac_script.dead = true;
							Global.pause_game = true;

						//invertibility enabled
						} else {

						Global.score += pac_script.ghost_combo;
						
						//show total ghost combo
						if (pac_script.ghost_combo == 250) {
							pock_man.GetComponentInChildren<manage_pickups>().activate_rate_text("Total ghost combo!");
						}
						
						//set and show ghost combo title
						GhostComboPopup.Activate(3.0f);
						GhostComboPopup.SetText("+" + pac_script.ghost_combo.ToString());

						Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
						new_dead.position = transform.position;
						new_dead.gameObject.SetActive (true);

						pac_script.ghost_combo += 50;

						Global.enemies.Remove(this);					
						Destroy(gameObject);
						if (Global.classic)
							killSpecialEnemy();
					}
				}

			} else {
				if (Time.timeSinceLevelLoad - spawnTime > 2.0f)
					isSpawned = true;

				count_down--;

				//animatoin of enemy respawning
				if (animation_type == 0)
					current_sprite.sprite = sprites[12 + (count_down / 5 % 4)];
				else
					current_sprite.sprite = sprites[3 + (count_down / 5 % 4)];

				//deactivation at end of level
				/*if (count_down < -8) {
					count_down = 0;
					Global.enemies.Remove(gameObject);
					Destroy(gameObject);
				}*/

				if (isSpawned) {
					if (animation_type == 0)
						current_sprite.sprite = sprites[direction];
					else
						current_sprite.sprite = sprites[0];
				}
					

			}
		}

		if (eating && Time.timeSinceLevelLoad - eatTime > 1.5f)
			eating = false;
	}

	void determine_direction() {
	
		if (enemy_pos == 10) {
			int x = (int)UnityEngine.Random.Range(0.0f, 3.9f);
			while (x == 0 && direction == 1 || x == 1 && direction == 0 || x == 2 && direction == 3 || x == 3 && direction == 2)
				x = (int)UnityEngine.Random.Range(0.0f, 3.9f);
			
			direction = x;
		}
		else if (direction == 0) {

			int x = (int)UnityEngine.Random.Range(0.0f, 1.9f);
			if (enemy_pos == 6)
			{
				if (x == 0)
					direction = 0;
				else
					direction = 3;
				
			}
			else if (enemy_pos == 7) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 2;
			}
			else if (enemy_pos == 8) {
				
				if (x == 0)
					direction = 2;
				else
					direction = 3;
			}
			else if (enemy_pos == 3)
				direction = 2;
			else if (enemy_pos == 4)
				direction = 3;
		}
		else if (direction == 1) {
			int x = (int)UnityEngine.Random.Range(0.0f, 1.9f);
			if (enemy_pos == 6){
				
				if (x == 0)
					direction = 1;
				else
					direction = 3;
				
			}
			else if (enemy_pos == 7) {
				
				if (x == 0)
					direction = 1;
				else
					direction = 2;
			}
			else if (enemy_pos == 9) {
				
				if (x == 0)
					direction = 2;
				else
					direction = 3;
			}
			else if (enemy_pos == 2)
				direction = 2;
			else if (enemy_pos == 5)
				direction = 3;
		}
		else if (direction == 2) {
			int x = (int)UnityEngine.Random.Range(0.0f, 1.9f);
			if (enemy_pos == 6) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 1;
				
			}
			else if (enemy_pos == 8) {
				
				if (x == 0)
					direction = 1;
				else
					direction = 2;
			}
			else if (enemy_pos == 9) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 2;
			}
			else if (enemy_pos == 4)
				direction = 1;
			else if (enemy_pos == 5)
				direction = 0;
		}
		else if (direction == 3) {

			int x = (int)UnityEngine.Random.Range(0.0f, 1.9f);
			if (enemy_pos == 7) {
				if (x == 0)
					direction = 0;
				else
					direction = 1;
				
			}
			else if (enemy_pos == 8) {
				
				if (x == 0)
					direction = 1;
				else
					direction = 3;
			}
			else if (enemy_pos == 9) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 3;
			}
			else if (enemy_pos == 2)
				direction = 0;
			else if (enemy_pos == 3)
				direction = 1;
		}

		setRotation();

	}

	public void convertToAlly() {
		isAlly = true;
		allyTime = Time.timeSinceLevelLoad;
		allyAnim = 1000;
		setAllySprite(true);
		enemy_type = 5;
		Collider.SetActive (true);
		bc.enabled = false;
	}

	public void deconvertFromAlly() {
		setAllySprite(false);
		bc.enabled = true;
		Collider.SetActive(false);
		enemy_type = 0;
		isAlly = false;
	}

	public void respawn_enemy() {

		count_down = 200;
		isSpawned = true;
		speed = Global.enemy_speed;

		if (Global.classic) //respaw enemy to the pre-defined position in classic mode
			transform.position = new Vector2(Global.endcoord_x/2.0f, Global.endcoord_y/2.0f);
        //respawn enemy to the camera position
		else {

            int j, k;
            if (enemy_type == 0 || enemy_type == 1 || enemy_type > 3) 
                j = (int)(camera.transform.position.y+4.0f)*2;
            else
                j = (int)(camera.transform.position.y-1.0f)*2;

            if (j > Global.level_height - 7)
            {
                gameObject.SetActive(false);
                return;
            }

            if (enemy_type == 0 || enemy_type == 3)
                k = 7;
            else
                k = 0;

            bool no_safe = false;
            for (; k < 14 && (Global.levelmatrix[j, k] == -1 || !no_safe); k++)
            {
                no_safe = true;
				if (Global.safety_coords != null)
	                foreach (var pos in Global.safety_coords)
	                {
	                    if (Math.Abs(pos.y * 2.0f - j) < 1.0 && Math.Abs(pos.x * 2.0f - k) < 1.0)
	                        no_safe = false;
	                }
            }

            if (k == 14)
            {
                k = 7;
                no_safe = false;
                for (; k > 0 && (Global.levelmatrix[j, k] == -1 || !no_safe); k--)
                {
                    no_safe = true;
					if (Global.safety_coords != null)
	                    foreach (var pos in Global.safety_coords)
	                    {
	                        if (Math.Abs(pos.y * 2.0f - j) < 1.0 && Math.Abs(pos.x * 2.0f - k) < 1.0)
	                            no_safe = false;
	                    }
                }

                if (k == 0)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }

            transform.position = new Vector2(k / 2.0f, j / 2.0f);
		}
		
		
		int enemy_pos = Global.levelmatrix[(int)transform.position.y*2,(int)transform.position.x*2];

		if (!Global.classic) {
			if (enemy_pos == 0 || enemy_pos == 3 || enemy_pos == 7 || enemy_pos == 11)
				direction = 1;
			else if  (enemy_pos == 1 || enemy_pos == 4 || enemy_pos == 5 || enemy_pos == 8 || enemy_pos == 9 || enemy_pos == 10)
				direction = 3;
			else if (enemy_pos == 2)
				direction = 0;

			setRotation();
		}
		
	}

	public bool IsAlly {
		get {
			return isAlly;
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
        //if enemy bump into a trap or ammo
		if (coll.gameObject.tag == "enemy_trap" || coll.gameObject.tag == "enemy_fire") {

			if (coll.gameObject.tag == "enemy_trap")
				Destroy (coll.gameObject);
			else
				coll.gameObject.SetActive (false);

            //clone an object containing dead animation and move it to the enemy's position
			Transform new_dead = (Transform)Instantiate (enemy_dead, transform.position, Quaternion.identity);
			new_dead.position = coll.transform.position;
			new_dead.gameObject.SetActive (true);

            Global.score += 100;

			Global.enemies.Remove(this);
			Destroy(gameObject);
			if (Global.classic)
				killSpecialEnemy();
			
        //go back if the enemy reaches the border of safe_zone
		} else if (coll.gameObject.tag == "safe_zone") {
			if (direction == 0)
				direction = 1;
			else if (direction == 1)
				direction = 0;
			else if (direction == 2)
				direction = 3;
			else if (direction == 3)
				direction = 2;

			setRotation();
		} else if (Global.classic &&
		 (coll.gameObject.tag == "apple" || coll.gameObject.tag == "pineapple" ||
		 coll.gameObject.tag == "melone" || coll.gameObject.tag == "bananna")) {
			Destroy(coll.gameObject);
			eatTime = Time.timeSinceLevelLoad;
			eating = true;
		}
	}

	void fix_enemy_pos(int x, int y) {
		int i, j;
		if (direction == 0) {
			for (i = x, j = y; i < 20 && Global.levelmatrix[j, i] == -1; i++);
			transform.position = new Vector3(i/2.0f+0.1f, j/2.0f, 0);
		}
		else if (direction == 1) {
			for (i = x, j = y; i > 0 && Global.levelmatrix[j, i] == -1; i--);
			transform.position = new Vector3(i/2.0f-0.1f, j/2.0f, 0);
		}
		else if (direction == 2) {
			for (i = x, j = y; j > 0 && Global.levelmatrix[j, i] == -1; j--);
			transform.position = new Vector3(i/2.0f, j/2.0f-0.1f, 0);
		}	
		else {
			for (i = x, j = y; j < Global.level_height && Global.levelmatrix[j, i] == -1; j++);
			transform.position = new Vector3(i/2.0f, j/2.0f+0.1f, 0);
		}
			
	}

	void setRotation() {
		if (animation_type == 1) {
			if (direction == 0)
				transform.localEulerAngles = new Vector3 (0, 0, 90);
			else if (direction == 1)
				transform.localEulerAngles = new Vector3 (0, 0, 270);
			else if (direction == 2)
				transform.localEulerAngles = new Vector3 (0, 0, 0);
			else if (direction == 3)
				transform.localEulerAngles = new Vector3 (0, 0, 180);
		}
	}

	void killSpecialEnemy() {
		if (enemy_type == 0)
			Global.followEnemyAlive = false;
		else if (enemy_type == 1)
			Global.blockenemyAlive = false;
	}

	void setAllySprite(bool value) {
		if (value) {
			if (animation_type == 0) {
				ally_sprite = 16;
				current_sprite.sprite = sprites [ally_sprite+direction];
			}
			else {
				ally_sprite = 7;
				current_sprite.sprite = sprites [ally_sprite];
			}
		} else {
			ally_sprite = 0;

			if (animation_type == 0) 
				current_sprite.sprite = sprites [ally_sprite+direction];
			else
				current_sprite.sprite = sprites [ally_sprite];
		}
	}

}
