using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBuilder : MonoBehaviour
{
    [SerializeField]
    InputField m_widthInput;
    [SerializeField]
    InputField m_heightInput;
    [SerializeField]
    InputField m_levelInput;
    [SerializeField]
    GameObject m_newMapMenu;
    [SerializeField]
    GameObject m_baseTile;

    int m_width;
    int m_height;
    int m_level;
    // Start is called before the first frame update
    void Start()
    {
        m_width = 1;
        m_height = 1;
        m_level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnWidthInputChange()
    {
        int num;
        if (!int.TryParse(m_widthInput.text, out num))
        {
            m_widthInput.text = (m_widthInput.text == "" || m_widthInput.text == " ") ? "" : m_width.ToString();
        }
        else
        {
            m_width = num;
        }
    }

    public void OnHeightInputChange()
    {
        int num;
        if (!int.TryParse(m_heightInput.text, out num))
        {
            m_heightInput.text = (m_heightInput.text == "" || m_heightInput.text == " ") ? "" : m_height.ToString();
        }
        else
        {
            m_height = num;
        }
    }

    public void OnLevelInputChange()
    {
        int num;
        if (!int.TryParse(m_levelInput.text, out num))
        {
            m_levelInput.text = (m_levelInput.text == "" || m_levelInput.text == " ") ? "" : m_level.ToString();
        }
        else
        {
            m_level = num;
        }
    }

    public void OnNewMapConfirm()
    {
        for (float z = 0.0f; z < m_level; z++)
        {
            for (float y = 0.0f; y < m_height; y++)
            {
                for (float x = 0.0f; x < m_width; x++)
                {
                    Instantiate(m_baseTile, m_baseTile.transform.parent).transform.position = new Vector3(x, z, y);
                }
            }
        }

        m_newMapMenu.SetActive(false);
    }

    public void OnNewMapCancel()
    {

    }

    void SetWidth(int _width)
    {
        m_width = _width;
    }

    void SetHeight(int _height)
    {
        m_height = _height;
    }
}
