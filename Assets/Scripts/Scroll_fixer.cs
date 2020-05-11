using UnityEngine;
using System.Collections;

public class Scroll_fixer : MonoBehaviour {

	public RectTransform rectTransform;

	public bool horizontal;
	public bool vertical;

	float movement_range;
	int levelState;
    public float packages;

	float movement_level;
	float next_level;
    float init_level;
    float end_level;
	public string key;

	// Use this for initialization
    void Start()
    {
        if (horizontal)
        {
			movement_range = rectTransform.position.x*2;
            movement_level = next_level = init_level = rectTransform.position.x;
            end_level = init_level - movement_range * packages;
			levelState = PlayerPrefs.GetInt(key, 0);
			if (levelState != 0) {
				movement_level = next_level += levelState * movement_range;
				rectTransform.position = new Vector3(movement_level, rectTransform.position.y, rectTransform.position.z);
			}

        }
    }

	// Update is called once per frame
	void Update () {
	
		if (horizontal) {

			if (next_level >= movement_level - 1.0f && next_level <= movement_level + 1.0f) {
				if (next_level < init_level && rectTransform.position.x >  movement_level + 50.0f) {
					next_level += movement_range;
					levelState++;
				}
				else if (next_level > end_level && rectTransform.position.x <  movement_level - 50.0f) {
					next_level -= movement_range;
					levelState--;
				}
					
			}

			if (rectTransform.position.x <  next_level + 10.0f && rectTransform.position.x >  next_level - 10.0f)
				movement_level = next_level;

			if (rectTransform.position.x < next_level - 10.0f)
				rectTransform.Translate(10.0f, 0, 0);
			else if (rectTransform.position.x > next_level + 10.0f)
				rectTransform.Translate(-10.0f, 0, 0);
		}

	}

	public void leftClick() {
		if (next_level < init_level) {
			next_level += movement_range;
			levelState++;
		}	
	}

	public void rightClick() {
		if (next_level > end_level) {
			next_level -= movement_range;
			levelState--;
		}
	}

	public void saveState() {
		PlayerPrefs.SetInt(key, levelState);
	}
}
