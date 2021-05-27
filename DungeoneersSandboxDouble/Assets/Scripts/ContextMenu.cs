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
    public class ItemCondition 
    {
        [SerializeField]
        public GameObject _object;
        [SerializeField]
        public string componentName;
        [SerializeField]
        public string methodName;
        [SerializeField]
        public object[] Params;
    }

    [Serializable]
    public class ConditionalItem
	{
        [HideInInspector]
        public Button button;
        [SerializeField]
        ItemCondition m_condition = new ItemCondition();
        public ItemCondition Condition { get { return m_condition; } set { m_condition = value; } }
        
        [SerializeField]
        public Item[] falseItems;
        [SerializeField]
        public Item[] trueItems;
    }

    [SerializeField]
    Button m_baseButton;
    [SerializeField]
    Item[] m_items;
    [SerializeField]
    ConditionalItem[] m_conditionalItems;

    bool visible;

    GameObject menu;

 //   //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
 //   private void OnValidate()
	//{
	//}

	// Start is called before the first frame update
	void Start()
    {
        menu = this.gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (m_items.Length - 1) * m_baseButton.GetComponent<RectTransform>().rect.height);

        for (int i = 0; i < m_items.Length; i++)
		{
            m_items[i].button = Instantiate(m_baseButton.gameObject, menu.transform).GetComponent<Button>();
            m_items[i].button.transform.GetChild(0).GetComponent<Text>().text = m_items[i].text;
            m_items[i].button.transform.position += new Vector3(0.0f, -m_items[i].button.transform.GetComponent<RectTransform>().rect.height*((float)i) + (3.0f*((float)i)), 0.0f); 
            m_items[i].button.gameObject.SetActive(true);
            m_items[i].button.onClick.AddListener(m_items[i].onItemClick.Invoke);
            m_items[i].button.onClick.AddListener(HideMenu);
        }
        for(int j = 0; j < m_conditionalItems.Length; j++)
		{
            //string methodName = m_conditionalItems[j].Condition.GetPersistentMethodName(0);
            //Type type = m_conditionalItems[j].Condition._object.GetType();
            //bool activeCondition = (bool)m_conditionalItems[j].Condition._object.GetType().GetMethod(m_conditionalItems[j].Condition.methodName).Invoke(m_conditionalItems[j].Condition._object, null);
            Component comp = m_conditionalItems[j].Condition._object.GetComponent(m_conditionalItems[j].Condition.componentName);
            bool activeCondition = (bool)comp.GetType().GetMethod(m_conditionalItems[j].Condition.methodName).Invoke(m_conditionalItems[j].Condition._object.GetComponent(m_conditionalItems[j].Condition.componentName), null);
            for (int k = 0; k < m_conditionalItems[j].falseItems.Length; k++)
			{
                m_conditionalItems[j].falseItems[k].button = Instantiate(m_baseButton.gameObject, menu.transform).GetComponent<Button>();
                m_conditionalItems[j].falseItems[k].button.transform.GetChild(0).GetComponent<Text>().text = m_conditionalItems[j].falseItems[k].text;
                m_conditionalItems[j].falseItems[k].button.transform.position += new Vector3(0.0f, -m_conditionalItems[j].falseItems[k].button.transform.GetComponent<RectTransform>().rect.height * ((float)k) + (3.0f * ((float)k)), 0.0f);
                m_conditionalItems[j].falseItems[k].button.gameObject.SetActive(true);
                m_conditionalItems[j].falseItems[k].button.onClick.AddListener(m_conditionalItems[j].falseItems[k].onItemClick.Invoke);
                m_conditionalItems[j].falseItems[k].button.onClick.AddListener(HideMenu);
                m_conditionalItems[j].falseItems[k].button.gameObject.SetActive(!activeCondition);
            }
            for(int l = 0; l < m_conditionalItems[j].trueItems.Length; l++)
			{
                m_conditionalItems[j].trueItems[l].button = Instantiate(m_baseButton.gameObject, menu.transform).GetComponent<Button>();
                m_conditionalItems[j].trueItems[l].button.transform.GetChild(0).GetComponent<Text>().text = m_conditionalItems[j].trueItems[l].text;
                m_conditionalItems[j].trueItems[l].button.transform.position += new Vector3(0.0f, -m_conditionalItems[j].trueItems[l].button.transform.GetComponent<RectTransform>().rect.height * ((float)l) + (3.0f * ((float)l)), 0.0f);
                m_conditionalItems[j].trueItems[l].button.gameObject.SetActive(true);
                m_conditionalItems[j].trueItems[l].button.onClick.AddListener(m_conditionalItems[j].trueItems[l].onItemClick.Invoke);
                m_conditionalItems[j].trueItems[l].button.onClick.AddListener(HideMenu);
                m_conditionalItems[j].trueItems[l].button.gameObject.SetActive(!activeCondition);
            }
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
        //conditionalItems[0].m_onItemClicked.GetPersistentTarget(0).GetType().GetMethod(conditionalItems[0].m_onItemClicked.GetPersistentMethodName(0)).Invoke(conditionalItems[0].m_onItemClicked.GetPersistentTarget(0), null);
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
        //for (int j = 1; j < transform.childCount; j++)
        //{
        //    transform.GetChild(j).gameObject.SetActive(true);
        //}
        for(int i = 0; i < m_conditionalItems.Length; i++)
		{
            Component comp = m_conditionalItems[i].Condition._object.GetComponent(m_conditionalItems[i].Condition.componentName);
            bool activeCondition = (bool)comp.GetType().GetMethod(m_conditionalItems[i].Condition.methodName).Invoke(m_conditionalItems[i].Condition._object.GetComponent(m_conditionalItems[i].Condition.componentName), null);

            if (activeCondition)
            {
                for(int j = 0; j < m_conditionalItems[i].trueItems.Length; j++)
				{
                    m_conditionalItems[i].trueItems[j].button.gameObject.SetActive(true);

                }
                for(int k = 0; k < m_conditionalItems[i].falseItems.Length; k++)
				{
                    m_conditionalItems[i].falseItems[k].button.gameObject.SetActive(false);
                }
            }
			else
			{
                for (int j = 0; j < m_conditionalItems[i].trueItems.Length; j++)
                {
                    m_conditionalItems[i].trueItems[j].button.gameObject.SetActive(false);

                }
                for (int k = 0; k < m_conditionalItems[i].falseItems.Length; k++)
                {
                    m_conditionalItems[i].falseItems[k].button.gameObject.SetActive(true);
                }
            }
        }
        menu.SetActive(true);
        transform.position = Input.mousePosition;
	}

    public void HideMenu()
	{
        //gameObject.SetActive(false);
        visible = false;
        //for (int j = 1; j < transform.childCount; j++)
        //{
        //    transform.GetChild(j).gameObject.SetActive(false);
        //}
        menu.SetActive(false);
    }
}
