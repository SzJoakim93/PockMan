using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogOk : MonoBehaviour {

	public Text Title;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ClickOk() {
		gameObject.SetActive(false);
	}

	public void InvokeDialog(string title) {
		gameObject.SetActive(true);
		Title.text = title;
	}
}
