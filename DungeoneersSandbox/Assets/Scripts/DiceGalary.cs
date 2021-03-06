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

public class DiceGalary : MonoBehaviour
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
    Mesh[] m_diceModels;
    [SerializeField]
    Texture2D[] textures;
    [SerializeField]
    GameObject m_notificationPanel;
    [SerializeField]
    GameObject m_saveFileWindow;

    List<Dice.DiceSkin> skins = new List<Dice.DiceSkin>();
    string[] files;
    int currentIndex = 0;
    int currentStartIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Get skin files from directory and store them in a collection
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
        }
        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/");
        }
        files = Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/").Concat(Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/")).ToArray();
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        if (!Directory.Exists(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/skins/"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/skins/");
        }
        if (!Directory.Exists(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/skins/purchased/"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/skins/purchased/");
        }
        files = Directory.GetFiles(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/skins/").Concat(Directory.GetFiles(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/skins/purchased/")).ToArray();
#endif
        //Read in file properties into list of trays to display
        int filedLines = 0;
        for (int j = 0; j < files.Length; j++)
        {
            BinaryReader file = new BinaryReader(File.Open(files[j], FileMode.Open));
            Dice.DiceSkin skinToAdd = new Dice.DiceSkin();
            skinToAdd.isTexture = file.ReadBoolean();
            skinToAdd.diceType = (Dice.DiceType)file.ReadInt32();
            if (!skinToAdd.isTexture)
            {
                skinToAdd.numbers = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                skinToAdd.die = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
            }
            else
            {
                int array_size = file.ReadInt32();
                Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                newTex.LoadImage(file.ReadBytes(array_size));
                skinToAdd.texture = newTex;
            }
            skins.Add(skinToAdd);
            file.Close();
            //if(j >= m_scrollContent.childCount)
            //{
            //    Instantiate(m_scrollContent.GetChild(m_scrollContent.childCount - 1), m_scrollContent).gameObject.name = "Line " + j;
            //    m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetChild(0).GetComponent<Text>().text = files[j].Split('/')[files[j].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
            //    m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().position = new Vector3(m_scrollContent.GetChild(0).GetComponent<RectTransform>().position.x, m_scrollContent.GetChild(0).GetComponent<RectTransform>().position.y - ((m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y+10)*j), m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().position.z);
            //    if (m_scrollContent.sizeDelta.y < ((m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y + 10) * (j + 1)))
            //    {
            //        m_scrollContent.sizeDelta = new Vector2(m_scrollContent.sizeDelta.x, m_scrollContent.sizeDelta.y + m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y + 10);
            //        UpdateLineSpacing();
            //    }
            //}
            //else
            //{
            //    m_scrollContent.GetChild(j).GetChild(0).GetComponent<Text>().text = files[j].Split('/')[files[j].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
            //}
            //int t = j;
            //m_scrollContent.GetChild(j).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
            if (filedLines < m_scrollContent.Length)
            {
                int t = j;
                m_scrollContent[filedLines].GetChild(0).GetComponent<Text>().text = files[j].Split('/')[files[j].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
                m_scrollContent[filedLines].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
                filedLines++;
            }
        }
        //Fill in visible lines for the list of dice
        if (filedLines < m_scrollContent.Length)
        {
            for (int j = 0; j < (m_scrollContent.Length - (files.Length % m_scrollContent.Length)); j++)
            {
                m_scrollContent[2 - j].GetChild(0).GetComponent<Text>().text = "-";
                m_scrollContent[2 - j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            }
        }
        //Set model to first dice file in list or if no files found hide model
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
        //Check if mouse is over die model ant toggle mobility depending on the state
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!m_mover.GetMobility() && Physics.Raycast(ray, out hit))
        {
            m_mover.SetMobility(true);
        }
        else if(!Input.GetMouseButton(0) && m_mover.GetMobility())
        {
            m_mover.SetMobility(false);
        }
    }

    /// <summary>
    /// Switch the die model to the next model/file in the list
    /// </summary>
    public void NextModel()
    {
        currentStartIndex = (currentStartIndex+ m_scrollContent.Length < files.Length) ? currentStartIndex+ m_scrollContent.Length : 0;
        for(int i = currentStartIndex; i < ((currentStartIndex + m_scrollContent.Length < files.Length)? currentStartIndex + m_scrollContent.Length : files.Length); i++)
        {
            int t = i;
            m_scrollContent[i% m_scrollContent.Length].GetChild(0).GetComponent<Text>().text = files[i].Split('/')[files[i].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
            m_scrollContent[i% m_scrollContent.Length].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            m_scrollContent[i% m_scrollContent.Length].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
        }
        if (currentStartIndex + m_scrollContent.Length > files.Length)
        {
            for(int j = 0; j < (m_scrollContent.Length - (files.Length % m_scrollContent.Length)); j++)
            {
                m_scrollContent[2-j].GetChild(0).GetComponent<Text>().text = "-";
                m_scrollContent[2-j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            }
        }
    }

    /// <summary>
    /// Switch the die model to the previous model/file in the list of saved skins
    /// </summary>
    public void PrevModel()
    {
        currentStartIndex = (currentStartIndex - m_scrollContent.Length >= 0) ? currentStartIndex - m_scrollContent.Length : files.Length -1;
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
    /// Set the skin of the model to the specified model/file in the list of saved skins
    /// </summary>
    /// <param name="_index">Index of the model/file in the list</param>
    public void SetSkin(int _index)
    {
        currentIndex = _index;
        m_currentModel.mesh = m_diceModels[(int)skins[_index].diceType];
        if (!skins[_index].isTexture)
        {
            m_materials.materials[0].SetTexture("_MainTex", null);
            m_materials.materials[1].SetTexture("_MainTex", null);
            m_materials.materials[0].color = skins[_index].numbers;
            m_materials.materials[1].color = skins[_index].die;
        }
        else
        {
            m_materials.materials[0].SetTexture("_MainTex", skins[_index].texture);
            m_materials.materials[1].SetTexture("_MainTex", skins[_index].texture);
            m_materials.materials[0].color = Color.white;
            m_materials.materials[1].color = Color.white;
        }
    }
    
    //[OBSOLETE]
    public void SaveSkin()
	{

	}

    /// <summary>
    /// Converts the material properties to a Texture2D object
    /// </summary>
    /// <returns>The texture converted from material properties</returns>
    Texture2D MaterialToTexture2D()
    {
        Texture2D newTexture = textures[(int)skins[currentIndex].diceType];

        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                if (newTexture.GetPixel(x, y) == Color.black)
                {
                    newTexture.SetPixel(x, y, (m_materials.materials[0].mainTexture == null) ? skins[currentIndex].numbers : ((Texture2D)m_materials.materials[0].mainTexture).GetPixel(x, y));
                }
                else if (newTexture.GetPixel(x, y) == Color.white)
                {
                    newTexture.SetPixel(x, y, (m_materials.materials[1].mainTexture == null) ? skins[currentIndex].die : ((Texture2D)m_materials.materials[1].mainTexture).GetPixel(x, y));
                }
            }
        }

        return newTexture;
    }

    /// <summary>
    /// Stores dice to be painted file in game directory and switches to dice editor scene
    /// </summary>
    public void EditSkin()
    {
        string path;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/";
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        path = UnityEngine.Application.persistentDataPath + "/" + "DungeoneersSamdbox/temp/";
#endif
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);

        }
        if (!skins[currentIndex].isTexture)
        {
            Texture2D _diceTex = MaterialToTexture2D();
            BinaryWriter _file = new BinaryWriter(File.Open(files[currentIndex], FileMode.Open));
            _file.Write(true);
            _file.Write((int)skins[currentIndex].diceType);
            _file.Write(_diceTex.EncodeToPNG().Length);
            _file.Write(_diceTex.width);
            _file.Write(_diceTex.height);
            _file.Write(_diceTex.EncodeToPNG());
            _file.Close();
            BinaryWriter pic = new BinaryWriter(File.Open(path + files[currentIndex].Split('/')[files[currentIndex].Split('/').Length - 1].Replace(".dsd", ".png"), FileMode.OpenOrCreate));
            pic.Write(_diceTex.EncodeToPNG());
            pic.Close();
        }
        else
        {
            BinaryWriter pic = new BinaryWriter(File.Open(path + files[currentIndex].Split('/')[files[currentIndex].Split('/').Length - 1].Replace(".dsd", ".png"), FileMode.OpenOrCreate));
            pic.Write(skins[currentIndex].texture.EncodeToPNG());
            pic.Close();
        }
        BinaryWriter file = new BinaryWriter(File.Open(path + "die_to_paint.dstd", FileMode.OpenOrCreate));
        file.Write(files[currentIndex]);
        file.Close();
        m_manager.ChangeScene("DiceEditor");
    }

    /// <summary>
    /// Opnes SaveFileDialog to save current skin
    /// </summary>
    public void ExportSkin()
    {
        if (!files[currentIndex].Contains("purchased")) {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = "C:/Users/" + Environment.UserName + "/Documents/";
            saveFileDialog1.Filter = "DS Dice files (*.dsd)|*.dsd";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = false;
            saveFileDialog1.FileName = files[currentIndex].Split('/')[files[currentIndex].Split('/').Length - 1];

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter file = new BinaryWriter(saveFileDialog1.OpenFile());
                file.Write(skins[currentIndex].isTexture);
                file.Write((int)skins[currentIndex].diceType);

                if (!skins[currentIndex].isTexture)
                {
                    file.Write(skins[currentIndex].numbers.r);
                    file.Write(skins[currentIndex].numbers.g);
                    file.Write(skins[currentIndex].numbers.b);
                    file.Write(skins[currentIndex].numbers.a);

                    file.Write(skins[currentIndex].die.r);
                    file.Write(skins[currentIndex].die.g);
                    file.Write(skins[currentIndex].die.b);
                    file.Write(skins[currentIndex].die.a);
                }
                else
                {
                    file.Write(skins[currentIndex].texture.EncodeToPNG().Length);
                    file.Write(skins[currentIndex].texture.width);
                    file.Write(skins[currentIndex].texture.height);
                    file.Write(skins[currentIndex].texture.EncodeToPNG());
                }
                file.Close();
            }
#elif(UNITY_ANDROID && !UNITY_EDITOR)
            m_saveFileWindow.SetActive(true);
#endif
        }
        else
        {
            StartCoroutine(ToggleActivation(m_notificationPanel, 1.0f));
        }
    }

    /// <summary>
    /// Saves file to a given path
    /// </summary>
    /// <param name="_filePath">The path which the file will be saved to</param>
    public void ExportSkin(string _filePath)
    {
        BinaryWriter file = new BinaryWriter(File.Open(_filePath, FileMode.OpenOrCreate));
        file.Write(skins[currentIndex].isTexture);
        file.Write((int)skins[currentIndex].diceType);

        if (!skins[currentIndex].isTexture)
        {
            file.Write(skins[currentIndex].numbers.r);
            file.Write(skins[currentIndex].numbers.g);
            file.Write(skins[currentIndex].numbers.b);
            file.Write(skins[currentIndex].numbers.a);

            file.Write(skins[currentIndex].die.r);
            file.Write(skins[currentIndex].die.g);
            file.Write(skins[currentIndex].die.b);
            file.Write(skins[currentIndex].die.a);
        }
        else
        {
            file.Write(skins[currentIndex].texture.EncodeToPNG().Length);
            file.Write(skins[currentIndex].texture.width);
            file.Write(skins[currentIndex].texture.height);
            file.Write(skins[currentIndex].texture.EncodeToPNG());
        }
        file.Close();
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
