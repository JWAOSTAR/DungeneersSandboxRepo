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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
