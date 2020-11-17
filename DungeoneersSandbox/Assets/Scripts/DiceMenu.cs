using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMenu : MonoBehaviour
{
    [HideInInspector]
    public Dice m_currentDie = null;

    // Update is called once per frame
    private void Update()
	{
		if(gameObject.activeSelf)
		{
            FindObjectOfType<DiceRoller>().ResetTimer();
		}
	}

	public void SetDie(Dice _die)
	{
        m_currentDie = _die;
        this.gameObject.transform.position = new Vector3(Input.mousePosition.x + (((Input.mousePosition.x > 0) ? -1 : 1) * (this.gameObject.GetComponent<RectTransform>().rect.width / 2.0f)), Input.mousePosition.y - (((Input.mousePosition.y < 0) ? -1 : 1) * (this.gameObject.GetComponent<RectTransform>().rect.height / 2.0f)), this.gameObject.transform.position.z);
    }

    public void ReRollDie()
	{
        if (m_currentDie != null)
        {
            m_currentDie.ReRoll();
            m_currentDie.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            m_currentDie = null;
        }
        this.gameObject.SetActive(false);
    }

    public void DeleteDie()
	{
        if (m_currentDie != null)
        {
            m_currentDie.Delete();
            m_currentDie = null;
        }
        this.gameObject.SetActive(false);
	}

    public void Cancel()
	{
        m_currentDie.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

	private void OnEnable()
	{
        this.gameObject.transform.position = new Vector3(Input.mousePosition.x + (((Input.mousePosition.x > 0) ? -1 : 1)*(this.gameObject.GetComponent<RectTransform>().rect.width / 2.0f)), Input.mousePosition.y - (((Input.mousePosition.y < 0) ? -1 : 1) * (this.gameObject.GetComponent<RectTransform>().rect.height / 2.0f)), this.gameObject.transform.position.z);
	} 
}
