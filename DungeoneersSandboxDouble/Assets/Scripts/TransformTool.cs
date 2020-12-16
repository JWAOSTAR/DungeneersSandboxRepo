using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformTool : MonoBehaviour
{
    [SerializeField]
    GameObject _currentGameObject;
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

    int activeAxis = 0;

    FreeMovingCameraController camControl;

    Vector3 initialScale = Vector3.one;
    public GameObject CurrentGameObject 
    { 
        get 
        { 
            return _currentGameObject; 
        } 
        set 
        { 
            _currentGameObject = value;
            if (_currentGameObject != null && _currentGameObject.TryGetComponent<MeshCollider>(out MeshCollider collider))
            {
                collider.enabled = false;
                transform.position = _currentGameObject.transform.position;
            }

            if (activeTransform[1])
			{
                SwitchActiveTransforms(1);
            }
            else if(activeTransform[2])
			{
                SwitchActiveTransforms(2);
            }
            else
			{
                SwitchActiveTransforms(0);
            }
        } 
    }

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
        initialScale = transform.localScale;
        RotX.SetActive(false);
        RotY.SetActive(false);
        RotZ.SetActive(false);
        SclX.SetActive(false);
        SclY.SetActive(false);
        SclZ.SetActive(false);
        CurrentGameObject = null;
        camControl = FindObjectOfType<FreeMovingCameraController>();
        SwitchActiveTransforms(3);
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
                    activeAxis = 1;

                }
                else if(hit.transform.gameObject == PosY)
				{
                    PosY.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 2;
                }
                else if (hit.transform.gameObject == PosZ)
                {
                    PosZ.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 4;
                }
                activeSelect = true;
            }
            else if (Rotation.Contains(hit.transform.gameObject))
			{
                if (hit.transform.gameObject == RotX)
                {
                    RotX.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 1;
                }
                else if (hit.transform.gameObject == RotY)
                {
                    RotY.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 2;
                }
                else if (hit.transform.gameObject == RotZ)
                {
                    RotZ.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 4;
                }
                activeSelect = true;
            }
            else if (Scaling.Contains(hit.transform.gameObject))
			{
                if (hit.transform.gameObject == SclX)
                {
                    SclX.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 1;
                }
                else if (hit.transform.gameObject == SclY)
                {
                    SclY.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 2;
                }
                else if (hit.transform.gameObject == SclZ)
                {
                    SclZ.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    activeAxis = 4;
                }
                activeSelect = true;
            }
            else if(transform.GetChild(0).gameObject == hit.transform.gameObject)
			{
                if (PosX.activeSelf)
                {
                    Array.ForEach(Position, pos => { pos.GetComponent<MeshRenderer>().material.color = Color.yellow; });
                }
                else if (RotX.activeSelf)
                {
                    Array.ForEach(Rotation, pos => { pos.GetComponent<MeshRenderer>().material.color = Color.yellow; });
                }
                else if (SclX.activeSelf)
                {
                    Array.ForEach(Scaling, pos => { pos.GetComponent<MeshRenderer>().material.color = Color.yellow; });
                }
                activeAxis = 7;
                activeSelect = true;
            }
        }
        else if(Input.GetMouseButton(0) && activeSelect)
		{
            if (PosX.activeSelf)
            {
				if (((activeAxis & 1) == 1))
				{
					if (Input.GetAxis("Mouse X") < 0)
					{
                        CurrentGameObject.transform.position -= new Vector3(0.05f, 0.0f, 0.0f);
                        transform.position -= new Vector3(0.05f, 0.0f, 0.0f);
                    }
                    else if(Input.GetAxis("Mouse X") > 0)
					{
                        CurrentGameObject.transform.position += new Vector3(0.05f, 0.0f, 0.0f);
                        transform.position += new Vector3(0.05f, 0.0f, 0.0f);
                    }
				}
                if((activeAxis & 2) == 2)
				{
                    if (Input.GetAxis("Mouse Y") < 0)
                    {
                        CurrentGameObject.transform.position -= new Vector3(0.0f, 0.05f, 0.0f);
                        transform.position -= new Vector3(0.0f, 0.05f, 0.0f);
                    }
                    else if (Input.GetAxis("Mouse Y") > 0)
                    {
                        CurrentGameObject.transform.position += new Vector3(0.0f, 0.05f, 0.0f);
                        transform.position += new Vector3(0.0f, 0.05f, 0.0f);
                    }
                }
                if((activeAxis & 4) == 4)
				{
                    if (Input.GetAxis("Mouse X") < 0)
                    {
                        CurrentGameObject.transform.position -= new Vector3(0.0f, 0.0f, 0.05f);
                        transform.position -= new Vector3(0.0f, 0.0f, 0.05f);
                    }
                    else if (Input.GetAxis("Mouse X") > 0)
                    {
                        CurrentGameObject.transform.position += new Vector3(0.0f, 0.0f, 0.05f);
                        transform.position += new Vector3(0.0f, 0.0f, 0.05f);
                    }
                }
            }
            else if(RotX.activeSelf)
			{
                if (((activeAxis & 1) == 1))
                {
                    if (Input.GetAxis("Mouse Y") < 0)
                    {
                        CurrentGameObject.transform.Rotate(new Vector3(-0.5f, 0.0f, 0.0f), Space.World);
                    }
                    else if (Input.GetAxis("Mouse Y") > 0)
                    {
                        CurrentGameObject.transform.Rotate(new Vector3(0.5f, 0.0f, 0.0f), Space.World);
                    }
                }
                if ((activeAxis & 2) == 2)
                {
                    if (Input.GetAxis("Mouse X") < 0)
                    {
                        CurrentGameObject.transform.Rotate(new Vector3(0.0f, 0.5f, 0.0f), Space.World);
                    }
                    else if (Input.GetAxis("Mouse X") > 0)
                    {
                        CurrentGameObject.transform.Rotate(new Vector3(0.0f, -0.5f, 0.0f), Space.World);
                    }
                }
                if ((activeAxis & 4) == 4)
                {
                    if (Input.GetAxis("Mouse Y") < 0)
                    {
                        CurrentGameObject.transform.Rotate(new Vector3(0.0f, 0.0f, -0.5f), Space.World);
                    }
                    else if (Input.GetAxis("Mouse Y") > 0)
                    {
                        CurrentGameObject.transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), Space.World);
                    }
                }
            }
            else if(SclX.activeSelf)
			{
                if (activeAxis != 7)
                {
                    if (((activeAxis & 1) == 1))
                    {
                        if (Input.GetAxis("Mouse X") < 0)
                        {
                            CurrentGameObject.transform.localScale -= new Vector3(0.05f, 0.0f, 0.0f);
                        }
                        else if (Input.GetAxis("Mouse X") > 0)
                        {
                            CurrentGameObject.transform.localScale += new Vector3(0.05f, 0.0f, 0.0f);
                        }
                    }
                    if ((activeAxis & 2) == 2)
                    {
                        if (Input.GetAxis("Mouse Y") < 0)
                        {
                            CurrentGameObject.transform.localScale -= new Vector3(0.0f, 0.05f, 0.0f);
                        }
                        else if (Input.GetAxis("Mouse Y") > 0)
                        {
                            CurrentGameObject.transform.localScale += new Vector3(0.0f, 0.05f, 0.0f);
                        }
                    }
                    if ((activeAxis & 4) == 4)
                    {
                        if (Input.GetAxis("Mouse X") < 0)
                        {
                            CurrentGameObject.transform.localScale -= new Vector3(0.0f, 0.0f, 0.05f);
                        }
                        else if (Input.GetAxis("Mouse X") > 0)
                        {
                            CurrentGameObject.transform.localScale += new Vector3(0.0f, 0.0f, 0.05f);
                        }
                    }
                }
				else
				{
                    if (Input.GetAxis("Mouse X") < 0)
                    {
                        CurrentGameObject.transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
                    }
                    else if (Input.GetAxis("Mouse Y") < 0)
					{
                        CurrentGameObject.transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
                    }
                    else if (Input.GetAxis("Mouse X") > 0)
                    {
                        CurrentGameObject.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
                    }
                    else if(Input.GetAxis("Mouse Y") > 0)
					{
                        CurrentGameObject.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
                    }
                }
            }
		}
        if(Input.GetMouseButtonUp(0) && activeSelect)
		{
            activeSelect = false;
            camControl.Mobility = true;
            activeAxis = 0;
            if (Array.Find(Position, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow)) != null) 
            {
                GameObject[] obj = Array.FindAll(Position, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow));
                if(obj.Contains(PosX))
				{
                    PosX.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                if(obj.Contains(PosY))
				{
                    PosY.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                if(obj.Contains(PosZ))
				{
                    PosZ.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
            else if(Array.Find(Rotation, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow)) != null)
			{
                GameObject[] obj = Array.FindAll(Rotation, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow));
                if (obj.Contains(RotX))
                {
                    RotX.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                if (obj.Contains(RotY))
                {
                    RotY.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                if (obj.Contains(RotZ))
                {
                    RotZ.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
            else if(Array.Find(Scaling, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow)) != null)
			{
                GameObject[] obj = Array.FindAll(Scaling, (n => n.GetComponent<MeshRenderer>().material.color == Color.yellow));
                if (obj.Contains(SclX))
                {
                    SclX.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                if (obj.Contains(SclY))
                {
                    SclY.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                if (obj.Contains(SclZ))
                {
                    SclZ.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
		}

        //Check for shortcut clicks for the transformation tools and change to the appropriate ones
        if(Input.GetKeyDown(KeyCode.R))
		{
            SwitchActiveTransforms(1);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            SwitchActiveTransforms(0);
        }
        else if (Input.GetKeyDown(KeyCode.S) && camControl != null && !camControl.Mobility)
        {
            SwitchActiveTransforms(2);
        }
        else if(Input.GetKeyDown(KeyCode.M))
		{
            SwitchActiveTransforms(3);
		}

        //Check for extentions for the short cut click 
		if (activeTransform[0] && Input.GetKeyDown(KeyCode.X))
		{
            activeAxis ^= 1;

        }
        else if(activeTransform[1] && Input.GetKeyDown(KeyCode.Y))
		{
            activeAxis ^= 2;
        }
        else if(activeTransform[2] && Input.GetKeyDown(KeyCode.Z))
		{
            activeAxis ^= 4;
        }

        float distance = (Camera.main.transform.position - transform.position).magnitude;
        float size = distance * 0.7f;
        transform.localScale = initialScale * size;
        //transform.forward = transform.position - Camera.main.transform.position;
    }

    /// <summary>
    /// Switch between the three transformation types
    /// </summary>
    /// <param name="_activeTool">Integer representing the transform tool type</param>
    public void SwitchActiveTransforms(int _activeTool)
	{
        if (_activeTool < activeTransform.Length && _activeTool >= 0)
        {
            Array.Clear(activeTransform, 0, activeTransform.Length);
            activeTransform[_activeTool] = true;
            if (camControl != null)
            {
                camControl.Mobility = false;
            }
        }
        switch(_activeTool)
		{
            case 0:
                {
                    PosX.transform.parent.gameObject.SetActive(true);
                    PosY.transform.parent.gameObject.SetActive(true);
                    PosZ.transform.parent.gameObject.SetActive(true);
                    Array.ForEach(Position, pos => pos.SetActive(true));
                    Array.ForEach(Rotation, rot => rot.SetActive(false));
                    Array.ForEach(Scaling, scl => scl.SetActive(false));
                }
                break;
            case 1:
                {
                    PosX.transform.parent.gameObject.SetActive(true);
                    PosY.transform.parent.gameObject.SetActive(true);
                    PosZ.transform.parent.gameObject.SetActive(true);
                    Array.ForEach(Position, pos => pos.SetActive(false));
                    Array.ForEach(Rotation, rot => rot.SetActive(true));
                    Array.ForEach(Scaling, scl => scl.SetActive(false));
                }
                break;
            case 2:
                {
                    PosX.transform.parent.gameObject.SetActive(true);
                    PosY.transform.parent.gameObject.SetActive(true);
                    PosZ.transform.parent.gameObject.SetActive(true);
                    Array.ForEach(Position, pos => pos.SetActive(false));
                    Array.ForEach(Rotation, rot => rot.SetActive(false));
                    Array.ForEach(Scaling, scl => scl.SetActive(true));
                }
                break;
            case 3:
                {
                    Array.Clear(activeTransform, 0, activeTransform.Length);
                    PosX.transform.parent.gameObject.SetActive(false);
                    PosY.transform.parent.gameObject.SetActive(false);
                    PosZ.transform.parent.gameObject.SetActive(false);
                    //TODO: Add camera movment unlock
                    if (camControl != null)
                    {
                        camControl.Mobility = true;
                    }
                }
                break;
            default:
                return;
		}
    }
}
