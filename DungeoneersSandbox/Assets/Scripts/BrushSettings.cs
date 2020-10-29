using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushSettings : MonoBehaviour
{
    [SerializeField]
    Toggle m_circleToggle;

    [SerializeField]
    Toggle m_squareToggle;

    [SerializeField]
    Brush m_brush;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //
        if(gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Changes the current brushes square/circle state
    /// </summary>
    /// <param name="_sqr">Iteger indicating wheather the state should be square or circle</param>
    public void BrushShapeToggle(int _sqr)
    {
        if(_sqr == 0)
        {
            m_circleToggle.SetIsOnWithoutNotify(!m_circleToggle.isOn);
        }
        else
        {
            m_squareToggle.SetIsOnWithoutNotify(!m_squareToggle.isOn);
        }
    }

    /// <summary>
    /// Sets the size of the currently active brush
    /// </summary>
    /// <param name="_size">Float value representing the size of the brush</param>
    public void SetBrushSize(float _size)
    {
        m_brush.Size = _size;
    }

    /// <summary>
    /// Sets the hardness/opacity of the currently active brush
    /// </summary>
    /// <param name="_hardness">Float value representing the hardness of the brush</param>
    public void SetBrushHardness(float _hardness)
    {
        m_brush.Hardness = _hardness;
    }
}
