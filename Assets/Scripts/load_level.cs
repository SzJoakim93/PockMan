﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class load_level : MonoBehaviour {

	public Transform player;
	public Transform Pickup_pack;
	public Transform Tile_pack;
	public GameObject Minor_pack;
    public GameObject flower_pack;
	public GameObject fire;

	public Transform turret;
	public Transform speed_sign1;
	public Transform speed_sign2;
	public Camera main_camera;

	Transform [] pickup_graph;
	Transform [] tiles;
	Transform [] minor_objects;
    Transform[] flowers;

	string level_graphics;
	
	string[] level_list = {"Textures/classic.bmp", "Textures/classic2.bmp", "Textures/classic3.bmp", "Textures/classic4.bmp", "Textures/classic5.bmp", //4.
		"Textures/classic6.bmp", "Textures/classic7.bmp", "Textures/classic8.bmp", "Textures/classic9.bmp", "Textures/classic10.bmp", //9.
		"Textures/town1.bmp", "Textures/town2.bmp", "Textures/town3.bmp", //12.
		"Textures/garden1.bmp", "Textures/garden2.bmp", "Textures/garden3.bmp", //15.
		"Textures/snow1.bmp", "Textures/snow2.bmp", //17.
		"Textures/desert1.bmp", "Textures/desert2.bmp", //19.
		"Textures/grave1.bmp", "Textures/grave2.bmp", //21
		"Textures/medieval1.bmp", "Textures/medieval2.bmp", "Textures/medieval3.bmp", //24
		"Textures/digital1.bmp", "Textures/digital2.bmp", "Textures/digital3.bmp", "Textures/digital4.bmp", "Textures/digital5.bmp", //29
		"Textures/space1.bmp", "Textures/space2.bmp", "Textures/space3.bmp", "Textures/space4.bmp", "Textures/space5.bmp" //34
	};

	int minor_offset = 0;

	// Use this for initialization
	void Start () {
		tiles = Tile_pack.GetComponentsInChildren<Transform>();
		pickup_graph = Pickup_pack.GetComponentsInChildren<Transform> ();
		minor_objects = Minor_pack.GetComponentsInChildren<Transform> ();
        flowers = flower_pack.GetComponentsInChildren<Transform>();

		Global.speed_zones = new List<Vector4> ();
		Global.slow_zones = new List<Vector4> ();


		load ();
	}

    public TextAsset[] level_paths;
	
	// Update is called once per frame
	void Update () {
	
	}

	public void load() {
		
		Global.level_height = 0;
        Global.remaining = 0;
        Global.max_score = 0;
		
		/*string level;
        
        if (Global.level < 100)
            level = Global.default_path + "/Levels/level" + Global.level + ".txt";
        else
            level = Global.default_path + "/Levels/clevel" + (Global.level - 100) + ".txt";*/
		//TextAsset level_txt = (TextAsset)Resources.Load("Levels/level" + Global.level + ".txt");
		//string[] lines = System.IO.File.ReadAllLines(level /*Global.levels[Global.level]*/);
        string[] lines = Regex.Split(level_paths[Global.level].text, "\r\n");
		
		int i;
		int level_graphics_int = -1;
		
		for (i = 0; i<500; i++)
			for (int j = 0; j<20; j++)
				Global.levelmatrix [i, j] = -1;
		
		i = 0;
		bool level_load = true;

		List<Vector3> turrets = new List<Vector3> ();

		foreach (string line in lines) {
			int k=0;
			if (level_load) {
				if (i == 0) {
					level_graphics = line;
					do
						level_graphics_int++;
					while (level_graphics_int < 22 && level_graphics != level_list[level_graphics_int]);

					if (level_graphics_int > 12 && level_graphics_int < 16)
						minor_offset = 4;
					else if (level_graphics_int > 15 && level_graphics_int < 18)
						minor_offset = 15;
					else if (level_graphics_int > 17 && level_graphics_int < 20)
						minor_offset = 16;
                    else if (level_graphics_int > 19 && level_graphics_int < 22)
                        minor_offset = 20;

					if (level_graphics_int == 5)
						main_camera.backgroundColor = new Color(0.375f, 0.25f, 0);
					else if (level_graphics_int == 6)
						main_camera.backgroundColor = new Color(0.125f, 0.25f, 0.5f);
					else if (level_graphics_int == 8)
						main_camera.backgroundColor = new Color(0.625f, 0, 0);
					else if (level_graphics_int == 7 || level_graphics_int == 9 || level_graphics_int == 11 || level_graphics_int == 12)
						main_camera.backgroundColor = new Color(0.125f, 0.75f, 0.25f);
					else if (level_graphics_int == 11 || level_graphics_int == 14 || level_graphics_int == 21)
						main_camera.backgroundColor = new Color(0, 0.25f, 0);
					else if (level_graphics_int == 10 || level_graphics_int == 13 || level_graphics_int == 15)
						main_camera.backgroundColor = new Color(0, 0.5f, 0);
					else if (level_graphics_int == 16)
						main_camera.backgroundColor = new Color(1, 1, 1);
					else if (level_graphics_int == 17)
						main_camera.backgroundColor = new Color(0.375f, 0.625f, 0.75f);
					else if (level_graphics_int == 18 || level_graphics_int == 19)
						main_camera.backgroundColor = new Color(0.63f, 0.51f, 0.27f);
					else if (level_graphics_int == 20)
						main_camera.backgroundColor = new Color(0.9f, 0.8f, 0.6f);
					
				}
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
					Global.classic = lines [3] == "1";
				else {
					if (line != "*") {
						Global.level_height++;

						for (int j=0; j<line.Length; j++) {
							if (line[j] != '\n' && line[j] != ' ')
							{
								if (line[j] == 'f' || line[j] == 's') {
									k++;
									if (line[j] == 'f')
										Instantiate(speed_sign1, new Vector3((j-k)*0.5f, (i-4)*0.5f, 0.0f), Quaternion.identity);
									else if (line[j] == 's')
										Instantiate(speed_sign2, new Vector3((j-k)*0.5f, (i-4)*0.5f, 0.0f), Quaternion.identity);
								}
								else if (line[j] == '<' || line[j] == '>' || line[j] == 'v' || line[j] == 'a') {
									/*if (line[j] == '<')
										Instantiate(turret, new Vector3(j*0.5f, (i-4)*0.5f, 0.0f), Quaternion.identity);
									else if (line[j] == '>')
										Instantiate(turret, new Vector3(j*0.5f, (i-4)*0.5f, 0.0f), Quaternion.Euler(0, 0, 180));
									else if (line[j] == 'a')
										Instantiate(turret, new Vector3(j*0.5f, (i-4)*0.5f, 0.0f), Quaternion.Euler(0, 0, 90));
									else if (line[j] == 'v')
										Instantiate(turret, new Vector3(j*0.5f, (i-4)*0.5f, 0.0f), Quaternion.Euler(0, 0, 270));*/

									if (line[j] == '<')
										turrets.Add(new Vector3((j-k)*0.5f, (i-4)*0.5f, 0.0f));
									else if (line[j] == '>')
										turrets.Add(new Vector3((j-k)*0.5f, (i-4)*0.5f, 180.0f));
									else if (line[j] == 'a')
										turrets.Add(new Vector3((j-k)*0.5f, (i-4)*0.5f, 90.0f));
									else if (line[j] == 'v')
										turrets.Add(new Vector3((j-k)*0.5f, (i-4)*0.5f, 270.0f));
								}
								else if (line[j] >= 'A' && line[j] <= 'L')
									Instantiate(minor_objects[line[j]-'A'+minor_offset+1], new Vector3((j-k)*0.5f + 0.25f, (i-4)*0.5f - 0.25f, 0.0f) , Quaternion.identity);
                                else if (line[j] >= 'M' && line[j] <= '\\')
                                    Instantiate(flowers[line[j]-'L'], new Vector3((j-k)*0.5f + 0.25f, (i-4)*0.5f - 0.25f, 0.0f) , Quaternion.identity);
								else {
									Global.levelmatrix[i-4,j-k] = line[j]-48;
									Instantiate(tiles[Global.levelmatrix[i-4,j-k]+level_graphics_int*12+1], new Vector3((j-k)*0.5f, (i-4)*0.5f, 0.0f), Quaternion.identity);
								}
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

					if (line[j] >= '0' && line[j] < 'D') {
						//if (Global.levelmatrix[i/2,(j-k)/2] == 0)
							Instantiate(pickup_graph[line[j]-48], new Vector3((j-k)*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						/*else if (Global.levelmatrix[i/2,j/2] == 1)
							Instantiate(pickup_graph[line[j]-48], new Vector3((j-k)*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);
						else
							Instantiate(pickup_graph[line[j]-48], new Vector3((j-k)*0.25f, (i-1)*0.25f, 0.0f), Quaternion.identity);*/

                            if (line[j] == '1')
                            {
                                Global.remaining++;
                                Global.max_score++;
                            }
                            else if (line[j] == '2')
                                Global.max_score += 10;
                            else if (line[j] > '2' && line[j] < '7')
                                Global.max_score += 100;
					}
				}
			}
			
			i++;
			
		}
		
		player.position = new Vector2 (Global.startcoord_x / 2.0f, Global.startcoord_y / 2.0f);

        if (player.position.x < 2.8f)
			main_camera.transform.position = new Vector3(2.8f, main_camera.transform.position.y, main_camera.transform.position.z);
		else if (player.position.x > 5.2f)
			main_camera.transform.position = new Vector3(5.2f, main_camera.transform.position.y, main_camera.transform.position.z);
		else
			main_camera.transform.position = new Vector3(player.position.x, main_camera.transform.position.y, main_camera.transform.position.z);
		
		if (Global.classic)
			fire.SetActive (false);

		foreach (Vector3 tr in turrets) 
			Instantiate(turret, new Vector3(tr.x, tr.y, 0.0f), Quaternion.Euler(0, 0, Mathf.RoundToInt(tr.z)));

		//Global.max_score = Global.remaining;
        Debug.Log(Global.max_score);

        if (Global.classic)
            Global.enemy_speed = 1.4f;
        else
            Global.enemy_speed = 1.3f;

	} 
}
