using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public static class OBJImporter
{
    public struct OBJ
    {
        public List<Vector3> verticies;
        public List<Vector2> uvs;
        public List<Vector3> normals;
        public List<int> vertexIndices;
        public List<int> uvIndices;
        public List<int> normalIndices;
        public List<int> subMeshStarts;
        public MeshTopology topology;
    }

    public static bool LoadOBJ(string _path, out OBJ _obj)
    {
        _obj = new OBJ();

        _obj.verticies = new List<Vector3>();
        _obj.uvs = new List<Vector2>();
        _obj.normals = new List<Vector3>();

        _obj.vertexIndices = new List<int>();
        _obj.uvIndices = new List<int>();
        _obj.normalIndices = new List<int>();
        _obj.subMeshStarts = new List<int>();

        List<Vector3> temp_vertices = new List<Vector3>();
        List<Vector2> temp_uvs = new List<Vector2>();
        List<Vector3> temp_normals = new List<Vector3>();

        if (File.Exists(_path))
        {
            StreamReader _file = File.OpenText(_path);
            int sides = 0;
            while(true)
            { 
                string line = string.Empty;

                line = _file.ReadLine();
                /*if(line.StartsWith("o"))
                {
                    int startingIndicie = _obj.vertexIndices.Count;
                    _obj.subMeshStarts.Add(startingIndicie);
                }
                else*/ if(line.StartsWith("vn"))
                {
                    Vector3 normal = new Vector3();
                    float.TryParse(line.Split(' ')[1], out normal.x);
                    float.TryParse(line.Split(' ')[2], out normal.y);
                    float.TryParse(line.Split(' ')[3], out normal.z);
                    //temp_normals.Add(normal);
                    _obj.normals.Add(normal);
                }
                else if (line.StartsWith("vt"))
                {
                    Vector2 uv = new Vector2();
                    float.TryParse(line.Split(' ')[1], out uv.x);
                    float.TryParse(line.Split(' ')[2], out uv.y);
                    //temp_uvs.Add(uv);
                    _obj.uvs.Add(uv);
                }
                else if (line.StartsWith("v"))
                {
                    Vector3 vertex = new Vector3();
                    float.TryParse(line.Split(' ')[1], out vertex.x);
                    float.TryParse(line.Split(' ')[2], out vertex.y);
                    float.TryParse(line.Split(' ')[3], out vertex.z);
                    // temp_vertices.Add(vertex);
                    _obj.verticies.Add(vertex);
                }
                else if(line.StartsWith("usemtl"))
                {
                    int startingIndicie = _obj.vertexIndices.Count;
                    _obj.subMeshStarts.Add(startingIndicie);
                }
                else if(line.StartsWith("f"))
                {
                    int[] vertexIndecies = new int[line.Split(' ').Length - 1];
                    int[] uvIndecies = new int[line.Split(' ').Length - 1];
                    int[] normalIndecies = new int[line.Split(' ').Length - 1];
                    for (int i = 1; i < line.Split(' ').Length; i++) {
                        if (!int.TryParse(line.Split(' ')[i].Split('/')[0], out vertexIndecies[i-1])) { return false; }
                        if (!int.TryParse(line.Split(' ')[i].Split('/')[1], out uvIndecies[i-1])) { return false; }
                        if (!int.TryParse(line.Split(' ')[i].Split('/')[2], out normalIndecies[i-1])) { return false; }
                    }
                    for(int iv = 0; iv < vertexIndecies.Length; iv++)
                    {
                        _obj.vertexIndices.Add(vertexIndecies[iv]);
                    }
                    for (int iu = 0; iu < uvIndecies.Length; iu++)
                    {
                        _obj.uvIndices.Add(uvIndecies[iu]);
                    }
                    for (int ip = 0; ip < normalIndecies.Length; ip++)
                    {
                        _obj.normalIndices.Add(normalIndecies[ip]);
                    }

                    if(sides == 0)
                    {
                        sides = vertexIndecies.Length;
                        switch(sides)
                        {
                            case 1:
                                _obj.topology = MeshTopology.Points;
                                break;
                            case 2:
                                _obj.topology = MeshTopology.Lines;
                                break;
                            case 3:
                                _obj.topology = MeshTopology.Triangles;
                                break;
                            case 4:
                                _obj.topology = MeshTopology.Quads;
                                break;
                            default:
                                return false;
                        }
                    }
                    else if (sides != vertexIndecies.Length)
                    {
                       return false;
                    }

                }
                if (_file.EndOfStream)
                {
                    break;
                }
            }

            _file.Close();

            //for(int i = 0; i < _obj.vertexIndices.Count; i++)
            //{
            //    Vector3 vertex = new Vector3(temp_vertices[(int)(_obj.vertexIndices[i] - 1)].x, temp_vertices[(int)(_obj.vertexIndices[i] - 1)].y, temp_vertices[(int)(_obj.vertexIndices[i] - 1)].z);
            //    Vector2 uv = new Vector2(temp_uvs[(int)(_obj.uvIndices[i] - 1)].x, temp_uvs[(int)(_obj.uvIndices[i] - 1)].y);
            //    Vector3 normal = new Vector3(temp_normals[(int)(_obj.normalIndices[i] - 1)].x, temp_normals[(int)(_obj.normalIndices[i] - 1)].y, temp_normals[(int)(_obj.normalIndices[i] - 1)].z);
            //    _obj.verticies.Add(vertex);
            //    _obj.uvs.Add(uv);
            //    _obj.normals.Add(normal);
            //}
        }
        else
        {
            return false;
        }

        return true;
    }

    public static void OBJToMesh(OBJ _obj, out Mesh _mesh)
    {
        _mesh = new Mesh();
        _mesh.subMeshCount = _obj.subMeshStarts.Count;
        //_mesh.vertices = _obj.verticies.ToArray();
        //_mesh.normals = _obj.normals.ToArray();
        //_mesh.uv = _obj.uvs.ToArray();
        //_mesh.triangles = _obj.vertexIndices.ToArray();

        List<Vector3> vertecies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();
        for (int s = 0; s < _obj.subMeshStarts.Count; s++) {
            for (int i = _obj.subMeshStarts[s]; i < (((s + 1) < _obj.subMeshStarts.Count) ? (_obj.subMeshStarts[s + 1]) : _obj.vertexIndices.Count); i++)
            {
                Vector3 vertex = new Vector3(_obj.verticies[_obj.vertexIndices[i] - 1].x, _obj.verticies[_obj.vertexIndices[i] - 1].y, _obj.verticies[_obj.vertexIndices[i] - 1].z);
                Vector2 uv = new Vector2(_obj.uvs[(int)(_obj.uvIndices[i] - 1)].x, _obj.uvs[(int)(_obj.uvIndices[i] - 1)].y);
                Vector3 normal = new Vector3(_obj.normals[(int)(_obj.normalIndices[i] - 1)].x, _obj.normals[(int)(_obj.normalIndices[i] - 1)].y, _obj.normals[(int)(_obj.normalIndices[i] - 1)].z);
                vertecies.Add(vertex);
                uvs.Add(uv);
                normals.Add(normal);
                tris.Add(i);
            }
        }

        _mesh.vertices = vertecies.ToArray();
        for (int j = 0; j < _obj.subMeshStarts.Count; j++)
        {
            _mesh.SetIndices(tris, _obj.subMeshStarts[j], (((j + 1) < _obj.subMeshStarts.Count) ? (_obj.subMeshStarts[j + 1] - _obj.subMeshStarts[j]) : (_obj.vertexIndices.Count - _obj.subMeshStarts[j])), _obj.topology, j);
        }
        _mesh.uv = uvs.ToArray();
        _mesh.normals = normals.ToArray();
        //_mesh.triangles = tris.ToArray();
        

        _mesh.RecalculateBounds();
        //_mesh.Optimize();
    }

    public static bool LoadMaterials(string _path, out Material[] _materials)
    {
        if (File.Exists(_path))
        {
            StreamReader _file = File.OpenText(_path);
            string _mtlFilePath = string.Empty;

            while(true)
            {
                string line = string.Empty;

                line = _file.ReadLine();

                if(line.StartsWith("mtllib"))
                {
                    if (_path.Split('/').Length > 1)
                    {
                        _mtlFilePath = _path.Replace(_path.Split('/')[_path.Split('/').Length - 1], line.Replace("mtllib ", ""));
                    }
                    else
                    {
                        _mtlFilePath = _path.Replace(_path.Split('\\')[_path.Split('\\').Length - 1], line.Replace("mtllib ", ""));
                    }
                    break;
                }

                if (_file.EndOfStream)
                {
                    break;
                }
            }
            _file.Close();
            if(_mtlFilePath == string.Empty || !File.Exists(_mtlFilePath))
            {
                _materials = null;
                return false;
            }
            else
            {
                _file = File.OpenText(_mtlFilePath);
                List<Material> materials = new List<Material>();
                while(true)
                {
                    string line = string.Empty;

                    line = _file.ReadLine();

                    if(line.StartsWith("newmtl"))
                    {
                        Material newmtl = new Material(Shader.Find("Standard (Specular setup)"));
                        newmtl.name = line.Replace("newmtl", "");
                        materials.Add(newmtl);
                    }
                    else if(line.StartsWith("Ka"))
                    {
                        Color col = new Color();
                        if(!float.TryParse(line.Split(' ')[1], out col.r)) { _materials = null;  return false; }
                        if(!float.TryParse(line.Split(' ')[2], out col.g)) { _materials = null;  return false; }
                        if(!float.TryParse(line.Split(' ')[3], out col.b)) { _materials = null;  return false; }
                        col.a = 1.0f;

                        materials[materials.Count - 1].SetColor("_EmissionColor", col);
                    }
                    else if (line.StartsWith("Kd"))
                    {
                        Color col = new Color();
                        if (!float.TryParse(line.Split(' ')[1], out col.r)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[2], out col.g)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[3], out col.b)) { _materials = null; return false; }
                        col.a = 1.0f;

                        materials[materials.Count - 1].SetColor("_Color", col);
                    }
                    else if (line.StartsWith("Ks"))
                    {
                        Color col = new Color();
                        if (!float.TryParse(line.Split(' ')[1], out col.r)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[2], out col.g)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[3], out col.b)) { _materials = null; return false; }
                        col.a = 1.0f;

                        materials[materials.Count - 1].SetColor("_SpecColor", col);
                    }
                    else if (line.StartsWith("Ns"))
                    {
                        float exp;
                        if(!float.TryParse(line.Split(' ')[1], out exp)) { _materials = null; return false; }

                        materials[materials.Count - 1].SetFloat("_SpecularHighlights", exp);
                    }
                    else if (line.StartsWith("d"))
                    {
                        Color col = materials[materials.Count - 1].GetColor("_Color");
                        if (!float.TryParse(line.Split(' ')[1], out col.a)) { _materials = null; return false; }

                        materials[materials.Count - 1].SetColor("_Color", col);
                    }
                    else if (line.StartsWith("Tr"))
                    {
                        Color col = materials[materials.Count - 1].GetColor("_Color");
                        if (!float.TryParse(line.Split(' ')[1], out col.a)) { _materials = null; return false; }
                        col.a = 1.0f - col.a;

                        materials[materials.Count - 1].SetColor("_Color", col);
                    }
                    else if (line.StartsWith("Ni"))
                    {
                        float ior;
                        if (!float.TryParse(line.Split(' ')[1], out ior)) { _materials = null; return false; }
                        materials[materials.Count - 1].SetFloat("_GlossyReflections", ior);
                    }
                    else if (line.StartsWith("illum"))
                    {
                        
                    }
                    else if(line.StartsWith("map_"))
                    {
                        Texture2D _tex = new Texture2D(2, 2);
                        if(line.StartsWith("map_Ka"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Ka ", ""))));
                            materials[materials.Count - 1].SetTexture("_EmissionMap", _tex);
                        }
                        else if(line.StartsWith("map_Kd"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Kd ", ""))));
                            materials[materials.Count - 1].SetTexture("_MainTex", _tex);
                        }
                        else if (line.StartsWith("map_Ks"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Ks ", ""))));
                            materials[materials.Count - 1].SetTexture("_SpecGlossMap", _tex);
                        }
                        else if (line.StartsWith("map_d"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_d ", ""))));
                            materials[materials.Count - 1].SetTexture("_OcclusionMap", _tex);
                        }
                        else if (line.StartsWith("map_bump"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_bump ", ""))));
                            materials[materials.Count - 1].SetTexture("_BumpMap", _tex);
                        }
                        else if (line.StartsWith("map_Bump"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Bump ", ""))));
                            materials[materials.Count - 1].SetTexture("_BumpMap", _tex);
                        }
                    }

                    if (_file.EndOfStream)
                    {
                        _materials = materials.ToArray();
                        break;
                    }
                }
            }
            _file.Close();
        }
        else
        {
            _materials = null;
            return false;
        }
        return true;
    }
}
