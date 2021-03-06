﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiceAxisMovement : MonoBehaviour
{
    bool m_movable = true;
    [SerializeField]
    float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_movable)
        {
            if (Input.GetKey(KeyCode.UpArrow) || ((Input.GetAxis("Mouse Y") > 0) && (Input.GetMouseButton(0))))
            {
                gameObject.transform.Rotate(new Vector3(speed, 0.0f, 0.0f), Space.World);
            }
            if (Input.GetKey(KeyCode.DownArrow) || ((Input.GetAxis("Mouse Y") < 0) && (Input.GetMouseButton(0))))
            {
                gameObject.transform.Rotate(new Vector3(-speed, 0.0f, 0.0f), Space.World);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || ((Input.GetAxis("Mouse X") < 0) && (Input.GetMouseButton(0))))
            {
                gameObject.transform.Rotate(new Vector3(0.0f, speed, 0.0f), Space.World);
            }
            if (Input.GetKey(KeyCode.RightArrow) || ((Input.GetAxis("Mouse X") > 0) && (Input.GetMouseButton(0))))
            {
                gameObject.transform.Rotate(new Vector3(0.0f, -speed, 0.0f), Space.World);
            }
        }
    }

    /// <summary>
    /// Set the mobile state of the object
    /// </summary>
    /// <param name="isMovable">Boolean value representing the state of the object</param>
    public void SetMobility(bool isMovable)
    {
        m_movable = isMovable;
    }

    /// <summary>
    /// Get the mobile state of the object
    /// </summary>
    /// <returns>Boolean representing whether or not the object is allowed to be moved</returns>
    public bool GetMobility()
    {
        return m_movable;
    }
}
