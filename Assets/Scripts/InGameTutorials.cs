using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameTutorials : MonoBehaviour {
    public GameObject tutorialPanel;
    public Text tutorialText;
    public GameObject [] tutorialGraph;
    public Language_manager language_Manager;

    public void tutorial_ok() {
        tutorialPanel.SetActive(false);
        Global.pause_game = false;
        Global.tutorial++;
        PlayerPrefs.SetInt("Tutorial", Global.tutorial);
    }

    public void invokeTutorial() {
        tutorialPanel.SetActive(true);
        Global.pause_game = true;
        switch (Global.tutorial)
        {
            case 0:
                tutorialText.text = language_Manager.GetTextByValue("Tutorial1");
                tutorialGraph[0].SetActive(true);
                tutorialGraph[1].SetActive(false);
                tutorialGraph[2].SetActive(false);
                break;
            case 1:
                tutorialText.text = language_Manager.GetTextByValue("Tutorial2");
                tutorialGraph[0].SetActive(false);
                tutorialGraph[1].SetActive(true);
                tutorialGraph[2].SetActive(false);
                break;
            case 2:
                tutorialText.text = language_Manager.GetTextByValue("Tutorial3");
                tutorialGraph[0].SetActive(false);
                tutorialGraph[1].SetActive(false);
                tutorialGraph[2].SetActive(true);
                break;
        }
    }
}