using System;
using System.IO;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileMaker : MonoBehaviour
{
    List<OBJImporter.OBJ> m_OBJobjects = new List<OBJImporter.OBJ>();
    List<FBXImporter.FBX> m_FBXobjects = new List<FBXImporter.FBX>();

    [SerializeField]
    TransformTool _transformTool;
    [SerializeField]
    Tile m_tile;
    [SerializeField]
    InputField[] m_transformInputs = new InputField[3];
    [SerializeField]
    InputField[] m_rotationInputs = new InputField[3];
    [SerializeField]
    InputField[] m_scaleInputs = new InputField[3];

    private void OnValidate()
    {
        //Make sure array size for m_transformInputs can not be changed in editor
        if (m_transformInputs.Length > 3)
        {
            Array.Resize(ref m_transformInputs, 3);
        }

        //Make sure array size for m_rotationInputs can not be changed in editor
        if (m_rotationInputs.Length > 3)
        {
            Array.Resize(ref m_rotationInputs, 3);
        }

        //Make sure array size for m_scaleInputs can not be changed in editor
        if (m_scaleInputs.Length > 3)
        {
            Array.Resize(ref m_scaleInputs, 3);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //m_tile.objects.Add(GameObject.Instantiate(newTile));
	}

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (m_tile.objects.Contains(hit.transform.gameObject))
                {
                    _transformTool.CurrentGameObject = hit.transform.gameObject;
                }
            }
			//else
			//{
                //_transformTool.CurrentGameObject = null;
            //}
        }
        if(Input.GetKeyDown(KeyCode.Escape))
		{
            _transformTool.CurrentGameObject = null;
        }
    }

    /// <summary>
    /// ImportOBJ opens a dialog to allow user to import an OBJ into the program
    /// </summary>
    public void ImportOBJ()
    {
        string file_path = string.Empty;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        openFileDialog1.InitialDirectory = "C:/Users/" + Environment.UserName + "/";
        openFileDialog1.Filter = "Wavefront files (*.obj)|*.obj|FBX files (*.fbx)|*.fbx|All files (*.*)|*.*";
        openFileDialog1.FilterIndex = 0;
        openFileDialog1.RestoreDirectory = false;

        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            file_path = openFileDialog1.FileName;
        }
#endif
        if (file_path != string.Empty && file_path != "")
        {
            switch (Path.GetExtension(file_path)) 
            {
                case ".obj":
                    {
                        OBJImporter.OBJ newOBJ = new OBJImporter.OBJ();
                        OBJImporter.LoadOBJ(file_path, out newOBJ);
                        GameObject newTile = new GameObject();
                        newTile.transform.parent = m_tile.transform;
                        MeshFilter filter = newTile.AddComponent<MeshFilter>();
                        MeshRenderer renderer = newTile.AddComponent<MeshRenderer>();
                        MeshCollider collider = newTile.AddComponent<MeshCollider>();
                        Mesh mesh = new Mesh();
                        OBJImporter.OBJToMesh(newOBJ, out mesh);
                        Material[] materials;
                        OBJImporter.LoadMaterials(file_path, out materials);
                        filter.mesh = mesh;
                        collider.sharedMesh = mesh;
                        renderer.materials = materials;
                        m_tile.objects.Add(newTile);
                        m_OBJobjects.Add(newOBJ);
                    }
                    break;
                case ".fbx":
					{
                        FBXImporter.FBX newFBX;
                        FBXImporter.LoadFBX(file_path, out newFBX);
                        GameObject newTile = new GameObject();
                        newTile.transform.parent = m_tile.transform;
                        newTile.name = Path.GetFileName(file_path);
                        MeshFilter filter = newTile.AddComponent<MeshFilter>();
                        MeshRenderer renderer = newTile.AddComponent<MeshRenderer>();
                        MeshCollider collider = newTile.AddComponent<MeshCollider>();
                        Mesh _mesh;
                        FBXImporter.FBXToMesh(newFBX, out _mesh);
                        Material[] materials;
                        FBXImporter.LoadMaterials(newFBX, out materials);
                        filter.mesh = _mesh;
                        collider.sharedMesh = _mesh;
                        renderer.materials = materials;
                        m_tile.objects.Add(newTile);
                        m_FBXobjects.Add(newFBX);
                    }
                    break;
                default:
                    break;
            }

        }
    }

    public void SetTransformInputField(float x, float y, float z)
	{
        m_transformInputs[0].text = x.ToString("0.0000");
        m_transformInputs[1].text = y.ToString("0.0000");
        m_transformInputs[2].text = z.ToString("0.0000");
	}

    public void SetRotationInputField(float x, float y, float z)
    {
        m_rotationInputs[0].text = x.ToString("0.0000");
        m_rotationInputs[1].text = y.ToString("0.0000");
        m_rotationInputs[2].text = z.ToString("0.0000");
    }

    public void SetScaleInputField(float x, float y, float z)
    {
        m_scaleInputs[0].text = x.ToString("0.0000");
        m_scaleInputs[1].text = y.ToString("0.0000");
        m_scaleInputs[2].text = z.ToString("0.0000");
    }
}
