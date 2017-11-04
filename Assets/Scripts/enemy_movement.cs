using UnityEngine;
using System.Collections;

public class enemy_movement : MonoBehaviour {

	int direction=0;
	public static float speed=0.02f;
	int enemy_pos;
	public int count_down;

	public Transform camera;
	public Sprite [] sprites;

	SpriteRenderer current_sprite;

	//bool selectable=true;

	// Use this for initialization
	void Start () {
		current_sprite = this.GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Global.ready_to_go == 0)
			if (count_down == 0) {

				if ((int)(transform.position.x * 40) % 20 == 0 && (int)(transform.position.y * 40) % 20 == 0) {
					int matrix_x = (int)(transform.position.x * 2);
					int matrix_y = (int)(transform.position.y * 2);
					enemy_pos = Global.levelmatrix [matrix_y, matrix_x];
				
					if (enemy_pos == 0 && (direction == 2 || direction == 3))
						direction = 0;
					else if (enemy_pos == 1 && (direction == 0 || direction == 1))
						direction = 2;

					if (transform.position.y > camera.transform.position.y + 3.0f && enemy_pos != 0 && enemy_pos != 2 && enemy_pos != 3 && enemy_pos != 7 && enemy_pos != 11)
						direction = 3;
					else if (transform.position.y < camera.transform.position.y - 3.0f && enemy_pos != 0 && enemy_pos != 6 && enemy_pos != 4 && enemy_pos != 5 && enemy_pos != 11)
						direction = 2;
					else
						determine_enemy_direction ();

					if (Global.inv_time == 0)
						current_sprite.sprite = sprites [direction];
				}

				if (Global.inv_time > 0) {
					if (Global.inv_time > 200) {
						current_sprite.sprite = sprites [4];
						speed = 0.01f;
					} else if (Global.inv_time < 200 && Global.inv_time / 20 % 2 == 0)
						current_sprite.sprite = sprites [4];
					else if (Global.inv_time < 200 && Global.inv_time / 20 % 2 == 1)
						current_sprite.sprite = sprites [5];
					if (Global.inv_time < 5)
						speed = 0.02f;
				}


				if (direction == 0)
					transform.Translate (-speed, 0, 0);
				else if (direction == 1)
					transform.Translate (speed, 0, 0);
				else if (direction == 2)
					transform.Translate (0, speed, 0);
				else if (direction == 3)
					transform.Translate (0, -speed, 0);

		} else {
			count_down--;
			current_sprite.sprite = sprites[6 + (count_down / 5 % 3)];
		}

	}

	void determine_enemy_direction()
	{
		if (enemy_pos == 10)
		{
			int x = (int)Random.Range(0.0f, 2.9f);
			while (x == 0 && direction == 1 || x == 1 && direction == 0 || x == 2 && direction == 3 || x == 3 && direction == 2)
				x = (int)Random.Range(0.0f, 2.9f);
			direction = x;
		}
		else if (direction == 0)
		{
			int x = (int)Random.Range(0.0f, 1.9f);
			if (enemy_pos == 6)
			{
				if (x == 0)
					direction = 0;
				else
					direction = 3;
				
			}
			else if (enemy_pos == 7)
			{
				if (x == 0)
					direction = 0;
				else
					direction = 2;
			}
			else if (enemy_pos == 8)
			{
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
		else if (direction == 1)
		{
			int x = (int)Random.Range(0.0f, 1.9f);
			if (enemy_pos == 6)
			{
				if (x == 0)
					direction = 1;
				else
					direction = 3;
				
			}
			else if (enemy_pos == 7)
			{
				if (x == 0)
					direction = 1;
				else
					direction = 2;
			}
			else if (enemy_pos == 9)
			{
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
		else if (direction == 2)
		{
			int x = (int)Random.Range(0.0f, 1.9f);
			if (enemy_pos == 6)
			{
				if (x == 0)
					direction = 0;
				else
					direction = 1;
				
			}
			else if (enemy_pos == 8)
			{
				if (x == 0)
					direction = 1;
				else
					direction = 2;
			}
			else if (enemy_pos == 9)
			{
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
		else if (direction == 3)
		{
			int x = (int)Random.Range(0.0f, 1.9f);
			if (enemy_pos == 7)
			{
				if (x == 0)
					direction = 0;
				else
					direction = 1;
				
			}
			else if (enemy_pos == 8)
			{
				if (x == 0)
					direction = 1;
				else
					direction = 3;
			}
			else if (enemy_pos == 9)
			{
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
	}

	public void respawn_enemy()
	{
		count_down = 50;

		if (Global.classic)
			transform.position = new Vector2(Global.endcoord_x/2.0f, Global.endcoord_y/2.0f);
		else
		{
			int j= (int)(camera.transform.position.y+2.5f)*2, k, l, m;
			if (j > Global.level_height - 7)
			{
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
				transform.position = new Vector2(m/2.0f, j/2.0f);
		}
		
		
		int enemy_pos = Global.levelmatrix[(int)transform.position.y*2,(int)transform.position.x*2];
		if (enemy_pos == 0 || enemy_pos == 3 || enemy_pos == 7 || enemy_pos == 11)
			direction = 1;
		else if  (enemy_pos == 1 || enemy_pos == 4 || enemy_pos == 5 || enemy_pos == 8 || enemy_pos == 9 || enemy_pos == 10)
			direction = 3;
		else if (enemy_pos == 2)
			direction = 0;
	}
}
