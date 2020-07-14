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
                if (Input.GetMouseButton(2))
                {
                    if (Input.GetAxis("Mouse Y") > 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.right * 0.2f, Space.World);
                    }
                    if (Input.GetAxis("Mouse Y") < 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.right*-0.2f, Space.World);
                    }
                    if (Input.GetAxis("Mouse X") > 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.up*-0.2f, Space.World);
                    }
                    if (Input.GetAxis("Mouse X") < 0)
                    {
                        Camera.main.transform.Rotate(Camera.main.transform.up*0.2f, Space.World);
                    }
                }
            
        }
    }

   public void SetMobility(bool _mobility)
    {
        m_mobile = _mobility;
    }

    public bool GetMobility()
    {
        return m_mobile;
    }
}
