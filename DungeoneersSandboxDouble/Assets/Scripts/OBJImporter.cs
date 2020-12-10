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

    /// <summary>
    /// Loads OBJ mesh data from given file into a given OBJ struct object
    /// </summary>
    /// <param name="_path">Path of the file to be read in</param>
    /// <param name="_obj">The given OBJ struct the data is to be stored in</param>
    /// <returns></returns>
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
            //Read in file to an IO stream
            StreamReader _file = File.OpenText(_path);
            int sides = 0;
            while(true)
            {
                //Get the first line of the file
                string line = string.Empty;
                line = _file.ReadLine();

				//if (line.StartsWith("o"))
				//{
				//	int startingIndicie = _obj.vertexIndices.Count;
				//	_obj.subMeshStarts.Add(startingIndicie);
				//}
				//else

                //Collects data if line starts with 'vn' representing Vertex Normal
				if (line.StartsWith("vn"))
                {
                    Vector3 normal = new Vector3();
                    float.TryParse(line.Split(' ')[1], out normal.x);
                    float.TryParse(line.Split(' ')[2], out normal.y);
                    float.TryParse(line.Split(' ')[3], out normal.z);
                    //temp_normals.Add(normal);
                    _obj.normals.Add(normal);
                }
                //Collects data if the line starts with 'vt' representing Vertex TexCoords
                else if (line.StartsWith("vt"))
                {
                    Vector2 uv = new Vector2();
                    float.TryParse(line.Split(' ')[1], out uv.x);
                    float.TryParse(line.Split(' ')[2], out uv.y);
                    //temp_uvs.Add(uv);
                    _obj.uvs.Add(uv);
                }
                //Collects data if the line starts with 'v' representing a Vertex
                else if (line.StartsWith("v"))
                {
                    Vector3 vertex = new Vector3();
                    float.TryParse(line.Split(' ')[1], out vertex.x);
                    float.TryParse(line.Split(' ')[2], out vertex.y);
                    float.TryParse(line.Split(' ')[3], out vertex.z);
                    // temp_vertices.Add(vertex);
                    _obj.verticies.Add(vertex);
                }
                //Notes new sub mesh if the line starts with 'usemtl' representing the end of vertex data and the start of material data 
                else if(line.StartsWith("usemtl"))
                {
                    int startingIndicie = _obj.vertexIndices.Count;
                    _obj.subMeshStarts.Add(startingIndicie);
                }
                //Collects data if the line starts with 'f' representing a face
                else if(line.StartsWith("f"))
                {
                    //Sets up collections for the diffrent mesh veriable indecie
                    int[] vertexIndecies = new int[line.Split(' ').Length - 1];
                    int[] uvIndecies = new int[line.Split(' ').Length - 1];
                    int[] normalIndecies = new int[line.Split(' ').Length - 1];
                    //Store indecie data in appropriate collection
                    for (int i = 1; i < line.Split(' ').Length; i++) {
                        if (!int.TryParse(line.Split(' ')[i].Split('/')[0], out vertexIndecies[i-1])) { return false; }
                        if (!int.TryParse(line.Split(' ')[i].Split('/')[1], out uvIndecies[i-1])) { return false; }
                        if (!int.TryParse(line.Split(' ')[i].Split('/')[2], out normalIndecies[i-1])) { return false; }
                    }
                    //Store the collected data in the refrenced OBJ struct object
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
                    //Determin topology of mesh
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

    /// <summary>
    /// Converts OBJ data stored in a struct OBJ object to a UnityEngine Mesh format
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_mesh"></param>
    public static void OBJToMesh(OBJ _obj, out Mesh _mesh)
    {
        //Set up mesh
        _mesh = new Mesh();
        _mesh.subMeshCount = _obj.subMeshStarts.Count;
        //_mesh.vertices = _obj.verticies.ToArray();
        //_mesh.normals = _obj.normals.ToArray();
        //_mesh.uv = _obj.uvs.ToArray();
        //_mesh.triangles = _obj.vertexIndices.ToArray();

        //Create data collections
        List<Vector3> vertecies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();

        //Loop through the OBJ struct object and collect and arrange pertenant data
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

        //Sort collected data
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

    /// <summary>
    /// Loads OBJ material data from given file into a given UnityEngine Material colection
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_materials"></param>
    /// <returns></returns>
    public static bool LoadMaterials(string _path, out Material[] _materials)
    {
        if (File.Exists(_path))
        {
            //Read in file to an IO stream
            StreamReader _file = File.OpenText(_path);
            string _mtlFilePath = string.Empty;

            while(true)
            {
                //Get the first line of the file
                string line = string.Empty;
                line = _file.ReadLine();

                //Gets the path to the material file if the line starts with 'mtllib' representing indication that a material is used on the mesh
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
            //Check if material is attached to mesh
            if(_mtlFilePath == string.Empty || !File.Exists(_mtlFilePath))
            {
                _materials = null;
                return false;
            }
            else
            {
                //Read in file to an IO stream
                _file = File.OpenText(_mtlFilePath);
                //Create collection to hold materials
                List<Material> materials = new List<Material>();
                while(true)
                {
                    //Get the first line of the file
                    string line = string.Empty;
                    line = _file.ReadLine();

                    //Attempt to replicate materail properties:
                    //Create new material and add it to the collection if the line starts with 'newmtl' meaning that it is the start of a new material
                    if(line.StartsWith("newmtl"))
                    {
                        Material newmtl = new Material(Shader.Find("Standard (Specular setup)"));
                        newmtl.name = line.Replace("newmtl", "");
                        materials.Add(newmtl);
                    }
                    //Collects and stores data if the line starts with 'Ka' representing the ambiant color value(RGB)[Range: 0-1] and is stored in the _EmissionColor variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("Ka"))
                    {
                        UnityEngine.Color col = new UnityEngine.Color();
                        if(!float.TryParse(line.Split(' ')[1], out col.r)) { _materials = null;  return false; }
                        if(!float.TryParse(line.Split(' ')[2], out col.g)) { _materials = null;  return false; }
                        if(!float.TryParse(line.Split(' ')[3], out col.b)) { _materials = null;  return false; }
                        col.a = 1.0f;

                        materials[materials.Count - 1].SetColor("_EmissionColor", col);
                    }
                    //Collects and stores data if the line starts with 'Kd' represnting the diffuse color value(RGB)[Range: 0-1] and is stored in the _Color variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("Kd"))
                    {
                        UnityEngine.Color col = new UnityEngine.Color();
                        if (!float.TryParse(line.Split(' ')[1], out col.r)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[2], out col.g)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[3], out col.b)) { _materials = null; return false; }
                        col.a = 1.0f;

                        materials[materials.Count - 1].SetColor("_Color", col);
                    }
                    //Collects and stores data if the line starts with 'Ks' represnting the specular color value(RGB)[Range: 0-1](0,0,0 => off) and is stored in the _SpecColor variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("Ks"))
                    {
                        UnityEngine.Color col = new UnityEngine.Color();
                        if (!float.TryParse(line.Split(' ')[1], out col.r)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[2], out col.g)) { _materials = null; return false; }
                        if (!float.TryParse(line.Split(' ')[3], out col.b)) { _materials = null; return false; }
                        col.a = 1.0f;

                        materials[materials.Count - 1].SetColor("_SpecColor", col);
                    }
                    //Collects and stores data if the line starts with 'Ns' represnting the specular exponent(float)[Range: 1-1000] and is stored in the _SpecularHighlights variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("Ns"))
                    {
                        float exp;
                        if(!float.TryParse(line.Split(' ')[1], out exp)) { _materials = null; return false; }

                        materials[materials.Count - 1].SetFloat("_SpecularHighlights", exp);
                    }
                    //Collects and stores data if the line starts with 'd' represnting the transparency value(float)[Range: 0-1](1=>opaque) and is stored in the alpha component of the _Color variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("d"))
                    {
                        UnityEngine.Color col = materials[materials.Count - 1].GetColor("_Color");
                        if (!float.TryParse(line.Split(' ')[1], out col.a)) { _materials = null; return false; }

                        materials[materials.Count - 1].SetColor("_Color", col);
                    }
                    //Collects and stores data if the line starts with 'Tr' represnting the transparency value(float)[Range: 0-1](1=>opaque) and is stored in the alpha component of the _Color variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("Tr"))
                    {
                        UnityEngine.Color col = materials[materials.Count - 1].GetColor("_Color");
                        if (!float.TryParse(line.Split(' ')[1], out col.a)) { _materials = null; return false; }
                        col.a = 1.0f - col.a;

                        materials[materials.Count - 1].SetColor("_Color", col);
                    }
                    //Collects and stores data if the line starts with 'Ni' represnting the IOR(Index of Refraction) value(float)[Range: 0.001-10] and is stored in the _GlossyReflections variable of Unity's legacy 'Standard (Specular setup)' shader
                    else if (line.StartsWith("Ni"))
                    {
                        float ior;
                        if (!float.TryParse(line.Split(' ')[1], out ior)) { _materials = null; return false; }
                        materials[materials.Count - 1].SetFloat("_GlossyReflections", ior);
                    }
                    //Checks if the line starts with 'illum' representing the Illumination model(Range: 0-10)[ref model sheet->https://en.wikipedia.org/wiki/Wavefront_.obj_file] used by the model
                    else if (line.StartsWith("illum"))
                    {
                        
                    }
                    //Checks if the line starts with 'map_' meaning the line refrences a image texture for a few of the previouse values
                    else if(line.StartsWith("map_"))
                    {
                        //Set up texture to store image texture data
                        Texture2D _tex = new Texture2D(2, 2);

                        //Loads in the texture data if the line starts with 'map_Ka' representing the image texture for the ambiant component of the material and is stored in the _EmissionMap variable of Unity's legacy 'Standard (Specular setup)' shader
                        if (line.StartsWith("map_Ka"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Ka ", ""))));
                            materials[materials.Count - 1].SetTexture("_EmissionMap", _tex);
                            materials[materials.Count - 1].EnableKeyword("_EMISSION");
                        }
                        //Loads in the texture data if the line starts with 'map_Kd' representing the image texture for the diffuse component of the material and is stored in the _MainTex variable of Unity's legacy 'Standard (Specular setup)' shader
                        else if (line.StartsWith("map_Kd"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Kd ", ""))));
                            materials[materials.Count - 1].SetTexture("_MainTex", _tex);
                        }
                        //Loads in the texture data if the line starts with 'map_Ks' representing the image texture for the specular component of the material and is stored in the _SpecGlossMap variable of Unity's legacy 'Standard (Specular setup)' shader
                        else if (line.StartsWith("map_Ks"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_Ks ", ""))));
                            materials[materials.Count - 1].SetTexture("_SpecGlossMap", _tex);
                        }
                        //Loads in the texture data if the line starts with 'map_d' representing the image texture for the trancparency of the material and is stored in the _OcclusionMap variable of Unity's legacy 'Standard (Specular setup)' shader
                        else if (line.StartsWith("map_d"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_d ", ""))));
                            materials[materials.Count - 1].SetTexture("_OcclusionMap", _tex);
                        }
                        //Loads in the texture data if the line starts with 'map_bump' representing the image texture for the normals destorted by the material and is stored in the _BumpMap variable of Unity's legacy 'Standard (Specular setup)' shader
                        else if (line.StartsWith("map_bump"))
                        {
                            _tex.LoadImage(File.ReadAllBytes(_path.Replace(((_path.Split('/').Length > 1) ? _path.Split('/')[_path.Split('/').Length - 1] : _path.Split('\\')[_path.Split('\\').Length - 1]), line.Replace("map_bump ", ""))));
                            materials[materials.Count - 1].SetTexture("_BumpMap", _tex);
                        }
                        //Loads in the texture data if the line starts with 'map_bump' representing the image texture for the normals destorted by the material and is stored in the _BumpMap variable of Unity's legacy 'Standard (Specular setup)' shader
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
