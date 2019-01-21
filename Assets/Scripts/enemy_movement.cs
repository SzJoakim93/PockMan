using UnityEngine;
using System;
using System.Collections;

public class enemy_movement : MonoBehaviour {

	int direction=0; //enemy's moving direction (0: <- , 1: -> , 2: ^ , 3: ˇ
	public static float speed=Global.enemy_speed; //determine enemy's speed from a global const
	int enemy_pos; //value of enemy's position on levelmatrix
	public int count_down; //use for enemy respawning and deactivating at the end of level

	public int enemy_type; //determine an AI sample

	public Transform camera; //camera position
	public Transform pock_man; //players's position
	public Transform enemy_dead; //a sample object to clne to the enemy psition in case of dead

	public Sprite [] sprites; //an arry containing all enym sprites

	SpriteRenderer current_sprite; //the enym's current sprite

	public GameObject collider; //the collider object that using for ally mode

	public short isAlly = 0;
	short ally_sprite = 0; //offset to ally sprite in sprites array

	BoxCollider2D bc; //the collider that disabled in allymode to not kill the player

	//bool selectable=true;

	// Use this for initialization
	void Start () {
		current_sprite = this.GetComponent<SpriteRenderer> ();
		collider.SetActive (false);

		bc = this.GetComponent<BoxCollider2D> ();

        if (Global.classic)
        {
            if (enemy_type == 0 || enemy_type == 2)
                enemy_type = 7;
            else if (enemy_type == 1 || enemy_type == 3)
                enemy_type = 6;

            Debug.Log("Classic AI activated");
        }

        Global.safety_coords = new System.Collections.Generic.List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Global.ready_to_go == 0 && Global.pause_enemy == 0 && !Global.pause_game)
			if (count_down == 0) {

                //get level matrix coordinates
				if ((int)(transform.position.x * 40) % 20 == 0 && (int)(transform.position.y * 40) % 20 == 0) {
					int matrix_x = (int)(transform.position.x * 2);
					int matrix_y = (int)(transform.position.y * 2);


                    //fix enemy position when go out of playground
					if (matrix_x < 0) {
						matrix_x = 0;
						transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
					} else if (matrix_x > 19) {
						matrix_x = 19;
						transform.position = new Vector3(10.5f, transform.position.y, transform.position.z);
					}

                    //get enemy pos from coordinates
					enemy_pos = Global.levelmatrix [matrix_y, matrix_x];

				    //fix direction
					if (enemy_pos == 0 && (direction == 2 || direction == 3))
						direction = 0;
					else if (enemy_pos == 1 && (direction == 0 || direction == 1))
						direction = 2;

                    //move back if the enemy go out of camera view
					if (transform.position.y > camera.transform.position.y + Global.view_range_top && enemy_pos != 0 && enemy_pos != 2 && enemy_pos != 3 && enemy_pos != 7 && enemy_pos != 11)
						direction = 3;
					else if (transform.position.y < camera.transform.position.y + Global.view_range_bottom + 1.0f && enemy_pos != 0 && enemy_pos != 6 && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 11)
						direction = 2;
                    //determine enemy's moving direction
					else
						determine_direction ();

                    //change enemy's sprite
					if (Global.inv_time == 0)
						current_sprite.sprite = sprites [direction+ally_sprite];

                    if (!Global.classic)
                    {
                        //respawn enemy when go out of camera view so far
                        if (transform.position.y < camera.transform.position.y - 3.2f)
                            respawn_enemy();

                        //deactivate enemy at top of level
                        if (matrix_y > Global.level_height - 10 && enemy_type < 4)
                            count_down = -1;
                    }
				}

				
                //invertibility time section
				if (Global.inv_time > 0) {
                    //enemy sprite will be bule if invertibility is active and change lower speed
					if (Global.inv_time > 200) {
						current_sprite.sprite = sprites [4];
						speed = 0.8f;
					} else if (Global.inv_time < 200 && Global.inv_time / 20 % 2 == 0)
						current_sprite.sprite = sprites [4];
					else if (Global.inv_time < 200 && Global.inv_time / 20 % 2 == 1)
						current_sprite.sprite = sprites [5];
					if (Global.inv_time < 5)
						speed = Global.enemy_speed;
				}

                //ally activation section
				if (isAlly > 0) {

                    //begining of ally
					if (isAlly == 1000) {
						ally_sprite = 10;
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


                //movo of enemy
				if (direction == 0)
					transform.Translate (-speed*Time.deltaTime, 0, 0);
				else if (direction == 1)
					transform.Translate (speed*Time.deltaTime, 0, 0);
				else if (direction == 2)
					transform.Translate (0, speed*Time.deltaTime, 0);
				else if (direction == 3)
					transform.Translate (0, -speed*Time.deltaTime, 0);

		} else if (count_down != 0) {  
			count_down--;

            //animatoin of enemy respawning
			current_sprite.sprite = sprites[6 + (count_down / 5 % 4)];

            //deactivation at end of level
			if (count_down < -8) {
				gameObject.SetActive(false);
				count_down = 0;
			}
		}

	}

	void determine_direction() {

        //if the gost out of region

		if ((enemy_type == 0 && (transform.position.x < 5 || transform.position.y < camera.position.y + 3.5f)) || //top-right
		    (enemy_type == 1 && (transform.position.x > 3.5f || transform.position.y < camera.position.y + 3.5f)) || //top-left
            (enemy_type == 2 && (transform.position.x > 3.5f || transform.position.y > camera.position.y + 4.0f)) || //bottom-left
            (enemy_type == 3 && (transform.position.x < 5 || transform.position.y > camera.position.y + 4.0f)) || //bottom-rigth
		    (enemy_type == 4 && Vector3.Distance (transform.position, pock_man.position) > 1.0f) ||
            (enemy_type == 6 && Vector3.Distance(transform.position, pock_man.position) > 4.0f) ||
            (enemy_type == 7 && Vector3.Distance(transform.position, pock_man.position) < 3.0f) ||
		    enemy_type == 5) {

                if (enemy_pos != 0 && enemy_pos != 1)
                {
                    if (enemy_type == 0)
                    {
                        if (transform.position.y < camera.position.y + 3.5f && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 6)
                            direction = 2;
                        else if (transform.position.x < 5 && enemy_pos != 2 && enemy_pos != 5 && enemy_pos != 9)
                            direction = 1;
                    }
                    else if (enemy_type == 1)
                    {
                        if (transform.position.y < camera.position.y + 3.5f && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 6)
                            direction = 2;
                        else if (transform.position.x > 3.5f && enemy_pos != 3 && enemy_pos != 4 && enemy_pos != 8)
                            direction = 0;
                    }
                    else if (enemy_type == 2)
                    {
                        if (transform.position.y > camera.position.y + 3.5f && enemy_pos != 3 && enemy_pos != 2 && enemy_pos != 7)
                            direction = 2;
                        else if (transform.position.x > 3.5f && enemy_pos != 3 && enemy_pos != 4 && enemy_pos != 8)
                            direction = 0;
                    }
                    else if (enemy_type == 3)
                    {
                        if (transform.position.y > camera.position.y + 3.5f && enemy_pos != 3 && enemy_pos != 2 && enemy_pos != 7)
                            direction = 2;
                        else if (transform.position.x < 5 && enemy_pos != 2 && enemy_pos != 5 && enemy_pos != 9)
                            direction = 1;
                    }
                }
                    
			if (enemy_pos == 10) {

				if (enemy_type == 0) {
                    if (transform.position.y < camera.position.y + 3.5f)
						direction = 2;
					else
						direction = 1;

				}
				else if (enemy_type == 1) {
                    if (transform.position.y < camera.position.y + 3.5f)
						direction = 2;
					else
						direction = 0;
					
				}
				else if (enemy_type == 2) {
                    if (transform.position.y > camera.position.y + 4.0f)
						direction = 3;
					else
						direction = 0;
					
				}
				else if (enemy_type == 3) {
                    if (transform.position.y > camera.position.y + 4.0f)
						direction = 3;
					else
						direction = 1;
					
				}
				else if (enemy_type == 4 || enemy_type == 6 || enemy_type == 7) {
					float dx = transform.position.x - pock_man.position.x;
					float dy = transform.position.y - pock_man.position.y;
					if (Mathf.Abs((int)dx) < Mathf.Abs((int)dy))
						if (transform.position.y < pock_man.position.y)
							direction = 2;
						else
							direction = 3;
					else {
						if (transform.position.x < pock_man.position.x)
							direction = 1;
						else
							direction = 0;
					}
				}
				else if (enemy_type == 5) {
					float dx = transform.position.x - Global.pock_front.x;
					float dy = transform.position.y - Global.pock_front.y;
					if (Mathf.Abs((int)dx) < Mathf.Abs((int)dy))
						if (transform.position.y < Global.pock_front.y)
							direction = 2;
					else
						direction = 3;
					else {
						if (transform.position.x < Global.pock_front.x)
							direction = 1;
						else
							direction = 0;
					}
				}
			}
			else if (direction == 0) {

				if (enemy_pos == 6)
				{
					if (((enemy_type == 2 || enemy_type == 3 || enemy_type == 0) && transform.position.y > camera.position.y + 4.0f) || 
					    ((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.y > pock_man.position.y + 0.1f) ||
					    (enemy_type == 5 && transform.position.y > Global.pock_front.y + 0.1f))
						direction = 3;
					
				}
				else if (enemy_pos == 7) {
					
					if (((enemy_type == 0 || enemy_type == 1 || enemy_type == 3) && transform.position.y < camera.position.y + 3.5f) || 
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.y < pock_man.position.y - 0.1f) ||
					    (enemy_type == 5 && transform.position.y < Global.pock_front.y - 0.1f))
						direction = 2;
				}
				else if (enemy_pos == 8) {
					if (((enemy_type == 2 || enemy_type == 3) && transform.position.y > camera.position.y + 4.0f) || 
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.y > pock_man.position.y + 0.1f) ||
					    (enemy_type == 5 && transform.position.y > Global.pock_front.y + 0.1f))
						direction = 3;
					else
						direction = 2;

				}
				else if (enemy_pos == 3)
					direction = 2;
				else if (enemy_pos == 4)
					direction = 3;
			}
			else if (direction == 1) {

				if (enemy_pos == 6){
					
					if (((enemy_type == 1 || enemy_type == 2 || enemy_type == 3) && transform.position.y > camera.position.y) || 
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.y > pock_man.position.y + 0.1f) ||
					    (enemy_type == 5 && transform.position.y > Global.pock_front.y + 0.1f))
						direction = 3;
					
				}
				else if (enemy_pos == 7) {
					
					if (((enemy_type == 2 || enemy_type == 0 || enemy_type == 1) && transform.position.y < camera.position.y + 3.5f) || 
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.y < pock_man.position.y - 0.1f) ||
					    (enemy_type == 5 && transform.position.y < Global.pock_front.y - 0.1f))
						direction = 2;
				}
				else if (enemy_pos == 9) {
					
					if (((enemy_type == 2 || enemy_type == 3) && transform.position.y > camera.position.y) || 
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.y > pock_man.position.y + 0.1f) ||
					    (enemy_type == 5 && transform.position.y > Global.pock_front.y + 0.1f))
						direction = 3;
					else
						direction = 2;
				}
				else if (enemy_pos == 2)
					direction = 2;
				else if (enemy_pos == 5)
					direction = 3;
			}
			else if (direction == 2) {

				if (enemy_pos == 6) {
					
					if (((enemy_type == 1 || enemy_type == 2) && transform.position.x > 3.5f) ||
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.x < pock_man.position.x - 0.1f) ||
					    (enemy_type == 5 && transform.position.x < Global.pock_front.x - 0.1f))
						direction = 0;
					else
						direction = 1;
					
				}
				else if (enemy_pos == 8) {
					
					if (((enemy_type == 0 || enemy_type == 3 || enemy_type == 2) && transform.position.x < 5) ||
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.x < pock_man.position.x - 0.1f) ||
					    (enemy_type == 5 && transform.position.x < Global.pock_front.x - 0.1f))
						direction = 1;
				}
				else if (enemy_pos == 9) {
					
					if (((enemy_type == 3 || enemy_type == 1 || enemy_type == 2) && transform.position.x > 3.5f) ||
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.x > pock_man.position.x + 0.1f) ||
					 	(enemy_type == 5 && transform.position.x > Global.pock_front.x + 0.1f))
						direction = 0;
				}
				else if (enemy_pos == 4)
					direction = 1;
				else if (enemy_pos == 5)
					direction = 0;
			}
			else if (direction == 3) {

				if (enemy_pos == 7) {

					if (((enemy_type == 1 || enemy_type == 2) && transform.position.x > 3.5f) ||
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.x > pock_man.position.x + 0.1f) ||
					    (enemy_type == 5 && transform.position.x > Global.pock_front.x + 0.1f))
						direction = 0;
					else
						direction = 1;
				}
				else if (enemy_pos == 8) {
					
					if (((enemy_type == 0 || enemy_type == 3 || enemy_type == 1) && transform.position.x < 5) ||
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.x < pock_man.position.x - 0.1f) ||
					    (enemy_type == 5 && transform.position.x < Global.pock_front.x - 0.1f))
						direction = 1;
				}
				else if (enemy_pos == 9) {
					
					if (((enemy_type == 0 || enemy_type == 1 || enemy_type == 2) && transform.position.x > 3.5f) ||
						((enemy_type == 4 || enemy_type == 6 || enemy_type == 7) && transform.position.x > pock_man.position.x + 0.1f) ||
					    (enemy_type == 5 && transform.position.x > Global.pock_front.x + 0.1f))
						direction = 0;
				}
				else if (enemy_pos == 2)
					direction = 0;
				else if (enemy_pos == 3)
					direction = 1;
			}



		} else {
			if (enemy_pos == 10) {
				int x = (int)UnityEngine.Random.Range(0.0f, 2.9f);
				while (x == 0 && direction == 1 || x == 1 && direction == 0 || x == 2 && direction == 3 || x == 3 && direction == 2)
                    x = (int)UnityEngine.Random.Range(0.0f, 2.9f);
				
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

            if (enemy_pos != 0 && enemy_pos != 1)
            {
                if (enemy_type == 0)
                {
                    if (transform.position.y < camera.position.y + 1.0f && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 6)
                        direction = 2;
                    else if (transform.position.x < 5 && enemy_pos != 2 && enemy_pos != 5 && enemy_pos != 9)
                        direction = 1;
                }
                else if (enemy_type == 1)
                {
                    if (transform.position.y < camera.position.y + 1.0f && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 6)
                        direction = 2;
                    else if (transform.position.x > 3.5f && enemy_pos != 3 && enemy_pos != 4 && enemy_pos != 8)
                        direction = 0;
                }
                else if (enemy_type == 2)
                {
                    if (transform.position.y > camera.position.y + 4.0f && enemy_pos != 3 && enemy_pos != 2 && enemy_pos != 7)
                        direction = 2;
                    else if (transform.position.x > 3.5f && enemy_pos != 3 && enemy_pos != 4 && enemy_pos != 8)
                        direction = 0;
                }
                else if (enemy_type == 3)
                {
                    if (transform.position.y > camera.position.y + 4.0f && enemy_pos != 3 && enemy_pos != 2 && enemy_pos != 7)
                        direction = 2;
                    else if (transform.position.x < 5 && enemy_pos != 2 && enemy_pos != 5 && enemy_pos != 9)
                        direction = 1;
                }
            }
		}

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
		if (enemy_pos == 0 || enemy_pos == 3 || enemy_pos == 7 || enemy_pos == 11)
			direction = 1;
		else if  (enemy_pos == 1 || enemy_pos == 4 || enemy_pos == 5 || enemy_pos == 8 || enemy_pos == 9 || enemy_pos == 10)
			direction = 3;
		else if (enemy_pos == 2)
			direction = 0;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
        //if enemy bump into a trap or ammo
		if (coll.gameObject.tag == "enemy_trap" || coll.gameObject.tag == "enemy_fire") {
			gameObject.SetActive (false);

			if (coll.gameObject.tag == "enemy_trap")
				Destroy (coll.gameObject);
			else
				coll.gameObject.SetActive (false);

            //clone an object containing dead animation and move it to the enemy's position
			Transform new_dead = (Transform)Instantiate (enemy_dead, transform.position, Quaternion.identity);
			new_dead.position = coll.transform.position;
			new_dead.gameObject.SetActive (true);

            Global.score += 100;

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
		}
	}

}
