using System;
using System.IO;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    List<OBJImporter.OBJ> m_OBJobjects = new List<OBJImporter.OBJ>();
    List<FBXImporter.FBX> m_FBXobjects = new List<FBXImporter.FBX>();
    Tile m_tile = new Tile();

    // Start is called before the first frame update
    void Start()
    {
        FBXImporter.FBX newFBX;
        FBXImporter.LoadFBX("C:\\Users\\JWAOSTAR\\Desktop\\Blender\\test_cube_textire_embed_bin.fbx", out newFBX);
        Mesh _mesh;
        FBXImporter.FBXToMesh(newFBX, out _mesh);
        GameObject newTile = new GameObject();
        MeshFilter filter = newTile.AddComponent<MeshFilter>();
        MeshRenderer renderer = newTile.AddComponent<MeshRenderer>();
        MeshCollider collider = newTile.AddComponent<MeshCollider>();
        Material[] materials;
        FBXImporter.LoadMaterials(newFBX, out materials);
        filter.mesh = _mesh;
        collider.sharedMesh = _mesh;
        renderer.materials = materials;
        m_tile.objects.Add(newTile);
        m_FBXobjects.Add(newFBX);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        openFileDialog1.Filter = "Wavefront files (*.obj)|*.obj";
        openFileDialog1.FilterIndex = 0;
        openFileDialog1.RestoreDirectory = false;

        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            file_path = openFileDialog1.FileName;
        }
#endif
        if (file_path != string.Empty && file_path != "")
        {
            OBJImporter.OBJ newOBJ = new OBJImporter.OBJ();
            OBJImporter.LoadOBJ(file_path, out newOBJ);
            GameObject newTile = new GameObject();
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
    }
}
