﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif
using UnityEngine;
using UnityEngine.UI;

public class TrayGalary : MonoBehaviour
{
	[SerializeField]
	RectTransform[] m_scrollContent;
	[SerializeField]
	DiceAxisMovement m_mover;
	[SerializeField]
	MeshFilter m_currentModel;
	[SerializeField]
	MeshRenderer m_materials;
	[SerializeField]
	SceneChanger m_manager;
	[SerializeField]
	Mesh[] m_trayModels;
	[SerializeField]
	GameObject m_notificationPanel;
	[SerializeField]
	GameObject m_exportWindow;
	//[SerializeField]
	//Texture2D[] textures;

	string[] files;
	int currentIndex = 0;
	int currentStartIndex = 0;

	// Start is called before the first frame update
	void Start()
	{
		//Get skin files from directory and store them in a collection
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
		if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/"))
		{
			Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/");
		}
		if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/"))
		{
			Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/");
		}
		files = Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/").Concat(Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/")).ToArray();
#elif (UNITY_ANDROID && !UNITY_EDITOR)
		if (!Directory.Exists(UnityEngine.Application.persistentDataPath+"/DungeoneersSamdbox/diceTrays/skins/"))
		{
			Directory.CreateDirectory(UnityEngine.Application.persistentDataPath+"/DungeoneersSamdbox/diceTrays/skins/");
		}
		if (!Directory.Exists(UnityEngine.Application.persistentDataPath+"/DungeoneersSamdbox/diceTrays/skins/purchased/"))
		{
			Directory.CreateDirectory(UnityEngine.Application.persistentDataPath+"/DungeoneersSamdbox/diceTrays/skins/purchased/");
		}
		files = Directory.GetFiles(UnityEngine.Application.persistentDataPath+"/DungeoneersSamdbox/diceTrays/skins/").Concat(Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/")).ToArray();
#endif
		//Read in file properties into list of trays to display
		int filedLines = 0;
		for (int j = 0; j < files.Length; j++)
		{
			BinaryReader file = new BinaryReader(File.Open(files[j], FileMode.Open));

			int currentModel = file.ReadInt32();
			int matType = file.ReadInt32();
			m_currentModel.mesh = m_trayModels[currentModel];

			if ((matType != 3) && (matType != 7) && (matType < 11) && (matType >= 0))
			{
				if ((matType & 4) == 4)
				{
					int arraySize = file.ReadInt32();
					Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
					newTex.LoadImage(file.ReadBytes(arraySize));
					m_materials.materials[1].color = Color.white;
					m_materials.materials[1].SetTexture("_MainTex", newTex);
				}
				else if ((matType & 8) == 8)
				{
					//TODO: Add in file reading when shader is written
					int arraySize = file.ReadInt32();
					Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
					newTex.LoadImage(file.ReadBytes(arraySize));
					Vector2 Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
					Vector2 Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
					Color bg_color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
					for (int y = 0; y < newTex.height; y++)
					{
						for (int x = 0; x < newTex.width; x++)
						{
							if (newTex.GetPixel(x, y).a == 0.0f)
							{
								newTex.SetPixel(x, y, bg_color);
							}
						}
					}
					newTex.Apply();
					m_materials.materials[1].color = Color.white;
					m_materials.materials[1].SetTexture("_MainTex", newTex);
					m_materials.materials[1].SetTextureScale("_MainTex", Tiling);
					m_materials.materials[1].SetTextureOffset("_MainTex", Offset);
				}
				else
				{
					m_materials.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
				}

				if ((matType & 1) == 1)
				{
					int arraySize = file.ReadInt32();
					Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
					newTex.LoadImage(file.ReadBytes(arraySize));
					m_materials.materials[0].color = Color.white;
					m_materials.materials[0].SetTexture("_MainTex", newTex);
				}
				else if ((matType & 2) == 2)
				{
					//TODO: Add in file reading when shader is written

					int arraySize = file.ReadInt32();
					Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
					newTex.LoadImage(file.ReadBytes(arraySize));
					Vector2 Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
					Vector2 Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
					Color bg_color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
					for (int y = 0; y < newTex.height; y++)
					{
						for (int x = 0; x < newTex.width; x++)
						{
							if (newTex.GetPixel(x, y).a == 0.0f)
							{
								newTex.SetPixel(x, y, bg_color);
							}
						}
					}
					newTex.Apply();
					m_materials.materials[0].color = Color.white;
					m_materials.materials[0].SetTexture("_MainTex", newTex);
					m_materials.materials[0].SetTextureScale("_MainTex", Tiling);
					m_materials.materials[0].SetTextureOffset("_MainTex", Offset);
				}
				else
				{
					m_materials.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
				}
			}
			file.Close();
			
			if (filedLines < m_scrollContent.Length)
			{
				int t = j;
				m_scrollContent[filedLines].GetChild(0).GetComponent<Text>().text = files[j].Split('/')[files[j].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
				m_scrollContent[filedLines].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
				filedLines++;
			}
		}
		//Fill in visible lines for the list of trays
		if (filedLines < m_scrollContent.Length)
		{
			for (int j = 0; j < (m_scrollContent.Length - (files.Length % m_scrollContent.Length)); j++)
			{
				m_scrollContent[2 - j].GetChild(0).GetComponent<Text>().text = "-";
				m_scrollContent[2 - j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			}
		}
		//Set model to first tray file in list or if no files found hide model
		if (files.Length > 0)
		{
			SetSkin(0);
		}
		else
		{
			m_currentModel.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		//Check if mouse is over tray model ant toggle mobility depending on the state
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (!m_mover.GetMobility() && Physics.Raycast(ray, out hit))
		{
			m_mover.SetMobility(true);
		}
		else if (!Input.GetMouseButton(0) && m_mover.GetMobility())
		{
			m_mover.SetMobility(false);
		}
	}

	/// <summary>
	/// Sets the skin and model of the display die
	/// </summary>
	/// <param name="_index">Index of which skin and model to switch to within the saved skins</param>
	public void SetSkin(int _index)
	{
		currentIndex = _index;
		BinaryReader file = new BinaryReader(File.Open(files[_index], FileMode.Open));

		int currentModel = file.ReadInt32();
		int matType = file.ReadInt32();
		m_currentModel.mesh = m_trayModels[currentModel];

		if ((matType != 3) && (matType != 7) && (matType < 11) && (matType >= 0))
		{
			if ((matType & 4) == 4)
			{
				int arraySize = file.ReadInt32();
				Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
				newTex.LoadImage(file.ReadBytes(arraySize));
				m_materials.materials[1].color = Color.white;
				m_materials.materials[1].SetTexture("_MainTex", newTex);
			}
			else if ((matType & 8) == 8)
			{
				//TODO: Add in file reading when shader is written
				int arraySize = file.ReadInt32();
				Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
				newTex.LoadImage(file.ReadBytes(arraySize));
				Vector2 Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
				Vector2 Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
				Color bg_color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
				for (int y = 0; y < newTex.height; y++)
				{
					for (int x = 0; x < newTex.width; x++)
					{
						if (newTex.GetPixel(x, y).a == 0.0f)
						{
							newTex.SetPixel(x, y, bg_color);
						}
					}
				}
				newTex.Apply();
				m_materials.materials[1].color = Color.white;
				m_materials.materials[1].SetTexture("_MainTex", newTex);
				m_materials.materials[1].SetTextureScale("_MainTex", Tiling);
				m_materials.materials[1].SetTextureOffset("_MainTex", Offset);
			}
			else
			{
				m_materials.materials[1].SetTexture("_MainTex", null);
				m_materials.materials[1].SetTextureScale("_MainTex", Vector2.one);
				m_materials.materials[1].SetTextureOffset("_MainTex", Vector2.one);
				m_materials.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
			}

			if ((matType & 1) == 1)
			{
				int arraySize = file.ReadInt32();
				Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
				newTex.LoadImage(file.ReadBytes(arraySize));
				m_materials.materials[0].color = Color.white;
				m_materials.materials[0].SetTexture("_MainTex", newTex);
			}
			else if ((matType & 2) == 2)
			{
				//TODO: Add in file reading when shader is written

				int arraySize = file.ReadInt32();
				Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
				newTex.LoadImage(file.ReadBytes(arraySize));
				Vector2 Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
				Vector2 Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
				Color bg_color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
				for (int y = 0; y < newTex.height; y++)
				{
					for (int x = 0; x < newTex.width; x++)
					{
						if (newTex.GetPixel(x, y).a == 0.0f)
						{
							newTex.SetPixel(x, y, bg_color);
						}
					}
				}
				newTex.Apply();
				m_materials.materials[0].color = Color.white;
				m_materials.materials[0].SetTexture("_MainTex", newTex);
				m_materials.materials[0].SetTextureScale("_MainTex", Tiling);
				m_materials.materials[0].SetTextureOffset("_MainTex", Offset);
			}
			else
			{
				m_materials.materials[0].SetTexture("_MainTex", null);
				m_materials.materials[0].SetTextureScale("_MainTex", Vector2.one);
				m_materials.materials[0].SetTextureOffset("_MainTex", Vector2.one);
				m_materials.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
			}
		}
		file.Close();
	}

	/// <summary>
	/// Goes to the next skin and model from the current skin and model
	/// </summary>
	public void NextModel()
	{
		currentStartIndex = (currentStartIndex + m_scrollContent.Length < files.Length) ? currentStartIndex + m_scrollContent.Length : 0;
		for (int i = currentStartIndex; i < ((currentStartIndex + m_scrollContent.Length < files.Length) ? currentStartIndex + m_scrollContent.Length : files.Length); i++)
		{
			int t = i;
			m_scrollContent[i % m_scrollContent.Length].GetChild(0).GetComponent<Text>().text = files[i].Split('/')[files[i].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
			m_scrollContent[i % m_scrollContent.Length].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			m_scrollContent[i % m_scrollContent.Length].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
		}
		if (currentStartIndex + m_scrollContent.Length > files.Length)
		{
			for (int j = 0; j < (m_scrollContent.Length - (files.Length % m_scrollContent.Length)); j++)
			{
				m_scrollContent[2 - j].GetChild(0).GetComponent<Text>().text = "-";
				m_scrollContent[2 - j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			}
		}
	}

	/// <summary>
	/// Goes to the previous skin and model from the current skin and model
	/// </summary>
	public void PrevModel()
	{
		currentStartIndex = (currentStartIndex - m_scrollContent.Length >= 0) ? currentStartIndex - m_scrollContent.Length : files.Length - 1;
		for (int i = currentStartIndex; i < ((currentStartIndex + m_scrollContent.Length < files.Length) ? currentStartIndex + m_scrollContent.Length : files.Length); i++)
		{
			int t = i;
			m_scrollContent[i % m_scrollContent.Length].GetChild(0).GetComponent<Text>().text = files[i].Split('/')[files[i].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
			m_scrollContent[i % m_scrollContent.Length].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			m_scrollContent[i % m_scrollContent.Length].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
		}
		if (currentStartIndex + m_scrollContent.Length > files.Length)
		{
			for (int j = 0; j < (m_scrollContent.Length - (files.Length % m_scrollContent.Length)); j++)
			{
				m_scrollContent[2 - j].GetChild(0).GetComponent<Text>().text = "-";
				m_scrollContent[2 - j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			}
		}
	}

	/// <summary>
	/// Open the SaveFileDialog
	/// </summary>
	public void ExportModel()
	{
		if (!files[currentIndex].Contains("purchased")) {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
			SaveFileDialog diceTraySaveDialog = new SaveFileDialog();
			diceTraySaveDialog.InitialDirectory = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/diceTray/";
			diceTraySaveDialog.Filter = "DS Dice Tray files (*.dst)|*.dst|All files (*.*)|*.*";
			diceTraySaveDialog.FilterIndex = 0;
			diceTraySaveDialog.RestoreDirectory = true;

			if (diceTraySaveDialog.ShowDialog() == DialogResult.OK)
			{
				File.Copy(files[currentIndex], diceTraySaveDialog.FileName);
			}
#elif (UNITY_ANDROID && !UNITY_EDITOR)
			m_exportWindow.SetActive(true);
#endif
		}
		else
		{
			StartCoroutine(ToggleActivation(m_notificationPanel, 1.0f));
		}
	}

	/// <summary>
	/// Saves the currently selected dice tray .dst(DS Dice Tray file)
	/// </summary>
	/// <param name="_filePath"></param>
	public void ExportModel(string _filePath)
	{
		File.Copy(files[currentIndex], _filePath);
	}

	/// <summary>
	/// Flips the active state of a given gameObject for a given time
	/// </summary>
	/// <param name="_gameObject">Game object to be toggled</param>
	/// <param name="_timeDelay">Time the game object will be toggled for</param>
	/// <returns></returns>
	IEnumerator ToggleActivation(GameObject _gameObject, float _timeDelay)
	{
		_gameObject.SetActive(!_gameObject.activeSelf);
		yield return new WaitForSeconds(_timeDelay);
		_gameObject.SetActive(!_gameObject.activeSelf);
	}
}
