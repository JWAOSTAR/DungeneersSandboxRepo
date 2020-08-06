using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinateRotation : MonoBehaviour
{
    [SerializeField]
    float speed = 0.025f;
    [SerializeField]
    bool XAxis = false;
    [SerializeField]
    bool YAxis = true;
    [SerializeField]
    bool ZAxis = false;

    bool m_mobile = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_mobile)
        {
            transform.Rotate(new Vector3(((XAxis) ? 1.0f : 0.0f), ((YAxis) ? 1.0f : 0.0f), ((ZAxis) ? 1.0f : 0.0f)), speed, Space.World);
        }
    }

    public void SetMobility(bool _isMobile)
    {
        m_mobile = _isMobile;
    }

    public bool GetMobility()
    {
        return m_mobile;
    }
}
