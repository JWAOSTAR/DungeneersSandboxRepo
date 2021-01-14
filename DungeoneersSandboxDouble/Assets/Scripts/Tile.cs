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

    public List<GameObject> objects = new List<GameObject>();
    public GameObject tile { get { return m_tile; } set { m_tile = value; } }

    ulong id;

    string tileName = "untitled";

    Vector3 size = new Vector3();
    Vector3 center = new Vector3();

    public string Name { get { return tileName; } set { tileName = value; } }
    public ulong ID { get { return id; } set { id = value; } }
    public Vector3 Size {  get { return size; } set { size = value; } }
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

    public void AddTag(string tag)
	{
        if(!m_tags.Contains(tag))
		{
            m_tags.Add(tag);
		}
	}

    public void RemoveTag(string tag)
	{
        if (m_tags.Contains(tag))
        {
            m_tags.Remove(tag);
        }
    }
}

