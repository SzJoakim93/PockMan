using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointAnimator : MonoBehaviour {

	public Text gain_points;
	public Text global_points;
	public PopupText NoDeadText;
	public Image [] stars;
	public GameObject [] sparks;
	public Sprite activeStar;
	public Language_manager language_Manager;
	string gainTitle;
	string allPointsTitle;
	int currentPoints;
	int currentRate;
	int [] trashold = new int[2];
	bool noDead;
	int rate;

	// Use this for initialization
	void Start () {
		currentPoints = 0;
		currentRate = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentPoints < Global.score) {
			currentPoints+=20;
			if (currentPoints >= Global.score) {
				currentPoints -= Global.score - currentPoints;
				if (noDead) {
					NoDeadText.Activate(2.5f);
					activateStar(currentRate);
					currentRate++;
				}
					
			}

			gain_points.text = gainTitle +  "\n" + (currentPoints/10).ToString ();

			for (int i = 0; i < trashold.Length; i++)
				if (currentPoints > trashold[i] && i == currentRate) {
					activateStar(i);
					currentRate++;
				}
		}
	}

	public void StartAnimation(int _rate, int _thrashold1, int _thrashold2, bool _noDead) {
		gainTitle = language_Manager.GetTextByValue("GainTitle");
		allPointsTitle = language_Manager.GetTextByValue("AllPointsTitle");
		global_points.text = allPointsTitle + "\n" + Global.global_points.ToString();
		this.rate = _rate;
		this.trashold[0] = _thrashold1;
		this.trashold[1] = _thrashold2;
		this.noDead = _noDead;
	}

	void activateStar(int i) {
		stars[i].sprite = activeStar;
		sparks[i].SetActive(true);			
	}
}
