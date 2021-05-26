using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ContextMenu : MonoBehaviour
{
    [Serializable]
    public class ItemClickEvent : UnityEvent { }

    [Serializable]
    public class Item
	{
        [HideInInspector]
        public Button button;
        [SerializeField]
        public string text;
        [SerializeField]
        ItemClickEvent m_onItemClicked = new ItemClickEvent();

        public ItemClickEvent onItemClick { get { return m_onItemClicked; } set { onItemClick = value; } }
	}

    [Serializable]
    public class ItemCondition : UnityEvent { }

    [Serializable]
    public class ConditionalItem
	{
        [HideInInspector]
        public Button button;
        [SerializeField]
        public string text;
        [SerializeField]
        public ItemClickEvent m_onItemClicked = new ItemClickEvent();
    }

    [SerializeField]
    Button m_baseButton;
    [SerializeField]
    Item[] m_items;
    [SerializeField]
    ConditionalItem[] conditionalItems;

    bool visible;

 //   //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
 //   private void OnValidate()
	//{
	//}

	// Start is called before the first frame update
	void Start()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (m_items.Length - 1) * m_baseButton.GetComponent<RectTransform>().rect.height);

        for (int i = 0; i < m_items.Length; i++)
		{
            m_items[i].button = Instantiate(m_baseButton.gameObject, this.transform).GetComponent<Button>();
            m_items[i].button.transform.GetChild(0).GetComponent<Text>().text = m_items[i].text;
            m_items[i].button.transform.position += new Vector3(0.0f, -m_items[i].button.transform.GetComponent<RectTransform>().rect.height*((float)i) + (3.0f*((float)i)), 0.0f); 
            m_items[i].button.gameObject.SetActive(true);
            m_items[i].button.onClick.AddListener(m_items[i].onItemClick.Invoke);
            m_items[i].button.onClick.AddListener(HideMenu);
        }
        HideMenu();
    }

	// Update is called once per frame
	void Update()
	{
        //Debug.Log("(" + Input.mousePosition.x.ToString("0.00") + ", " + Input.mousePosition.y.ToString("0.00") + ")");

  //      if (Input.GetMouseButtonDown(1))
		//{
		//	ShowMenu();
		//}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            HideMenu();
		}
        //TODO: Add code to check if one of the conditions are met to show the conditional button
        conditionalItems[0].m_onItemClicked.GetPersistentTarget(0).GetType().GetMethod(conditionalItems[0].m_onItemClicked.GetPersistentMethodName(0)).Invoke(conditionalItems[0].m_onItemClicked.GetPersistentTarget(0), null);
    }

    public void ShowMenu()
	{
        //gameObject.SetActive(true);
        if ((Input.mousePosition.x > Screen.width / 2) && (Input.mousePosition.y > Screen.height / 2))
        {
            GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().pivot = new Vector2(1, 1);
        }
        else if ((Input.mousePosition.x > Screen.width / 2) && (Input.mousePosition.y < Screen.height / 2))
        {
            GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().pivot = new Vector2(1, 0);
        }
        else if ((Input.mousePosition.x < Screen.width / 2) && (Input.mousePosition.y < Screen.height / 2))
        {
            GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        }
        else if ((Input.mousePosition.x < Screen.width / 2) && (Input.mousePosition.y > Screen.height / 2))
        {
            GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        }
        visible = true;
        for (int j = 1; j < transform.childCount; j++)
        {
            transform.GetChild(j).gameObject.SetActive(true);
        }
        transform.position = Input.mousePosition;
	}

    public void HideMenu()
	{
        //gameObject.SetActive(false);
        visible = false;
        for (int j = 1; j < transform.childCount; j++)
        {
            transform.GetChild(j).gameObject.SetActive(false);
        }
    }
}
