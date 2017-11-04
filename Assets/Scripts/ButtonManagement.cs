using UnityEngine;
using System.Collections;

public class ButtonManagement : MonoBehaviour {

	// Use this for initialization
	public void new_game_btn() {
		Application.LoadLevel ("ingame");
	}

	public void quit_btn() {
		Application.Quit();
	}
}
