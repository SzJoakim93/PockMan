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

	// Use this for initialization
	void Start () {
        Set_Language();
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

        Global.element_list = language_xml.DocumentElement.SelectNodes("/Languages/" + Global.current_language + "/element");

        foreach (XmlNode element in Global.element_list)
        {
            GameObject founded_obj = GameObject.Find(element.Attributes["name"].Value);

            if (founded_obj != null)
                founded_obj.GetComponent<Text>().text = element.InnerText;
        }

        foreach (var x in inactiveObjects)
            x.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	} 
}
