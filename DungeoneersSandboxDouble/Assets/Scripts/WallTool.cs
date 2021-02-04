using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallTool : MonoBehaviour
{
    enum handle_pivots
	{
        South = 1,
        West,
        North,
        East,
        
	}
    Wall m_currentWall = null;
    Transform m_currentPivot = null;
    Transform m_currentHandle = null;
    handle_pivots[] pivots = new handle_pivots[] { handle_pivots.North, handle_pivots.East, handle_pivots.South, handle_pivots.West };
    float dot;
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
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, Mathf.Infinity)) 
		{
            if(m_handles.Contains(hit.transform.gameObject))
			{
                int index = Array.IndexOf(m_handles, hit.transform.gameObject);
                m_currentHandle = m_handles[index].transform;
                if (index > 0) {
                    m_currentPivot = m_handles[(int)pivots[index - 1]].transform;
                }
                else
				{
                    m_currentPivot = null;
				}
                dot = Vector3.Dot(m_currentHandle.position, Camera.main.transform.forward);
            }
            else if(hit.transform.gameObject.TryGetComponent<Wall>(out m_currentWall))
			{
                Array.ForEach(m_handles, h => h.SetActive(true));
			}
        }

        if(Input.GetMouseButton(0) && m_currentHandle != null && m_currentPivot != null && m_currentWall != null)
		{
            //TODO: Add handle movement
            dot /= Mathf.Abs(dot);
            if (Input.GetAxis("Mouse X") > 0 /*|| Input.GetAxis("Mouse Y") > 0*/)
            {
                if (m_currentHandle.gameObject == m_handles[1] && (m_currentHandle.position + m_currentHandle.up * -dot * 0.05f).z > 0.7f ||
                    m_currentHandle.gameObject == m_handles[2] && (m_currentHandle.position + m_currentHandle.up * -dot * 0.05f).x > 0.7f ||
                    m_currentHandle.gameObject == m_handles[3] && (m_currentHandle.position + m_currentHandle.up * -dot * 0.05f).z < -0.7f ||
                    m_currentHandle.gameObject == m_handles[4] && (m_currentHandle.position + m_currentHandle.up * -dot * 0.05f).x < -0.7f)
                {
                    m_currentHandle.position += m_currentHandle.up * -dot * 0.05f;
                }
            }
            else if(Input.GetAxis("Mouse X") < 0 /*|| Input.GetAxis("Mouse Y") < 0*/)
			{
                if (m_currentHandle.gameObject == m_handles[1] && (m_currentHandle.position + m_currentHandle.up * dot * 0.05f).z > 0.7f ||
                    m_currentHandle.gameObject == m_handles[2] && (m_currentHandle.position + m_currentHandle.up * dot * 0.05f).x > 0.7f ||
                    m_currentHandle.gameObject == m_handles[3] && (m_currentHandle.position + m_currentHandle.up * dot * 0.05f).z < -0.7f ||
                    m_currentHandle.gameObject == m_handles[4] && (m_currentHandle.position + m_currentHandle.up * dot * 0.05f).x < -0.7f)
                {
                    m_currentHandle.position += m_currentHandle.up * dot * 0.05f;
                }
            }


			if ((m_currentHandle.position.y == m_currentPivot.position.y) && (m_currentHandle.position.x == m_currentPivot.position.x) ||(m_currentHandle.position.z == m_currentPivot.position.z))
			{
                Vector3 midpooint;
                float distance = Vector3.Distance(m_currentHandle.position, m_currentPivot.position) - 0.7f;

                if (m_currentHandle.position.x == m_currentPivot.position.x) {
                    m_currentWall.transform.localScale = new Vector3(m_currentWall.transform.localScale.x, m_currentWall.transform.localScale.y, distance);
                }
				else 
                {
                    m_currentWall.transform.localScale = new Vector3(distance, m_currentWall.transform.localScale.y, m_currentWall.transform.localScale.z);
                }

                if(m_currentWall.transform.localScale.x < 1.0f)
				{
                    m_currentWall.transform.localScale = new Vector3(1.0f, m_currentWall.transform.localScale.y, m_currentWall.transform.localScale.z);

                }
                if(m_currentWall.transform.localScale.y < 1.0f)
				{
                    m_currentWall.transform.localScale = new Vector3(m_currentWall.transform.localScale.x, 1.0f, m_currentWall.transform.localScale.z);
                }
                if(m_currentWall.transform.localScale.z < 1.0f)
				{
                    m_currentWall.transform.localScale = new Vector3(m_currentWall.transform.localScale.x, m_currentWall.transform.localScale.y, 1.0f);
                }

                m_currentWall.transform.position = (m_currentHandle.position + m_currentPivot.position) / 2.0f;

                if(m_currentHandle.gameObject == m_handles[1] || m_currentHandle.gameObject == m_handles[3])
				{
                    m_handles[2].transform.position = new Vector3(m_handles[2].transform.position.x, m_handles[2].transform.position.y, m_currentWall.transform.position.z);
                    m_handles[4].transform.position = new Vector3(m_handles[4].transform.position.x, m_handles[4].transform.position.y, m_currentWall.transform.position.z);
				}
				else
				{
                    m_handles[1].transform.position = new Vector3(m_currentWall.transform.position.x, m_handles[1].transform.position.y, m_handles[1].transform.position.z);
                    m_handles[3].transform.position = new Vector3(m_currentWall.transform.position.x, m_handles[3].transform.position.y, m_handles[3].transform.position.z);
                }


            }
        }


    }
}
