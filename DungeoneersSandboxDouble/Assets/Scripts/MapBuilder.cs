﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapBuilder : MonoBehaviour
{
    public enum MAP_TOOLS
	{
        TILE_SELECT,
        LEVEL_SELECT,
        LEVEL_SPACING,
        SHOW,
        HIDE,
        DELETE
	}

    [SerializeField]
    InputField m_widthInput;
    [SerializeField]
    InputField m_heightInput;
    [SerializeField]
    InputField m_levelInput;
    [SerializeField]
    InputField m_addLevels;
    [SerializeField]
    GameObject m_newMapMenu;
    [SerializeField]
    GameObject m_baseTile;
    //[SerializeField]
    //Transform m_startingPos;
    [SerializeField]
    FreeMovingCameraController m_cameraController;
    [SerializeField]
    List<Light> m_lights = new List<Light>();

    Tile[,,] m_map;
    MAP_TOOLS currentTool = MAP_TOOLS.TILE_SELECT;
    //public MAP_TOOLS CurrentTools { get { return currentTool; } set { currentTool = value; } }
    List<Vector3Int> m_selected = new List<Vector3Int>();
    List<ParticleSystem> m_particleEffects = new List<ParticleSystem>();

    int m_width;
    int m_height;
    int m_level;
    List<float> m_spacing = new List<float>();
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

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.transform.parent.gameObject.name.Contains("Tile"))
            {
                if (!m_selected.Contains(new Vector3Int((int)hit.transform.parent.position.x, (int)hit.transform.parent.position.z, (int)hit.transform.parent.position.y))) {
                    Select((int)hit.transform.parent.position.y, (int)hit.transform.parent.position.x, (int)hit.transform.parent.position.z, (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
                }
				else
				{
                    Deselect((int)hit.transform.parent.position.y, (int)hit.transform.parent.position.x, (int)hit.transform.parent.position.z);
				}
            }
        }

        if((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.H))
		{
            ShowAll();
		}
        else if (Input.GetKeyDown(KeyCode.H))
        {
            Hide();        
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
            m_spacing.Add(1.0f);
            for (float y = 0.0f; y < m_height; y++)
            {
                for (float x = 0.0f; x < m_width; x++)
                {
                    m_map[(int)x, (int)y, (int)z] = new Tile();
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
    /// Adds a level to the map
    /// </summary>
    /// <param name="l">At which level the new level will be added at</param>
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
                        //oldTiles[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                        m_map[(int)x, (int)y, (int)z] = new Tile();
                        m_map[(int)x, (int)y, (int)z].tile = Instantiate(m_baseTile, m_baseTile.transform.parent);
                        m_map[(int)x, (int)y, (int)z].tile.SetActive(true);
                        m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                    }
                    else if(z < l)
					{
                        m_map[(int)x, (int)y, (int)z] = new Tile();
                        m_map[(int)x, (int)y, (int)z].tile = oldTiles[(int)x, (int)y, (int)z].tile;
                        m_map[(int)x, (int)y, (int)z].tile.SetActive(true);

                    }
                    else
					{
                        m_map[(int)x, (int)y, (int)z] = new Tile();
                        m_map[(int)x, (int)y, (int)z].tile = oldTiles[(int)x, (int)y, (int)z - 1].tile;
                        m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                        m_map[(int)x, (int)y, (int)z].tile.SetActive(true);
                    }
                }
            }

        }
    }

    /// <summary>
    /// Function called on the add level confirmation
    /// </summary>
    public void AddLevels()
	{
        AddLevels(int.Parse(m_addLevels.text));
	}

    /// <summary>
    /// Adds a given number of levels at the position of currently selected levels
    /// </summary>
    /// <param name="ls"></param>
    void AddLevels(int ls)
	{
        List<int> levels = new List<int>();
        for (int i = 0; i < m_selected.Count; i++)
        {
            if (!levels.Contains(m_selected[i].z))
            {
                levels.Add(m_selected[i].z);
            }
        }
        for (int j = 0; j < levels.Count; j++)
        {
            for(int k = 0; k < ls; k++)
			{
                AddLevel(levels[j]);
			}
        }
        DeselectAll();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lvl"></param>
    /// <param name="shift"></param>
    void RemoveLevel(int lvl, bool shift = false)
	{
        for(int y = 0; y < m_height; y++)
		{
            for(int x = 0; x < m_width; x++)
			{
                if (m_map[x, y, lvl].tile != null)
                {
                    Destroy(m_map[x, y, lvl].tile);
                    m_map[x, y, lvl].tile = null;
                }
            }
		}

        if(lvl != (m_level - 1))
		{
            for(int z = lvl+1; z < m_level; z++)
			{
                for (int y = 0; y < m_height; y++)
                {
                    for (int x = 0; x < m_width; x++)
                    {
                        m_map[x, y, z - 1].tile = m_map[x, y, z].tile;
                        if (shift)
                        {
                            m_map[x, y, z - 1].tile.transform.position = new Vector3(x, z - 1, y);
                        }
                    }
                }
            }
		}
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    void AddTile(int x, int y, int z)
    {
        if(m_map[x,y,z].tile == null)
		{
            m_map[x, y, z].tile = Instantiate(m_baseTile, m_baseTile.transform.parent);
            m_map[x, y, z].tile.SetActive(true);
            m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
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

    /// <summary>
    /// Hides the tiles and levels in the command line
    /// </summary>
    public void Hide()
	{
        for (int i = 0; i < m_selected.Count; i++)
        {
            m_map[m_selected[i].x, m_selected[i].y, m_selected[i].z].tile.SetActive(false);
        }
        //string tile = string.Empty;
        //if (currentTool == MAP_TOOLS.LEVEL_SELECT)
        //{
        //    List<int> levels = new List<int>();
        //    for (int i = 0; i < m_selected.Count; i++)
        //    {
        //        if (!levels.Contains(m_selected[i].z))
        //        {
        //            levels.Add(m_selected[i].z);
        //        }
        //    }
        //    for (int j = 0; j < levels.Count; j++)
        //    {
        //        tile += "L" + levels[j].ToString() + ((j + 1 == levels.Count) ? "" : ", ");
        //    }
        //}
        //else if (currentTool == MAP_TOOLS.TILE_SELECT)
        //{
        //    for (int i = 0; i < m_selected.Count; i++)
        //    {
        //        tile += "T(" + m_selected[i].x.ToString() + ", " + m_selected[i].y.ToString() + ", " + m_selected[i].z.ToString() + ")" + ((i + 1 == m_selected.Count) ? "" : ", ");
        //    }
        //}
        //ParseCommand("/hide " + tile /*GetComponentInChildren<InputField>().text*/);
    }

    /// <summary>
    /// Shows the tiles and levels in the command line
    /// </summary>
    public void Show()
	{
        for(int i = 0; i < m_selected.Count; i++)
		{
            m_map[m_selected[i].x, m_selected[i].y, m_selected[i].z].tile.SetActive(true);
		}
  //      string tile = string.Empty;
  //      if(currentTool == MAP_TOOLS.LEVEL_SELECT)
		//{
  //          List<int> levels = new List<int>();
  //          for (int i = 0; i < m_selected.Count; i++)
		//	{
  //              if (!levels.Contains(m_selected[i].z))
  //              {
  //                  levels.Add(m_selected[i].z);
  //              }
  //          }
  //          for(int j = 0; j < levels.Count; j++)
		//	{
  //              tile += "L" + levels[j].ToString() + ((j+1 == levels.Count) ? "" : ", ");
		//	}
		//}
  //      else if(currentTool == MAP_TOOLS.TILE_SELECT)
		//{
  //          for(int i = 0; i < m_selected.Count; i++)
		//	{
  //              tile += "T(" + m_selected[i].x.ToString() + ", " + m_selected[i].y.ToString() + ", " + m_selected[i].z.ToString() + ")" + ((i + 1 == m_selected.Count) ? "" : ", ");
  //          }
		//}
        //ParseCommand("/show " + tile /*GetComponentInChildren<InputField>().text*/);
    }

    /// <summary>
    /// Shows all the tiles and levels in the command line
    /// </summary>
    public void ShowAll()
    {
        string tile = string.Empty;
        for(int z = 0; z < m_level; z++)
		{
            for(int y = 0; y < m_height; y++)
			{
                for(int x = 0; x < m_width; x++)
				{
                    if(!m_map[x, y, z].tile.activeSelf)
					{
                        m_map[x, y, z].tile.SetActive(true);
                    }
				}
			}
		}
    }

    /// <summary>
    /// Registers tile(s) selected by user
    /// </summary>
    /// <param name="l">Level of the tile(s) selected</param>
    /// <param name="x">x-coordinate of the tile selected</param>
    /// <param name="y">y-coordinate of the tile selected</param>
    /// <param name="shift">Boolean indicating if shift is being held</param>
    void Select(int l, int x = -1, int y = -1, bool shift = false)
	{
		if (currentTool == MAP_TOOLS.LEVEL_SELECT || currentTool == MAP_TOOLS.TILE_SELECT)
		{
            if (!shift)
            {
                DeselectAll();
            }
            switch (currentTool)
			{
                case MAP_TOOLS.LEVEL_SELECT:
					{
                        for (int iy = 0; iy < m_height; iy++)
						{
                            for(int ix = 0; ix < m_width; ix++)
							{
                                if (!m_selected.Contains(new Vector3Int(ix, iy, l))) 
                                { 
                                    m_selected.Add(new Vector3Int(ix, iy, l));
                                    m_map[ix, iy, l].tile.GetComponentInChildren<OnHoverHighlight>().Selected = true;
                                    MeshRenderer mr = m_map[ix, iy, l].tile.GetComponentInChildren<MeshRenderer>();
                                    for (int i = 0; i < mr.materials.Length; i++)
                                    {
                                        //Color temp = new Color(m_meshRenderer.materials[i].color.r, m_meshRenderer.materials[i].color.g, m_meshRenderer.materials[i].color.b);
                                        mr.materials[i].shader = Shader.Find("DS/OutlineShader");
                                        mr.materials[i].SetFloat("_OutlineThickness", 0.02f);
                                    }
                                }
							}
						}
					}
                    break;
                case MAP_TOOLS.TILE_SELECT:
					{
                        if (!m_selected.Contains(new Vector3Int(x, y, l)))
                        {
                            m_selected.Add(new Vector3Int(x, y, l));
                            m_map[x, y, l].tile.GetComponentInChildren<OnHoverHighlight>().Selected = true;
                            MeshRenderer mr = m_map[x, y, l].tile.GetComponentInChildren<MeshRenderer>();
                            for (int i = 0; i < mr.materials.Length; i++)
                            {
                                //Color temp = new Color(m_meshRenderer.materials[i].color.r, m_meshRenderer.materials[i].color.g, m_meshRenderer.materials[i].color.b);
                                mr.materials[i].shader = Shader.Find("DS/OutlineShader");
                                mr.materials[i].SetFloat("_OutlineThickness", 0.02f);
                            }
                        }
                    }
                    break;
                default:
                    break;
			}
		}
	}

    void Deselect(int l, int x = -1, int y = -1)
	{
        if (currentTool == MAP_TOOLS.TILE_SELECT) {
            if (m_selected.Contains(new Vector3Int(x, y, l)))
            {
                m_map[x, y, l].tile.GetComponentInChildren<OnHoverHighlight>().Selected = false;
                MeshRenderer mr = m_map[x, y, l].tile.GetComponentInChildren<MeshRenderer>();
                for (int j = 0; j < mr.materials.Length; j++)
                {
                    //Color temp = new Color(m_meshRenderer.materials[i].color.r, m_meshRenderer.materials[i].color.g, m_meshRenderer.materials[i].color.b);
                    mr.materials[j].shader = Shader.Find("Standard");
                }
                m_selected.Remove(new Vector3Int(x, y, l));
            }
        }
        else if(currentTool == MAP_TOOLS.LEVEL_SELECT)
		{
            for(int i = m_selected.Count - 1; i >= 0; i--)
			{
                if(m_selected[i].z == l)
				{
                    m_map[m_selected[i].x, m_selected[i].y, m_selected[i].z].tile.GetComponentInChildren<OnHoverHighlight>().Selected = false;
                    MeshRenderer mr = m_map[m_selected[i].x, m_selected[i].y, m_selected[i].z].tile.GetComponentInChildren<MeshRenderer>();
                    for (int j = 0; j < mr.materials.Length; j++)
                    {
                        //Color temp = new Color(m_meshRenderer.materials[i].color.r, m_meshRenderer.materials[i].color.g, m_meshRenderer.materials[i].color.b);
                        mr.materials[j].shader = Shader.Find("Standard");
                    }
                    m_selected.Remove(m_selected[i]);
                }
			}
		}
	}

    void DeselectAll()
	{
        for(int i = 0; i < m_selected.Count; i++)
		{
            m_map[m_selected[i].x, m_selected[i].y, m_selected[i].z].tile.GetComponentInChildren<OnHoverHighlight>().Selected = false;
            MeshRenderer mr = m_map[m_selected[i].x, m_selected[i].y, m_selected[i].z].tile.GetComponentInChildren<MeshRenderer>();
            for (int j = 0; j < mr.materials.Length; j++)
            {
                //Color temp = new Color(m_meshRenderer.materials[i].color.r, m_meshRenderer.materials[i].color.g, m_meshRenderer.materials[i].color.b);
                mr.materials[j].shader = Shader.Find("Standard");
            }
        }
        m_selected.Clear();
    }

    /// <summary>
    /// Parses and executes the commands in the command line
    /// </summary>
    /// <param name="commandLine">The string to be parsed for commands</param>
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
				{
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {
                                if (lvl < m_level)
                                {
                                    for (int y = 0; y < m_height; y++)
                                    {
                                        for (int x = 0; x < m_width; x++)
                                        {
                                            m_map[x, y, lvl].tile.SetActive(true);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
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
                                m_map[x, y, z].tile.SetActive(true);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('T', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                }
                break;
            case "/hide":
				{
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {
                                if (lvl < m_level) 
                                {
                                    for (int y = 0; y < m_height; y++)
                                    {
                                        for (int x = 0; x < m_width; x++)
                                        {
                                            m_map[x, y, lvl].tile.SetActive(false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
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
                                m_map[x, y, z].tile.SetActive(false);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('T', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                }
                break;
            case "/delete":
				{
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {
                                RemoveLevel(lvl);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
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
                                DeleteTile(x, y, z);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('T', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
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
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(),out int lvl))
                            {
                                AddLevel(lvl);
                            }
							else
							{
                                //TODO: Add some kind of error display
                                break;
							}
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                    if(commandLine.Contains("T"))
					{
                        int currentInstance = commandLine.IndexOf('T');
                        int lastInstance = commandLine.LastIndexOf('T');
                        do
                        {
                            int len = commandLine.IndexOf(')', currentInstance) - (commandLine.IndexOf(commandLine[currentInstance + 1]) + 1);
                            string coords = commandLine.Substring(commandLine.IndexOf(commandLine[currentInstance + 1]) + 1, len);
                            int x, y, z;
                            if(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 3 && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], out x) && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out y) && int.TryParse(coords.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[2], out z) )
							{
                                AddTile(x, y, z);
							}
							else
							{
                                break;
							}
                            currentInstance = commandLine.IndexOf('T', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                }
                break;
            case "/addTag":
				{
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {
                                if (lvl < m_level)
                                {
                                    for (int y = 0; y < m_height; y++)
                                    {
                                        for (int x = 0; x < m_width; x++)
                                        {
                                            string tag = commandLine.Substring(commandLine.IndexOf('"', currentInstance) + 1, commandLine.IndexOf('"', commandLine.IndexOf('"', currentInstance) + 1));
                                            m_map[x, y, lvl].AddTag(tag);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
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
                                len = commandLine.IndexOf('"', commandLine.IndexOf('"', currentInstance) + 1) - (commandLine.IndexOf('"', currentInstance) + 1);
                                string tag = commandLine.Substring(commandLine.IndexOf('"', currentInstance) + 1, len);
                                m_map[x, y, z].AddTag(tag);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('T', currentInstance+1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                }
                break;

            case "/removeTag":
				{
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {
                                if (lvl < m_level)
                                {
                                    for (int y = 0; y < m_height; y++)
                                    {
                                        for (int x = 0; x < m_width; x++)
                                        {
                                            string tag = commandLine.Substring(commandLine.IndexOf('"', currentInstance) + 1, commandLine.IndexOf('"', commandLine.IndexOf('"', currentInstance) + 1));
                                            m_map[x, y, lvl].RemoveTag(tag);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
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
                                len = commandLine.IndexOf('"', commandLine.IndexOf('"', currentInstance) + 1) - (commandLine.IndexOf('"', currentInstance) + 1);
                                string tag = commandLine.Substring(commandLine.IndexOf('"', currentInstance) + 1, len);
                                m_map[x, y, z].RemoveTag(tag);
                            }
                            else
                            {
                                break;
                            }
                            currentInstance = commandLine.IndexOf('T', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                }
                break;
            case "/setSpacing":
				{
                    int len = commandLine.IndexOf(')') - commandLine.IndexOf('(') - 1;
                    string clf = commandLine.Substring(commandLine.IndexOf('(') + 1, len);
                    //string clf = commandLine.Trim().TrimStart('(').TrimEnd(')');

                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {
                            
                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {
                                
                                if (lvl < (m_level - 1) && float.TryParse(clf, out float spacing))
                                {
                                    m_spacing[lvl] = spacing;
                                    if (m_spacing[lvl] < 1)
                                    {
                                        m_spacing[lvl] = 1;
                                    }
                                    for (float z = (float)lvl + 1.0f; z < m_level; z++)
                                    {
                                        float nSpace = m_spacing.GetRange(0, ((int)(z))).ToArray().Sum();
                                        for (float y = 0.0f; y < m_height; y++)
                                        {
                                            for (float x = 0.0f; x < m_width; x++)
                                            {
                                                m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, nSpace, y);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                    else if (float.TryParse(clf, out float spacing))
					{
                        for (float z = 1.0f; z < m_level; z++)
                        {
                            m_spacing[((int)z) - 1] = spacing;
                            if (m_spacing[((int)z) - 1] < 1)
                            {
                                m_spacing[((int)z) - 1] = 1;
                            }
                            for (float y = 0.0f; y < m_height; y++)
                            {
                                for (float x = 0.0f; x < m_width; x++)
                                {
                                    m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z*spacing, y);
                                }
                            }

                        }
                        m_spacing[m_spacing.Count - 1] = spacing;
                    }
                }
                break;
            case "/addSpacing":
				{
                    int len = commandLine.IndexOf(')') - commandLine.IndexOf('(') - 1;
                    string clf = commandLine.Substring(commandLine.IndexOf('(') + 1, len);
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {

                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {

                                if (lvl < (m_level - 1) && float.TryParse(clf, out float spacing))
                                {
                                    m_spacing[lvl] += spacing;
                                    for (float z = (float)lvl + 1.0f; z < m_level; z++)
                                    {
                                        float nSpace = m_spacing.GetRange(0, ((int)(z - 1.0f))).ToArray().Sum();
                                        for (float y = 0.0f; y < m_height; y++)
                                        {
                                            for (float x = 0.0f; x < m_width; x++)
                                            {
                                                m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, nSpace, y);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                    else if (float.TryParse(clf, out float spacing))
                    {
                        for (float z = 1.0f; z < m_level; z++)
                        {
                            m_spacing[((int)z) - 1] += spacing;
                            float nSpace = m_spacing.GetRange(0, ((int)(z))).ToArray().Sum();
                            for (float y = 0.0f; y < m_height; y++)
                            {
                                for (float x = 0.0f; x < m_width; x++)
                                {
                                    m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, nSpace, y);
                                }
                            }

                        }
                        m_spacing[m_spacing.Count - 1] += spacing;
                    }
                }
                break;
            case "/subtractSpacing":
				{
                    int len = commandLine.IndexOf(')') - commandLine.IndexOf('(') - 1;
                    string clf = commandLine.Substring(commandLine.IndexOf('(') + 1, len);
                    if (commandLine.Contains("L"))
                    {
                        int currentInstance = commandLine.IndexOf('L');
                        int lastInstance = commandLine.LastIndexOf('L');
                        do
                        {

                            if (int.TryParse(commandLine[currentInstance + 1].ToString(), out int lvl))
                            {

                                if (lvl < (m_level - 1) && float.TryParse(clf, out float spacing))
                                {
                                    m_spacing[lvl] -= spacing;
                                    if (m_spacing[lvl] < 1)
                                    {
                                        m_spacing[lvl] = 1;
                                    }
                                    for (float z = (float)lvl + 1.0f; z < m_level; z++)
                                    {
                                        float nSpace = m_spacing.GetRange(0, ((int)(z))).ToArray().Sum();
                                        for (float y = 0.0f; y < m_height; y++)
                                        {
                                            for (float x = 0.0f; x < m_width; x++)
                                            {
                                                m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, nSpace, y);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Add some kind of error display
                                break;
                            }
                            currentInstance = commandLine.IndexOf('L', currentInstance + 1);
                        }
                        while (lastInstance > currentInstance && currentInstance > 0);
                    }
                    else if (float.TryParse(clf, out float spacing))
                    {
                        for (float z = 1.0f; z < m_level; z++)
                        {
                            m_spacing[((int)z) - 1] -= spacing;
							if (m_spacing[((int)z) - 1] < 1) 
                            {
                                m_spacing[((int)z) - 1] = 1;
                            }
                            float nSpace = m_spacing.GetRange(0, ((int)(z - 1.0f))).ToArray().Sum();
                            for (float y = 0.0f; y < m_height; y++)
                            {
                                for (float x = 0.0f; x < m_width; x++)
                                {
                                    m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, nSpace, y);
                                }
                            }

                        }
                        m_spacing[m_spacing.Count - 1] -= spacing;
                    }
                }
                break;
        }
        if (GetComponentInChildren<InputField>() != null) 
        {
            GetComponentInChildren<InputField>().SetTextWithoutNotify(string.Empty);
        }
	}

    /// <summary>
    /// Sets the current tool
    /// </summary>
    /// <param name="t">Intiger representing the tool type</param>
    public void SetCurrentTool(int t)
	{
        currentTool = (MAP_TOOLS)t;
	}
}
