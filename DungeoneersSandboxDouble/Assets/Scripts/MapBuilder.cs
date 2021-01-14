using System;
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
    FreeMovingCameraController m_cameraController;

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
        if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !m_cameraController.Mobility)
        {
            m_cameraController.Mobility = true;
        }
        else if(m_cameraController.Mobility)
        {
            m_cameraController.Mobility = false;
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

    void AddLevel(int l)
	{
        if(l > m_level + 1 || l < 0)
		{
            //TODO: Add some kind of error display
            return;
		}
        Tile[,,] oldTiles = new Tile[m_width, m_height, m_level];
        Array.Copy(m_map, oldTiles, m_map.Length);
        m_level++;
        m_map = new Tile[m_width, m_height, m_level];
        for (float z = 0.0f; z < m_level; z++)
		{
            for (float y = 0.0f; y < m_height; y++)
            {
                for (float x = 0.0f; x < m_width; x++)
                {
                    if (z == l) {
                        m_map[(int)x, (int)y, (int)z].tile = Instantiate(m_baseTile, m_baseTile.transform.parent);
                        m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                    }
                    else if(z < l)
					{
                        m_map[(int)x, (int)y, (int)z].tile = oldTiles[(int)x, (int)y, (int)z].tile;
                    }
                    else
					{
                        m_map[(int)x, (int)y, (int)z].tile = oldTiles[(int)x, (int)y, (int)z].tile;
                        m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y+1);
                    }
                }
            }

        }
    }

    void AddTile(int x, int y, int z)
    {
        if(m_map[x,y,z].tile == null)
		{
            m_map[x, y, z].tile = Instantiate(m_baseTile, m_baseTile.transform.parent);
            m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
        }
    }

    void DeleteTile(int x, int y, int z)
	{
        if (m_map[x, y, z].tile != null)
        {
            Destroy(m_map[x, y, z].tile);
            m_map[x, y, z].tile = null;

        }
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

    public void ParseCommand(string commandLine)
	{
        if(commandLine == string.Empty || commandLine == " ")
		{
            return;
		}
        string command = commandLine.Split(' ')[0];
        commandLine = commandLine.Replace(command, "");
        switch(command)
		{
			case "/show":

                break;
            case "/hide":

                break;
            case "/delete":
				{
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            int lvl = 0;
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out lvl))
                            {
                                //AddLevel(lvl);
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance);
                        }
                        while (lastInstance > currentInstance);
                    }
                    if (commandLine.Contains("T"))
                    {
                        int currentInstance = commandLine.IndexOf('T');
                        int lastInstance = commandLine.LastIndexOf('T');
                        do
                        {
                            int len = commandLine.IndexOf(')', currentInstance) - (commandLine.IndexOf(commandLine[currentInstance + 1]) + 1);
                            string coords = commandLine.Substring(commandLine.IndexOf(commandLine[currentInstance + 1]) + 1, len);
                            int x, y, z;
                            if (coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 3 && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], out x) && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out y) && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[2], out z))
                            {
                                //AddTile(x, y, z);
                                DeleteTile(x, y, z);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('T', currentInstance);
                        }
                        while (lastInstance > currentInstance);
                    }
                }
                break;
            case "/add":
                {
                    if (commandLine.Contains("L")) 
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            int lvl = 0;
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(),out lvl))
                            {
                                //AddLevel(lvl);
                            }
							else
							{
                                //TODO: Add some kind of error display
                                break;
							}
                            currentInstance = commandLine.IndexOf('L', currentInstance);
                        }
                        while (lastInstance > currentInstance);
                    }
                    if(commandLine.Contains("T"))
					{
                        int currentInstance = commandLine.IndexOf('T');
                        int lastInstance = commandLine.LastIndexOf('T');
                        do
                        {
                            string coords = commandLine.Substring(commandLine.IndexOf(commandLine[currentInstance + 1]) + 1, commandLine.IndexOf(')', currentInstance));
                            int x, y, z;
                            if(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 3 && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], out x) && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out y) && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[2], out z) )
							{
                                AddTile(x, y, z);
							}
							else
							{
                                break;
							}
                            currentInstance = commandLine.IndexOf('T', currentInstance);
                        }
                        while (lastInstance > currentInstance);
                    }
                }
                break;
            case "/tag":

                break;
		}
        GetComponentInChildren<InputField>().SetTextWithoutNotify(string.Empty);
	}
}
