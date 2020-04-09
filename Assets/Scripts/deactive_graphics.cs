using UnityEngine;
using System.Collections;

public class deactive_graphics : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void deactive() {
		//gameObject.SetActive (false);
		Destroy (gameObject);
	}
}
