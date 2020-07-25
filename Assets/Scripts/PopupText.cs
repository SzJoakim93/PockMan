using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupText : MonoBehaviour {
	float startTime = 0;
	float interval = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - startTime > interval) {
			gameObject.SetActive(false);
		}
	}

	public void Activate(float _interval) {
		gameObject.SetActive(true);
		startTime = Time.timeSinceLevelLoad;
		this.interval = _interval;
	}

	public void ActivateForReadableTime() {
		int length = GetComponent<Text>().text.Length;
		Activate(length/10.0f + 3.0f);
	}

	public bool isActive() {
		return gameObject.activeInHierarchy;
	}
}
