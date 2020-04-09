using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Global : MonoBehaviour {

	public static string level_graphics;
	//public static string level_path="level2.txt";
	public static int [,] levelmatrix = new int[500, 20];
	public static int level_height;
	public static float view_range_top = 5.0f;
	public static float view_range_bottom = -2.8f;
	public static bool classic;
	public static int startcoord_x, startcoord_y, endcoord_x, endcoord_y;
	public static int max_enemy = 7;
	public static float enemy_speed;
	public static int enemy_animation_offset = 0;

	public static int inv_time=0;
	public static int ready_to_go;
	public static int enemy_active;
	public static int pause_enemy=0, rewind=0, pause=0, double_score=0, magneton=0, ammo=0, mines=0;
	public static int max_thunder=2;
	public static int safe_counter=0;
	public static int clone_reamining=0;

	public static int max_pause = 300, max_double = 500, max_magneton = 1000, max_ammo = 3, max_mines = 3, max_safe = 50, max_clone = 1500, max_fruits = 10, max_convert = 1;
	public static int level_pause = 1, level_double = 1, level_magneton = 1, level_ammo = 1, level_mines = 1, level_safe = 1, level_clone = 1, level_fruits = 1, level_convert = 1, level_thunder = 1;


	public static int dropping_mode=0;

	public static int score=0; //current scroe of the game
	public static int remaining=0; //the ramaining scores of level
	public static int max_score=0; //all avaiable scores on the level

	public static int global_points=0;
	public static int global_stars = 0;

	public static int selected_upgrade = -1;

	public static int[] own_cards = new int[4] ;
	public static int [] card_remaining = new int[4];
	public static int ac = -1;
	public static int next_card_stars = 25;

	public static bool pause_game;

	public static int unlocked_levels = 1;
	public static int unlocked_clevels = 1;

	public static bool isPlayed = false;

	public static int level_menu = 0;

    public static XmlNodeList element_list;

    public static string option_lang;
    public static string option_mus;
    public static string option_switch;

    public static string current_language = "HUN";
    public static bool music_enabled = true;

	#if UNITY_EDITOR
	public static string default_path = Application.dataPath;
	#elif UNITY_ANDROID
	public static string default_path =  "jar:file://" + Application.dataPath + "!/assets/";
	#elif UNITY_IPHONE
		public static string default_path = Application.dataPath + "/Raw";
	#else
	public static string default_path = Application.dataPath + "/";
	#endif
	
	//public static string [] levels = new string[] {"level1.txt", "level2.txt", "level3.txt", "level4.txt", "level5.txt"};
	public static int level = 30;

	public static List<Vector4> speed_zones;
	public static List<Vector4> slow_zones;
    public static List<Vector2> safety_coords;
	public static List<GameObject> enemies;

    public static bool Free_slot_exist()
    {
        for (int i = 0; i < 4; i++)
            if (Global.own_cards[i] == -1)
                return true;
        return false;
    }

    public static string GetTextByValue(string attribute, string value)
    {
        foreach (XmlNode element in Global.element_list)
            if (element.Attributes["name"].Value == value)
                return element.InnerText;
        return null;
    }

}
