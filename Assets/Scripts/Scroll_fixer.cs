﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scroll_fixer : MonoBehaviour {

	public RectTransform rectTransform;

	public bool horizontal;
	public bool vertical;

	float movement_range;
	int stateValue;
    public float packages;

	float movement_level;
	float next_level;
    float init_level;
    float end_level;
	public string key;
	public GameObject scrollPackage;
	public Sprite scrollPointerActive;
	public Sprite scrollPointerInActive;
	Image [] scrollPointers;

	// Use this for initialization
    void Start()
    {
		scrollPointers = scrollPackage.GetComponentsInChildren<Image>();

        if (horizontal)
        {
			movement_range = rectTransform.position.x*2;
            movement_level = next_level = init_level = rectTransform.position.x;
            end_level = init_level - movement_range * packages;
			stateValue = PlayerPrefs.GetInt(key, 0);
			scrollPointers[-stateValue].sprite = scrollPointerActive;
			if (stateValue != 0) {
				movement_level = next_level += stateValue * movement_range;
				rectTransform.position = new Vector3(movement_level, rectTransform.position.y, rectTransform.position.z);
			}

        }
    }

	// Update is called once per frame
	void Update () {
	
		if (horizontal) {

			if (next_level >= movement_level - 1.0f && next_level <= movement_level + 1.0f) {
				if (next_level < init_level && rectTransform.position.x >  movement_level + 50.0f)
					leftClick();
				else if (rectTransform.position.x <  movement_level - 50.0f)
					rightClick();
					
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
			scrollPointers[-stateValue].sprite = scrollPointerInActive;
			stateValue++;
			scrollPointers[-stateValue].sprite = scrollPointerActive;
		}	
	}

	public void rightClick() {
		if (next_level > end_level) {
			next_level -= movement_range;
			scrollPointers[-stateValue].sprite = scrollPointerInActive;
			stateValue--;
			scrollPointers[-stateValue].sprite = scrollPointerActive;
		}
	}

	public void pointClick(int x) {
		scrollPointers[-stateValue].sprite = scrollPointerInActive;
		stateValue = -x;
		scrollPointers[-stateValue].sprite = scrollPointerActive;
		next_level = init_level + movement_range * stateValue;
	}

	public void saveState() {
		if (key != "")
			PlayerPrefs.SetInt(key, stateValue);
	}
}
