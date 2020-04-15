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
	public Text ghost_combo_txt;
	public Text rate_text;

	public Sprite [] sprites; //an arry containing all enym sprites

	SpriteRenderer current_sprite; //the enym's current sprite

	GameObject collider; //the collider object that using for ally mode

	public short isAlly;
	short ally_sprite = 0; //offset to ally sprite in sprites array

	BoxCollider2D bc; //the collider that disabled in allymode to not kill the player

	public int animation_type;
	pac_movement pac_script;

	BalancedSearch searchAI;
	int matrix_x;
	int matrix_y;

	//bool selectable=true;

	// Use this for initialization
	void Start () {
		if (Global.classic)
			speed = Global.enemy_speed;
		else
			speed = Global.enemy_speed * 0.8f;
		current_sprite = this.GetComponent<SpriteRenderer> ();
		Transform [] temp = this.GetComponentsInChildren<Transform> (true);
		collider = temp [1].gameObject;
		collider.SetActive (false);

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

		matrix_x = (int)(transform.position.x * 2);
		matrix_y = (int)(transform.position.y * 2);
		enemy_pos = Global.levelmatrix [matrix_y, matrix_x];

		count_down = 50;

		searchAI = new BalancedSearch();
	}
	
	// Update is called once per frame
	void Update () {

		if (Global.ready_to_go == 0 && Global.pause_enemy == 0 && !Global.pause_game) {
			if (count_down == 0) {

                //get level matrix coordinates
				if ((int)(transform.position.x * 40) % 20 == 0 && (int)(transform.position.y * 40) % 20 == 0) {
					matrix_x = (int)(transform.position.x * 2);
					matrix_y = (int)(transform.position.y * 2);

                    //get enemy pos from coordinates
					enemy_pos = Global.levelmatrix [matrix_y, matrix_x];

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
					else if (isAlly > 0 && transform.position.y < camera.transform.position.y + Global.view_range_bottom + 1.0f && enemy_pos != 0 && enemy_pos != 6 && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 11) {
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
								enemy_type == 2 && Vector3.Distance(transform.position, pock_man.position) > 3.55f ||
								enemy_type == 3 && Vector3.Distance(transform.position, pock_man.position) > 7.1f))
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
										searchAI.search(matrix_x, matrix_y, pac_script.front_x, pac_script.front_y, 5);
										break;
									case 3:
										searchAI.search(matrix_x, matrix_y, 4, pac_script.front_y, 10);
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
					if (Global.inv_time == 0) {
						if (animation_type == 0)
							current_sprite.sprite = sprites [direction+ally_sprite];
					}

					//deactivate enemy at top of level
					if (!Global.classic && matrix_y > Global.level_height - 5) {
						Global.enemies.Remove(gameObject);
						Destroy(gameObject);
                    }
				}

				//fix enemy position when go out of playground
				if (((enemy_pos == 1 || enemy_pos == 2 || enemy_pos == 5 || enemy_pos == 9) && (int)(transform.position.x * 2.0f) > matrix_x) ||
					((enemy_pos == 1 || enemy_pos == 3 || enemy_pos == 4 || enemy_pos == 8) && (int)(transform.position.x * 2.0f) < matrix_x) ||
					((enemy_pos == 0 || enemy_pos == 4 || enemy_pos == 5 || enemy_pos == 6) && (int)(transform.position.y * 2.0f) > matrix_y) ||
					((enemy_pos == 0 || enemy_pos == 2 || enemy_pos == 3 || enemy_pos == 7) && (int)(transform.position.y * 2.0f) < matrix_y)) {
						transform.position = new Vector3(matrix_x/2.0f, matrix_y/2.0f, 0);
						determine_direction();
					}

				
                //invertibility time section
				if (Global.inv_time > 0) {
                    //enemy sprite will be blue if invertibility is active and change lower speed
					if (Global.inv_time > 200) {
						if (animation_type == 0)
							current_sprite.sprite = sprites [direction+4];
						else
							current_sprite.sprite = sprites [1];
						speed = 0.8f;
					} else if (Global.inv_time < 200 && Global.inv_time / 20 % 2 == 0) {
						if (animation_type == 0)
							current_sprite.sprite = sprites [direction+4];
						else
							current_sprite.sprite = sprites [1];
					}
					else if (Global.inv_time < 200 && Global.inv_time / 20 % 2 == 1)
						if (animation_type == 0)
							current_sprite.sprite = sprites [direction+8];
						else
							current_sprite.sprite = sprites [2];
					if (Global.inv_time < 5) {
						speed = (Global.classic ? Global.enemy_speed : Global.enemy_speed * 0.8f);
						if (animation_type < 5)
							current_sprite.sprite = sprites [ally_sprite];
					}
				}

                //ally activation section
				if (isAlly > 0) {
					//begining of ally
					if (isAlly == 1000) {
						if (animation_type == 0)
							ally_sprite = 16;
						else {
							ally_sprite = 7;
							current_sprite.sprite = sprites [ally_sprite];
						}
							
						collider.SetActive (true);
						bc.enabled = false;
							
					}

                    //end of ally
					if (isAlly == 1) {
						ally_sprite = 0;
						bc.enabled = true;
						collider.SetActive(false);
					}
					
					isAlly--;
				}

				if (!Global.classic && transform.position.y < camera.transform.position.y + Global.view_range_bottom) {
					Global.enemies.Remove(gameObject);			
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
				if (isAlly == 0 && Global.pause_enemy == 0 && !pac_script.dead && count_down == 0 && Vector2.Distance(transform.position, pock_man.position) < 0.25f) {
						
						//enemy kills the player
						if (Global.inv_time == 0) {
							pac_script.anim.SetBool ("dead", true);
							pac_script.dead = true;
							Global.pause_game = true;

						//invertibility enabled
						} else {

						
						//Global.enemy_active--;
						
						Global.score += pac_script.ghost_combo;
						
						//show total ghost combo
						if (pac_script.ghost_combo == 250) {
							rate_text.text = "Total ghost combo!";
							rate_text.gameObject.SetActive(true);
							pac_script.rate_countdown = 100;
						}
						
						//set and show ghost combo title
						pac_script.ghost_combo_countdown = 100;
						ghost_combo_txt.gameObject.SetActive(true);
						ghost_combo_txt.text = "+" + pac_script.ghost_combo.ToString();

						Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
						new_dead.position = transform.position;
						new_dead.gameObject.SetActive (true);

						pac_script.ghost_combo += 50;

						Global.enemies.Remove(gameObject);					
						Destroy(gameObject);
						Debug.Log("Enemies count: " + Global.enemies.Count);
					}
				}

			} else if (count_down != 0) {  
				count_down--;

				//animatoin of enemy respawning
				if (animation_type == 0)
					current_sprite.sprite = sprites[12 + (count_down / 5 % 4)];
				else
					current_sprite.sprite = sprites[3 + (count_down / 5 % 4)];

				//deactivation at end of level
				if (count_down < -8) {
					count_down = 0;
					Global.enemies.Remove(gameObject);
					Destroy(gameObject);
				}

				if (count_down == 0) {
					if (animation_type == 0)
						current_sprite.sprite = sprites[direction];
					else
						current_sprite.sprite = sprites[0];
				}
					

			}
		}
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


	public void respawn_enemy() {

		count_down = 50;
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


			/*int j= (int)(camera.transform.position.y+2.5f)*2, k, l, m;
			if (j > Global.level_height - 7) {

				gameObject.SetActive(false);
				return;
			}
			  

			for (k=0, l=7, m=7; k % 2 == 0 && Global.levelmatrix[j,l] == -1 || k % 2 == 1 && Global.levelmatrix[j,m] == -1; k++)
				if (k % 2 == 0)
					l++;
				else
					m--;

			if (k % 2 == 0)
				transform.position = new Vector2(l/2.0f, j/2.0f);
			else
				transform.position = new Vector2(m/2.0f, j/2.0f);*/
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

			Global.enemies.Remove(gameObject);
			Destroy(gameObject);

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

}
