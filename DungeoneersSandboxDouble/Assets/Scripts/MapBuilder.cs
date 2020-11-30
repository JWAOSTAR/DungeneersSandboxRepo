using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBuilder : MonoBehaviour
{

    struct Tile
    {
        public int id;
        public GameObject tile;
    }

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
    //[SerializeField]
    //Transform m_startingPos;
    [SerializeField]
    MapBuilderCameraController m_cameraController;

    Tile[,,] m_map;

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
        if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !m_cameraController.GetMobility())
        {
            m_cameraController.SetMobility(true);
        }
        else if(m_cameraController.GetMobility())
        {
            m_cameraController.SetMobility(false);
        }
    }

    /// <summary>
    /// Function called when the width value is changed on the new map GUI
    /// </summary>
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

    /// <summary>
    /// Function called when the height(depth) value is changed on the new map GUI
    /// </summary>
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

    /// <summary>
    /// Function called when the level/floor value is changed on the new map GUI
    /// </summary>
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

    /// <summary>
    /// Instanciate and arrange all tiles when the confirmation button is clicked
    /// </summary>
    public void OnNewMapConfirm()
    {

        m_map = new Tile[m_width,m_height,m_level];

        for (float z = 0.0f; z < m_level; z++)
        {
            for (float y = 0.0f; y < m_height; y++)
            {
                for (float x = 0.0f; x < m_width; x++)
                {
                    m_map[(int)x, (int)y, (int)z].tile = Instantiate(m_baseTile, m_baseTile.transform.parent);
                    m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                }
            }
        }
        m_baseTile.SetActive(false);
        m_newMapMenu.SetActive(false);

        Camera.main.transform.position = new Vector3(0.0f, 0.75f, 0.0f);
    }

    /// <summary>
    /// Resets all the new map GUI values to default
    /// </summary>
    public void OnNewMapCancel()
    {
        m_width = 1;
        m_height = 1;
        m_level = 1;
        m_widthInput.text = "";
        m_heightInput.text = "";
        m_levelInput.text = "";
    }

    /// <summary>
    /// Sets the width value for the map
    /// </summary>
    /// <param name="_width">Float value the width of the map is to be changed to</param>
    void SetWidth(int _width)
    {
        m_width = _width;
    }

    /// <summary>
    /// Sets the height value for the map
    /// </summary>
    /// <param name="_height">Float value the height of the map is to be changed to</param>
    void SetHeight(int _height)
    {
        m_height = _height;
    }
}
