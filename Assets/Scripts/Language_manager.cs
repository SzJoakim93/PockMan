using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Language_manager : MonoBehaviour {

    public TextAsset language;

    public GameObject canvas;

    XmlDocument language_xml;
    XmlNodeList elementList;
    XmlNodeList dropDownList;
    XmlNodeList dynamicTexts;

	// Use this for initialization
	void Start () {
        Set_Language();
        dynamicTexts = language_xml.DocumentElement.SelectNodes("/Languages/" + Global.current_language + "/dynamic-text");
	}

    public void Set_Language()
    {
        var allObjects = canvas.GetComponentsInChildren<Transform>(true);
        
        List<GameObject> inactiveObjects = new List<GameObject>();

        foreach (var x in allObjects)
            if (!x.gameObject.activeSelf)
                inactiveObjects.Add(x.gameObject);

        foreach (var x in inactiveObjects)
            x.SetActive(true);

        //allObjects.Select(x => { x.gameObject.SetActive(true); return x; });

        language_xml = new XmlDocument();
        language_xml.LoadXml(language.text);

        elementList = language_xml.DocumentElement.SelectNodes("/Languages/" + Global.current_language + "/element");

        foreach (XmlNode element in elementList)
        {
            GameObject founded_obj = GameObject.Find(element.Attributes["name"].Value);

            if (founded_obj != null)
                founded_obj.GetComponent<Text>().text = element.InnerText;
        }


        dropDownList = language_xml.DocumentElement.SelectNodes("/Languages/" + Global.current_language + "/drop-down");

        foreach (XmlNode dropDown in dropDownList)
        {
            GameObject founded_obj = GameObject.Find(dropDown.Attributes["name"].Value);

            if (founded_obj != null) {
                Dropdown dropDownObj = founded_obj.GetComponent<Dropdown>();


                foreach (XmlNode option in dropDown.ChildNodes) {
                    dropDownObj.options[int.Parse(option.Attributes["name"].Value)].text = option.InnerText;
                    dropDownObj.GetComponentInChildren<Text>().text = dropDownObj.options[dropDownObj.value].text;
                }
            }
        }


        foreach (var x in inactiveObjects)
            x.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetTextByValue(string value)
    {
        foreach (XmlNode element in dynamicTexts)
            if (element.Attributes["name"].Value == value)
                return element.InnerText;
        return null;
    }

}
