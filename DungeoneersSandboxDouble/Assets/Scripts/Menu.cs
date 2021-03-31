using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMenu", menuName = "DungoneersSandbox/Sub-Context Menu", order = 4)]
public class Menu : ScriptableObject
{
	[Serializable]
	public struct MenuItem
	{
		public string text;
		public UnityEngine.Object item;
	}

	[SerializeField]
	MenuItem[] itmes;
}
