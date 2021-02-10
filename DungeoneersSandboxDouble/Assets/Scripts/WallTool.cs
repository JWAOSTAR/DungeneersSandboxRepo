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
        if (Input.GetMouseButton(0) && m_currentHandle != null && m_currentPivot != null && m_currentWall != null)
		{
            //TODO: Add handle movement
            dot /= Mathf.Abs(dot);
            Plane plane = new Plane(Vector3.up, 0);
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mouseWorldPos;
            if(Input.GetAxis("Mouse X") != 0 && plane.Raycast(mRay, out float dist))
			{
                mouseWorldPos = mRay.GetPoint(dist);
                if (m_currentHandle.gameObject == m_handles[1] && mouseWorldPos.z > m_currentHandle.position.z && (m_currentHandle.localPosition + m_currentHandle.up * 0.05f).z > 0.7f ||
                    m_currentHandle.gameObject == m_handles[2] && mouseWorldPos.x > m_currentHandle.position.x && (m_currentHandle.localPosition + m_currentHandle.up * 0.05f).x > 0.7f ||
                    m_currentHandle.gameObject == m_handles[3] && mouseWorldPos.z < m_currentHandle.position.z && (m_currentHandle.localPosition + m_currentHandle.up * 0.05f).z < -0.7f ||
                    m_currentHandle.gameObject == m_handles[4] && mouseWorldPos.x < m_currentHandle.position.x && (m_currentHandle.localPosition + m_currentHandle.up * 0.05f).x < -0.7f)
                {
                    m_currentHandle.localPosition += m_currentHandle.up * 0.05f;
                }
                else if(m_currentHandle.gameObject == m_handles[1] && mouseWorldPos.z < m_currentHandle.position.z && (m_currentHandle.localPosition + m_currentHandle.up * -0.05f).z > 0.7f ||
                        m_currentHandle.gameObject == m_handles[2] && mouseWorldPos.x < m_currentHandle.position.x && (m_currentHandle.localPosition + m_currentHandle.up * -0.05f).x > 0.7f ||
                        m_currentHandle.gameObject == m_handles[3] && mouseWorldPos.z > m_currentHandle.position.z && (m_currentHandle.localPosition + m_currentHandle.up * -0.05f).z < -0.7f ||
                        m_currentHandle.gameObject == m_handles[4] && mouseWorldPos.x > m_currentHandle.position.x && (m_currentHandle.localPosition + m_currentHandle.up * -0.05f).x < -0.7f)
                {
                    m_currentHandle.localPosition += m_currentHandle.up * -0.05f;
                }
                m_currentHandle.localPosition = new Vector3(m_currentHandle.localPosition.x, 0.0f, m_currentHandle.localPosition.z);
                m_currentPivot.localPosition = new Vector3(m_currentPivot.localPosition.x, 0.0f, m_currentPivot.localPosition.z);
            }


			if ((m_currentHandle.position.y == m_currentPivot.position.y) && (m_currentHandle.localPosition.x == m_currentPivot.localPosition.x) || (m_currentHandle.localPosition.z == m_currentPivot.localPosition.z))
			{
                Vector3 midpooint;
                float distance = Vector3.Distance(m_currentHandle.position, m_currentPivot.position) - 0.7f;
                Vector3 scale;

                if (m_currentHandle.localPosition.x == m_currentPivot.localPosition.x) {
                    //m_currentWall.transform.localScale = new Vector3(m_currentWall.transform.localScale.x, m_currentWall.transform.localScale.y, distance);
                    scale = new Vector3(m_currentWall.transform.localScale.x, m_currentWall.transform.localScale.y, distance);
                }
				else 
                {
                    //m_currentWall.transform.localScale = new Vector3(distance, m_currentWall.transform.localScale.y, m_currentWall.transform.localScale.z);
                    scale = new Vector3(distance, m_currentWall.transform.localScale.y, m_currentWall.transform.localScale.z);
                }

                if(m_currentWall.transform.localScale.x < 1.0f)
				{
                    //m_currentWall.transform.localScale = new Vector3(1.0f, m_currentWall.transform.localScale.y, m_currentWall.transform.localScale.z);
                    scale = new Vector3(1.0f, m_currentWall.transform.localScale.y, m_currentWall.transform.localScale.z);

                }
                if(m_currentWall.transform.localScale.y < 1.0f)
				{
                    //m_currentWall.transform.localScale = new Vector3(m_currentWall.transform.localScale.x, 1.0f, m_currentWall.transform.localScale.z);
                    scale = new Vector3(m_currentWall.transform.localScale.x, 1.0f, m_currentWall.transform.localScale.z);
                }
                if(m_currentWall.transform.localScale.z < 1.0f)
				{
                    //m_currentWall.transform.localScale = new Vector3(m_currentWall.transform.localScale.x, m_currentWall.transform.localScale.y, 1.0f);
                    scale = new Vector3(m_currentWall.transform.localScale.x, m_currentWall.transform.localScale.y, 1.0f);
                }

                //m_currentWall.transform.position = (m_currentHandle.position + m_currentPivot.position) / 2.0f;
                midpooint = (m_currentHandle.position + m_currentPivot.position) / 2.0f;
                if(m_currentHandle.localPosition.x == m_currentPivot.localPosition.x)
				{
                    m_currentWall.SetScaleHeight(scale.z, midpooint, m_currentHandle.gameObject == m_handles[1]);
				}
				else
				{
                    m_currentWall.SetScaleWidth(scale.x, midpooint, m_currentHandle.gameObject == m_handles[4]);
                }

                if (m_currentHandle.gameObject == m_handles[1] || m_currentHandle.gameObject == m_handles[3])
				{
                    m_handles[2].transform.position = new Vector3(m_handles[2].transform.position.x, m_handles[2].transform.position.y, m_currentWall.OriginCenter.slice.transform.position.z);
                    m_handles[4].transform.position = new Vector3(m_handles[4].transform.position.x, m_handles[4].transform.position.y, m_currentWall.OriginCenter.slice.transform.position.z);
				}
				else
				{
                    m_handles[1].transform.position = new Vector3(m_currentWall.OriginCenter.slice.transform.position.x, m_handles[1].transform.position.y, m_handles[1].transform.position.z);
                    m_handles[3].transform.position = new Vector3(m_currentWall.OriginCenter.slice.transform.position.x, m_handles[3].transform.position.y, m_handles[3].transform.position.z);
                }
            }
        }

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

        if(Input.GetMouseButtonUp(0) && m_currentHandle != null)
		{
            m_handles[1].transform.position = new Vector3(m_handles[1].transform.position.x, m_handles[1].transform.position.y, m_currentWall.OriginCenter.Bottom.slice.transform.position.z + 0.7f);
            m_handles[2].transform.position = new Vector3(m_currentWall.OriginCenter.Left.slice.transform.position.x + 0.7f, m_handles[2].transform.position.y, m_handles[2].transform.position.z);
            m_handles[3].transform.position = new Vector3(m_handles[3].transform.position.x, m_handles[3].transform.position.y, m_currentWall.OriginCenter.Top.slice.transform.position.z - 0.7f);
            m_handles[4].transform.position = new Vector3(m_currentWall.OriginCenter.Right.slice.transform.position.x - 0.7f, m_handles[4].transform.position.y, m_handles[4].transform.position.z);

            m_currentHandle = null;
            m_currentPivot = null;
        }

        if(Input.GetKeyDown(KeyCode.R))
		{
            m_currentWall.ResetWall();
            m_handles[1].transform.position = new Vector3(m_currentWall.OriginCenter.slice.transform.position.x, m_handles[1].transform.position.y, m_currentWall.OriginCenter.Bottom.slice.transform.position.z + 0.7f);
            m_handles[2].transform.position = new Vector3(m_currentWall.OriginCenter.Left.slice.transform.position.x + 0.7f, m_handles[2].transform.position.y, m_currentWall.OriginCenter.slice.transform.position.z);
            m_handles[3].transform.position = new Vector3(m_currentWall.OriginCenter.slice.transform.position.x, m_handles[3].transform.position.y, m_currentWall.OriginCenter.Top.slice.transform.position.z - 0.7f);
            m_handles[4].transform.position = new Vector3(m_currentWall.OriginCenter.Right.slice.transform.position.x - 0.7f, m_handles[4].transform.position.y, m_currentWall.OriginCenter.slice.transform.position.z);
        }

    }
}
