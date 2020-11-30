using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
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
	public struct Properties
	{
		public char TypeCode;
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
					current.properties[i].TypeCode = (char)_file.ReadByte();
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
								int length = _file.ReadInt32();
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
								current.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'd':
							{
								int length = _file.ReadInt32();
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
								current.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'l':
							{
								int length = _file.ReadInt32();
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
								current.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'i':
							{
								int length = _file.ReadInt32();
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
								current.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
							}
							break;
						case 'b':
							{
								int length = _file.ReadInt32();
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
								current.properties[i].Data = new byte[_data.Length * 4];
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
						current.properties[i].TypeCode = (char)_file.ReadByte();
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
									int length = _file.ReadInt32();
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
									current.properties[i].Data = new byte[_data.Length * 4];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'd':
								{
									int length = _file.ReadInt32();
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
									current.properties[i].Data = new byte[_data.Length * 4];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'l':
								{
									int length = _file.ReadInt32();
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
									current.properties[i].Data = new byte[_data.Length * 4];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'i':
								{
									int length = _file.ReadInt32();
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
									current.properties[i].Data = new byte[_data.Length * 4];
									Buffer.BlockCopy(_data, 0, current.properties[i].Data, 0, current.properties[i].Data.Length);
								}
								break;
							case 'b':
								{
									int length = _file.ReadInt32();
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
									current.properties[i].Data = new byte[_data.Length * 4];
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
		List<int> tris = new List<int>();

		Node objectNode;

		for(int i = 0; i < _fbx.node.NestedNodes.Count; i++)
		{
			if(_fbx.node.NestedNodes[i].Name == "Objects")
			{
				for(int j = 0; j < _fbx.node.NestedNodes[i].NestedNodes.Count; j++)
				{
					if(_fbx.node.NestedNodes[i].NestedNodes[j].Name == "Geometry")
					{
						for(int k = 0; k < _fbx.node.NestedNodes[i].NestedNodes[j].NestedNodes.Count; k++)
						{
							if(_fbx.node.NestedNodes[i].NestedNodes[j].NestedNodes[k].Name == "Vertices")
							{
								int tester = 0;
							}
						}

					}
				}
			}
		}

		vertecies.Add(new Vector3());

		return true;
	}
}
