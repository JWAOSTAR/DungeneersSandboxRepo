using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{

    bool m_modelMobile = false;
    Coroutine m_mobilityCountdown = null;

    [SerializeField]
    DiceAxisMovement m_mover;
    [SerializeField]
    InfinateRotation m_rotator;
    [SerializeField]
    MeshCollider m_colider;
    [SerializeField]
    MeshFilter m_mesh;
    GameObject[] menues = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        m_mover.SetMobility(m_modelMobile);
        m_rotator.SetMobility(!m_modelMobile);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButton(0) && !m_modelMobile)
        {
            if(m_mobilityCountdown != null)
            {
                StopCoroutine(m_mobilityCountdown);
                m_mobilityCountdown = null;
            }
            m_modelMobile = true;
            m_mover.SetMobility(m_modelMobile);
            m_rotator.SetMobility(!m_modelMobile);
        }
        else if(m_modelMobile)
        {
            m_modelMobile = false;
            m_mobilityCountdown = StartCoroutine("StartRotationCountdown");
        }
    }

    IEnumerator StartRotationCountdown()
    {
        yield return new WaitForSeconds(10);
        m_mover.SetMobility(m_modelMobile);
        m_rotator.SetMobility(!m_modelMobile);
    }

    public void OpenMenu(int _menu)
    {
        for(int i = 0; i < menues.Length; i++)
        {
            if(i == _menu)
            {
                menues[i].SetActive(true);
            }
            else
            {
                menues[i].SetActive(false);
            }
        }
    }

}
