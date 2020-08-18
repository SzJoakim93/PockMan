using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Global : MonoBehaviour {

	public static string level_graphics;
	public static int [,] levelmatrix = new int[500, 20];
	public static int level_height;
	public static float view_range_top = 5.0f;
	public static float view_range_bottom = -2.8f;
	public static bool classic;
	public static int startcoord_x, startcoord_y, endcoord_x, endcoord_y;
	public static int max_enemy = 7;
	public static float enemy_speed;
	public static int enemy_animation_offset = 0;
	public static int enemy_active;
	public static int safe_counter=0;
	public static int clone_reamining=0;
	public static PowerUpPieceBased Ammo;
	public static PowerUpPieceBased Mine;
	public static PowerUpPieceBased Thunder;
	public static PowerUpPieceBased ConvertEnemy;
	public static PowerUpPieceBased SafeZone;
	public static PowerUpPieceBased DiamondRush;
	public static PowerUpTimeBased Invertibility;
	public static PowerUpTimeBased PauseEnemy;
	public static PowerUpTimeBased ClonePlayer;
	public static PowerUpTimeBased Magneton;
	public static PowerUpTimeBased DoubleScore;
	public static PowerUpTimeBased LevelPause;
	public static PowerUpTimeBased LevelRewind;
	

	public static int dropping_mode=0;

	public static int score=0; //current scroe of the game
	public static int remaining=0; //the ramaining scores of level
	public static int max_score=0; //all avaiable scores on the level

	public static int global_points=0;
	public static int global_stars = 0;

	public static int[] own_cards = new int[4] ;
	public static int [] card_remaining = new int[4];
	public static int ac;
	public static int next_card_stars;

	public static bool pause_game;

	public static int unlocked_levels;
	public static int unlocked_clevels;

	public static bool isPlayed = false;

	public static int level_menu = 0;

    public static string current_language;
    public static bool music_enabled;
	public static int controll_type;

	public static bool followEnemyAlive;
	public static bool blockenemyAlive;
	public static int selectedCharacter;
	public static int level = 30;
	public static int max_level = 30;
	public static float enemy_rise;
	public static bool isStarted = false;

	public static List<Vector4> speed_zones;
	public static List<Vector4> slow_zones;
    public static List<Vector2> safety_coords;
	public static List<enemy_movement> enemies;
	public static int tutorial;

	/*public static bool LevelCompletedEvent;
	public static bool GameOverEvent;
	public static bool ReadyToGoEvent;*/

    public static bool Free_slot_exist()
    {
        for (int i = 0; i < 4; i++)
            if (Global.own_cards[i] == -1)
                return true;
        return false;
    }

}
