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
    }

    public void ToggleView()
    {
        topDown = !topDown;
    }
}
