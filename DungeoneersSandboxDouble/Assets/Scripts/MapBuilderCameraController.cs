using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilderCameraController : MonoBehaviour
{
    bool m_mobile = false;
    float speed = 0.025f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Camera controller
        if(m_mobile)
        {
                if (Input.GetKey(KeyCode.W) || (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0))
                {
                    transform.localPosition = transform.localPosition + transform.forward * speed;
                }
                if (Input.GetKey(KeyCode.S) || (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0))
                {
                    transform.localPosition = transform.localPosition - transform.forward * speed;
                }
                if (Input.GetKey(KeyCode.D) || (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0))
                {
                    transform.localPosition = transform.localPosition + transform.right * speed;
                }
                if (Input.GetKey(KeyCode.A) || (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0))
                {
                    transform.localPosition = transform.localPosition - transform.right * speed;
                }

                if(Input.mouseScrollDelta.y > 0)
                {
                    transform.localPosition = transform.localPosition + transform.forward * speed * 4.0f;
                }
                else if(Input.mouseScrollDelta.y < 0)
                {
                    transform.localPosition = transform.localPosition - transform.forward * speed * 4.0f;
                }

                if (Input.GetMouseButton(2))
                {
                    if (Input.GetAxis("Mouse X") > 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.up*-0.3f, Space.World);
                    }
                    else if (Input.GetAxis("Mouse Y") > 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.right * 0.3f, Space.World);
                    }
                    if (Input.GetAxis("Mouse X") < 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.up*0.3f, Space.World);
                    }
                    else if (Input.GetAxis("Mouse Y") < 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.right*-0.3f, Space.World);
                    }
                }
            
        }
    }

    /// <summary>
    /// Set the mobile state of the object
    /// </summary>
    /// <param name="_mobility">Boolean value representing the state of the object</param>
    public void SetMobility(bool _mobility)
    {
        m_mobile = _mobility;
    }

    /// <summary>
    /// Get the mobile state of the object
    /// </summary>
    /// <returns>Boolean representing whether or not the object is allowed to be moveds</returns>
    public bool GetMobility()
    {
        return m_mobile;
    }
}
