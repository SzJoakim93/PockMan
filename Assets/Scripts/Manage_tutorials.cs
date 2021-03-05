using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manage_tutorials : MonoBehaviour {

    public Text main_text;
    public Text DropText;
    public PopupText TutorialText;
    public GameObject GameTitle;
    
    bool main_text_active = false;

    public Text upgrade_btn;
    public Text dropping_btn;

    public Image drop_btn1;
    public Image drop_btn2;
    public Image drop_btn3;

    public Image own_place;
    public Text free_card_text;

    public GameObject back_btn;
    public Language_manager language_Manager;

    int tutorial_frame = 0;

    Color red = new Color(1.0f, 0.0f, 0.0f);
    Color white = new Color(1.0f, 1.0f, 1.0f);
    Color gold = new Color (1.0f, 0.82f, 0.22f, 1.0f);
    Color orange = new Color (1.0f, 0.82f, 0.25f, 1.0f);

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Global.global_points > 2000 && Global.Ammo.Level == 1 &&
            Global.Thunder.Level == 1 &&
            Global.Mine.Level == 1 &&
            Global.Magneton.Level == 1)
        {
            if (!main_text_active)
            {
                setActiveTutorialText(true);
                main_text.text = language_Manager.GetTextByValue("Tutorial11");
                upgrade_btn.color = red;
            }
            
        }
        else if (Global.global_stars > 25 && Global.global_stars < 31)
        {

            if (Global.level_menu == 3 || Global.level_menu == 4)
            {
                if (Global.own_cards[0] != -1 && tutorial_frame == 0)
                    tutorial_frame = 1;

                if (tutorial_frame > 0 && tutorial_frame < 10) {
                    if (!TutorialText.isActive()) {
                        if (tutorial_frame == 1) {
                            own_place.color = red;
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial1"));
                            back_btn.SetActive(false);
                            DropText.gameObject.SetActive(false);
                        } else if (tutorial_frame == 2)
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial2"));
                        else if (tutorial_frame == 3)
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial3"));
                        else if (tutorial_frame == 4)
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial4"));
                        else if (tutorial_frame == 5) {
                            own_place.color = white;
                            drop_btn1.color = red;
                            drop_btn2.color = red;
                            drop_btn3.color = red;
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial5"));
                        } else if (tutorial_frame == 6)
                        {
                            drop_btn1.color = white;
                            drop_btn2.color = white;
                            drop_btn3.color = white;
                            free_card_text.color = red;
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial6"));
                        } else if (tutorial_frame == 7)
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial7"));
                        else if (tutorial_frame == 8)
                            TutorialText.SetText(language_Manager.GetTextByValue("Tutorial8"));
                        else if (tutorial_frame == 9) {
                            DropText.gameObject.SetActive(true);
                            back_btn.SetActive(true);
                        }

                        if (tutorial_frame < 9)
                            TutorialText.ActivateForReadableTime();
                        tutorial_frame++;
                    }
                }
            }
            else if ((Global.level_menu == 1 || Global.level_menu == 2) && !main_text_active)
            {
                setActiveTutorialText(true);
                main_text.text = language_Manager.GetTextByValue("Tutorial10");
                dropping_btn.color = red;
            }
            
        }
        else if (main_text_active)
        {
            setActiveTutorialText(false);
            upgrade_btn.color = orange;
        }
	}

    void setActiveTutorialText(bool x) {
        GameTitle.SetActive(!x);
        main_text_active = x;
        main_text.gameObject.SetActive(x);
    }
}
