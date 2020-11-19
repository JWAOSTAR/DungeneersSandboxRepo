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
	public struct Node
	{
		public ulong EndOffset;
		public ulong NumProperties;
		public ulong PropetiesListLength;
		public string Name;
		public Properties[] properties;
		public Node[] NestedNodes;
		//public byte[] nullRecords;
	}

    public struct FBX
	{
		public byte[] header;
		public Node node;
	}

	public static bool LoadFBX(string _path, out FBX _fbx)
	{
		_fbx = new FBX();
		_fbx.node = new Node();

		if(File.Exists(_path))
		{

			BinaryReader _file = new BinaryReader(File.Open(_path, FileMode.Open));
			_fbx.header = _file.ReadBytes(Encoding.ASCII.GetBytes("Kaydara FBX Binary  \0\x1a\0").Length);
			uint version = _file.ReadUInt32();
			_fbx.node.EndOffset = ((FBX_Versions)version < FBX_Versions.v7_5) ?_file.ReadUInt32() : _file.ReadUInt64();
			_fbx.node.NumProperties = ((FBX_Versions)version < FBX_Versions.v7_5) ? _file.ReadUInt32() : _file.ReadUInt64();
			_fbx.node.PropetiesListLength = ((FBX_Versions)version < FBX_Versions.v7_5) ?_file.ReadUInt32() : _file.ReadUInt64();
			//int nameLength = _file.ReadByte();
			_fbx.node.Name = Encoding.ASCII.GetString(_file.ReadBytes(_file.ReadByte()));
			
			if(_fbx.node.PropetiesListLength > 0)
			{
				_fbx.node.properties = new Properties[_fbx.node.NumProperties];
				for(ulong i = 0; i < _fbx.node.NumProperties; i++)
				{
					_fbx.node.properties[i].TypeCode = (char)_file.ReadByte();
					switch(_fbx.node.properties[i].TypeCode)
					{
						case 'Y':
							_fbx.node.properties[i].Data = BitConverter.GetBytes(_file.ReadUInt16());
							break;
						case 'C':
							_fbx.node.properties[i].Data = BitConverter.GetBytes(_file.ReadByte());
							break;
						case 'I':
							_fbx.node.properties[i].Data = BitConverter.GetBytes(_file.ReadInt32());
							break;
						case 'F':
							_fbx.node.properties[i].Data = BitConverter.GetBytes(_file.ReadSingle());
							break;
						case 'D':
							_fbx.node.properties[i].Data = BitConverter.GetBytes(_file.ReadDouble());
							break;
						case 'L':
							_fbx.node.properties[i].Data = BitConverter.GetBytes(_file.ReadInt64());
							break;
						case 'f':
							{
								int length = _file.ReadInt32();
								int encoding = _file.ReadInt32();
								int compressedLength = _file.ReadInt32();
								float[] _data = new float[length];
								long endPos = _file.BaseStream.Position + compressedLength;
								BinaryReader br = _file;
								if(encoding != 0)
								{
									//if(encoding != 1)
									//{
									//	return false;
									//}
									_file.BaseStream.Position += 2;
									DeflateStream decode = new DeflateStream(_file.BaseStream, CompressionMode.Decompress);
									br = new BinaryReader(decode);
								}
								for(int j = 0; j < length; j++)
								{
									_data[j] = br.ReadSingle();
								}
								if (encoding != 0)
								{
									_file.BaseStream.Position = endPos;
								}
								_fbx.node.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, _fbx.node.properties[i].Data, 0, _fbx.node.properties[i].Data.Length);
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
								_fbx.node.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, _fbx.node.properties[i].Data, 0, _fbx.node.properties[i].Data.Length);
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
								_fbx.node.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, _fbx.node.properties[i].Data, 0, _fbx.node.properties[i].Data.Length);
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
								_fbx.node.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, _fbx.node.properties[i].Data, 0, _fbx.node.properties[i].Data.Length);
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
								_fbx.node.properties[i].Data = new byte[_data.Length * 4];
								Buffer.BlockCopy(_data, 0, _fbx.node.properties[i].Data, 0, _fbx.node.properties[i].Data.Length);
							}
							break;
						case 'S':
							{
								string str = Encoding.ASCII.GetString(_file.ReadBytes(_file.ReadInt32()));
								if(str.Contains("\0\x1"))
								{
									string[] t = str.Split(new [] { "\0\x1" }, StringSplitOptions.None);
									string corrected_str = string.Empty;
									for(int j = t.Length - 1; j >= 0; j--)
									{
										if(j == (t.Length - 1))
										{
											corrected_str += "::";
										}
										corrected_str += str[j];
									}
									str = corrected_str;
								}
								_fbx.node.properties[i].Data = Encoding.ASCII.GetBytes(str);
							}
							break;
						case 'R':
							_fbx.node.properties[i].Data = _file.ReadBytes(_file.ReadInt32());
							break;
						default:
							return false;
					}
				}
			}

			while(true)
			{

				break;
			}

		}
		else
		{
			return false;
		}
		return true;
	}
}
