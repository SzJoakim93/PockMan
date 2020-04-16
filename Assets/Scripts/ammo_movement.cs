using UnityEngine;
using System.Collections;

public class ammo_movement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//moving ammo
		transform.Translate(3.0f*Time.deltaTime, 0, 0);

		int matrix_x = (int)(transform.position.x * 2);
		int matrix_y = (int)(transform.position.y * 2);

		if (matrix_x < 0 || matrix_x > 17 || matrix_y < 0 || matrix_y > Global.level_height || Global.levelmatrix[matrix_y, matrix_x] == -1)
			gameObject.SetActive(false);
	}
}
