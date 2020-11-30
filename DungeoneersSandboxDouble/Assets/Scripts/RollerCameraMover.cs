using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCameraMover : MonoBehaviour
{
    bool topDown = false;
    float speed = 0.025f;
    float zoomSpeed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || (Input.GetButton("Vertical") &&Input.GetAxisRaw("Vertical")>0))
        {
            transform.localPosition = transform.localPosition + ((topDown) ? transform.up : transform.forward) * speed;
        }
        if (Input.GetKey(KeyCode.S) || (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0))
        {
            transform.localPosition = transform.localPosition - ((topDown) ? transform.up : transform.forward) * speed;
        }
        if (Input.GetKey(KeyCode.D) || (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0))
        {
            transform.localPosition = transform.localPosition + transform.right * speed;
        }
        if (Input.GetKey(KeyCode.A) || (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0))
        {
            transform.localPosition = transform.localPosition - transform.right * speed;
        }
        if(topDown)
        {
            if(Input.mouseScrollDelta.y > 0.0f)
            {
                transform.localPosition = transform.localPosition + transform.forward * zoomSpeed;
            }
            else if (Input.mouseScrollDelta.y < 0.0f)
            {
                transform.localPosition = transform.localPosition - transform.forward * zoomSpeed;
            }
        }

        if (Input.GetMouseButton(2)) {
            if (Input.GetAxis("Mouse Y") > 0)
            {
                Camera.main.transform.Rotate(Camera.main.transform.right, 0.01f);
            }
            if (Input.GetAxis("Mouse Y") < 0)
            {
                Camera.main.transform.Rotate(Camera.main.transform.right, -0.01f);
            }
            if (Input.GetAxis("Mouse X") > 0)
            {
                Camera.main.transform.Rotate(Camera.main.transform.up, -0.01f);
            }
            if (Input.GetAxis("Mouse X") < 0)
            {
                Camera.main.transform.Rotate(Camera.main.transform.up, 0.01f);
            }
        }

    }

    /// <summary>
    /// Switches between the camera's top-down view and 3/4s view
    /// </summary>
    public void ToggleView()
    {
        topDown = !topDown;
    }
}
