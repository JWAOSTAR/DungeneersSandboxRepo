﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class DiceGalary : MonoBehaviour
{
    [SerializeField]
    RectTransform m_scrollContent;
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

    List<Dice.DiceSkin> skins = new List<Dice.DiceSkin>();
    string[] files;
    int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
        }
        files = Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
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
            if(j >= m_scrollContent.childCount)
            {
                Instantiate(m_scrollContent.GetChild(m_scrollContent.childCount - 1), m_scrollContent).gameObject.name = "Line " + j;
                m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetChild(0).GetComponent<Text>().text = files[j].Split('/')[files[j].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
                m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().position = new Vector3(m_scrollContent.GetChild(0).GetComponent<RectTransform>().position.x, m_scrollContent.GetChild(0).GetComponent<RectTransform>().position.y - ((m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y+10)*j), m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().position.z);
                if (m_scrollContent.sizeDelta.y < ((m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y + 10) * (j + 1)))
                {
                    m_scrollContent.sizeDelta = new Vector2(m_scrollContent.sizeDelta.x, m_scrollContent.sizeDelta.y + m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y + 10);
                    UpdateLineSpacing();
                }
            }
            else
            {
                m_scrollContent.GetChild(j).GetChild(0).GetComponent<Text>().text = files[j].Split('/')[files[j].Split('/').Length - 1].Replace(".dsd", "").Replace("_", " ");
            }
            int t = j;
            m_scrollContent.GetChild(j).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSkin(t); });
        }
        SetSkin(m_scrollContent.childCount - 1);
    }

    // Update is called once per frame
    void Update()
    {
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

    void UpdateLineSpacing()
    {
        for(int i = 1; i < m_scrollContent.childCount + 1; i++)
        {
            m_scrollContent.GetChild(i - 1).GetComponent<RectTransform>().position = new Vector3(m_scrollContent.GetChild(0).GetComponent<RectTransform>().position.x, ((m_scrollContent.GetChild(i - 1).GetComponent<RectTransform>().sizeDelta.y + 13) * i), m_scrollContent.GetChild(i - 1).GetComponent<RectTransform>().position.z);
        }
    }

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

    public void EditSkin()
    {
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
            BinaryWriter pic = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/" + files[currentIndex].Split('/')[files[currentIndex].Split('/').Length - 1].Replace(".dsd", ".png"), FileMode.OpenOrCreate));
            pic.Write(_diceTex.EncodeToPNG());
            pic.Close();
        }
        else
        {
            BinaryWriter pic = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/" + files[currentIndex].Split('/')[files[currentIndex].Split('/').Length - 1].Replace(".dsd", ".png"), FileMode.OpenOrCreate));
            pic.Write(skins[currentIndex].texture.EncodeToPNG());
            pic.Close();
        }
        BinaryWriter file = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/die_to_paint.dstd", FileMode.OpenOrCreate));
        file.Write(files[currentIndex]);
        file.Close();
        m_manager.ChangeScene("DiceEditor");
    }

    public void ExportSkin()
    {
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
    }
}