using UnityEngine;
using System.Collections;

public class Turret_shoting : MonoBehaviour {

	float left;
	float right;
	float top;
	float down;
	int activate;
	int direction;
	bool isActive;

	public Transform player;
	Transform ammo;

	// Use this for initialization
	void Start () {
		left = transform.position.x - 0.1f;
		right = transform.position.x + 0.1f;
		top = transform.position.y + 0.1f;
		down = transform.position.y - 0.1f;
		activate = 0;
		isActive = false;

		ammo = transform.Find ("ammo");

		int matrix_x = (int)(transform.position.x * 2);
		int matrix_y = (int)(transform.position.y * 2);
		
		if (Mathf.RoundToInt(transform.eulerAngles.z) == 180) {
			int i=matrix_x;
			for (; i>0 && Global.levelmatrix[matrix_y, i] != 3 && Global.levelmatrix[matrix_y, i] != 4 && Global.levelmatrix[matrix_y, i] != 8; i--)
				;
			right = transform.position.x + 0.1f;
			left = i / 2.0f - 0.1f;
			direction = 0;

		} else if (Mathf.RoundToInt(transform.eulerAngles.z) == 0) {
			int i=matrix_x;
			for (; i>=0 && i<20 && Global.levelmatrix[matrix_y, i] != 2 && Global.levelmatrix[matrix_y, i] != 5 && Global.levelmatrix[matrix_y, i] != 9; i++)
				;
			left = transform.position.x - 0.1f;
			right = i / 2.0f + 0.1f;
			direction = 1;
		} else if (Mathf.RoundToInt(transform.eulerAngles.z) == 90) {
			int i=matrix_x;
			for (; i>=0 && i<Global.level_height && Global.levelmatrix[i, matrix_x] != 4 && Global.levelmatrix[i, matrix_x] != 5 && Global.levelmatrix[i, matrix_x] != 6; i++)
				;
			down = transform.position.y - 0.1f;
			top = i / 2.0f + 0.1f;
			direction = 2;
		} else if (Mathf.RoundToInt(transform.eulerAngles.z) == 270) {
			int i=matrix_x;
			for (; i>0 && Global.levelmatrix[i, matrix_x] != 2 && Global.levelmatrix[i, matrix_x] != 3 && Global.levelmatrix[i, matrix_x] != 7; i--)
				;
			top = transform.position.y + 0.1f;
			down = i / 2.0f - 0.1f;
			direction = 3;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (activate == 0 && !isActive && player.position.x >= left && player.position.x <= right && player.position.y >= down && player.position.y <= top)
			activate = 50;

		if (activate > 0) {
			activate--;

			if (activate == 1) {
				ammo.gameObject.SetActive(true);
				isActive = true;
				ammo.position = transform.position;
			}
		}

		if (ammo.gameObject.activeInHierarchy) {
			ammo.Translate (2.0f * Time.deltaTime, 0, 0);
			if (ammo.position.x <= left || ammo.position.x >= right || ammo.position.y >= top || ammo.position.y <= down) {
				ammo.gameObject.SetActive(false);
				isActive = false;
			}
				
		}
	}
}
