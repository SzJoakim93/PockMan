using UnityEngine;
using System.Collections;

public class dropper_manager : MonoBehaviour {

	bool collision=false;

	public Transform pickup;
	public Transform [] fruits;
	public Transform pock_man;
	public Transform safety_road;
	short counter=0;
	short direction;
	int pos;
	public Transform camera;
	public Transform enemy_dead;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (Global.dropping_mode < 2) {
			transform.Translate (-0.5f, 0, 0);

			if (transform.position.x < 0.0f)
				transform.Translate (8.0f, -0.5f, 0);

			if (transform.position.y < pock_man.position.y - 3.0f)
				gameObject.SetActive (false);

			int matrix_x = (int)(transform.position.x * 2);
			int matrix_y = (int)(transform.position.y * 2);
			int current_pos = 0;

			if (matrix_x > -1 && matrix_y > -1)
				current_pos = Global.levelmatrix [matrix_y, matrix_x];

			if (matrix_x > -1 && matrix_y > -1 && current_pos != -1) {
				if (!collision && Global.dropping_mode == 0) {
					Instantiate (pickup, transform.position, Quaternion.identity);
					if (current_pos == 0) {
						Vector3 offset = new Vector3 (0.25f, 0, 0);
						Instantiate (pickup, transform.position + offset, Quaternion.identity);
					} else if (current_pos == 1) {
						Vector3 offset = new Vector3 (0, 0.25f, 0);
						Instantiate (pickup, transform.position + offset, Quaternion.identity);
					}
				} else if (Global.dropping_mode == 1) {
					counter++;
					if (counter == Global.max_fruits) {
						int i = Random.Range (0, 3);
						Instantiate (fruits [i], transform.position, Quaternion.identity);
						counter = 0;
					}

				} 
			}

			collision = false;
		} else if (Global.dropping_mode == 2) {
			int matrix_x = (int)(transform.position.x * 2);
			int matrix_y = (int)(transform.position.y * 2);
			pos = Global.levelmatrix [matrix_y, matrix_x];

			
			if (pos == 0 && (direction == 2 || direction == 3))
				direction = 0;
			else if (pos == 1 && (direction == 0 || direction == 1))
				direction = 2;
			
			if (transform.position.y < camera.position.y - 2.8f && pos != 0 && pos != 6 && pos != 4 && pos != 5 && pos != 11)
				direction = 2;
			else
				determine_direction ();

            if (!collision)
            {
                Instantiate(safety_road, transform.position, Quaternion.identity);
                Global.safety_coords.Add(new Vector2(transform.position.x, transform.position.y));
            }

			if (direction == 0)
				transform.Translate (-0.5f, 0, 0);
			else if (direction == 1)
				transform.Translate (0.5f, 0, 0);
			else if (direction == 2)
				transform.Translate (0, 0.5f, 0);
			else if (direction == 3)
				transform.Translate (0, -0.5f, 0);

			Global.safe_counter--;

			collision = false;

			if (Global.safe_counter == 0)
				gameObject.SetActive(false);
		}

	}

	void determine_direction() {
		if (pos == 10) {
			short x = (short)Random.Range(0.0f, 2.9f);
			while (x == 0 && direction == 1 || x == 1 && direction == 0 || x == 2 && direction == 3 || x == 3 && direction == 2)
				x = (short)Random.Range(0.0f, 2.9f);
			
			direction = x;
		}
		else if (direction == 0) {
			
			int x = (int)Random.Range(0.0f, 1.9f);
			if (pos == 6)
			{
				if (x == 0)
					direction = 0;
				else
					direction = 3;
				
			}
			else if (pos == 7) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 2;
			}
			else if (pos == 8) {
				
				if (x == 0)
					direction = 2;
				else
					direction = 3;
			}
			else if (pos == 3)
				direction = 2;
			else if (pos == 4)
				direction = 3;
		}
		else if (direction == 1) {
			int x = (int)Random.Range(0.0f, 1.9f);
			if (pos == 6){
				
				if (x == 0)
					direction = 1;
				else
					direction = 3;
				
			}
			else if (pos == 7) {
				
				if (x == 0)
					direction = 1;
				else
					direction = 2;
			}
			else if (pos == 9) {
				
				if (x == 0)
					direction = 2;
				else
					direction = 3;
			}
			else if (pos == 2)
				direction = 2;
			else if (pos == 5)
				direction = 3;
		}
		else if (direction == 2) {
			int x = (int)Random.Range(0.0f, 1.9f);
			if (pos == 6) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 1;
				
			}
			else if (pos == 8) {
				
				if (x == 0)
					direction = 1;
				else
					direction = 2;
			}
			else if (pos == 9) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 2;
			}
			else if (pos == 4)
				direction = 1;
			else if (pos == 5)
				direction = 0;
		}
		else if (direction == 3) {
			
			int x = (int)Random.Range(0.0f, 1.9f);
			if (pos == 7) {
				if (x == 0)
					direction = 0;
				else
					direction = 1;
				
			}
			else if (pos == 8) {
				
				if (x == 0)
					direction = 1;
				else
					direction = 3;
			}
			else if (pos == 9) {
				
				if (x == 0)
					direction = 0;
				else
					direction = 3;
			}
			else if (pos == 2)
				direction = 0;
			else if (pos == 3)
				direction = 1;
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (Global.dropping_mode < 2 && coll.gameObject.tag == "pickup" || Global.dropping_mode == 2 && coll.gameObject.tag == "safe_zone") {
			collision = true;
		}

		if (Global.dropping_mode == 2 && coll.gameObject.tag == "enemy") {
			coll.gameObject.SetActive (false);
			Global.enemy_active--;
			Global.score += 10;
		
			Transform new_dead = (Transform)Instantiate (enemy_dead, transform.position, Quaternion.identity);
			new_dead.position = coll.transform.position;
			new_dead.gameObject.SetActive (true);

		}
	}
}
