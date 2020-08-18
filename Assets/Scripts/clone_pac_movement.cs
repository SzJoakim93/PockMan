using UnityEngine;
using System.Collections;

public class clone_pac_movement : MonoBehaviour {

	int pos, direction;
	public Transform [] enemy;
	public Transform camera;
	short enemy_index = 0;
	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Global.pause_game) {

			float distance = 0;
			for (short i = 0; i < 5; i++) {
				float curr_distance = Vector3.Distance (transform.position, enemy[i].position);
				if (curr_distance > distance) {
					distance = curr_distance;
					enemy_index = i;
				}
			}

			if ((int)(transform.position.x * 40) % 20 == 0 && (int)(transform.position.y * 40) % 20 == 0) {
				int matrix_x = (int)(transform.position.x * 2);
				int matrix_y = (int)(transform.position.y * 2);
				pos = Global.levelmatrix [matrix_y, matrix_x];
			
				if (pos == 0 && (direction == 2 || direction == 3))
					direction = 0;
				else if (pos == 1 && (direction == 0 || direction == 1))
					direction = 2;
			
				if (transform.position.y > camera.transform.position.y + 4.8f && pos != 0 && pos != 2 && pos != 3 && pos != 7 && pos != 11)
					direction = 3;
				else if (transform.position.y < camera.transform.position.y - 2.8f && pos != 0 && pos != 6 && pos != 4 && pos != 5 && pos != 11)
					direction = 2;
				else
					determine_direction ();

				if (direction == 0)
					transform.localEulerAngles = new Vector3 (0, 180, 0);
				else if (direction == 1)
					transform.localEulerAngles = new Vector3 (0, 0, 0);
				else if (direction == 2)
					transform.localEulerAngles = new Vector3 (0, 0, 90);
				else if (direction == 3)
					transform.localEulerAngles = new Vector3 (0, 180, 270);

			}

			transform.Translate (speed*Time.deltaTime, 0, 0);

			if (Global.clone_reamining > 0) {
				Global.clone_reamining--;
				if (Global.clone_reamining == 1)
					gameObject.SetActive(false);
			}

		}
	}

	void determine_direction() {
		if (pos == 10) {
			float dx = transform.position.x - enemy[enemy_index].position.x;
			float dy = transform.position.y - enemy[enemy_index].position.y;
			if (Mathf.Abs((int)dx) < Mathf.Abs((int)dy))
				if (transform.position.y < enemy[enemy_index].position.y)
					direction = 2;
			else
				direction = 3;
			else {
				if (transform.position.x < enemy[enemy_index].position.x)
					direction = 1;
				else
					direction = 0;
			}
		}
		else if (direction == 0) {
			
			if (pos == 6)
			{
				if (transform.position.y > enemy[enemy_index].position.y + 0.1f)
					direction = 3;
				
			}
			else if (pos == 7) {
				
				if (transform.position.y < enemy[enemy_index].position.y - 0.1f)
					direction = 2;
			}
			else if (pos == 8) {
				if (transform.position.y > enemy[enemy_index].position.y + 0.1f)
					direction = 3;
				else
					direction = 2;
				
			}
			else if (pos == 3)
				direction = 2;
			else if (pos == 4)
				direction = 3;
		}
		else if (direction == 1) {
			
			if (pos == 6){
				
				if (transform.position.y > enemy[enemy_index].position.y + 0.1f)
					direction = 3;
				
			}
			else if (pos == 7) {
				
				if (transform.position.y < enemy[enemy_index].position.y - 0.1f)
					direction = 2;
			}
			else if (pos == 9) {
				
				if (transform.position.y > enemy[enemy_index].position.y + 0.1f)
					direction = 3;
				else
					direction = 2;
			}
			else if (pos == 2)
				direction = 2;
			else if (pos == 5)
				direction = 3;
		}
		else if (direction == 2) {
			
			if (pos == 6) {
				
				if (transform.position.x < enemy[enemy_index].position.x - 0.1f)
					direction = 1;
				else
					direction = 0;
				
			}
			else if (pos == 8) {
				
				if (transform.position.x < enemy[enemy_index].position.x - 0.1f)
					direction = 1;
			}
			else if (pos == 9) {
				
				if (transform.position.x > enemy[enemy_index].position.x + 0.1f)
					direction = 0;
			}
			else if (pos == 4)
				direction = 1;
			else if (pos == 5)
				direction = 0;
		}
		else if (direction == 3) {
			
			if (pos == 7) {
				
				if (transform.position.x > enemy[enemy_index].position.x + 0.1f)
					direction = 0;
				else
					direction = 1;
			}
			else if (pos == 8) {
				
				if (transform.position.x < enemy[enemy_index].position.x - 0.1f)
					direction = 1;
			}
			else if (pos == 9) {
				
				if (transform.position.x > enemy[enemy_index].position.x + 0.1f)
					direction = 0;
			}
			else if (pos == 2)
				direction = 0;
			else if (pos == 3)
				direction = 1;
		}
	}
}
