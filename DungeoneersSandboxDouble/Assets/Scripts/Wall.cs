using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public enum TileScetion
	{
        Center,
        Edge,
        Corner,
        InnerCorner
	}

    [SerializeField]
    GameObject[] m_slices = new GameObject[4];

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    private void OnValidate()
    {
        //Make sure array size for m_handles can not be changed in editor
        if (m_slices.Length != 5)
        {
            Array.Resize(ref m_slices, 5);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
