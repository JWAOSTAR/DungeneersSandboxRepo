using System;
using System.IO;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedExtentions;
using System.Linq;

public class TileMaker : MonoBehaviour
{
    public enum TileFace
	{
        Bottom = 1,
        South,
        West, 
        North,
        Top,
        East
	}

    List<OBJImporter.OBJ> m_OBJobjects = new List<OBJImporter.OBJ>();
    List<FBXImporter.FBX> m_FBXobjects = new List<FBXImporter.FBX>();
    Material m_tileMaterial;
    Vector4[] TileFaceCoordinates = new Vector4[6];

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
    [SerializeField]
    Texture2D baseTile;
    [SerializeField]
    Image[] TileFaces = new Image[6];

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

        //Make sure array size for TileFaces can not be changed in editor
        if (TileFaces.Length > 6)
        {
            Array.Resize(ref TileFaces, 6);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //m_tile.objects.Add(GameObject.Instantiate(newTile));
        m_tileMaterial = new Material(Shader.Find("Standard"));
        m_tile.GetComponent<MeshRenderer>().material = m_tileMaterial;
        Color[] faceColors = new Color[] { Color.blue, Color.green, Color.cyan, Color.red, Color.magenta, new Color(1.0f, 1.0f, 0.0f) };
        for (int i = 0; i < faceColors.Length; i++) 
        {
            bool foundCol = false;
            TileFaceCoordinates[i].x = 0.0f;
            TileFaceCoordinates[i].y = -1.0f;
            TileFaceCoordinates[i].z = -1.0f;
            TileFaceCoordinates[i].w = -1.0f;
            for (int y = 0; y < baseTile.height; y++)
            {
                for (int x = (int)TileFaceCoordinates[i].x; x < baseTile.width; x++)
                {
                    if (!foundCol && baseTile.GetPixel(x, y) == faceColors[i])
                    {
                        foundCol = true;
                        TileFaceCoordinates[i].x = x;
                        TileFaceCoordinates[i].y = y;
                    }
                    else if(foundCol && baseTile.GetPixel(x, y) != faceColors[i])
					{
                        if(x == (int)TileFaceCoordinates[i].x)
						{
                            TileFaceCoordinates[i].y = y - 1;
                        }
                        if(TileFaceCoordinates[i].z == -1.0f)
						{
                            TileFaceCoordinates[i].z = x - 1;
                        }
                        break;
					}
                }
                if(TileFaceCoordinates[i].y != -1.0f)
				{
                    break;
				}
            }
        }

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

    public void SetObjectScaleX(string _val)
	{
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.localScale = new Vector3(float.Parse(_val), _transformTool.CurrentGameObject.transform.localScale.y, _transformTool.CurrentGameObject.transform.localScale.z);
        }
    }

    public void SetObjectScaleY(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {        
            _transformTool.CurrentGameObject.transform.localScale = new Vector3(_transformTool.CurrentGameObject.transform.localScale.x, float.Parse(_val), _transformTool.CurrentGameObject.transform.localScale.z);
        }


    }

    public void SetObjectScaleZ(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.localScale = new Vector3(_transformTool.CurrentGameObject.transform.localScale.x, _transformTool.CurrentGameObject.transform.localScale.y, float.Parse(_val));
        }
    }

    public void SetObjectPositionX(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.position = new Vector3(float.Parse(_val), _transformTool.CurrentGameObject.transform.position.y, _transformTool.CurrentGameObject.transform.position.z);
        }
    }

    public void SetObjectPositionY(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.position = new Vector3(_transformTool.CurrentGameObject.transform.position.x, float.Parse(_val), _transformTool.CurrentGameObject.transform.position.z);
        }
    }

    public void SetObjectPositionZ(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.position = new Vector3(_transformTool.CurrentGameObject.transform.position.x, _transformTool.CurrentGameObject.transform.position.y, float.Parse(_val));
        }
    }

    public void SetObjectRotationX(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.rotation = Quaternion.Euler(float.Parse(_val), _transformTool.CurrentGameObject.transform.rotation.eulerAngles.y, _transformTool.CurrentGameObject.transform.rotation.eulerAngles.z);
        }
    }

    public void SetObjectRotationY(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.rotation = Quaternion.Euler(_transformTool.CurrentGameObject.transform.rotation.eulerAngles.x, float.Parse(_val), _transformTool.CurrentGameObject.transform.rotation.eulerAngles.z);
        }
    }

    public void SetObjectRotationZ(string _val)
    {
        if (_transformTool.CurrentGameObject != null)
        {
            _transformTool.CurrentGameObject.transform.rotation = Quaternion.Euler(_transformTool.CurrentGameObject.transform.rotation.eulerAngles.x, _transformTool.CurrentGameObject.transform.rotation.eulerAngles.y, float.Parse(_val));
        }
    }

    public void SetFaceTexture(int face)
	{
        string file_path = string.Empty;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        openFileDialog1.InitialDirectory = "C:/Users/" + Environment.UserName + "/";
        openFileDialog1.Filter = "PNG files (*.png)|*.png|JPEG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
        openFileDialog1.FilterIndex = 0;
        openFileDialog1.RestoreDirectory = false;

        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            file_path = openFileDialog1.FileName;
        }
#endif
        if (file_path != string.Empty && file_path != "")
        {
            Texture2D newTex = new Texture2D(2, 2);
            newTex.LoadImage(File.ReadAllBytes(file_path));
            TileFaces[face - 1].sprite = Sprite.Create(newTex, new Rect(0.0f, 0.0f, newTex.width, newTex.height), new Vector2(0.0f, 0.0f));
            newTex.ScaleBilinear((int)(TileFaceCoordinates[face - 1].z - TileFaceCoordinates[face - 1].x), (int)(TileFaceCoordinates[face - 1].w - TileFaceCoordinates[face - 1].y));
            
            if(m_tileMaterial.mainTexture == null)
			{
                m_tileMaterial.mainTexture = new Texture2D((int)(TileFaceCoordinates[face - 1].z - TileFaceCoordinates[face - 1].x), (int)(TileFaceCoordinates[face - 1].w - TileFaceCoordinates[face - 1].y));
                ((Texture2D)(m_tileMaterial.mainTexture)).SetPixels(Enumerable.Repeat(Color.clear, ((Texture2D)(m_tileMaterial.mainTexture)).GetPixels().Length).ToArray());
            }

            for(int y = (int)TileFaceCoordinates[face - 1].y; y < (int)TileFaceCoordinates[face - 1].w; y++)
			{
                for(int x = (int)TileFaceCoordinates[face - 1].x; x < (int)TileFaceCoordinates[face - 1].z; x++)
				{
                    ((Texture2D)(m_tileMaterial.mainTexture)).SetPixel(x, y, newTex.GetPixel(x, y));
                }
			}
        }
	}


}
