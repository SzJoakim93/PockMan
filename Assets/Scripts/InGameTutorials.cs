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
        PlayerPrefs.SetInt("Tutorial", Global.tutorial);
    }

    /*
    1: smooth movement
    2: shooting
    4: mine
    8: invertibility
     */
    public void invokeTutorial(int x) {
        tutorialPanel.SetActive(true);
        Global.pause_game = true;

        for (int i = 0; i < tutorialGraph.Length; i++)
            tutorialGraph[i].SetActive(false);
        tutorialGraph[x].SetActive(true);
        tutorialText.text = language_Manager.GetTextByValue("Tutorial" + (x+1).ToString());
    }
}