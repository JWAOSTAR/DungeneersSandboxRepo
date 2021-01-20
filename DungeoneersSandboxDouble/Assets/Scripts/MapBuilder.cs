using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //[SerializeField]
    //Transform m_startingPos;
    [SerializeField]
    FreeMovingCameraController m_cameraController;
    [SerializeField]
    List<Light> m_lights = new List<Light>();

    Tile[,,] m_map;
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
    /// 
    /// </summary>
    /// <param name="l"></param>
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
                        m_map[(int)x, (int)y, (int)z].tile = Instantiate(m_baseTile, m_baseTile.transform.parent);
                        m_map[(int)x, (int)y, (int)z].tile.SetActive(true);
                        m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                    }
                    else if(z < l)
					{
                        m_map[(int)x, (int)y, (int)z].tile = oldTiles[(int)x, (int)y, (int)z].tile;

                    }
                    else
					{
                        m_map[(int)x, (int)y, (int)z].tile = oldTiles[(int)x, (int)y, (int)z - 1].tile;
                        m_map[(int)x, (int)y, (int)z].tile.transform.position = new Vector3(x, z, y);
                    }
                }
            }

        }
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
        ParseCommand("/hide " + GetComponentInChildren<InputField>().text);
    }

    /// <summary>
    /// Shows the tiles and levels in the command line
    /// </summary>
    public void Show()
	{
        ParseCommand("/show " + GetComponentInChildren<InputField>().text);
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
        GetComponentInChildren<InputField>().SetTextWithoutNotify(string.Empty);
	}
}
