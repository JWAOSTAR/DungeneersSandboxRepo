using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceSetSeleclector : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField]
    Dice.DiceType[] diceTypes = new Dice.DiceType[7];
    [SerializeField]
    MeshRenderer[] dice = new MeshRenderer[7];
    //List<Vector2> uvs_test = new List<Vector2>();
    //List<Color> color_test = new List<Color>();
    List<Dice.DiceSkin> skins = new List<Dice.DiceSkin>();
    Predicate<Dice.DiceSkin>[] searchFuncs = { FindD4, FindD6, FindD8, FindD10_1, FindD10_10, FindD12, FindD20};
    int[] currentSkins = { 0, 0, 0, 0, 0, 0, 0 };

    [Space]
    [SerializeField]
    ColorPicker m_numberColor;
    [SerializeField]
    ColorPicker m_dieColor;
    [SerializeField]
    Image m_numberBlockColor;
    [SerializeField]
    Image m_dieBlockColor;

    void Start()
    {
        m_numberBlockColor.color = m_numberColor.CurrentColor;
        m_dieBlockColor.color = m_dieColor.CurrentColor;
        m_numberColor.gameObject.SetActive(false);
        m_dieColor.gameObject.SetActive(false);

        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
        }

        for(int k = 0; k < dice.Length; k++)
        {
            Dice.DiceSkin skinToAdd = new Dice.DiceSkin();
            skinToAdd.isTexture = false;
            skinToAdd.diceType = (Dice.DiceType)k;
            skinToAdd.numbers = Color.black; 
            skinToAdd.die = Color.red;
            skins.Add(skinToAdd);
        }

        string[] files = Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/");
        for (int j = 0; j < files.Length; j++) { 
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
        }
        
        for (int i = 0; i < dice.Length; i++)
        {
            if (skins.FindAll(searchFuncs[i]).Count > 0)
            {
                if (!skins.FindAll(searchFuncs[i])[0].isTexture)
                {
                    dice[i].materials[0].color = skins.FindAll(searchFuncs[i])[0].numbers;
                    dice[i].materials[1].color = skins.FindAll(searchFuncs[i])[0].die;
                }
                else
                {
                    dice[i].materials[0].SetTexture("_MainTex", skins.FindAll(searchFuncs[i])[0].texture);
                    dice[i].materials[1].SetTexture("_MainTex", skins.FindAll(searchFuncs[i])[0].texture);
                }
            }
            else
            {
                dice[i].materials[0].color = Color.black;
                dice[i].materials[1].color = Color.red;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextSkin(int die)
    {
        if (skins.FindAll(searchFuncs[die]).Count > 1) 
        {
            List<Dice.DiceSkin> skinList = skins.FindAll(searchFuncs[die]);
            currentSkins[die] = (currentSkins[die] + 1 < skinList.Count) ? ++currentSkins[die] : 0;
            
            if (!skinList[currentSkins[die]].isTexture)
            {
                dice[die].materials[0].SetTexture("_MainTex", null);
                dice[die].materials[1].SetTexture("_MainTex", null);
                dice[die].materials[0].color = skinList[currentSkins[die]].numbers;
                dice[die].materials[1].color = skinList[currentSkins[die]].die;
            }
            else
            {
                dice[die].materials[0].SetTexture("_MainTex", skinList[currentSkins[die]].texture);
                dice[die].materials[1].SetTexture("_MainTex", skinList[currentSkins[die]].texture);
                dice[die].materials[0].color = Color.white;
                dice[die].materials[1].color = Color.white;
            }
        }
    }

    public void PrevSkin(int die)
    {
        if (skins.FindAll(searchFuncs[die]).Count > 1)
        {
            List<Dice.DiceSkin> skinList = skins.FindAll(searchFuncs[die]);
            currentSkins[die] = (currentSkins[die] - 1 >= 0) ? --currentSkins[die] : (skinList.Count - 1);
            
            if (!skinList[currentSkins[die]].isTexture)
            {
                dice[die].materials[0].SetTexture("_MainTex", null);
                dice[die].materials[1].SetTexture("_MainTex", null);
                dice[die].materials[0].color = skinList[currentSkins[die]].numbers;
                dice[die].materials[1].color = skinList[currentSkins[die]].die;
            }
            else
            {
                dice[die].materials[0].SetTexture("_MainTex", skinList[currentSkins[die]].texture);
                dice[die].materials[1].SetTexture("_MainTex", skinList[currentSkins[die]].texture);
                dice[die].materials[0].color = Color.white;
                dice[die].materials[1].color = Color.white;
            }
        }
    }

    public void SaveActiveDice()
    {
        BinaryWriter file = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/active_dice_set.dss", FileMode.OpenOrCreate));
        for(int i = 0; i < dice.Length; i++)
        {
            bool isTexture = (skins.FindAll(searchFuncs[i]).Count > 0) ? skins.FindAll(searchFuncs[i])[currentSkins[i]].isTexture : false;
            file.Write(isTexture);
            file.Write(i);
            if (!isTexture)
            {
                file.Write(dice[i].materials[0].color.r);
                file.Write(dice[i].materials[0].color.g);
                file.Write(dice[i].materials[0].color.b);
                file.Write(dice[i].materials[0].color.a);

                file.Write(dice[i].materials[1].color.r);
                file.Write(dice[i].materials[1].color.g);
                file.Write(dice[i].materials[1].color.b);
                file.Write(dice[i].materials[1].color.a);
            }
            else
            {
                file.Write(skins.FindAll(searchFuncs[i])[currentSkins[i]].texture.EncodeToPNG().Length);
                file.Write(skins.FindAll(searchFuncs[i])[currentSkins[i]].texture.width);
                file.Write(skins.FindAll(searchFuncs[i])[currentSkins[i]].texture.height);
                file.Write(skins.FindAll(searchFuncs[i])[currentSkins[i]].texture.EncodeToPNG());
            }
        }
        file.Close();
    }

    public void SetNumColor(Color _color)
    {
        for (int i = 0; i < dice.Length; i++)
        {
            Dice.DiceSkin ds = skins[skins.FindIndex(0, searchFuncs[i])];
            ds.numbers = _color;
            skins[skins.FindIndex(0, searchFuncs[i])] = ds;
            if (currentSkins[i] == 0)
            {
                dice[i].materials[0].color = skins[skins.FindIndex(0, searchFuncs[i])].numbers;
                dice[i].materials[1].color = skins[skins.FindIndex(0, searchFuncs[i])].die;
            }
        }
        m_numberBlockColor.color = _color;
    }

    public void SetDieColor(Color _color)
    {
        for (int i = 0; i < dice.Length; i++)
        {
            Dice.DiceSkin ds = skins[skins.FindIndex(0, searchFuncs[i])];
            ds.die = _color;
            skins[skins.FindIndex(0, searchFuncs[i])] = ds;
            if(currentSkins[i] == 0)
            {
                dice[i].materials[0].color = skins[skins.FindIndex(0, searchFuncs[i])].numbers;
                dice[i].materials[1].color = skins[skins.FindIndex(0, searchFuncs[i])].die;
            }
        }
        m_dieBlockColor.color = _color;
    }

    private static bool FindD4(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D4); }
    private static bool FindD6(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D6); }
    private static bool FindD8(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D8); }
    private static bool FindD10_1(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D10); }
    private static bool FindD10_10(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D10_10s); }
    private static bool FindD12(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D12); }
    private static bool FindD20(Dice.DiceSkin d){ return (d.diceType == Dice.DiceType.D20); }


}
