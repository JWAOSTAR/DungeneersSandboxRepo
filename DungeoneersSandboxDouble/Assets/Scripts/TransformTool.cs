using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformTool : MonoBehaviour
{
    //[SerializeField]
    GameObject _currentGameObject;
    public GameObject CurrentGameObject { get { return _currentGameObject; } set { _currentGameObject = value; } }
    [SerializeField]
    GameObject[] Position = new GameObject[3];
    [SerializeField]
    GameObject[] Rotation = new GameObject[3];
    [SerializeField]
    GameObject[] Scaling = new GameObject[3];
    public GameObject PosX { get { return Position[0]; } }
    public GameObject PosY { get { return Position[1]; } }
    public GameObject PosZ { get { return Position[2]; } }
    public GameObject RotX { get { return Rotation[0]; } }
    public GameObject RotY { get { return Rotation[1]; } }
    public GameObject RotZ { get { return Rotation[2]; } }
    public GameObject SclX { get { return Scaling[0]; } }
    public GameObject SclY { get { return Scaling[1]; } }
    public GameObject SclZ { get { return Scaling[2]; } }

    bool[] activeTransform = new bool[] { true, false, false };

    bool activeSelect = false;

    private void OnValidate()
	{
        //Make sure array size for Position can not be changed in editor
        if (Position.Length > 3)
		{
            Array.Resize(ref Position, 3);
		}

        //Make sure array size for Rotation can not be changed in editor
        if (Rotation.Length > 3)
        {
            Array.Resize(ref Rotation, 3);
        }

        //Make sure array size for Scaling can not be changed in editor
        if (Scaling.Length > 3)
		{
            Array.Resize(ref Scaling, 3);
		}
    }

	// Start is called before the first frame update
	void Start()
    {
        RotX.SetActive(false);
        RotY.SetActive(false);
        RotZ.SetActive(false);
        SclX.SetActive(false);
        SclY.SetActive(false);
        SclZ.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
		{
			if (Position.Contains(hit.transform.gameObject))
			{
				if (hit.transform.gameObject == PosX)
				{
                    PosX.GetComponent<MeshRenderer>().material.color = Color.yellow;
				}
                else if(hit.transform.gameObject == PosY)
				{
                    PosY.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else if (hit.transform.gameObject == PosZ)
                {
                    PosZ.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                activeSelect = true;
            }
            else if (Rotation.Contains(hit.transform.gameObject))
			{
                if (hit.transform.gameObject == RotX)
                {
                    RotX.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else if (hit.transform.gameObject == RotY)
                {
                    RotY.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else if (hit.transform.gameObject == RotZ)
                {
                    RotZ.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                activeSelect = true;
            }
            else if (Scaling.Contains(hit.transform.gameObject))
			{
                if (hit.transform.gameObject == SclX)
                {
                    SclX.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else if (hit.transform.gameObject == SclY)
                {
                    SclY.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else if (hit.transform.gameObject == SclZ)
                {
                    SclZ.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                activeSelect = true;
            }
		}

        if(Input.GetMouseButtonUp(0))
		{
            activeSelect = false;
            if (Array.Find(Position, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow)) != null) 
            {
                GameObject obj = Array.Find(Position, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow));
                if(obj == PosX)
				{
                    obj.GetComponent<MeshRenderer>().material.color = Color.red;
				}
                else if(obj == PosY)
				{
                    obj.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                else if(obj == PosZ)
				{
                    obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
            else if(Array.Find(Rotation, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow)) != null)
			{
                GameObject obj = Array.Find(Rotation, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow));
                if (obj == RotX)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else if (obj == RotY)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                else if (obj == RotZ)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
            else if(Array.Find(Scaling, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow)) != null)
			{
                GameObject obj = Array.Find(Scaling, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow));
                if (obj == SclX)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else if (obj == SclY)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                else if (obj == SclZ)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
		}
    }
}
