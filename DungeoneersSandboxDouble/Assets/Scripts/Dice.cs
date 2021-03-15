using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{

    [Serializable]
    public class DiceEvent : UnityEvent { }

    public enum DiceType
    {
        D4 = 0,
        D6,
        D8,
        D10,
        D10_10s,
        D12,
        D20,
    }

    public struct DiceSkin
    {
        public DiceType diceType;
        public bool isTexture;
        public Color numbers;
        public Color die;
        public Texture2D texture;
    }

    public DiceType diceType;
    [NonSerialized]
    public int currentValue;
    Rigidbody rb;
    //Transform staticTransform;
    Transform currentTransform;
    Transform prevTransform;
    bool m_isMoving;
    [NonSerialized]
    public int AdvDAdv = 0;

    [SerializeField]
    DiceEvent m_onMove = new DiceEvent();
    [SerializeField]
    DiceEvent m_onStop = new DiceEvent();
    [SerializeField]
    DiceEvent m_onCollision = new DiceEvent();
    [SerializeField]
    DiceEvent m_onDiceDelete = new DiceEvent();
    [SerializeField]
    DiceEvent m_onMouseClick = new DiceEvent();
    [SerializeField]
    GameObject m_globalDiceUI;
    Transform m_diceUI;

    bool m_mouseOver = false;

    // Start is called before the first frame update
    void Start()
    {
        currentValue = 1;
        rb = this.GetComponent<Rigidbody>();
        currentTransform /*= staticTransform*/ = transform;
        m_isMoving = false;
        m_diceUI = transform.Find("3DUI");
    }

    // Update is called once per frame
    void Update()
    {
        prevTransform = currentTransform;
        currentTransform = transform;
        if(!m_isMoving && rb.velocity.sqrMagnitude > 0.01f && rb.angularVelocity.sqrMagnitude > 0.01f)
        {
            m_isMoving = true;
            onMove();
        }
        else if (m_isMoving && (rb.velocity.sqrMagnitude < 0.01f && rb.angularVelocity.sqrMagnitude < 0.01f))
        {
            m_isMoving = false;
            onStop();
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            if(hit.transform.gameObject == this.gameObject && Input.GetMouseButtonDown(0))
			{
                m_diceUI.GetChild(0).gameObject.SetActive(false);
                //m_diceUI.GetChild(1).gameObject.SetActive(true);
                FindObjectOfType<ContextMenu>().ShowMenu();
                //m_globalDiceUI.SetActive(true);
                m_globalDiceUI.GetComponent<DiceMenu>().SetDie(this);
                m_mouseOver = true;
            }
        }
        else if(m_mouseOver && hit.transform.gameObject != this.gameObject)
		{
            m_diceUI.gameObject.SetActive(m_diceUI.GetChild(1).gameObject.activeSelf);
            m_mouseOver = false;
        }


        m_diceUI.LookAt(GameObject.Find("Main Camera").transform);
        m_diceUI.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }

    /// <summary>
    /// Get the boolean value that reprisents 
    /// </summary>
    /// <returns></returns>
    bool isMoving()
    {
        return m_isMoving;
    }

    /// <summary>
    /// Function called when gameObject is in motion and calls all listening functions
    /// </summary>
    void onMove()
    {
       //m_diceUI.gameObject.SetActive(false);
        m_onMove.Invoke();
        FindObjectOfType<DiceRoller>().ResetTimer();
    }

    /// <summary>
    /// Function called when gameObject has rested from motion and calls all listening functions
    /// </summary>
    void onStop()
    {
        UpdateValue();
        //m_diceUI.gameObject.SetActive(true);
        m_onStop.Invoke();
    }

    /// <summary>
    /// Deletes the gameObject itself and calls all listening functions
    /// </summary>
    public void Delete()
    {
        m_onDiceDelete.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// Adds force and spin to re-roll die
    /// </summary>
    public void ReRoll()
    {
        //Vector3[] spinDir = { /*new Vector3(20.0f, 0.0f, 0.0f), new Vector3(-20.0f, 0.0f, 0.0f),*/ new Vector3(0.0f, 20.0f, 0.0f), new Vector3(0.0f, -20.0f, 0.0f)/*, new Vector3(0.0f, 0.0f, 20.0f), new Vector3(0.0f, 0.0f, -20.0f)*/ };
        rb.AddForce(new Vector3(0.0f, 250.0f, 0.0f));
        rb.AddTorque(new Vector3(Random.Range(20.0f, 100.0f), /*Random.Range(20.0f, 100.0f)*/0.0f, Random.Range(20.0f, 100.0f)));
    }

    //OnMouseEnter is called when the mouse entered the GUIElement or Colider
    private void OnMouseEnter()
    {
        Debug.Log("Mouse enter " + gameObject.name);
        m_diceUI.gameObject.SetActive(true);
    }

	//OnMouseOver is called every frame while the mouse is over the GUIElement or Colider
	private void OnMouseOver()
	{
		Debug.Log("Mouse is over " + gameObject.name);
		if (Input.GetMouseButtonDown(0))
		{
			m_diceUI.GetChild(0).gameObject.SetActive(false);
			//m_diceUI.GetChild(1).gameObject.SetActive(true);
			FindObjectOfType<ContextMenu>().ShowMenu();
			//m_globalDiceUI.SetActive(true);
			m_globalDiceUI.GetComponent<DiceMenu>().SetDie(this);

		}
	}

	//OnMouseExit is called when the mouse is no longer over the GUIElement or Colider
	private void OnMouseExit()
    {
        Debug.Log("Mouse exit " + gameObject.name);
        m_diceUI.gameObject.SetActive(m_diceUI.GetChild(1).gameObject.activeSelf);
    }
    /// <summary>
    /// Change the on hover menu values
    /// </summary>
    void UpdateValue()
    {
        Transform topNum = null;
        foreach (Transform n in GetComponentsInChildren<Transform>())
        {

            if (topNum == null)
            {
                topNum = n;
                continue;
            }
            if (n.position.y > topNum.position.y)
            {
                topNum = n;
            }
            if(!int.TryParse(topNum.name, out int j))
            {
                topNum = null;
            }
        
        }

        currentValue = int.Parse(topNum.name);
        m_diceUI.GetComponentInChildren<TextMeshPro>().text = topNum.name;
        m_diceUI.SetPositionAndRotation(topNum.position + Vector3.up*0.236f, transform.rotation);
        m_diceUI.LookAt(GameObject.Find("Main Camera").transform);
        m_diceUI.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }

	private void OnCollisionEnter(Collision collision)
	{
        m_onCollision.Invoke();
    }
}
