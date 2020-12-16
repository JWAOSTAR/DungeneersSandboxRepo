using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreeMovingCameraController : MonoBehaviour
{
    Vector2 previousRotation = Vector2.zero;
    float speed = 0.025f;
    bool moveable = false;

    public bool Mobility { get { return moveable; } set { moveable = value; } }
    // Start is called before the first frame update
    void Start()
    {
        previousRotation = new Vector2(Camera.main.transform.rotation.x, Camera.main.transform.rotation.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mobility)
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
            if (Input.GetMouseButton(2))
            {
                previousRotation.x += Input.GetAxis("Mouse X") * 10.0f;
                previousRotation.y -= Input.GetAxis("Mouse Y") * 10.0f;
                previousRotation.x = Mathf.Repeat(previousRotation.x, 360.0f);
                Camera.main.transform.rotation = Quaternion.Euler(previousRotation.y, previousRotation.x, 0);
            }
        }
    }
}
