using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class manage_pickups : MonoBehaviour {

	public Text score_text;
	public Text Combo_text;
	public Text ammo_text;
	public Text mine_text;

	BoxCollider2D boxCollider;
	public GameObject [] enemy_ally;
	public bool isEnemy;
	public short enemy_index;
	public Transform enemy_dead;
	public Transform clone_man;
	public Transform thunder;
	public Transform dropper;
	
	public PopupText RateText;

	public Sound_manager sound_manager;
	public InGameTutorials Tutorials;
	public Language_manager language_Manager;


	float comboBeginTime = -10.0f;
	bool endCombo = true;
	int combo_count; //counting the points that picked up from the beginning of new combo
	int combo; //combo points that adding to the scores

	float card_bonus = 1.0f;
	string rateGood;
	string rateNice;
	string ratePerfect;
	string rateWonderfull;
	string rateMarvelous;

	enemy_movement [] enemy_scripts;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		combo_count = 0;
		combo = 0;

		if (Global.ac > -1) {
			if (Global.own_cards [Global.ac] == 2)
				PowerUpTimeBased.CardBonus = 1.2f;
			else if (Global.own_cards [Global.ac] == 7)
				PowerUpTimeBased.CardBonus= 1.3f;
			else if (Global.own_cards [Global.ac] == 12)
				PowerUpTimeBased.CardBonus = 1.4f;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (rateGood == null) {
			rateGood = language_Manager.GetTextByValue("RateGood");
			rateNice = language_Manager.GetTextByValue("RateNice");
			ratePerfect = language_Manager.GetTextByValue("RatePerfect");
			rateWonderfull = language_Manager.GetTextByValue("RateWonderfull");
			rateMarvelous = language_Manager.GetTextByValue("RateMarvelous");
		}

		if (Global.Magneton.IsEnd()) {
			boxCollider.size = new Vector2 (0.4f, 0.4f);
		}


		//End of combo points and adding to the scores
		if (!endCombo && Time.timeSinceLevelLoad - comboBeginTime > 1.0f) {

			endCombo = true;
			if (combo >= 250)
				activate_rate_text(rateMarvelous + "\n" + combo);
			else if (combo >= 200)
				activate_rate_text(rateWonderfull + "\n" + combo);
			else if (combo >= 150)
				activate_rate_text(ratePerfect + "\n" + combo);
			else if (combo >= 100)
				activate_rate_text(rateNice + "\n" + combo);
			else if (combo >= 50)
				activate_rate_text(rateGood + "\n" + combo);

			Global.score += combo;
			score_text.text = Global.score.ToString ();
			combo_count = 0;
			combo = 0;
			Combo_text.text = combo.ToString ();
		}

	}

    //appearing rate text
	public void activate_rate_text(string title) {
		RateText.SetText(title);
		RateText.Activate(3.0f);
	}

	void add_point(int value) {
		if (Global.DoubleScore.IsActive()) {
			Global.score += value*2;
			combo_count += value*2;
		} else {
			Global.score += value;
			combo_count += value;
		}

		score_text.text = Global.score.ToString ();
		comboBegin();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		 if (coll.gameObject.tag == "pickup") {

			Destroy (coll.gameObject);
			if (Global.DoubleScore.IsActive()) {
				Global.score += 2;
				combo_count += 2;
			}
			else {
				Global.score++;
				combo_count++;
			}

			if (combo_count % 10 == 0) {

                if (combo < 30)
                    combo += 10;
                else if (combo < 75)
                    combo += 15;
                else if (combo < 200)
                    combo += 25;
                else
                    combo += 50;
				Combo_text.text = combo.ToString();
			}

			score_text.text = Global.score.ToString ();
			Global.remaining--;
			comboBegin();
			sound_manager.PlaySound(0);

		} else if (coll.gameObject.tag == "invertibility") {
			Destroy (coll.gameObject);
			add_point(10);
			Global.Invertibility.Activate();
            sound_manager.PlaySound(1);
			if ((Global.tutorial & 8) != 8) {
				Tutorials.invokeTutorial(3);
				Global.tutorial = Global.tutorial | 8;
			}

		} else if (coll.gameObject.tag == "bananna") {
			Destroy (coll.gameObject);
			add_point(100);
            sound_manager.PlaySound(2);

		} else if (coll.gameObject.tag == "pineapple") {
			Destroy (coll.gameObject);
			add_point(100);
            sound_manager.PlaySound(2);

		} else if (coll.gameObject.tag == "melone") {
			Destroy (coll.gameObject);
			add_point(100);
            sound_manager.PlaySound(2);

		} else if (coll.gameObject.tag == "apple") {
			Destroy (coll.gameObject);
			add_point(100);
            sound_manager.PlaySound(2);

		} else if (coll.gameObject.tag == "rewind") {
			Destroy (coll.gameObject);
			Global.LevelRewind.Activate();
            comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "pause") {
			Destroy (coll.gameObject);
			Global.LevelPause.Activate();
			comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "pause_enemy") {
			Destroy (coll.gameObject);
			Global.PauseEnemy.Activate();
			comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "double_score") {
			Destroy (coll.gameObject);
			Global.DoubleScore.Activate();
			comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "magneton") {
			Destroy (coll.gameObject);
			Global.Magneton.Activate();
			comboBegin();
			boxCollider.size = new Vector2 (4.0f, 4.0f);
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "mine") {
			Destroy (coll.gameObject);
			Global.Mine.Charge();
			mine_text.text = "X " + Global.Mine.Quantity;
			comboBegin();
            sound_manager.PlaySound(4);
			if ((Global.tutorial & 4) != 4) {
				Tutorials.invokeTutorial(2);
				Global.tutorial = Global.tutorial | 4;
			}

		} else if (coll.gameObject.tag == "ammo") {
			Destroy (coll.gameObject);
			Global.Ammo.Charge();
			ammo_text.text = "X " + Global.Ammo.Quantity;
			comboBegin();
            sound_manager.PlaySound(4);
			if ((Global.tutorial & 2) != 2) {
				Tutorials.invokeTutorial(1);
				Global.tutorial = Global.tutorial | 2;
			}

		} else if (coll.gameObject.tag == "convert_enemy") {
			for (int i=0; i<Global.ConvertEnemy.MaxQuantity; i++) {
				var enemy_to_convert = getNearestenemy();
				if (enemy_to_convert != null)
					enemy_to_convert.GetComponent<enemy_movement>().convertToAlly();
			}
			
			Destroy (coll.gameObject);
			comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "clone") {
			Global.DoubleScore.Activate();
			Destroy (coll.gameObject);
			clone_man.gameObject.SetActive (true);
			clone_man.position = coll.transform.position;
			comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "extend_points") {
			Global.dropping_mode = 0;
			Destroy (coll.gameObject);
			dropper.position = new Vector2(8.0f, Mathf.Round(transform.parent.position.y) + 4.0f);
			dropper.gameObject.SetActive(true);
			comboBegin();
            sound_manager.PlaySound(4);

		} else if (coll.gameObject.tag == "extend_fruits") {
			Global.dropping_mode = 1;
			Destroy (coll.gameObject);
			dropper.position = new Vector2(8.0f, Mathf.Round(transform.parent.position.y) + 10.0f);
			dropper.gameObject.SetActive(true);
			comboBegin();
            sound_manager.PlaySound(4);
			
		} else if (coll.gameObject.tag == "safety_road") {
			Global.dropping_mode = 2;
			Destroy (coll.gameObject);
			Global.SafeZone.Charge();
			dropper.position = new Vector2( Mathf.Round(transform.parent.position.x), Mathf.Round(transform.parent.position.y));
			dropper.gameObject.SetActive(true);
			comboBegin();
            Global.safety_coords.Clear();
            sound_manager.PlaySound(4);
			
		} else if (coll.gameObject.tag == "thunder") {
			Destroy (coll.gameObject);
            sound_manager.PlaySound(3);

			for (short i = 0; i < Global.Thunder.MaxQuantity && Global.enemies.Count > 0; i++) {

				var enemy_to_dead = getNearestenemy();

				if (enemy_to_dead != null) {
					enemy_movement enemy_script = enemy_to_dead.GetComponent<enemy_movement>();
					Transform new_thunder = (Transform)Instantiate(thunder, transform.position, Quaternion.identity);
					new_thunder.position = enemy_to_dead.transform.position;
					new_thunder.gameObject.SetActive (true);
					Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
					new_dead.position = enemy_to_dead.transform.position;
					new_dead.gameObject.SetActive (true);

					if (enemy_script.enemy_type == 0)
						Global.followEnemyAlive = false;
					else if (enemy_script.enemy_type == 1)
						Global.blockenemyAlive = false;
					Global.enemies.Remove(enemy_to_dead);
					Destroy(enemy_to_dead.gameObject);
				}

				Global.score += 100;
			}

			comboBegin();

			
		} else if (isEnemy && coll.gameObject.tag == "enemy" && transform.parent != null && coll.gameObject != transform.parent.gameObject) {
				Global.score += 10;
				
				Transform new_dead = (Transform)Instantiate(enemy_dead, transform.position, Quaternion.identity);
				new_dead.position = coll.transform.position;
				new_dead.gameObject.SetActive (true);

				Global.enemies.Remove(coll.GetComponent<enemy_movement>());
				Destroy(coll.gameObject);
		}
	
	}

	enemy_movement getNearestenemy() {
		enemy_movement nearest_enemy = null;
		float distance = 999.0f;
		foreach (var enemy in Global.enemies) {
			if (enemy.IsAlly)
				continue;
			float curr_distance = Vector3.Distance (transform.position, enemy.transform.position);
			if (curr_distance < distance && enemy.gameObject.activeInHierarchy) {
				distance = curr_distance;
				nearest_enemy = enemy;
			}
		}

		return nearest_enemy;
	}

	void comboBegin() {
		comboBeginTime = Time.timeSinceLevelLoad;
		endCombo = false;
	}
}
