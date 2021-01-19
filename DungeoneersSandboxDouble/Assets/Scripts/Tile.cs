using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField]
    GameObject m_tile;
    [SerializeField]
    List<string> m_tags = new List<string>();

    /// <summary>
    /// The collection of objects on the tile
    /// </summary>
    public List<GameObject> objects = new List<GameObject>();
    /// <summary>
    /// The tile base GameObject
    /// </summary>
    public GameObject tile { get { return m_tile; } set { m_tile = value; } }

    ulong id;

    string tileName = "untitled";

    Vector3 size = new Vector3();
    Vector3 center = new Vector3();

    /// <summary>
    /// Name given to the Tile
    /// </summary>
    public string Name { get { return tileName; } set { tileName = value; } }
    /// <summary>
    /// Unique Tile ID
    /// </summary>
    public ulong ID { get { return id; } set { id = value; } }
    /// <summary>
    /// Size of the collider surrounding the objects of the Tile
    /// </summary>
    public Vector3 Size {  get { return size; } set { size = value; } }
    /// <summary>
    /// Center of the collider surrounding the objects of the Tile
    /// </summary>
    public Vector3 Center {  get { return center; } set { center = value; } }

    // Start is called before the first frame update
    void Start()
	{
        
        for(int i = 0; i < 10; i++)
		{
            id += ((ulong)UnityEngine.Random.Range((int)0, (int)10) * (ulong)Math.Pow(10, i));
        }
        int t = 0;
	}
    
    /// <summary>
    /// Adds a tag to the list of tags stored in the Tile
    /// </summary>
    /// <param name="tag">String value used as the given name of the tag being added</param>
    public void AddTag(string tag)
	{
        if(!m_tags.Contains(tag))
		{
            m_tags.Add(tag);
		}
	}

    /// <summary>
    /// Removes a tag from the stored list of tags in the Tile
    /// </summary>
    /// <param name="tag">String value of the name of the tag to be removed</param>
    public void RemoveTag(string tag)
	{
        if (m_tags.Contains(tag))
        {
            m_tags.Remove(tag);
        }
    }
}

