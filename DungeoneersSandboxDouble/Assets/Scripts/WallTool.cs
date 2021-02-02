using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTool : MonoBehaviour
{
    Wall m_currentWall = null;
    [SerializeField]
    GameObject[] m_handles = new GameObject[5];

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    private void OnValidate()
	{
        //Make sure array size for m_handles can not be changed in editor
        if (m_handles.Length != 5)
        {
            Array.Resize(ref m_handles, 5);
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
