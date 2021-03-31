using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProceduralObject", menuName = "DungoneersSandbox/Procedural Object", order = 3)]
public class ProceduralObject : ScriptableObject
{
	[SerializeField]
	bool singleObject = false;
	public GameObject[] objectOptions;
	[SerializeField]
	Material[] materialOptions;
	[Space]
	[SerializeField]
	[Range(1, 9)]
	int tileVolume = 1;
	[Space]
	[SerializeField]
	bool varientHeight = false;
	[SerializeField]
	float height = 1.0f;
	[Space]
	[SerializeField]
	bool singleColor = true;
	public Color[] tileColors = new Color[1];
	[Space]
	public Material tileMaterial;

	private void OnValidate()
	{
		if (singleColor)
		{
			Array.Resize(ref tileColors, 1);
		}
		else
		{
			Array.Resize(ref tileColors, 6);
		}
		if(height < 0.0f)
		{
			height = 1.0f;
		}
		if(singleObject)
		{
			Array.Resize(ref objectOptions, 1);
			varientHeight = false;
		}
	}
}
