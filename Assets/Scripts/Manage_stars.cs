using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manage_stars : MonoBehaviour {

	Image [] stars;

	public Sprite active_star;
	//public Sprite inactive_star;

	public int level;
	public bool classic;
	// Use this for initialization
	void Start () {

		stars = gameObject.GetComponentsInChildren<Image> ();

		if (!classic && PlayerPrefs.HasKey("Level_star" + level)) {
				int rate = PlayerPrefs.GetInt("Level_star" + level);

				if (rate > 0)
					stars[0].sprite = active_star;
				if (rate > 1)
					stars[1].sprite = active_star;
				if (rate > 2)
					stars[2].sprite = active_star;
		}
		else if (classic && PlayerPrefs.HasKey("Classic_level_star" + level)) {
			int rate = PlayerPrefs.GetInt("Classic_level_star" + level);
			
			if (rate > 0)
				stars[0].sprite = active_star;
			if (rate > 1)
				stars[1].sprite = active_star;
			if (rate > 2)
				stars[2].sprite = active_star;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
