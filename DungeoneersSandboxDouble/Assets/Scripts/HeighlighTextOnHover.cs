using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HeighlighTextOnHover : MonoBehaviour
{
    [SerializeField]
    Color m_originalColor;
    [SerializeField]
    Color m_heighlightColor;

    Text m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHover()
	{
        m_text.color = m_heighlightColor;
    }

    public void OnHoverExit()
	{
        m_text.color = m_originalColor;
	}
}
