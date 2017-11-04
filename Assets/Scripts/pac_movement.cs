using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class pac_movement : MonoBehaviour {

	public float speed;
	public Transform [] tiles;
	public Transform[] pickup_graph;
	public Transform camera;
	public Transform enemy_dead;

	public Text score_text;
	public Text life_text;
	public Text ready_to_go;

	public GameObject [] enemy;
	enemy_movement [] enemy_scripts = new enemy_movement[5];

	string level_graphics;

	bool dead;

	Animator anim;

	//static int[,] levelmatrix = new int[500, 500];
	int req_direction;
	int pac_direction;
	float camera_pos;
	int score=0;
	int life=3;

	List<Transform> pickups;
	List<Transform> invertibility;

	// Use this for initialization
	void Start () {
		pickups = new List<Transform> ();
		invertibility = new List<Transform> ();
		load_level(@"level1.txt");
		camera_pos = camera.transform.position.y;
		req_direction = -1;
		pac_direction = 1;
		dead = false;
		Global.ready_to_go = 100;
		anim = gameObject.GetComponent<Animator> ();

		for (int i = 0; i < 5; i++) {
			enemy [i].SetActive (false);
			enemy_scripts[i] = enemy[i].GetComponent<enemy_movement>();
		}
			
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.ready_to_go == 0) {

			if (Input.GetKey (KeyCode.LeftArrow))
				req_direction = 0;
			else if (Input.GetKey (KeyCode.RightArrow))
				req_direction = 1;
			else if (Input.GetKey (KeyCode.UpArrow))
				req_direction = 2;
			else if (Input.GetKey (KeyCode.DownArrow))
				req_direction = 3;
			else if (Input.GetKey (KeyCode.R))
				new_life ();


			int matrix_x = (int)(transform.position.x * 2);
			int matrix_y = (int)(transform.position.y * 2);
			int current_pos = Global.levelmatrix [matrix_y, matrix_x];


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


				if (req_direction == 0)
					transform.localEulerAngles = new Vector3 (0, 180, 0);
				else if (req_direction == 1)
					transform.localEulerAngles = new Vector3 (0, 0, 0);
				else if (req_direction == 2)
					transform.localEulerAngles = new Vector3 (0, 0, 90);
				else if (req_direction == 3)
					transform.localEulerAngles = new Vector3 (0, 180, 270);

				pac_direction = req_direction;
				req_direction = -1;
			}

			/*if (pac_direction == 0 && transform.position.x > 2.0f)
			camera.transform.Translate (-0.002f, 0, 0);
		else if (pac_direction == 1 && transform.position.x < 8.0f)
			camera.transform.Translate (0.002f, 0, 0);*/


			camera.transform.Translate (0, 0.003f, 0);

			for (int i = 0; i < 5; i++) {
				if (!enemy [i].active && Time.frameCount % 500 == 0) {
					enemy [i].SetActive (true);
					//enemy_scripts [i].count_down = 50;
					enemy_scripts [i].respawn_enemy ();
					break;
				}
			}

			if (Global.inv_time > 0)
				Global.inv_time--;

			if (!dead && (pac_direction == 0 && (current_pos != 3 && current_pos != 4 && current_pos != 8 && current_pos != 1 || (int)(transform.position.x * 40) % 20 != 0) ||
				pac_direction == 1 && (current_pos != 2 && current_pos != 5 && current_pos != 9 && current_pos != 1 || (int)(transform.position.x * 40) % 20 != 0) ||
				pac_direction == 2 && (current_pos != 4 && current_pos != 5 && current_pos != 6 && current_pos != 0 || (int)(transform.position.y * 40) % 20 != 0) ||
				pac_direction == 3 && (current_pos != 2 && current_pos != 3 && current_pos != 7 && current_pos != 0 && current_pos != 11 || (int)(transform.position.y * 40) % 20 != 0)))
				transform.Translate (speed, 0, 0);

		} else {
			Global.ready_to_go--;
			if (Global.ready_to_go == 1)
				ready_to_go.gameObject.SetActive(false);
		}

	}

	void respawn_player(float x, float y)
	{
		transform.position = new Vector2 (x, y);
		
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

		if (pac_direction == 0)
			transform.localEulerAngles = new Vector3(0,180,0);
		else if (pac_direction == 1)
			transform.localEulerAngles = new Vector3(0,0,0);
		else if (pac_direction == 2)
			transform.localEulerAngles = new Vector3(0,0,90);
		else if (pac_direction == 3)
			transform.localEulerAngles = new Vector3(0,180,270);
	}

	void new_life()
	{	
		pac_direction = 1;
		
		int i, j=((int)camera.transform.position.y)*2, k;
		
		//set_camera_y(camera_offset.y-240);
		if (Global.classic)
			respawn_player(Global.startcoord_x/2.0f, Global.startcoord_y/2.0f);
		else
		{
			for (k=0; Global.levelmatrix[j,k] == -1; k++);
			respawn_player(k/2.0f, j/2.0f);
		}

		for (i=0; i<5; i++)
			if (enemy[i].gameObject.active)
			enemy_scripts[i].respawn_enemy();

		life--;
		life_text.text = "X " + life.ToString ();

		ready_to_go.gameObject.SetActive (true);
		Global.ready_to_go = 100;

		dead = false;
		anim.SetBool ("dead", false);
	}

	void load_level(string path) {

		Global.level_height = 0;

		string[] lines = System.IO.File.ReadAllLines(path);

		int i;

		for (i = 0; i<500; i++)
			for (int j = 0; j<500; j++)
				Global.levelmatrix [i, j] = -1;

		i = 0;
		bool level_load = true;

		foreach (string line in lines) {

			if (level_load) {
				if (i == 0)
					level_graphics = line;
				else if (i == 1) {
					string [] coords = lines [1].Split (' ');
					Global.startcoord_x = int.Parse (coords [0]);
					Global.startcoord_y = int.Parse (coords [1]);
				}
				else if (i == 2) {
					string [] coords = lines[2].Split (' ');
					Global.endcoord_x = int.Parse (coords [0]);
					Global.endcoord_y = int.Parse (coords [1]);
				}
				else if ( i == 3) 
					Global.classic = bool.Parse (lines [3]);
				else {
					if (line != "*") {
						Global.level_height++;
						for (int j=0; j<line.Length; j++) {
							if (line[j] != '\n' && line[j] != ' ')
							{
								Global.levelmatrix[i-4,j] = line[j]-48;
								Instantiate(tiles[Global.levelmatrix[i-4,j]], new Vector3(j*0.5f, (i-4)*0.5f, 0.0f), Quaternion.identity);
							}
						}
					}
					else {
						level_load = false;
						i=0;
					}
				}
			}
			else {
				if (line == "*")
					break;
				for (int j=0; j<line.Length; j++) {
					if (line[j] == '1') {
						Transform new_obj;
						if (Global.levelmatrix[i/2,j/2] == 0)
							new_obj = (Transform) Instantiate(pickup_graph[0], new Vector3(j*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						else if (Global.levelmatrix[i/2,j/2] == 1)
							new_obj = (Transform) Instantiate(pickup_graph[0], new Vector3(j*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						else
							new_obj = (Transform) Instantiate(pickup_graph[0], new Vector3(j*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);

						pickups.Add(new_obj);
					}
					else if (line[j] == '2') {
						Transform new_obj;
						if (Global.levelmatrix[i/2,j/2] == 0)
							new_obj = (Transform) Instantiate(pickup_graph[1], new Vector3(j*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						else if (Global.levelmatrix[i/2,j/2] == 1)
							new_obj = (Transform) Instantiate(pickup_graph[1], new Vector3(j*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						else
							new_obj = (Transform) Instantiate(pickup_graph[1], new Vector3(j*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						
						invertibility.Add(new_obj);
					}
				}
			}

			i++;

		}

		transform.position = new Vector2 (Global.startcoord_x / 2.0f, Global.startcoord_y / 2.0f);
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "enemy" && !dead && coll.gameObject.active) {
			if (Global.inv_time == 0) {
				anim.SetBool ("dead", true);
				dead = true;
			} else {
				coll.gameObject.SetActive (false);
				score += 5;

				enemy_dead.gameObject.SetActive(true);
				enemy_dead.transform.position = coll.transform.position;
			}
		} else if (coll.gameObject.tag == "pickup") {
			Destroy (coll.gameObject);
			score++;
			score_text.text = score.ToString ();
		} else if (coll.gameObject.tag == "invertibility") {
			Destroy (coll.gameObject);
			score += 5;
			Global.inv_time = 500;
			score_text.text = score.ToString ();
		}
	}

}