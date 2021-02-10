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

    [SerializeField]
    Button m_baseButton;
    [SerializeField]
    Item[] m_items;

    bool visible;

 //   //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
 //   private void OnValidate()
	//{
	//}

	// Start is called before the first frame update
	void Start()
    {
        for(int i = 0; i < m_items.Length; i++)
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
  //      if(Input.GetMouseButtonDown(1))
		//{
  //          ShowMenu();
		//}
        if(Input.GetKeyDown(KeyCode.Escape))
		{
            HideMenu();
		}
	}

	public void ShowMenu()
	{
        //gameObject.SetActive(true);
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
