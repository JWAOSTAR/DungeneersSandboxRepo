using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MinitureControler : MonoBehaviour
{
	bool m_moving;

	public bool Moving { get { return m_moving; } set { m_moving = value; } }
    // Start is called before the first frame update
    void Start()
    {
		m_moving = false;
	}

	// Update is called once per frame
	void Update()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButtonDown(0)) {
			if (m_moving) {
				if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name.Contains("Tile"))
				{
					this.gameObject.transform.position = new Vector3(hit.collider.transform.parent.transform.position.x, hit.collider.transform.parent.transform.position.y + 0.05f, hit.collider.transform.parent.transform.position.z);
					m_moving = false;
				}
			}
			else if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.transform.parent == this.transform)
			{
				m_moving = true;
			}
		}
	}

	void MoveConfirmed()
	{
		int pos = ((int)(this.gameObject.transform.position.x * ((int)Mathf.Pow(10.0f, 2.0f))) + (int)(this.gameObject.transform.position.y * ((int)Mathf.Pow(10.0f, 5.0f))) + (int)(this.gameObject.transform.position.z * ((int)Mathf.Pow(10.0f, 8.0f))));
	}
}
