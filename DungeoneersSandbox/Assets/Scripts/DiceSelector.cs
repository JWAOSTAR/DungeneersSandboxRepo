﻿using System;
using System.IO;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using UnityEditor;

public class DiceSelector : MonoBehaviour
{
    //public enum DiceType
    //{
    //    D4 = 0,
    //    D6,
    //    D8,
    //    D10,
    //    D10_10s,
    //    D12,
    //    D20,
    //}
    [SerializeField]
    Mesh[] models;
    [SerializeField]
    private Texture2D[] textures;

    [SerializeField]
    MeshFilter currentModel;
    [SerializeField]
    Dice.DiceType startingDiceType;
    [NonSerialized]
    public Dice.DiceType currentDiceType;
    [SerializeField]
    Text diceTextField;
    [SerializeField]
    MeshRenderer modelRenderProperties;
    [SerializeField]
    ColorPicker numberColorPicker;
    [SerializeField]
    ColorPicker dieColorPicker;
    [SerializeField]
    DiceAxisMovement diceMover;
    [SerializeField]
    GameObject saveDialog;
    private const int SIZE = 2;
    /*public*/ Color[] baseDiceColors = new Color[SIZE];
    [SerializeField]
    Image[] colorBlocks = new Image[2];
    List<Color> colorOptions = new List<Color>();
    int[] currentColorIndex = { -1, -1 };
    bool isSavedFile = false;
    bool isTexture = false;
    
    void OnValidate()
    {
        if (baseDiceColors.Length != SIZE)
        {
            Debug.LogWarning("Don't change the 'baseDiceColors' field's array size!");
            Array.Resize(ref baseDiceColors, SIZE);
        }
        if (colorBlocks.Length != SIZE)
        {
            Debug.LogWarning("Don't change the 'baseDiceColors' field's array size!");
            Array.Resize(ref colorBlocks, SIZE);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Shader.Find("DS/DicePainterShader");
        Shader.Find("DS/EdgeFixShader");
        Color[] cl = { Color.black, Color.grey, Color.white, Color.red, Color.magenta, Color.blue, Color.cyan, Color.yellow, Color.green };
        colorOptions.AddRange(cl);
        currentDiceType = startingDiceType;
        if (currentModel && (models.Length > 0))
        {
            currentModel.mesh = models[(int)startingDiceType];
            diceTextField.text = (startingDiceType != Dice.DiceType.D10_10s) ? startingDiceType.ToString() : "D10(10s)";
        }  
        if(modelRenderProperties)
        {
            baseDiceColors[0] = numberColorPicker.CurrentColor;
            baseDiceColors[1] = dieColorPicker.CurrentColor;
            for (int i = 0; i < baseDiceColors.Length; i++) {
                
                modelRenderProperties.materials[i].color = baseDiceColors[i];
                colorBlocks[i].color = baseDiceColors[i];
                if(colorOptions.Contains(baseDiceColors[i]))
                {
                    currentColorIndex[i] = colorOptions.IndexOf(baseDiceColors[i]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!diceMover.GetMobility() && !numberColorPicker.isActiveAndEnabled && !dieColorPicker.isActiveAndEnabled && !saveDialog.activeSelf) 
        {
            diceMover.SetMobility(true);
        }
        else if(diceMover.GetMobility() && (numberColorPicker.isActiveAndEnabled || dieColorPicker.isActiveAndEnabled || saveDialog.activeSelf))
        {
            diceMover.SetMobility(false);
        }
    }

    public void SetNumColor(Color _color)
    {
        modelRenderProperties.materials[0].SetTexture("_MainTex", null);
        if (!isTexture || modelRenderProperties.materials[1] == null)
        {
            isTexture = false;
        }
        currentColorIndex[0] = -1;
        modelRenderProperties.materials[0].color = _color;
        colorBlocks[0].color = _color;
    }

    public void SetDiceColor(Color _color)
    {
        modelRenderProperties.materials[1].SetTexture("_MainTex", null);
        if(!isTexture || modelRenderProperties.materials[0] == null)
        {
            isTexture = false;
        }
        currentColorIndex[1] = -1;
        modelRenderProperties.materials[1].color = _color;
        colorBlocks[1].color = _color;
    }

    public void NextModel()
    {
        currentDiceType = (currentDiceType != Dice.DiceType.D20) ? (Dice.DiceType)((int)currentDiceType + 1) : Dice.DiceType.D4;
        currentModel.mesh = models[(int)currentDiceType];
        diceTextField.text = (currentDiceType != Dice.DiceType.D10_10s) ? currentDiceType.ToString() : "D10(10s)";
    }

    public void PrevModel()
    {
        currentDiceType = (currentDiceType != Dice.DiceType.D4) ? (Dice.DiceType)((int)currentDiceType - 1) : Dice.DiceType.D20;
        currentModel.mesh = models[(int)currentDiceType];
        diceTextField.text = (currentDiceType != Dice.DiceType.D10_10s) ? currentDiceType.ToString() : "D10(10s)";
    }

    public void NextNumColor()
    {
        currentColorIndex[0] = (currentColorIndex[0] + 1 < colorOptions.Count) ? (currentColorIndex[0] + 1) : 0;
        modelRenderProperties.materials[0].color = colorOptions[currentColorIndex[0]];
        colorBlocks[0].color = colorOptions[currentColorIndex[0]];
    }

    public void PrevNumColor()
    {
        currentColorIndex[0] = (currentColorIndex[0] - 1 >= 0) ? (currentColorIndex[0] - 1) : (colorOptions.Count - 1);
        modelRenderProperties.materials[0].color = colorOptions[currentColorIndex[0]];
        colorBlocks[0].color = colorOptions[currentColorIndex[0]];
    }

    public void NextDieColor()
    {
        currentColorIndex[1] = (currentColorIndex[1] + 1 < colorOptions.Count) ? (currentColorIndex[1] + 1) : 0;
        modelRenderProperties.materials[1].color = colorOptions[currentColorIndex[1]];
        colorBlocks[1].color = colorOptions[currentColorIndex[1]];
    }

    public void PrevDieColor()
    {
        currentColorIndex[1] = (currentColorIndex[1] - 1 >= 0) ? (currentColorIndex[1] - 1) : (colorOptions.Count - 1);
        modelRenderProperties.materials[1].color = colorOptions[currentColorIndex[1]];
        colorBlocks[1].color = colorOptions[currentColorIndex[1]];
    }

    public void SaveData()
    {
        string path;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/";
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        path = UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/";
#endif
        if (!Directory.Exists(path + "dice/skins/"))
        {
            Directory.CreateDirectory(path + "dice/skins/");
        }
        if (!Directory.Exists(path + "temp/"))
        {
            Directory.CreateDirectory(path + "temp/");
        }

        if (isSavedFile)
        {
            SaveData(path + "dice/skins/" + saveDialog.GetComponentInChildren<InputField>().text + ".dsd");
        }
        else
        {
            SaveData(path + "temp/" + saveDialog.GetComponentInChildren<InputField>().text + ".dsd");
        }
    }

    void SaveData(String path)
    {
        BinaryWriter file = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));

        file.Write(isTexture);

        file.Write((int)currentDiceType);

        if (!isTexture) {
            file.Write(modelRenderProperties.materials[0].color.r);
            file.Write(modelRenderProperties.materials[0].color.g);
            file.Write(modelRenderProperties.materials[0].color.b);
            file.Write(modelRenderProperties.materials[0].color.a);

            file.Write(modelRenderProperties.materials[1].color.r);
            file.Write(modelRenderProperties.materials[1].color.g);
            file.Write(modelRenderProperties.materials[1].color.b);
            file.Write(modelRenderProperties.materials[1].color.a);
        }
        else
        {
            file.Write(((Texture2D)modelRenderProperties.materials[0].mainTexture).EncodeToPNG().Length);
            file.Write(((Texture2D)modelRenderProperties.materials[0].mainTexture).width);
            file.Write(((Texture2D)modelRenderProperties.materials[0].mainTexture).height);
            file.Write(((Texture2D)modelRenderProperties.materials[0].mainTexture).EncodeToPNG());
        }
        file.Close();

    }

    public void LoadData(String path)
    {
        if(File.Exists(path))
        {
            BinaryReader file = new BinaryReader(File.Open(path, FileMode.Open));
            if(!file.ReadBoolean())
            {
                file.ReadInt32();
                for (int i = 0; i < 2; i++)
                {
                    colorBlocks[i].color = modelRenderProperties.materials[i].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                    if (colorOptions.Contains(modelRenderProperties.materials[i].color))
                    {
                        currentColorIndex[i] = colorOptions.IndexOf(modelRenderProperties.materials[i].color);
                    }
                }

                //colorBlocks[1].color = modelRenderProperties.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                //if (colorOptions.Contains(modelRenderProperties.materials[1].color))
                //{
                //    currentColorIndex[1] = colorOptions.IndexOf(modelRenderProperties.materials[1].color);
                //}
            }
        }
    }

    public void SaveDialogConfirmed()
    {
        string path;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/";
#elif(UNITY_ANDROID && !UNITY_EDITOR)
        path = UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/";
#endif
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (!Directory.Exists(path + "dice/"))
        {
            Directory.CreateDirectory(path + "dice/");
        }

        if (!Directory.Exists(path + "dice/skins/"))
        {
            Directory.CreateDirectory(path + "dice/skins/");
        }

        isSavedFile = true;
        
        SaveData(path + "dice/skins/" + saveDialog.GetComponentInChildren<InputField>().text + ".dsd");
    }

    public void SaveDialogCanceled()
    {
        if (!isSavedFile)
        {
            saveDialog.GetComponentInChildren<InputField>().text = "untitled";
        }
    }

    public void GenerateTextureFromMaterial()
    {
        string path;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/";
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        path = UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/";
#endif
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (!Directory.Exists(path+"temp/"))
        {
            Directory.CreateDirectory(path + "temp/");
        }


        Texture2D newTexture = textures[(int)currentDiceType];

        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                if (newTexture.GetPixel(x, y) == Color.black)
                {
                    newTexture.SetPixel(x, y, (modelRenderProperties.materials[0].mainTexture == null) ? colorBlocks[0].color : ((Texture2D)modelRenderProperties.materials[0].mainTexture).GetPixel(x, y));
                }
                else if (newTexture.GetPixel(x, y) == Color.white)
                {
                    newTexture.SetPixel(x, y, (modelRenderProperties.materials[1].mainTexture == null) ? colorBlocks[1].color : ((Texture2D)modelRenderProperties.materials[1].mainTexture).GetPixel(x, y));
                }
            }
        }

        BinaryWriter file = new BinaryWriter(File.Open(path + "temp/" + saveDialog.GetComponentInChildren<InputField>().text + ".png", FileMode.OpenOrCreate));

        file.Write(newTexture.EncodeToPNG());

        file.Close();

        file = new BinaryWriter(File.Open(path + "temp/die_to_paint.dstd", FileMode.OpenOrCreate));
        if (isSavedFile)
        {
            file.Write(path+"dice/skins/" + saveDialog.GetComponentInChildren<InputField>().text + ".dsd");
        }
        else
        {
            file.Write(path + "temp/" + saveDialog.GetComponentInChildren<InputField>().text + ".dsd");
        }

        file.Close();
    }

    public void LoadFile()
    {
        String file_path = string.Empty;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        openFileDialog1.InitialDirectory = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/";
        openFileDialog1.Filter = "DS Dice files (*.dsd)|*.dsd|PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|JPEG files(*.jpeg)|*.jpeg";
        openFileDialog1.FilterIndex = 0;
        openFileDialog1.RestoreDirectory = false;

        if(openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            file_path = openFileDialog1.FileName;
        }
#endif
        if (file_path != string.Empty && file_path != "")
        {
            BinaryReader file = new BinaryReader(File.Open(file_path, FileMode.Open));
            //Dice.DiceSkin skinToAdd = new Dice.DiceSkin();
            if (file_path.EndsWith(".dsd")) {
                isTexture = file.ReadBoolean();
                currentDiceType = (Dice.DiceType)file.ReadInt32();
                currentModel.mesh = models[(int)currentDiceType];
                diceTextField.text = (currentDiceType != Dice.DiceType.D10_10s) ? currentDiceType.ToString() : "D10(10s)";
                if (!isTexture)
                {
                    modelRenderProperties.materials[0].SetTexture("_MainTex", null);
                    modelRenderProperties.materials[1].SetTexture("_MainTex", null);
                    SetNumColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
                    SetDiceColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
                }
                else
                {
                    int array_size = file.ReadInt32();
                    Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                    newTex.LoadImage(file.ReadBytes(array_size));
                    modelRenderProperties.materials[0].SetTexture("_MainTex", newTex);
                    modelRenderProperties.materials[1].SetTexture("_MainTex", newTex);
                    modelRenderProperties.materials[0].SetColor("_Color", Color.white);
                    modelRenderProperties.materials[1].SetColor("_Color", Color.white);
                }
                file.Close();
            }
            else
            {
                file.Close();
                isTexture = true;
                Texture2D newTex = new Texture2D(2, 2);
                newTex.LoadImage(File.ReadAllBytes(file_path));
                modelRenderProperties.materials[0].SetTexture("_MainTex", newTex);
                modelRenderProperties.materials[1].SetTexture("_MainTex", newTex);
                modelRenderProperties.materials[0].SetColor("_Color", Color.white);
                modelRenderProperties.materials[1].SetColor("_Color", Color.white);
            }
        }

        //        String file_path;
        //#if (UNITY_STANDALONE || UNITY_EDITOR)
        //        file_path = EditorUtility.OpenFilePanel("", "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/", "dsd");
        //#elif (UNITY_IOS || UNITY_ANDROID)
        //#endif
        //        if(file_path != null)
        //        {
        //            BinaryReader file = new BinaryReader(File.Open(file_path, FileMode.Open));
        //            //Dice.DiceSkin skinToAdd = new Dice.DiceSkin();
        //            isTexture = file.ReadBoolean();
        //            currentDiceType = (Dice.DiceType)file.ReadInt32();
        //            currentModel.mesh = models[(int)currentDiceType];
        //            diceTextField.text = (currentDiceType != Dice.DiceType.D10_10s) ? currentDiceType.ToString() : "D10(10s)";
        //            if (!isTexture)
        //            {
        //                modelRenderProperties.materials[0].SetTexture("_MainTex", null);
        //                modelRenderProperties.materials[1].SetTexture("_MainTex", null);
        //                SetNumColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
        //                SetDiceColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
        //            }
        //            else
        //            {
        //                int array_size = file.ReadInt32();
        //                Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
        //                newTex.LoadImage(file.ReadBytes(array_size));
        //                modelRenderProperties.materials[0].SetTexture("_MainTex", newTex);
        //                modelRenderProperties.materials[1].SetTexture("_MainTex", newTex);
        //            }
        //            file.Close();
        //        }

    }
}
