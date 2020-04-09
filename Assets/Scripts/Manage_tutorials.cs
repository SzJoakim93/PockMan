using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manage_tutorials : MonoBehaviour {

    public Text main_text;
    public Text drop_text;
    
    bool main_text_active = false;

    public RawImage upgrade_btn;
    public RawImage dropping_btn;

    public Image drop_btn1;
    public Image drop_btn2;
    public Image drop_btn3;

    public Image own_place;
    public Text free_card_text;

    public GameObject back_btn;

    int tutorial_frame = 0;

    Color red = new Color(1.0f, 0.0f, 0.0f);
    Color white = new Color(1.0f, 1.0f, 1.0f);
    Color gold = new Color (1.0f, 0.82f, 0.22f, 1.0f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Global.global_stars > 7 && Global.level_pause == 1 && Global.level_thunder == 1)
        {
            if (!main_text_active)
            {
                main_text.gameObject.SetActive(true);
                main_text_active = true;
                upgrade_btn.color = red;
            }
            
        }
        else if (Global.global_stars > 25 && Global.global_stars < 31)
        {

            if (Global.level_menu == 3 || Global.level_menu == 4)
            {
                if (Global.own_cards[0] != -1 && tutorial_frame == 0)
                    tutorial_frame = 1;

                if (tutorial_frame > 0 && tutorial_frame < 9000)
                {
                    tutorial_frame++;

                    if (tutorial_frame == 250)
                    {
                        drop_text.gameObject.SetActive(true);
                        drop_text.color = red;
                        own_place.color = red;
                        drop_text.text = "You can find your own\ndropping cards here.";
                        back_btn.SetActive(false);
                    }
                    else if (tutorial_frame == 700)
                        drop_text.text = "Simply click a card you want to activate.";
                    else if (tutorial_frame == 1300)
                        drop_text.text = "This card will make effect while playing";
                    else if (tutorial_frame == 1800)
                        drop_text.text = "Basically all cards can be used by 5 times.";
                    else if (tutorial_frame == 2300)
                        drop_text.text = "This will be decreased after completing a level successfully";
                    else if (tutorial_frame == 2800)
                        drop_text.text = "You can always own 4 cards only";
                    else if (tutorial_frame == 3300)
                    {
                        own_place.color = white;
                        drop_btn1.color = red;
                        drop_btn2.color = red;
                        drop_btn3.color = red;
                        drop_text.text = "Getting more cards you can always drop for points here";
                    }
                    else if (tutorial_frame == 4000)
                    {
                        drop_btn1.color = white;
                        drop_btn2.color = white;
                        drop_btn3.color = white;
                        free_card_text.color = red;
                        drop_text.text = "But it worth more to collect stars to get dropping cards for FREE.";
                    }
                    else if (tutorial_frame == 4500)
                        drop_text.text = "This title always shows the number of stars required for the next FREE dropping.";
                    else if (tutorial_frame == 5200)
                    {
                        free_card_text.color = gold;
                        drop_text.text = "There are 3 category of cards.";
                    }
                    else if (tutorial_frame == 5700)
                        drop_text.text = "Those are basic, silver and gold cards containing more and more powerfull effects";
                    else if (tutorial_frame == 6400)
                    {
                        drop_text.text = "Tap a card to activate";
                        drop_text.color = gold;
                        back_btn.SetActive(true);

                    }

                }
            }
            else if ((Global.level_menu == 1 || Global.level_menu == 2) && !main_text_active)
            {
                main_text.gameObject.SetActive(true);
                main_text_active = true;

                main_text.text = "You can always check your own dropping cards";

                dropping_btn.color = red;
            }
            
        }
        else if (main_text_active)
        {
            main_text.gameObject.SetActive(false);
            main_text_active = false;
            upgrade_btn.color = white;
        }
	}
}
