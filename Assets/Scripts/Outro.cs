using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Outro : MonoBehaviour {
	public PopupText MainTitle;
	public Language_manager languageManager;
	int clip = 0;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!MainTitle.isActive()) {
			if (clip >= 7) {
				ReturnToMenu();
				return;
			}
				
			if (Global.classic && (clip == 1 || clip == 5))
				MainTitle.GetComponent<Text>().text = languageManager.GetTextByValue("Outro" + clip + ".classic");
			else
				MainTitle.GetComponent<Text>().text = languageManager.GetTextByValue("Outro" + clip);
			MainTitle.ActivateForReadableTime();
			clip++;

			if (clip == 5 &&
				(Global.classic && Global.unlocked_levels == 30 ||
				!Global.classic && Global.unlocked_clevels == 30))
				clip++;
		}
	}

	public void ReturnToMenu() {
		if (Global.classic)
			Global.level_menu = 2;
		else
			Global.level_menu = 1;
		SceneManager.LoadScene("Menu");
	}
}
