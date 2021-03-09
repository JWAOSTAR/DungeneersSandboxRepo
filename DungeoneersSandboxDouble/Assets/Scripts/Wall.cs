using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public enum TileScetion
	{
        Center,
        Edge,
        Corner,
        InnerCorner
	}

    public class Slice
	{
        public GameObject slice;
        public int cornerNeighbors = 0;
        public Slice[] sliceNeighbors = new Slice[] { null,null,null,null,null,null,null,null };
        public Slice Top { get { return sliceNeighbors[0]; } set { sliceNeighbors[0] = value; } }
        public Slice TopRight { get { return sliceNeighbors[1]; } set { sliceNeighbors[1] = value; } }
        public Slice Right { get { return sliceNeighbors[2]; } set { sliceNeighbors[2] = value; } }
        public Slice BottomRight { get { return sliceNeighbors[3]; } set { sliceNeighbors[3] = value; } }
        public Slice Bottom { get { return sliceNeighbors[4]; } set { sliceNeighbors[4] = value; } }
        public Slice BottomLeft { get { return sliceNeighbors[5]; } set { sliceNeighbors[5] = value; } }
        public Slice Left { get { return sliceNeighbors[6]; } set { sliceNeighbors[6] = value; } }
        public Slice TopLeft { get { return sliceNeighbors[7]; } set { sliceNeighbors[7] = value; } }
	}

    //GameObject m_wall;
    Slice m_originCenter = new Slice();

    [SerializeField]
    GameObject[] m_slices = new GameObject[4];
    [SerializeField]
    Vector3 m_positionOffset;
    [SerializeField]
    Vector3 m_rotationOffset;
    [SerializeField]
    Vector3 m_scaleOffset;
    [SerializeField]
    float m_allowance;

    [HideInInspector]
    public GameObject uploadStorage;
    public Slice OriginCenter { get { return m_originCenter; } }


    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    private void OnValidate()
    {
        //Make sure array size for m_handles can not be changed in editor
        if (m_slices.Length != 4)
        {
            Array.Resize(ref m_slices, 4);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uploadStorage = GameObject.Find("HiddenObject");
        uploadStorage.hideFlags = HideFlags.HideInHierarchy;

        m_originCenter.sliceNeighbors[0] = new Slice();
        m_originCenter.sliceNeighbors[1] = new Slice();
        m_originCenter.sliceNeighbors[2] = new Slice();
        m_originCenter.sliceNeighbors[3] = new Slice();
        m_originCenter.sliceNeighbors[4] = new Slice();
        m_originCenter.sliceNeighbors[5] = new Slice();
        m_originCenter.sliceNeighbors[6] = new Slice();
        m_originCenter.sliceNeighbors[7] = new Slice();

        m_originCenter.TopLeft.Right = m_originCenter.Top;
        m_originCenter.TopLeft.BottomRight = m_originCenter;
        m_originCenter.TopLeft.Bottom = m_originCenter.Left;

        m_originCenter.Top.Left = m_originCenter.TopLeft;
        m_originCenter.Top.BottomLeft = m_originCenter.Left;
        m_originCenter.Top.Bottom = m_originCenter;
        m_originCenter.Top.BottomRight = m_originCenter.Right;
        m_originCenter.Top.Right = m_originCenter.TopRight;

        m_originCenter.TopRight.Left = m_originCenter.Top;
        m_originCenter.TopRight.BottomLeft = m_originCenter;
        m_originCenter.TopRight.Bottom = m_originCenter.Right;

        m_originCenter.Right.TopLeft = m_originCenter.Top;
        m_originCenter.Right.Top = m_originCenter.TopRight;
        m_originCenter.Right.Left = m_originCenter;
        m_originCenter.Right.BottomLeft = m_originCenter.Bottom;
        m_originCenter.Right.Bottom = m_originCenter.BottomRight;

        m_originCenter.BottomRight.Top = m_originCenter.Right;
        m_originCenter.BottomRight.TopLeft = m_originCenter;
        m_originCenter.BottomRight.Left = m_originCenter.Bottom;

        m_originCenter.Bottom.Left = m_originCenter.BottomLeft;
        m_originCenter.Bottom.TopLeft = m_originCenter.Left;
        m_originCenter.Bottom.Top = m_originCenter;
        m_originCenter.Bottom.TopRight = m_originCenter.Right;
        m_originCenter.Bottom.Right = m_originCenter.BottomRight;

        m_originCenter.BottomLeft.Top = m_originCenter.Left;
        m_originCenter.BottomLeft.TopRight = m_originCenter;
        m_originCenter.BottomLeft.Right = m_originCenter.Bottom;

        m_originCenter.Left.Top = m_originCenter.TopLeft;
        m_originCenter.Left.TopRight = m_originCenter.Top;
        m_originCenter.Left.Right = m_originCenter;
        m_originCenter.Left.BottomRight = m_originCenter.Bottom;
        m_originCenter.Left.Bottom = m_originCenter.BottomLeft;
        //GenerateOrigin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateOrigin()
	{
        if (m_slices[(int)TileScetion.Center] != null && m_slices[(int)TileScetion.Corner] != null && m_slices[(int)TileScetion.Edge] != null)
        {
            m_originCenter.slice = Instantiate(m_slices[(int)TileScetion.Center], this.transform);
            m_originCenter.slice.SetActive(true);

            m_originCenter.TopLeft.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
            m_originCenter.TopLeft.slice.SetActive(true);

            m_originCenter.TopRight.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
            m_originCenter.TopRight.slice.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
            m_originCenter.TopRight.slice.SetActive(true);

            m_originCenter.BottomRight.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
            m_originCenter.BottomRight.slice.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
            m_originCenter.BottomRight.slice.SetActive(true);

            m_originCenter.BottomLeft.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
            m_originCenter.BottomLeft.slice.transform.Rotate(0.0f, 270.0f, 0.0f, Space.World);
            m_originCenter.BottomLeft.slice.SetActive(true);

            m_originCenter.Top.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
            m_originCenter.Top.slice.SetActive(true);

            m_originCenter.Right.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
            m_originCenter.Right.slice.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
            m_originCenter.Right.slice.SetActive(true);

            m_originCenter.Bottom.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
            m_originCenter.Bottom.slice.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
            m_originCenter.TopRight.slice.SetActive(true);

            m_originCenter.Left.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
            m_originCenter.Left.slice.transform.Rotate(0.0f, 270.0f, 0.0f, Space.World);
            m_originCenter.Left.slice.SetActive(true);


            m_originCenter.slice.name = "Center";
            m_originCenter.TopLeft.slice.name = "TopLeft";
            m_originCenter.TopRight.slice.name = "TopRight";
            m_originCenter.BottomRight.slice.name = "BottomRight";
            m_originCenter.BottomLeft.slice.name = "BottomLeft";
            m_originCenter.Top.slice.name = "Top";
            m_originCenter.Right.slice.name = "Right";
            m_originCenter.Bottom.slice.name = "Bottom";
            m_originCenter.Left.slice.name = "Left";

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.localPosition = new Vector3(m_positionOffset.x, m_positionOffset.y, m_positionOffset.z);
                transform.GetChild(i).transform.localScale = new Vector3(m_scaleOffset.x, m_scaleOffset.y, m_scaleOffset.z);
                transform.GetChild(i).transform.Rotate(m_rotationOffset.x, m_rotationOffset.y, m_rotationOffset.z, Space.Self);
            }
        }
	}

	public void ResetWall()
	{
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.localPosition = new Vector3(m_positionOffset.x, m_positionOffset.y, m_positionOffset.z);
            transform.GetChild(i).transform.localScale = new Vector3(m_scaleOffset.x, m_scaleOffset.y, m_scaleOffset.z);
        }
    }

	public void SetScaleWidth(float width, Vector3 midPoint, bool west = false)
	{
        if(midPoint == null)
		{
            midPoint = Vector3.zero;
		}

        m_originCenter.slice.transform.localScale = new Vector3(width*m_scaleOffset.x*3.0f, m_originCenter.slice.transform.localScale.y, m_originCenter.slice.transform.localScale.z);
        m_originCenter.Top.slice.transform.localScale = m_originCenter.Bottom.slice.transform.localScale = new Vector3(width*m_scaleOffset.x*3.0f, m_originCenter.Top.slice.transform.localScale.y, m_originCenter.Top.slice.transform.localScale.z);

        m_originCenter.slice.transform.position = new Vector3(midPoint.x + m_positionOffset.x + ((west ? -1 : 1) * m_allowance), m_originCenter.slice.transform.position.y, midPoint.z + m_positionOffset.z);
        m_originCenter.Bottom.slice.transform.position = new Vector3(midPoint.x + m_positionOffset.x + ((west ? -1 : 1) * m_allowance), m_originCenter.Bottom.slice.transform.position.y, m_originCenter.Bottom.slice.transform.position.z);
        m_originCenter.Top.slice.transform.position = new Vector3(midPoint.x + m_positionOffset.x + ((west ? -1 : 1) * m_allowance), m_originCenter.Top.slice.transform.position.y, m_originCenter.Top.slice.transform.position.z);

        if (!west) {
            float LR_distance = Vector3.Distance(m_originCenter.Right.slice.transform.position, m_originCenter.slice.transform.position);

            m_originCenter.Left.slice.transform.position = new Vector3(m_originCenter.slice.transform.position.x + LR_distance, m_originCenter.Left.slice.transform.position.y, m_originCenter.Left.slice.transform.position.z);
            m_originCenter.TopLeft.slice.transform.position = new Vector3(m_originCenter.slice.transform.position.x + LR_distance, m_originCenter.TopLeft.slice.transform.position.y, m_originCenter.TopLeft.slice.transform.position.z);
            m_originCenter.BottomLeft.slice.transform.position = new Vector3(m_originCenter.slice.transform.position.x + LR_distance, m_originCenter.BottomLeft.slice.transform.position.y, m_originCenter.BottomLeft.slice.transform.position.z);
        }
		else
		{
            float LR_distance = Vector3.Distance(m_originCenter.Left.slice.transform.position, m_originCenter.slice.transform.position);

            m_originCenter.Right.slice.transform.position = new Vector3(m_originCenter.slice.transform.position.x - LR_distance, m_originCenter.Right.slice.transform.position.y, m_originCenter.Left.slice.transform.position.z);
            m_originCenter.TopRight.slice.transform.position = new Vector3(m_originCenter.slice.transform.position.x - LR_distance, m_originCenter.TopRight.slice.transform.position.y, m_originCenter.TopLeft.slice.transform.position.z);
            m_originCenter.BottomRight.slice.transform.position = new Vector3(m_originCenter.slice.transform.position.x - LR_distance, m_originCenter.BottomRight.slice.transform.position.y, m_originCenter.BottomRight.slice.transform.position.z);
        }
    }

    public void SetScaleHeight(float height, Vector3 midPoint, bool north = false)
    {
        if (midPoint == null)
        {
            midPoint = Vector3.zero;
        }

        m_originCenter.slice.transform.localScale = new Vector3(m_originCenter.slice.transform.localScale.x, height * m_scaleOffset.y * 3.0f, m_originCenter.slice.transform.localScale.z);
        m_originCenter.Right.slice.transform.localScale = m_originCenter.Left.slice.transform.localScale = new Vector3(height * m_scaleOffset.x * 3.0f, m_originCenter.Right.slice.transform.localScale.y, m_originCenter.Right.slice.transform.localScale.z);

        m_originCenter.slice.transform.position = new Vector3(midPoint.x + m_positionOffset.x, m_originCenter.slice.transform.position.y, midPoint.z + m_positionOffset.z + ((north ? 1 : -1)*m_allowance));
        m_originCenter.Right.slice.transform.position = new Vector3(m_originCenter.Right.slice.transform.position.x, m_originCenter.Right.slice.transform.position.y, midPoint.z + m_positionOffset.z + ((north ? 1 : -1) * m_allowance));
        m_originCenter.Left.slice.transform.position = new Vector3(m_originCenter.Left.slice.transform.position.x, m_originCenter.Left.slice.transform.position.y, midPoint.z + m_positionOffset.z + ((north ? 1 : -1) * m_allowance));
        if (!north)
        {
            float TB_distance = m_originCenter.Bottom.slice.transform.position.z - m_originCenter.slice.transform.position.z;

            m_originCenter.Top.slice.transform.position = new Vector3(m_originCenter.Top.slice.transform.position.x, m_originCenter.Top.slice.transform.position.y, m_originCenter.slice.transform.position.z - TB_distance);
            m_originCenter.TopLeft.slice.transform.position = new Vector3(m_originCenter.TopLeft.slice.transform.position.x, m_originCenter.TopLeft.slice.transform.position.y, m_originCenter.slice.transform.position.z - TB_distance);
            m_originCenter.TopRight.slice.transform.position = new Vector3(m_originCenter.TopRight.slice.transform.position.x, m_originCenter.TopRight.slice.transform.position.y, m_originCenter.slice.transform.position.z - TB_distance);
        }
		else
		{
            float TB_distance = m_originCenter.Top.slice.transform.position.z - m_originCenter.slice.transform.position.z;

            m_originCenter.Bottom.slice.transform.position = new Vector3(m_originCenter.Bottom.slice.transform.position.x, m_originCenter.Bottom.slice.transform.position.y, m_originCenter.slice.transform.position.z - TB_distance);
            m_originCenter.BottomLeft.slice.transform.position = new Vector3(m_originCenter.BottomLeft.slice.transform.position.x, m_originCenter.BottomLeft.slice.transform.position.y, m_originCenter.slice.transform.position.z - TB_distance);
            m_originCenter.BottomRight.slice.transform.position = new Vector3(m_originCenter.BottomRight.slice.transform.position.x, m_originCenter.BottomRight.slice.transform.position.y, m_originCenter.slice.transform.position.z - TB_distance);
        }
    }

    public void SetSlice(int i)
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
                        MeshFilter filter;
                        MeshRenderer renderer;
                        MeshCollider collider;
                        if (m_slices[i] == null)
                        {
                            m_slices[i] = new GameObject();
                            m_slices[i].transform.parent = uploadStorage.transform;
                            m_slices[i].SetActive(false);
                            filter = m_slices[i].AddComponent<MeshFilter>();
                            renderer = m_slices[i].AddComponent<MeshRenderer>();
                            collider = m_slices[i].AddComponent<MeshCollider>();
                        }
						else
						{
                            filter = m_slices[i].GetComponent<MeshFilter>();
                            renderer = m_slices[i].GetComponent<MeshRenderer>();
                            collider = m_slices[i].GetComponent<MeshCollider>();
                        }
                        //m_slices[i].transform.parent = this.transform;
                        Mesh mesh = new Mesh();
                        OBJImporter.OBJToMesh(newOBJ, out mesh);
                        Material[] materials;
                        OBJImporter.LoadMaterials(file_path, out materials);
                        filter.mesh = mesh;
                        collider.sharedMesh = mesh;
                        renderer.materials = materials;
                    }
                    break;
                case ".fbx":
                    {
                        FBXImporter.FBX newFBX;
                        FBXImporter.LoadFBX(file_path, out newFBX);
                        MeshFilter filter;
                        MeshRenderer renderer;
                        MeshCollider collider;
                        if (m_slices[i] == null)
                        {
                            m_slices[i] = new GameObject();
                            m_slices[i].transform.parent = uploadStorage.transform;
                            m_slices[i].SetActive(false);
                            filter = m_slices[i].AddComponent<MeshFilter>();
                            renderer = m_slices[i].AddComponent<MeshRenderer>();
                            collider = m_slices[i].AddComponent<MeshCollider>();
                        }
                        else
                        {
                            filter = m_slices[i].GetComponent<MeshFilter>();
                            renderer = m_slices[i].GetComponent<MeshRenderer>();
                            collider = m_slices[i].GetComponent<MeshCollider>();
                        }
						//newTile.transform.parent = m_tile.transform;
						//newTile.name = Path.GetFileName(file_path);
						//FBXImporter.OutputFBXToFile(newFBX, "C:/Users/JWAOSTAR/Desktop/");
						Mesh _mesh;
						FBXImporter.FBXToMesh(newFBX, out _mesh);
						Material[] materials;
						FBXImporter.LoadMaterials(newFBX, out materials);
						filter.mesh = _mesh;
						collider.sharedMesh = _mesh;
						renderer.materials = materials;
					}
                    break;
                default:
                    //return false;
                    break;
            }

        }
        //return true;
    }
}
