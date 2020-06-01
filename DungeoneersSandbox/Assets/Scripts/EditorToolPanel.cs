using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorToolPanel : MonoBehaviour
{
    [SerializeField]
    Image m_primaryPanel;
    [SerializeField]
    ColorPicker m_primaryColorPicker;
    [Space]
    [SerializeField]
    Image m_secondaryPanel;
    [SerializeField]
    ColorPicker m_secondaryColorPicker;

    public Color PrimaryColor { get { return m_primaryPanel.color; } }
    public Color SecondaryColor { get { return m_secondaryPanel.color; } }
    // Start is called before the first frame update
    void Start()
    {
        m_primaryColorPicker.gameObject.SetActive(false);
        m_secondaryColorPicker.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPanelsToDefault()
    {
        m_primaryPanel.color = Color.black;
        m_secondaryPanel.color = Color.white;

        m_primaryColorPicker.SetCurrentColor(Color.black, true);
        m_secondaryColorPicker.SetCurrentColor(Color.white, true);

    }

    public void SwitchColors()
    {
        Color temp_color = m_primaryPanel.color;
        m_primaryPanel.color = m_secondaryPanel.color;
        m_secondaryPanel.color = temp_color;

        m_primaryColorPicker.SetCurrentColor(m_primaryPanel.color, true);
        m_secondaryColorPicker.SetCurrentColor(m_secondaryPanel.color, true);
    }

    public void SetPrimaryColor(Color _color)
    {
        m_primaryPanel.color = _color;
    }

    public void SetPrimaryColor(float _r, float _g, float _b, float _a)
    {
        m_primaryPanel.color = new Color(_r, _g, _b, _a);
    }

    public void SetSecondaryColor(Color _color)
    {
        m_secondaryPanel.color = _color;
    }

    public void SetSecondaryColor(float _r, float _g, float _b, float _a)
    {
        m_secondaryPanel.color = new Color(_r, _g, _b, _a);
    }
}
