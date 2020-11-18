using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    internal static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if ((double)currentValue.r == (double)newValue.r && (double)currentValue.g == (double)newValue.g && ((double)currentValue.b == (double)newValue.b && (double)currentValue.a == (double)newValue.a))
                return false;
            currentValue = newValue;
            return true;
        }

        public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
        {
            if (currentValue.Equals(newValue))
                return false;
            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals((object)newValue))
                return false;
            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((object)currentValue == null && (object)newValue == null || (object)currentValue != null && currentValue.Equals((object)newValue))
                return false;
            currentValue = newValue;
            return true;
        }
    }
}
//[AddComponentMenu("UI/Slider2D", 34)]
//[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class Slider2D : MonoBehaviour
{
    [Serializable]
    public class Slider2DEvent : UnityEvent { }

    [SerializeField]
    float m_minXValue = 0.0f;
    [SerializeField]
    float m_maxXValue = 0.0f;
    [SerializeField]
    float m_xValue = 0.0f;
    [Space]
    [SerializeField]
    float m_minYValue = 0.0f;
    [SerializeField]
    float m_maxYValue = 0.0f;
    [SerializeField]
    float m_yValue = 0.0f;

    float m_xOffset = 0.0f;
    float m_yOffset = 0.0f;
    float m_xScaleOffset = 0.0f;
    float m_yScaleOffset = 0.0f;

    public float xValue { set { m_xValue = value; m_onValueChange.Invoke(); } get { return m_xValue; } }
    public float yValue { set { m_yValue = value; m_onValueChange.Invoke(); } get { return m_yValue; } }

    [Space]

    [SerializeField]
    RectTransform knob;
    [SerializeField]
    Canvas canvus;

    [Space]

    [SerializeField]
    Slider2DEvent m_onValueChange = new Slider2DEvent();

    //This function is called when the script is loaded or value is changed in the inspector(Called in the editor only)
    void OnValidate()
    {
        if(m_xValue > m_maxXValue)
        {
            m_xValue = m_maxXValue;
        }

        if (m_yValue > m_maxYValue)
        {
            m_yValue = m_maxYValue;
        }

        if (m_xValue < m_minXValue)
        {
            m_xValue = m_minXValue;
        }

        if (m_yValue < m_minYValue)
        {
            m_yValue = m_minYValue;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_xOffset = transform.parent.GetComponent<RectTransform>().anchoredPosition.x;
        m_yOffset = transform.parent.GetComponent<RectTransform>().anchoredPosition.y;
        m_xScaleOffset = transform.parent.GetComponent<RectTransform>().localScale.x;
        m_yScaleOffset = transform.parent.GetComponent<RectTransform>().localScale.y;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvus.transform as RectTransform, Input.mousePosition, canvus.worldCamera, out pos);

            if (pos.x > (m_xScaleOffset *-78.0f + (m_xOffset)) && pos.x < (m_xScaleOffset *76.0f + ( m_xOffset)) && pos.y > (m_yScaleOffset *-78.0f +  (m_yOffset)) && pos.y < (m_yScaleOffset *76.0f + ( m_yOffset))) 
            {
                if (pos.x > (m_xScaleOffset * 73 + m_xOffset))
                {
                    pos.x = (m_xScaleOffset * 73 + m_xOffset);
                }
                else if (pos.x < (m_xScaleOffset * -75.0f + m_xOffset))
                {
                    pos.x = (m_xScaleOffset * -75.0f + m_xOffset);
                }

                if (pos.y > (m_yScaleOffset * 73 + m_yOffset))
                {
                    pos.y = (m_yScaleOffset * 73 + m_yOffset);
                }
                else if (pos.y < (m_yScaleOffset * -75.0f + m_yOffset))
                {
                    pos.y = (m_yScaleOffset * -75.0f + m_yOffset);
                }

                //TODO: Readjust Values to account for scale and postion offsets
                xValue = (pos.x - (-75.0f + m_xOffset));
                yValue = (pos.y - (-75.0f + m_yOffset));
                knob.position = canvus.transform.TransformPoint(pos);
            }
        }
    }

    //OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down on the mouse
    private void OnMouseDrag()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvus.transform as RectTransform, Input.mousePosition, canvus.worldCamera, out pos);
        knob.position = canvus.transform.TransformPoint(pos);
    }

    /// <summary>
    /// Sets the horizantal coordinate of the Slider2D handle without notifying listening objects
    /// </summary>
    /// <param name="x">New horizantal coordinate</param>
    public void SetXWithoutNotify(float x)
    {
        m_xValue = x;
        //knob.position = new Vector3(x, knob.position.y, 0.0f);
    }

    /// <summary>
    /// Sets the vertical coordinate of the Slider2D handle without notifying listening objects
    /// </summary>
    /// <param name="y">New vertical coordinate</param>
    public void SetYWithoutNotify(float y)
    {
        m_yValue = y;
        //knob.position = new Vector3(knob.position.x, y, 0.0f);
    }

    /// <summary>
    /// Sets the coordinates of the Slider2D handle without notifying listening objects
    /// </summary>
    /// <param name="x">New horizantal coordinate</param>
    /// <param name="y">New vertical coordinate</param>
    public void SetValueWithoutNotify(float x, float y)
    {
        SetXWithoutNotify(x);
        SetYWithoutNotify(y);
    }
}

