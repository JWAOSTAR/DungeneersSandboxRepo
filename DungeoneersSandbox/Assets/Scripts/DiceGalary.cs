using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DiceGalary : MonoBehaviour
{
    [SerializeField]
    RectTransform m_scrollContent;

    List<Dice.DiceSkin> skins = new List<Dice.DiceSkin>();

    // Start is called before the first frame update
    void Start()
    {
        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
        }
        string[] files = Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
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
                    m_scrollContent.sizeDelta = new Vector2(m_scrollContent.sizeDelta.x, m_scrollContent.sizeDelta.y + m_scrollContent.GetChild(m_scrollContent.childCount - 1).GetComponent<RectTransform>().sizeDelta.y + 20);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
