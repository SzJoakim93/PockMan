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

		int current_pos = Global.levelmatrix [matrix_y, matrix_x];
		
		if (current_pos != 0 && current_pos != 1)
			gameObject.SetActive(false);
	}
}
