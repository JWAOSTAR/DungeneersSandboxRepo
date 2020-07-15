using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class OBJImporter
{
    public struct OBJ
    {
        public List<Vector3> verticies;
        public List<Vector2> uvs;
        public List<Vector3> normals;
        public List<uint> vertexIndices;
        public List<uint> uvIndices;
        public List<uint> normalIndices;
    }

    public static bool LoadOBJ(string _path, out OBJ _obj)
    {
        _obj = new OBJ();

        _obj.verticies = new List<Vector3>();
        _obj.uvs = new List<Vector2>();
        _obj.normals = new List<Vector3>();

        _obj.vertexIndices = new List<uint>();
        _obj.uvIndices = new List<uint>();
        _obj.normalIndices = new List<uint>();

        List<Vector3> temp_vertices = new List<Vector3>();
        List<Vector2> temp_uvs = new List<Vector2>();
        List<Vector3> temp_normals = new List<Vector3>();

        if (File.Exists(_path))
        {
            StreamReader _file = File.OpenText(_path);

            while(true)
            {
                string line = string.Empty;

                line = _file.ReadLine();

                if(_file.EndOfStream)
                {
                    break;
                }

                if(line.StartsWith("vn"))
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
                else if(line.StartsWith("f"))
                {
                    uint[] vertexIndecies = new uint[3];
                    uint[] uvIndecies = new uint[3];
                    uint[] normalIndecies = new uint[3];
                    for (int i = 1; i < line.Split(' ').Length; i++) {
                        if (!uint.TryParse(line.Split(' ')[i].Split('/')[0], out vertexIndecies[i-1])) { return false; }
                        if (!uint.TryParse(line.Split(' ')[i].Split('/')[1], out uvIndecies[i-1])) { return false; }
                        if (!uint.TryParse(line.Split(' ')[i].Split('/')[2], out normalIndecies[i-1])) { return false; }
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
                }
            }

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
}
