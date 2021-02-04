using System;
using System.Collections;
using System.Collections.Generic;
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
       GenerateOrigin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateOrigin()
	{
        m_originCenter.sliceNeighbors[0] = new Slice();
        m_originCenter.sliceNeighbors[1] = new Slice();
        m_originCenter.sliceNeighbors[2] = new Slice();
        m_originCenter.sliceNeighbors[3] = new Slice();
        m_originCenter.sliceNeighbors[4] = new Slice();
        m_originCenter.sliceNeighbors[5] = new Slice();
        m_originCenter.sliceNeighbors[6] = new Slice();
        m_originCenter.sliceNeighbors[7] = new Slice();

        m_originCenter.slice = Instantiate(m_slices[(int)TileScetion.Center], this.transform);
        m_originCenter.TopLeft.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
        m_originCenter.TopRight.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
        m_originCenter.TopRight.slice.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
        m_originCenter.BottomRight.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform); 
        m_originCenter.BottomRight.slice.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
        m_originCenter.BottomLeft.slice = Instantiate(m_slices[(int)TileScetion.Corner], this.transform);
        m_originCenter.BottomLeft.slice.transform.Rotate(0.0f, 270.0f, 0.0f, Space.World);
        m_originCenter.Top.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
        m_originCenter.Right.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
        m_originCenter.Right.slice.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
        m_originCenter.Bottom.slice  = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
        m_originCenter.Bottom.slice.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
        m_originCenter.Left.slice = Instantiate(m_slices[(int)TileScetion.Edge], this.transform);
        m_originCenter.Left.slice.transform.Rotate(0.0f, 270.0f, 0.0f, Space.World);

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

        for (int i = 0; i < transform.childCount; i++)
		{
            transform.GetChild(i).transform.localPosition = new Vector3(m_positionOffset.x, m_positionOffset.y, m_positionOffset.z);
            transform.GetChild(i).transform.localScale = new Vector3(m_scaleOffset.x, m_scaleOffset.y, m_scaleOffset.z);
            transform.GetChild(i).transform.Rotate(m_rotationOffset.x,m_rotationOffset.y,m_rotationOffset.z, Space.Self);
		}
	}
}
