using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using UnityEngine;

public static class FBXImporter
{
	public enum FBX_Versions
	{
		V6 = 6000,
		V6_1 = 6100,
		v7 = 7000,
		v7_1 = 7100,
		v7_2 = 7200,
		v7_3 = 7300,
		v7_4 = 7400,
		v7_5 = 7500,
	}
	public class Properties
	{
		public char TypeCode = ' ';
		public int Length = -1;
		public byte[] Data;
	}
	public class Node
	{
		public ulong EndOffset;
		public ulong NumProperties;
		public ulong PropetiesListLength;
		public string Name;
		public Properties[] properties;
		public List<Node> NestedNodes = new List<Node>();
		public bool isNull;
		public Node parent;
		//public byte[] nullRecords;
	}

	public class FBX
	{
		public byte[] header;
		public uint version;
		public Node node;
	}

	public static bool LoadFBX(string _path, out FBX _fbx)
	{
		_fbx = new FBX();
		_fbx.node = new Node();

		if (File.Exists(_path))
		{
			_fbx.node.NestedNodes.Add(new Node() { parent = _fbx.node });
			Node current = _fbx.node.NestedNodes[0];
			BinaryReader _file = new BinaryReader(File.Open(_path, FileMode.Open));
			_fbx.header = _file.ReadBytes(Encoding.ASCII.GetBytes("Kaydara FBX Binary  \0\x1a\0").Length);
			uint version = _file.ReadUInt32();
			_fbx.version = version;
			current.EndOffset = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
			current.NumProperties = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
			current.PropetiesListLength = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
			//int nameLength = _file.ReadByte();
			current.Name = Encoding.ASCII.GetString(_file.ReadBytes(_file.ReadByte()));
			current.isNull = false;

			if (current.PropetiesListLength > 0)
			{
				current.properties = new Properties[current.NumProperties];
				for (ulong i = 0; i < current.NumProperties; i++)
				{
					current.properties[i] = new Properties { TypeCode = (char)_file.ReadByte() };
					switch (current.properties[i].TypeCode)
					{
						case 'Y':
							current.properties[i].Data = BitConverter.GetBytes(_file.ReadUInt16());
							break;
						case 'C':
							current.properties[i].Data = BitConverter.GetBytes(_file.ReadByte());
							break;
						case 'I':
							current.properties[i].Data = BitConverter.GetBytes(_file.ReadInt32());
							break;
						case 'F':
							current.properties[i].Data = BitConverter.GetBytes(_file.ReadSingle());
							break;
						case 'D':
							current.properties[i].Data = BitConverter.GetBytes(_file.ReadDouble());
							break;
						case 'L':
							current.properties[i].Data = BitConverter.GetBytes(_file.ReadInt64());
							break;
						case 'f':
							{
								int length = current.properties[i].Length = _file.ReadInt32();
								int encoding = _file.ReadInt32();
								int compressedLength = _file.ReadInt32();
								float[] _data = new float[length];
								long endPos = _file.BaseStream.Position + compressedLength;
								BinaryReader br = _file;
								if (encoding != 0)
								{
									//if(encoding != 1)
									//{
									//	return false;
									//}
									_file.BaseStream.Position += 2;
									DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
									br = new BinaryReader(decode);
								}
								for (int j = 0; j < length; j++)
								{
									_data[j] = br.ReadSingle();
								}
								if (encoding != 0)
								{
									_file.BaseStream.Position = endPos;
								}
								current.properties[i].Data = new byte[_data.Length * sizeof(float)];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'd':
							{
								int length = current.properties[i].Length = _file.ReadInt32();
								int encoding = _file.ReadInt32();
								int compressedLength = _file.ReadInt32();
								double[] _data = new double[length];
								long endPos = _file.BaseStream.Position + compressedLength;
								BinaryReader br = _file;
								if (encoding != 0)
								{
									//if(encoding != 1)
									//{
									//	return false;
									//}
									_file.BaseStream.Position += 2;
									DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
									br = new BinaryReader(decode);
								}
								for (int j = 0; j < length; j++)
								{
									_data[j] = br.ReadDouble();
								}
								if (encoding != 0)
								{
									_file.BaseStream.Position = endPos;
								}
								current.properties[i].Data = new byte[_data.Length * sizeof(double)];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'l':
							{
								int length = current.properties[i].Length = _file.ReadInt32();
								int encoding = _file.ReadInt32();
								int compressedLength = _file.ReadInt32();
								long[] _data = new long[length];
								long endPos = _file.BaseStream.Position + compressedLength;
								BinaryReader br = _file;
								if (encoding != 0)
								{
									//if(encoding != 1)
									//{
									//	return false;
									//}
									_file.BaseStream.Position += 2;
									DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
									br = new BinaryReader(decode);
								}
								for (int j = 0; j < length; j++)
								{
									_data[j] = br.ReadInt64();
								}
								if (encoding != 0)
								{
									_file.BaseStream.Position = endPos;
								}
								current.properties[i].Data = new byte[_data.Length * sizeof(long)];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'i':
							{
								int length = current.properties[i].Length = _file.ReadInt32();
								int encoding = _file.ReadInt32();
								int compressedLength = _file.ReadInt32();
								int[] _data = new int[length];
								long endPos = _file.BaseStream.Position + compressedLength;
								BinaryReader br = _file;
								if (encoding != 0)
								{
									//if(encoding != 1)
									//{
									//	return false;
									//}
									_file.BaseStream.Position += 2;
									DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
									br = new BinaryReader(decode);
								}
								for (int j = 0; j < length; j++)
								{
									_data[j] = br.ReadInt32();
								}
								if (encoding != 0)
								{
									_file.BaseStream.Position = endPos;
								}
								current.properties[i].Data = new byte[_data.Length * sizeof(int)];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'b':
							{
								int length = current.properties[i].Length = _file.ReadInt32();
								int encoding = _file.ReadInt32();
								int compressedLength = _file.ReadInt32();
								bool[] _data = new bool[length];
								long endPos = _file.BaseStream.Position + compressedLength;
								BinaryReader br = _file;
								if (encoding != 0)
								{
									//if(encoding != 1)
									//{
									//	return false;
									//}
									_file.BaseStream.Position += 2;
									DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
									br = new BinaryReader(decode);
								}
								for (int j = 0; j < length; j++)
								{
									_data[j] = br.ReadBoolean();
								}
								if (encoding != 0)
								{
									_file.BaseStream.Position = endPos;
								}
								current.properties[i].Data = new byte[_data.Length * sizeof(bool)];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'S':
							{
								string str = Encoding.ASCII.GetString(_file.ReadBytes(_file.ReadInt32()));
								if (str.Contains("\0\x1"))
								{
									string[] t = str.Split(new[] { "\0\x1" }, StringSplitOptions.None);
									string corrected_str = string.Empty;
									for (int j = t.Length - 1; j >= 0; j--)
									{
										if (j == (t.Length - 1))
										{
											corrected_str += "::";
										}
										corrected_str += t[j];
									}
									str = corrected_str;
								}
								current.properties[i].Data = Encoding.ASCII.GetBytes(str);
							}
							break;
						case 'R':
							current.properties[i].Data = _file.ReadBytes(_file.ReadInt32());
							break;
						default:
							return false;
					}
				}
			}

			long endoffset = (long)current.EndOffset;
			if ((endoffset - _file.BaseStream.Position) > 0)
			{
				current.NestedNodes.Add(new Node() { parent = current });
				current = current.NestedNodes[current.NestedNodes.Count - 1];
			}
			else
			{
				while (current.parent != null)
				{
					current = current.parent;
					endoffset = (long)current.EndOffset;
					if ((endoffset - _file.BaseStream.Position) > 0)
					{
						current.NestedNodes.Add(new Node() { parent = current });
						current = current.NestedNodes[current.NestedNodes.Count - 1];
						break;
					}
				}
			}
			while (true)
			{
				current.EndOffset = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
				if (current.EndOffset == 0)
				{
					current.isNull = true;
					//while (current.parent != null)
					//{
					current = current.parent;

					long endOffset = (long)current.EndOffset;
					if (endOffset == 0)
					{
						if (current.parent == null)
						{
							break;
						}
					}
					_file.BaseStream.Position = endOffset;
					current = current.parent;
					if (current != null)
					{
						current.NestedNodes.Add(new Node() { parent = current });
						current = current.NestedNodes[current.NestedNodes.Count - 1];
					}
					//}
					continue;
				}
				current.NumProperties = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
				current.PropetiesListLength = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
				current.Name = Encoding.ASCII.GetString(_file.ReadBytes(_file.ReadByte()));
				if (current.PropetiesListLength > 0)
				{
					current.properties = new Properties[current.NumProperties];
					for (ulong i = 0; i < current.NumProperties; i++)
					{
						current.properties[i] = new Properties { TypeCode = (char)_file.ReadByte() };
						switch (current.properties[i].TypeCode)
						{
							case 'Y':
								current.properties[i].Data = BitConverter.GetBytes(_file.ReadUInt16());
								break;
							case 'C':
								current.properties[i].Data = BitConverter.GetBytes(_file.ReadByte());
								break;
							case 'I':
								current.properties[i].Data = BitConverter.GetBytes(_file.ReadInt32());
								break;
							case 'F':
								current.properties[i].Data = BitConverter.GetBytes(_file.ReadSingle());
								break;
							case 'D':
								current.properties[i].Data = BitConverter.GetBytes(_file.ReadDouble());
								break;
							case 'L':
								current.properties[i].Data = BitConverter.GetBytes(_file.ReadInt64());
								break;
							case 'f':
								{
									int length = current.properties[i].Length = _file.ReadInt32();
									int encoding = _file.ReadInt32();
									int compressedLength = _file.ReadInt32();
									float[] _data = new float[length];
									long endPos = _file.BaseStream.Position + compressedLength;
									BinaryReader br = _file;
									if (encoding != 0)
									{
										//if(encoding != 1)
										//{
										//	return false;
										//}
										_file.BaseStream.Position += 2;
										DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
										br = new BinaryReader(decode);
									}
									for (int j = 0; j < length; j++)
									{
										_data[j] = br.ReadSingle();
									}
									if (encoding != 0)
									{
										_file.BaseStream.Position = endPos;
									}
									current.properties[i].Data = new byte[_data.Length * sizeof(float)];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'd':
								{
									int length = current.properties[i].Length = _file.ReadInt32();
									int encoding = _file.ReadInt32();
									int compressedLength = _file.ReadInt32();
									double[] _data = new double[length];
									long endPos = _file.BaseStream.Position + compressedLength;
									BinaryReader br = _file;
									if (encoding != 0)
									{
										//if(encoding != 1)
										//{
										//	return false;
										//}
										_file.BaseStream.Position += 2;
										DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
										br = new BinaryReader(decode);
									}
									for (int j = 0; j < length; j++)
									{
										_data[j] = br.ReadDouble();
									}
									if (encoding != 0)
									{
										_file.BaseStream.Position = endPos;
									}
									current.properties[i].Data = new byte[_data.Length * sizeof(double)];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'l':
								{
									int length = current.properties[i].Length = _file.ReadInt32();
									int encoding = _file.ReadInt32();
									int compressedLength = _file.ReadInt32();
									long[] _data = new long[length];
									long endPos = _file.BaseStream.Position + compressedLength;
									BinaryReader br = _file;
									if (encoding != 0)
									{
										//if(encoding != 1)
										//{
										//	return false;
										//}
										_file.BaseStream.Position += 2;
										DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
										br = new BinaryReader(decode);
									}
									for (int j = 0; j < length; j++)
									{
										_data[j] = br.ReadInt64();
									}
									if (encoding != 0)
									{
										_file.BaseStream.Position = endPos;
									}
									current.properties[i].Data = new byte[_data.Length * sizeof(long)];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'i':
								{
									int length = current.properties[i].Length = _file.ReadInt32();
									int encoding = _file.ReadInt32();
									int compressedLength = _file.ReadInt32();
									int[] _data = new int[length];
									long endPos = _file.BaseStream.Position + compressedLength;
									BinaryReader br = _file;
									if (encoding != 0)
									{
										//if(encoding != 1)
										//{
										//	return false;
										//}
										_file.BaseStream.Position += 2;
										DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
										br = new BinaryReader(decode);
									}
									for (int j = 0; j < length; j++)
									{
										_data[j] = br.ReadInt32();
									}
									if (encoding != 0)
									{
										_file.BaseStream.Position = endPos;
									}
									current.properties[i].Data = new byte[_data.Length * sizeof(int)];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'b':
								{
									int length = current.properties[i].Length = _file.ReadInt32();
									int encoding = _file.ReadInt32();
									int compressedLength = _file.ReadInt32();
									bool[] _data = new bool[length];
									long endPos = _file.BaseStream.Position + compressedLength;
									BinaryReader br = _file;
									if (encoding != 0)
									{
										//if(encoding != 1)
										//{
										//	return false;
										//}
										_file.BaseStream.Position += 2;
										DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
										br = new BinaryReader(decode);
									}
									for (int j = 0; j < length; j++)
									{
										_data[j] = br.ReadBoolean();
									}
									if (encoding != 0)
									{
										_file.BaseStream.Position = endPos;
									}
									current.properties[i].Data = new byte[_data.Length * sizeof(bool)];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'S':
								{
									string str = Encoding.ASCII.GetString(_file.ReadBytes(_file.ReadInt32()));
									if (str.Contains("\0\x1"))
									{
										string[] t = str.Split(new[] { "\0\x1" }, StringSplitOptions.None);
										string corrected_str = string.Empty;
										for (int j = t.Length - 1; j >= 0; j--)
										{
											if (!(j == (t.Length - 1)))
											{
												corrected_str += "::";
											}
											corrected_str += t[j];
										}
										str = corrected_str;
									}
									current.properties[i].Data = Encoding.ASCII.GetBytes(str);
								}
								break;
							case 'R':
								current.properties[i].Data = _file.ReadBytes(_file.ReadInt32());
								break;
							default:
								return false;
						}
					}
				}

				endoffset = (long)current.EndOffset;
				if ((endoffset - _file.BaseStream.Position) > 0)
				{
					current.NestedNodes.Add(new Node() { parent = current });
					current = current.NestedNodes[current.NestedNodes.Count - 1];
				}
				else
				{
					while (current.parent != null)
					{
						current = current.parent;
						endoffset = (long)current.EndOffset;
						if (((endoffset - _file.BaseStream.Position) > 0) || (current.parent == null))
						{
							current.NestedNodes.Add(new Node() { parent = current });
							current = current.NestedNodes[current.NestedNodes.Count - 1];
							break;
						}
					}
				}

			}
			//_fbx.node.NestedNodes.Add(LoadNode(_fbx, _fbx.node));

		}
		else
		{
			return false;
		}
		Console.ReadLine();
		return true;
	}

	public static bool FBXToMesh(FBX _fbx, out Mesh _mesh)
	{
		_mesh = new Mesh();
		List<Vector3> vertecies = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<Vector3> normals = new List<Vector3>();
		int[] vertexIndices = new int[0];
		int[] uvIndices = new int[0];
		//int[] normalIndices;
		List<int> tris = new List<int>();
		List<int> submeshStarts = new List<int>();
		List<Vector3> vertexTranslation = new List<Vector3>();

		List<Node> MeshNode = _fbx.node.NestedNodes.Find(n => n.Name == "Objects").NestedNodes.FindAll(n => n.Name == "Geometry");
		List<Node> ModelNode = _fbx.node.NestedNodes.Find(n => n.Name == "Objects").NestedNodes.FindAll(n => n.Name == "Model");
		if(MeshNode == null)
		{
			return false;
		}
		List<MeshTopology> topology = new List<MeshTopology>();

		for (int k = 0; k < MeshNode.Count; k++) {
			submeshStarts.Add(vertexIndices.Length);
			if (Encoding.ASCII.GetString(ModelNode[k].NestedNodes[1].NestedNodes[4].properties[0].Data).Contains("Translation")) {
				vertexTranslation.Add(new Vector3((float)BitConverter.ToDouble(ModelNode[k].NestedNodes[1].NestedNodes[4].properties[4].Data, 0), (float)BitConverter.ToDouble(ModelNode[k].NestedNodes[1].NestedNodes[4].properties[5].Data, 0), (float)BitConverter.ToDouble(ModelNode[k].NestedNodes[1].NestedNodes[4].properties[6].Data, 0)));
			}
			else
			{
				vertexTranslation.Add(Vector3.one);
			}
			Node VertexIndeciesNode = MeshNode[k].NestedNodes.Find(n => n.Name == "PolygonVertexIndex");
			if (VertexIndeciesNode == null)
			{
				return false;
			}
			vertexIndices = vertexIndices.Concat(new int[(VertexIndeciesNode.properties[0].Length)]).ToArray();
			Buffer.BlockCopy(VertexIndeciesNode.properties[0].Data, 0, vertexIndices, (submeshStarts[k] * sizeof(int)), VertexIndeciesNode.properties[0].Data.Length);
			for (int l = submeshStarts[k]; vertexIndices.Length > l; l++)
			{
				if (vertexIndices[l] < 0)
				{
					vertexIndices[l] = -(vertexIndices[l] + 1);
					if (l == (3 + submeshStarts[k]))
					{
						topology.Add(MeshTopology.Quads);
					}
					else if (l == (2 + submeshStarts[k]))
					{
						topology.Add(MeshTopology.Triangles);
					}
				}
				vertexIndices[l] += vertecies.Count;
			}

			Node VerteciesNode = MeshNode[k].NestedNodes.Find(n => n.Name == "Vertices");
			if (VerteciesNode == null)
			{
				return false;
			}
			double[] verts = new double[(VerteciesNode.properties[0].Length)];
			Buffer.BlockCopy(VerteciesNode.properties[0].Data, 0, verts, 0, VerteciesNode.properties[0].Data.Length);
			for (int l = 0; l < verts.Length; l += 3)
			{
				vertecies.Add(new Vector3((float)verts[l]+vertexTranslation[k].x, (float)verts[l + 1] + vertexTranslation[k].y, (float)verts[l + 2] + vertexTranslation[k].z));
			}

			Node NormalNode = MeshNode[k].NestedNodes.Find(n => n.Name == "LayerElementNormal").NestedNodes.Find(n => n.Name == "Normals");
			if (NormalNode == null)
			{
				return false;
			}
			double[] norms = new double[(NormalNode.properties[0].Length)];
			Buffer.BlockCopy(NormalNode.properties[0].Data, 0, norms, 0, NormalNode.properties[0].Data.Length);
			Node NormalWNode = MeshNode[k].NestedNodes.Find(n => n.Name == "LayerElementNormal").NestedNodes.Find(n => n.Name == "NormalsW");

			double[] normsW = new double[0];
			if (NormalWNode != null)
			{
				normsW = new double[(NormalWNode.properties[0].Length)];
				Buffer.BlockCopy(NormalWNode.properties[0].Data, 0, normsW, 0, NormalWNode.properties[0].Data.Length);
			}

			for (int i = 0, j = 0; i < norms.Length; i += 3, j++)
			{
				normals.Add(new Vector3((float)(norms[i] / ((NormalWNode == null) ? 1.0D : normsW[j])), (float)(norms[i + 1] / ((NormalWNode == null) ? 1.0D : normsW[j])), (float)(norms[i + 2] / ((NormalWNode == null) ? 1.0D : normsW[j]))));
			}

			Node UVIndeciesNode = MeshNode[k].NestedNodes.Find(n => n.Name == "LayerElementUV").NestedNodes.Find(n => n.Name == "UVIndex");
			uvIndices = uvIndices.Concat(new int[(UVIndeciesNode.properties[0].Length)]).ToArray();
			Buffer.BlockCopy(UVIndeciesNode.properties[0].Data, 0, uvIndices, (submeshStarts[k] * sizeof(int)), UVIndeciesNode.properties[0].Data.Length);
			if (submeshStarts[k] > 0)
			{
				for (int l = submeshStarts[k]; l < uvIndices.Length; l++)
				{
					uvIndices[l] += uvs.Count;
				}
			}

			Node UVNode = MeshNode[k].NestedNodes.Find(n => n.Name == "LayerElementUV").NestedNodes.Find(n => n.Name == "UV");
			if (UVNode == null)
			{
				return false;
			}
			double[] texcords = new double[(UVNode.properties[0].Length)];
			Buffer.BlockCopy(UVNode.properties[0].Data, 0, texcords, 0, UVNode.properties[0].Data.Length);
			for (int i = 0; i < texcords.Length; i += 2)
			{
				uvs.Add(new Vector2((float)texcords[i], (float)texcords[i + 1]));
			}
		}
		List<Vector3> out_verts = new List<Vector3>();
		List<Vector3> out_normals = new List<Vector3>();
		List<Vector2> out_uvs = new List<Vector2>();
		List<int> indecies = new List<int>();
		for (int i = 0; i < vertexIndices.Length; i++)
		{
			out_verts.Add(vertecies[vertexIndices[i]]);
			//out_normals.Add(normals[vertexIndices[i]]);
			out_uvs.Add(uvs[uvIndices[i]]);
			indecies.Add(i);
		}
		_mesh.subMeshCount = submeshStarts.Count;
		_mesh.vertices = out_verts.ToArray();
		_mesh.normals = normals.ToArray();
		_mesh.uv = out_uvs.ToArray();
		for (int j = 0; j < submeshStarts.Count; j++) {
			_mesh.SetIndices(indecies.GetRange(submeshStarts[j], (((j+1) != submeshStarts.Count) ? submeshStarts[j + 1] : (indecies.Count - submeshStarts[j]))).ToArray(), topology[j], j);
			int[] check = _mesh.GetIndices(j);
		}
		_mesh.RecalculateBounds();
		return true;
	}

	public static bool LoadMaterials(FBX _fbx, out Material[] _materials)
	{
		List<Node> ModelNode = _fbx.node.NestedNodes.Find(n => n.Name == "Objects").NestedNodes.FindAll(n => n.Name == "Model");
		List<Node> MaterialNode = _fbx.node.NestedNodes.Find(n => n.Name == "Objects").NestedNodes.FindAll(n => n.Name == "Material");
		List<Node> ImageNode = _fbx.node.NestedNodes.Find(n => n.Name == "Objects").NestedNodes.FindAll(n => n.Name == "Video");
		List<Node> TextureNode = _fbx.node.NestedNodes.Find(n => n.Name == "Objects").NestedNodes.FindAll(n => n.Name == "Texture");
		Node ConnectionsNode = _fbx.node.NestedNodes.Find(n => n.Name == "Connections");
		_materials = new Material[ModelNode.Count];
		Dictionary<long, List<Texture2D>> TextureImages = new Dictionary<long, List<Texture2D>>();

		if (ImageNode != null && ImageNode.Count > 0) 
		{
			List<Node> TextureRefrenceNode = ConnectionsNode.NestedNodes.FindAll(n => (n.properties != null && (BitConverter.ToInt64(n.properties[1].Data, 0) == (BitConverter.ToInt64(ImageNode[0].properties[0].Data, 0)))));
			for (int i = 0; i < TextureRefrenceNode.Count; i++) {
				Node contentNode = ImageNode.Find(n => (n.properties != null && (BitConverter.ToInt64(n.properties[0].Data, 0)) == (BitConverter.ToInt64(TextureRefrenceNode[i].properties[1].Data, 0)))).NestedNodes.Find(n => n.Name == "Content");
				if (contentNode != null)
				{
					long texID = BitConverter.ToInt64(TextureRefrenceNode[i].properties[2].Data, 0);
					TextureImages.Add(texID, new List<Texture2D>());
					TextureImages[texID].Add(new Texture2D(2, 2));
					TextureImages[texID].Last().LoadImage(contentNode.properties[0].Data);
				}
			}
		}

		for (int i = 0; i < MaterialNode.Count; i++)
		{
			Material _newMat = new Material(Shader.Find("Standard (Specular setup)"));
			List<Node> MaterialRefrenceNode = ConnectionsNode.NestedNodes.FindAll(n => (n.properties != null && (BitConverter.ToInt64(n.properties[1].Data, 0) == (BitConverter.ToInt64(MaterialNode[i].properties[0].Data, 0)))));
			if (MaterialRefrenceNode == null || MaterialRefrenceNode.Count == 0)
			{
				continue;
			}
			string shadingMethod = Encoding.ASCII.GetString(MaterialNode[i].NestedNodes.Find(n => n.Name == "ShadingModel").properties[0].Data);

			if (shadingMethod != "lambert" && shadingMethod != "phong")
			{
				return false;
			}
			else if (shadingMethod == "phong")
			{
				Node specularColorNode = MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "SpecularColor");
				_newMat.SetColor("_SpecColor", new Color ( (float)BitConverter.ToDouble(specularColorNode.properties[4].Data, 0), (float)BitConverter.ToDouble(specularColorNode.properties[5].Data, 0), (float)BitConverter.ToDouble(specularColorNode.properties[6].Data, 0) ));
				_newMat.SetFloat("_GlossyReflections", (float)BitConverter.ToDouble(MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "ReflectionFactor").properties[4].Data, 0));
			}

			Node ambientColorNode = MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "AmbientColor");
			Node diffuseColorNode = MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "DiffuseColor");
			_newMat.SetColor("_EmissionColor", new Color ( (float)BitConverter.ToDouble(ambientColorNode.properties[4].Data, 0), (float)BitConverter.ToDouble(ambientColorNode.properties[5].Data, 0), (float)BitConverter.ToDouble(ambientColorNode.properties[6].Data, 0) ));
			float transparencyFactor = (float)BitConverter.ToDouble(MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "TransparencyFactor").properties[4].Data, 0);
			float diffuseFactor = (float)BitConverter.ToDouble(MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "DiffuseFactor").properties[4].Data, 0);
			_newMat.SetColor("_Color", new Color ( (float)BitConverter.ToDouble(diffuseColorNode.properties[4].Data, 0)* diffuseFactor, (float)BitConverter.ToDouble(diffuseColorNode.properties[5].Data, 0)*diffuseFactor, (float)BitConverter.ToDouble(diffuseColorNode.properties[6].Data, 0)* diffuseFactor, transparencyFactor));
			float opacity = (float)BitConverter.ToDouble(MaterialNode[i].NestedNodes[3].NestedNodes.Find(n => Encoding.ASCII.GetString(n.properties[0].Data) == "Opacity").properties[4].Data, 0);

			for (int j = 0; j < ModelNode.Count; j++)
			{
				for (int k = 0; k < MaterialRefrenceNode.Count; k++)
				{
					if (ModelNode[j].properties != null && MaterialRefrenceNode[k].properties != null && BitConverter.ToInt64(ModelNode[j].properties[0].Data, 0) == BitConverter.ToInt64(MaterialRefrenceNode[k].properties[2].Data, 0))
					{
						_materials[j] = _newMat;
						Console.WriteLine(Encoding.ASCII.GetString(ModelNode[j].properties[1].Data) + " LINKED TO " + Encoding.ASCII.GetString(MaterialNode[i].properties[1].Data));
					}
				}
			}

		}
		return true;
	}
}
