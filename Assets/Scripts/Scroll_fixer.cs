using UnityEngine;
using System.Collections;

public class Scroll_fixer : MonoBehaviour {

	public RectTransform rectTransform;

	public bool horizontal;
	public bool vertical;

	public float movement_range;
    public float packages;

	float movement_level;
	float next_level;
    float init_level;
    float end_level;

	// Use this for initialization
    void Start()
    {
        if (horizontal)
        {
            movement_level = next_level = init_level = rectTransform.position.x;
            end_level = init_level - movement_range * packages;
        }
    }

	// Update is called once per frame
	void Update () {
	
		if (horizontal) {

			if (next_level >= movement_level - 1.0f && next_level <= movement_level + 1.0f) {
				if (next_level < init_level && rectTransform.position.x >  movement_level + 50.0f)
					next_level += movement_range;
				else if (next_level > end_level && rectTransform.position.x <  movement_level - 50.0f)
					next_level -= movement_range;
			}

			if (rectTransform.position.x <  next_level + 10.0f && rectTransform.position.x >  next_level - 10.0f)
				movement_level = next_level;

			if (rectTransform.position.x < next_level - 10.0f)
				rectTransform.Translate(10.0f, 0, 0);
			else if (rectTransform.position.x > next_level + 10.0f)
				rectTransform.Translate(-10.0f, 0, 0);
		}

	}
}
