using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    GameObject m_tile;

    public List<GameObject> objects = new List<GameObject>();
    public GameObject tile { get { return m_tile; } set { m_tile = value; } }

    ulong id;

    // Start is called before the first frame update
    void Start()
	{
        
        for(int i = 0; i < 10; i++)
		{
            id += ((ulong)UnityEngine.Random.Range((int)0, (int)10) * (ulong)Math.Pow(10, i));
        }
        int t = 0;
	}

}
