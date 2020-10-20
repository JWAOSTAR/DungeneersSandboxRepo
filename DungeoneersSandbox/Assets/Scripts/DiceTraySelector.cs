using System;
using System.Security.Permissions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif

public class DiceTraySelector : MonoBehaviour
{
    enum MATERIAL_0_TYPE
    {
        COLOR,
        TEXTURE,
        TILE,
    }

    enum MATERIAL_1_TYPE
    {
        COLOR,
        TEXTURE = 4,
        TILE = 8,
    }

    struct PatternProperties
    {
        public Vector2 Tiling;
        public Vector2 Offset;
    }

    [SerializeField]
    MeshFilter m_currentMesh;
    [SerializeField]
    Mesh[] m_meshOptions;
    [SerializeField]
    MeshRenderer m_trayMaterials;
    [SerializeField]
    Text m_modelName;
    [SerializeField]
    ColorPicker m_innerColorPicker;
    [SerializeField]
    ColorPicker m_outterColorPicker;
    [SerializeField]
    Image m_innerColorBlock;
    [SerializeField]
    Image m_outterColorBlock;
    [SerializeField]
    DiceAxisMovement m_trayMover;
    [SerializeField]
    SceneChanger m_manager;
    [SerializeField]
    GameObject m_patternPropertiesMenu;
    [SerializeField]
    GameObject m_FileOpenWindow;

    [SerializeField]
    Sprite m_nullImage;
    [SerializeField]
    Image m_InnerPatternImage;
    [SerializeField]
    Image m_OutterPatternImage;
    [SerializeField]
    Image m_patternUIImage;
    [SerializeField]
    InputField m_XTilling;
    [SerializeField]
    InputField m_YTilling;
    [SerializeField]
    InputField m_XOffset;
    [SerializeField]
    InputField m_YOffset;

    int currentModel = 0;
    int currentPattern = 0;
    MATERIAL_0_TYPE mat0type = MATERIAL_0_TYPE.COLOR;
    MATERIAL_1_TYPE mat1type = MATERIAL_1_TYPE.COLOR;
    PatternProperties pattern0;
    PatternProperties pattern1;
    bool pattern0uploaded = false;
    bool pattern1uploaded = false;

    // Start is called before the first frame update
    void Start()
    {
        m_trayMover.SetMobility(false);
        m_currentMesh.mesh = m_meshOptions[currentModel];
        m_innerColorPicker.gameObject.SetActive(false);
        m_outterColorPicker.gameObject.SetActive(false);
        m_patternPropertiesMenu.SetActive(false);

        pattern0.Tiling = new Vector2(1.0f, 1.0f);
        pattern0.Offset = new Vector2(0.0f, 0.0f);

        pattern1.Tiling = new Vector2(1.0f, 1.0f);
        pattern1.Offset = new Vector2(0.0f, 0.0f);

        m_XTilling.text = 1.0f.ToString("0.00");
        m_YTilling.text = 1.0f.ToString("0.00");

        m_XOffset.text = 0.0f.ToString("0.00");
        m_YOffset.text = 0.0f.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_trayMover.GetMobility() && Input.GetKey(KeyCode.LeftShift))
        {
            m_trayMover.SetMobility(true);
        }
        else if (m_trayMover.GetMobility() && !Input.GetKey(KeyCode.LeftShift))
        {
            m_trayMover.SetMobility(false);
        }
    }

    public void NextModel()
    {
        currentModel = ((currentModel + 1) < m_meshOptions.Length) ? currentModel + 1 : 0;
        m_currentMesh.mesh = m_meshOptions[currentModel];
        m_modelName.text = "Model " + currentModel;
    }

    public void PrevModel()
    {
        currentModel = ((currentModel - 1) >= 0) ? currentModel - 1 : m_meshOptions.Length - 1;
        m_currentMesh.mesh = m_meshOptions[currentModel];
        m_modelName.text = "Model " + currentModel;
    }

    public void SetInnerColor(Color _color)
    {
        if (!pattern0uploaded)
        {
            m_trayMaterials.materials[1].color = _color;
        }
        else
        {
            for (int y = 0; y < m_trayMaterials.materials[1].mainTexture.height; y++)
            {
                for (int x = 0; x < m_trayMaterials.materials[1].mainTexture.width; x++)
                {
                    if (m_InnerPatternImage.sprite.texture.GetPixel(x, y).a == 0.0f)
                    {
                        ((Texture2D)m_trayMaterials.materials[1].mainTexture).SetPixel(x, y, _color);
                    }
                }
            }
            ((Texture2D)m_trayMaterials.materials[1].mainTexture).Apply();
        }
        m_innerColorBlock.color = _color;
    }

    public void SetOutterColor(Color _color)
    {
        if (!pattern1uploaded)
        {
            m_trayMaterials.materials[0].color = _color;
        }
        else
        {
            for (int y = 0; y < m_trayMaterials.materials[0].mainTexture.height; y++)
            {
                for (int x = 0; x < m_trayMaterials.materials[0].mainTexture.width; x++)
                {
                    if (m_OutterPatternImage.sprite.texture.GetPixel(x, y).a == 0.0f)
                    {
                        ((Texture2D)m_trayMaterials.materials[0].mainTexture).SetPixel(x, y, _color);
                    }
                }
            }
            ((Texture2D)m_trayMaterials.materials[0].mainTexture).Apply();
        }
        m_outterColorBlock.color = _color;
        
    }

    public void OpenSaveDialog()
    {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        SaveFileDialog diceTraySaveDialog = new SaveFileDialog();
        diceTraySaveDialog.InitialDirectory = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/diceTray/";
        diceTraySaveDialog.Filter = "DS Dice Tray files (*.dst)|*.dst|All files (*.*)|*.*";
        diceTraySaveDialog.FilterIndex = 0;
        diceTraySaveDialog.RestoreDirectory = true;

        if(diceTraySaveDialog.ShowDialog() == DialogResult.OK)
        {
            SaveDiceTray(diceTraySaveDialog.FileName);
        }
#endif
    }

    public void OpenFileDialog()
    {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog diceTrayOpenDialog = new OpenFileDialog();
        diceTrayOpenDialog.InitialDirectory = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/diceTray/";
        diceTrayOpenDialog.Filter = "DS Dice Tray files (*.dst)|*.dst|PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
        diceTrayOpenDialog.FilterIndex = 0;
        diceTrayOpenDialog.RestoreDirectory = true;

        if (diceTrayOpenDialog.ShowDialog() == DialogResult.OK)
        {
            BinaryReader file = new BinaryReader(File.Open(diceTrayOpenDialog.FileName, FileMode.Open));
            if (diceTrayOpenDialog.FileName.EndsWith(".dst")) {
                currentModel = file.ReadInt32();
                int matType = file.ReadInt32();
                m_currentMesh.mesh = m_meshOptions[currentModel];
                m_modelName.text = "Model " + currentModel;

                if ((matType != 3) && (matType != 7) && (matType < 11) && (matType >= 0))
                {
                    if ((matType & 4) == 4)
                    {
                        int arraySize = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(arraySize));
                        m_trayMaterials.materials[1].color = Color.white;
                        m_trayMaterials.materials[1].SetTexture("_MainTex", newTex);

                        mat0type = MATERIAL_0_TYPE.TEXTURE;
                    }
                    else if ((matType & 8) == 8)
                    {
                        //TODO: Add in file reading when shader is written
                        int arraySize = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(arraySize));
                        pattern0.Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
                        pattern0.Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
                        m_innerColorPicker.SetCurrentColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
                        m_innerColorBlock.color = m_innerColorPicker.CurrentColor;
                        m_InnerPatternImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
                        Texture2D tempTex = new Texture2D(newTex.width, newTex.height);
                        tempTex.LoadImage(newTex.EncodeToPNG());
                        for (int y = 0; y < tempTex.height; y++)
                        {
                            for (int x = 0; x < tempTex.width; x++)
                            {
                                if (tempTex.GetPixel(x, y).a == 0.0f)
                                {
                                    tempTex.SetPixel(x, y, m_innerColorPicker.CurrentColor);
                                }
                            }
                        }
                        tempTex.Apply();
                        m_trayMaterials.materials[1].color = Color.white;
                        m_trayMaterials.materials[1].SetTexture("_MainTex", tempTex);
                        m_trayMaterials.materials[1].SetTextureScale("_MainTex", pattern0.Tiling);
                        m_trayMaterials.materials[1].SetTextureOffset("_MainTex", pattern0.Offset);

                        mat0type = MATERIAL_0_TYPE.TILE;
                        pattern0uploaded = true;
                    }
                    else
                    {
                        m_trayMaterials.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());

                        mat0type = MATERIAL_0_TYPE.COLOR;
                    }

                    if ((matType & 1) == 1)
                    {
                        int arraySize = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(arraySize));
                        m_trayMaterials.materials[0].color = Color.white;
                        m_trayMaterials.materials[0].SetTexture("_MainTex", newTex);

                        mat1type = MATERIAL_1_TYPE.TEXTURE;
                    }
                    else if ((matType & 2) == 2)
                    {
                        //TODO: Add in file reading when shader is written

                        int arraySize = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(arraySize));
                        pattern1.Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
                        pattern1.Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
                        m_outterColorPicker.SetCurrentColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
                        m_outterColorBlock.color = m_outterColorPicker.CurrentColor;
                        m_OutterPatternImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
                        Texture2D tempTex = new Texture2D(newTex.width, newTex.height);
                        tempTex.LoadImage(newTex.EncodeToPNG());
                        for (int y = 0; y < tempTex.height; y++)
                        {
                            for (int x = 0; x < tempTex.width; x++)
                            {
                                if (tempTex.GetPixel(x, y).a == 0.0f)
                                {
                                    tempTex.SetPixel(x, y, m_outterColorPicker.CurrentColor);
                                }
                            }
                        }
                        tempTex.Apply();
                        m_trayMaterials.materials[0].color = Color.white;
                        m_trayMaterials.materials[0].SetTexture("_MainTex", tempTex);
                        m_trayMaterials.materials[0].SetTextureScale("_MainTex", pattern1.Tiling);
                        m_trayMaterials.materials[0].SetTextureOffset("_MainTex", pattern1.Offset);
                        mat1type = MATERIAL_1_TYPE.TILE;
                        pattern1uploaded = true;
                    }
                    else
                    {
                        m_trayMaterials.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());

                        mat1type = MATERIAL_1_TYPE.COLOR;
                    }
                }
                file.Close();
            }
            else if(diceTrayOpenDialog.FileName.EndsWith(".png") || diceTrayOpenDialog.FileName.EndsWith(".jpg") || diceTrayOpenDialog.FileName.EndsWith(".jpeg"))
            {
                mat0type = MATERIAL_0_TYPE.TEXTURE;
                mat1type = MATERIAL_1_TYPE.TEXTURE;

                file.Close();
                Texture2D newTex = new Texture2D(2, 2);
                newTex.LoadImage(File.ReadAllBytes(diceTrayOpenDialog.FileName));
                m_trayMaterials.materials[0].color = Color.white;
                m_trayMaterials.materials[1].color = Color.white;
                m_trayMaterials.materials[0].SetTexture("_MainTex", newTex);
                m_trayMaterials.materials[1].SetTexture("_MainTex", newTex);
            }
            
        }
#elif(UNITY_ANDROID && !UNITY_EDITOR)
        m_FileOpenWindow.SetActive(true);
#endif
    }

    public void UploadInnerPatternImage()
    {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog patternImageOpenDialog = new OpenFileDialog();
        patternImageOpenDialog.InitialDirectory = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/diceTray/";
        patternImageOpenDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
        patternImageOpenDialog.FilterIndex = 0;
        patternImageOpenDialog.RestoreDirectory = true;
        if(patternImageOpenDialog.ShowDialog() == DialogResult.OK)
        {
            m_patternPropertiesMenu.SetActive(true);
            Texture2D newTex = new Texture2D(2, 2);
            newTex.LoadImage(File.ReadAllBytes(patternImageOpenDialog.FileName));
            newTex.alphaIsTransparency = true;
            m_InnerPatternImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
            Texture2D tempTex = new Texture2D(newTex.width, newTex.height);
            tempTex.LoadImage(newTex.EncodeToPNG());
            for (int y = 0; y < tempTex.height; y++)
            {
                for (int x = 0; x < tempTex.width; x++)
                {
                    if (tempTex.GetPixel(x, y).a == 0.0f)
                    {
                        tempTex.SetPixel(x, y, m_innerColorPicker.CurrentColor);
                    }
                }
            }
            tempTex.Apply();
            m_trayMaterials.materials[1].color = Color.white;
            m_trayMaterials.materials[1].SetTexture("_MainTex", tempTex);
            pattern0uploaded = true;
            mat0type = MATERIAL_0_TYPE.TILE;
        }
#endif
    }

    public void UploadOutterPatternImage()
    {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog patternImageOpenDialog = new OpenFileDialog();
        patternImageOpenDialog.InitialDirectory = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/diceTray/";
        patternImageOpenDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
        patternImageOpenDialog.FilterIndex = 0;
        patternImageOpenDialog.RestoreDirectory = true;
        if (patternImageOpenDialog.ShowDialog() == DialogResult.OK)
        {
            m_patternPropertiesMenu.SetActive(true);
            Texture2D newTex = new Texture2D(2, 2);
            newTex.LoadImage(File.ReadAllBytes(patternImageOpenDialog.FileName));
            newTex.alphaIsTransparency = true;
            m_OutterPatternImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
            Texture2D tempTex = new Texture2D(newTex.width, newTex.height);
            tempTex.LoadImage(newTex.EncodeToPNG());
            for(int y = 0; y < tempTex.height; y++)
            {
                for(int x = 0; x < tempTex.width; x++)
                {
                    if (tempTex.GetPixel(x, y).a == 0.0f)
                    {
                        tempTex.SetPixel(x, y, m_outterColorPicker.CurrentColor);
                    }
                }
            }
            tempTex.Apply();
            m_trayMaterials.materials[0].color = Color.white;
            m_trayMaterials.materials[0].SetTexture("_MainTex", tempTex);
            pattern1uploaded = true;
            mat1type = MATERIAL_1_TYPE.TILE;
        }
#endif
    }

    public void SaveDiceTray(string path)
    {
        BinaryWriter file = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));

        file.Write(currentModel);

        file.Write((((int)mat0type) + ((int)mat1type)));

        switch (mat0type)
        {
            case MATERIAL_0_TYPE.COLOR:
                {
                    file.Write(m_trayMaterials.materials[1].color.r);
                    file.Write(m_trayMaterials.materials[1].color.g);
                    file.Write(m_trayMaterials.materials[1].color.b);
                    file.Write(m_trayMaterials.materials[1].color.a);
                }
                break;
            case MATERIAL_0_TYPE.TEXTURE:
                {
                    Texture2D tex2D = (Texture2D)m_trayMaterials.materials[1].mainTexture;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[1].mainTexture.width);
                    file.Write(m_trayMaterials.materials[1].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                }
                break;
            case MATERIAL_0_TYPE.TILE:
                {
                    Texture2D tex2D = m_InnerPatternImage.sprite.texture;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[1].mainTexture.width);
                    file.Write(m_trayMaterials.materials[1].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                    file.Write(m_trayMaterials.materials[1].GetTextureScale("_MainTex").x);
                    file.Write(m_trayMaterials.materials[1].GetTextureScale("_MainTex").y);
                    file.Write(m_trayMaterials.materials[1].GetTextureOffset("_MainTex").x);
                    file.Write(m_trayMaterials.materials[1].GetTextureOffset("_MainTex").y);
                    file.Write(m_innerColorPicker.CurrentColor.r);
                    file.Write(m_innerColorPicker.CurrentColor.g);
                    file.Write(m_innerColorPicker.CurrentColor.b);
                    file.Write(m_innerColorPicker.CurrentColor.a);
                }
                break;
            default:
                break;
        }

        switch (mat1type)
        {
            case MATERIAL_1_TYPE.COLOR:
                {
                    file.Write(m_trayMaterials.materials[0].color.r);
                    file.Write(m_trayMaterials.materials[0].color.g);
                    file.Write(m_trayMaterials.materials[0].color.b);
                    file.Write(m_trayMaterials.materials[0].color.a);
                }
                break;
            case MATERIAL_1_TYPE.TEXTURE:
                {
                    Texture2D tex2D = (Texture2D)m_trayMaterials.materials[0].mainTexture;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[0].mainTexture.width);
                    file.Write(m_trayMaterials.materials[0].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                }
                break;
            case MATERIAL_1_TYPE.TILE:
                {
                    Texture2D tex2D = m_OutterPatternImage.sprite.texture;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[0].mainTexture.width);
                    file.Write(m_trayMaterials.materials[0].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                    file.Write(m_trayMaterials.materials[0].GetTextureScale("_MainTex").x);
                    file.Write(m_trayMaterials.materials[0].GetTextureScale("_MainTex").y);
                    file.Write(m_trayMaterials.materials[0].GetTextureOffset("_MainTex").x);
                    file.Write(m_trayMaterials.materials[0].GetTextureOffset("_MainTex").y);
                    file.Write(m_outterColorPicker.CurrentColor.r);
                    file.Write(m_outterColorPicker.CurrentColor.g);
                    file.Write(m_outterColorPicker.CurrentColor.b);
                    file.Write(m_outterColorPicker.CurrentColor.a);
                }
                break;
            default:
                break;
        }

        file.Close();

        //m_manager.ChangeScene("DiceSetSelector");
    }

    public void SaveDiceTray()
    {
        BinaryWriter file;
        string rootPath = UnityEngine.Application.persistentDataPath;
        
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        file = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/active_dice_tray.dst", FileMode.OpenOrCreate));
#elif(UNITY_ANDROID && !UNITY_EDITOR)
        //AndroidJavaClass jc = new AndroidJavaClass("java.io.File");
        if (!Directory.Exists(rootPath + "/DungeoneersSamdbox/dice/"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/");
        }
        file = new BinaryWriter(File.Open(UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/dice/active_dice_tray.dst", FileMode.OpenOrCreate));
#endif

        //file.Write(currentModel);

        //file.Write(m_trayMaterials.materials[1].color.r);
        //file.Write(m_trayMaterials.materials[1].color.g);
        //file.Write(m_trayMaterials.materials[1].color.b);
        //file.Write(m_trayMaterials.materials[1].color.a);

        //file.Write(m_trayMaterials.materials[0].color.r);
        //file.Write(m_trayMaterials.materials[0].color.g);
        //file.Write(m_trayMaterials.materials[0].color.b);
        //file.Write(m_trayMaterials.materials[0].color.a);

        file.Write(currentModel);

        file.Write((((int)mat0type) + ((int)mat1type)));

        switch (mat0type)
        {
            case MATERIAL_0_TYPE.COLOR:
                {
                    file.Write(m_trayMaterials.materials[1].color.r);
                    file.Write(m_trayMaterials.materials[1].color.g);
                    file.Write(m_trayMaterials.materials[1].color.b);
                    file.Write(m_trayMaterials.materials[1].color.a);
                }
                break;
            case MATERIAL_0_TYPE.TEXTURE:
                {
                    Texture2D tex2D = (Texture2D)m_trayMaterials.materials[1].mainTexture/*new Texture2D(m_trayMaterials.materials[1].mainTexture.width, m_trayMaterials.materials[1].mainTexture.height, TextureFormat.RGBA32, false)*/;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[1].mainTexture.width);
                    file.Write(m_trayMaterials.materials[1].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                }
                break;
            case MATERIAL_0_TYPE.TILE:
                {
                    Texture2D tex2D = m_InnerPatternImage.sprite.texture;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[1].mainTexture.width);
                    file.Write(m_trayMaterials.materials[1].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                    file.Write(m_trayMaterials.materials[1].GetTextureScale("_MainTex").x);
                    file.Write(m_trayMaterials.materials[1].GetTextureScale("_MainTex").y);
                    file.Write(m_trayMaterials.materials[1].GetTextureOffset("_MainTex").x);
                    file.Write(m_trayMaterials.materials[1].GetTextureOffset("_MainTex").y);
                    file.Write(m_innerColorPicker.CurrentColor.r);
                    file.Write(m_innerColorPicker.CurrentColor.g);
                    file.Write(m_innerColorPicker.CurrentColor.b);
                    file.Write(m_innerColorPicker.CurrentColor.a);
                }
                break;
            default:
                break;
        }

        switch (mat1type)
        {
            case MATERIAL_1_TYPE.COLOR:
                {
                    file.Write(m_trayMaterials.materials[0].color.r);
                    file.Write(m_trayMaterials.materials[0].color.g);
                    file.Write(m_trayMaterials.materials[0].color.b);
                    file.Write(m_trayMaterials.materials[0].color.a);
                }
                break;
            case MATERIAL_1_TYPE.TEXTURE:
                {
                    Texture2D tex2D = (Texture2D)m_trayMaterials.materials[0].mainTexture/*new Texture2D(m_trayMaterials.materials[0].mainTexture.width, m_trayMaterials.materials[0].mainTexture.height, TextureFormat.RGBA32, false)*/;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[0].mainTexture.width);
                    file.Write(m_trayMaterials.materials[0].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                }
                break;
            case MATERIAL_1_TYPE.TILE:
                {
                    Texture2D tex2D = m_OutterPatternImage.sprite.texture;
                    file.Write(tex2D.EncodeToPNG().Length);
                    file.Write(m_trayMaterials.materials[0].mainTexture.width);
                    file.Write(m_trayMaterials.materials[0].mainTexture.height);
                    file.Write(tex2D.EncodeToPNG());
                    file.Write(m_trayMaterials.materials[0].GetTextureScale("_MainTex").x);
                    file.Write(m_trayMaterials.materials[0].GetTextureScale("_MainTex").y);
                    file.Write(m_trayMaterials.materials[0].GetTextureOffset("_MainTex").x);
                    file.Write(m_trayMaterials.materials[0].GetTextureOffset("_MainTex").y);
                    file.Write(m_outterColorPicker.CurrentColor.r);
                    file.Write(m_outterColorPicker.CurrentColor.g);
                    file.Write(m_outterColorPicker.CurrentColor.b);
                    file.Write(m_outterColorPicker.CurrentColor.a);
                }
                break;
            default:
                break;
        }

        file.Close();
        //m_manager.ChangeScene("DiceSetSelector");
    }
    public void LoadDiceTray(string _path)
    {
        //#if (UNITY_ANDROID && !UNITY_EDITOR)
        BinaryReader file = new BinaryReader(File.Open(_path, FileMode.Open));
        if (_path.EndsWith(".dst"))
        {
            currentModel = file.ReadInt32();
            int matType = file.ReadInt32();
            m_currentMesh.mesh = m_meshOptions[currentModel];
            m_modelName.text = "Model " + currentModel;

            if ((matType != 3) && (matType != 7) && (matType < 11) && (matType >= 0))
            {
                if ((matType & 4) == 4)
                {
                    int arraySize = file.ReadInt32();
                    Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                    newTex.LoadImage(file.ReadBytes(arraySize));
                    m_trayMaterials.materials[1].color = Color.white;
                    m_trayMaterials.materials[1].SetTexture("_MainTex", newTex);

                    mat0type = MATERIAL_0_TYPE.TEXTURE;
                }
                else if ((matType & 8) == 8)
                {
                    //TODO: Add in file reading when shader is written
                    int arraySize = file.ReadInt32();
                    Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                    newTex.LoadImage(file.ReadBytes(arraySize));
                    pattern0.Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
                    pattern0.Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
                    m_innerColorPicker.SetCurrentColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
                    m_innerColorBlock.color = m_innerColorPicker.CurrentColor;
                    m_InnerPatternImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
                    Texture2D tempTex = new Texture2D(newTex.width, newTex.height);
                    tempTex.LoadImage(newTex.EncodeToPNG());
                    for (int y = 0; y < tempTex.height; y++)
                    {
                        for (int x = 0; x < tempTex.width; x++)
                        {
                            if (tempTex.GetPixel(x, y).a == 0.0f)
                            {
                                tempTex.SetPixel(x, y, m_innerColorPicker.CurrentColor);
                            }
                        }
                    }
                    tempTex.Apply();
                    m_trayMaterials.materials[1].color = Color.white;
                    m_trayMaterials.materials[1].SetTexture("_MainTex", tempTex);
                    m_trayMaterials.materials[1].SetTextureScale("_MainTex", pattern0.Tiling);
                    m_trayMaterials.materials[1].SetTextureOffset("_MainTex", pattern0.Offset);

                    mat0type = MATERIAL_0_TYPE.TILE;
                    pattern0uploaded = true;
                }
                else
                {
                    m_trayMaterials.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());

                    mat0type = MATERIAL_0_TYPE.COLOR;
                }

                if ((matType & 1) == 1)
                {
                    int arraySize = file.ReadInt32();
                    Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                    newTex.LoadImage(file.ReadBytes(arraySize));
                    m_trayMaterials.materials[0].color = Color.white;
                    m_trayMaterials.materials[0].SetTexture("_MainTex", newTex);

                    mat1type = MATERIAL_1_TYPE.TEXTURE;
                }
                else if ((matType & 2) == 2)
                {
                    //TODO: Add in file reading when shader is written

                    int arraySize = file.ReadInt32();
                    Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                    newTex.LoadImage(file.ReadBytes(arraySize));
                    pattern1.Tiling = new Vector2(file.ReadSingle(), file.ReadSingle());
                    pattern1.Offset = new Vector2(file.ReadSingle(), file.ReadSingle());
                    m_outterColorPicker.SetCurrentColor(new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle()));
                    m_outterColorBlock.color = m_outterColorPicker.CurrentColor;
                    m_OutterPatternImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
                    Texture2D tempTex = new Texture2D(newTex.width, newTex.height);
                    tempTex.LoadImage(newTex.EncodeToPNG());
                    for (int y = 0; y < tempTex.height; y++)
                    {
                        for (int x = 0; x < tempTex.width; x++)
                        {
                            if (tempTex.GetPixel(x, y).a == 0.0f)
                            {
                                tempTex.SetPixel(x, y, m_outterColorPicker.CurrentColor);
                            }
                        }
                    }
                    tempTex.Apply();
                    m_trayMaterials.materials[0].color = Color.white;
                    m_trayMaterials.materials[0].SetTexture("_MainTex", tempTex);
                    m_trayMaterials.materials[0].SetTextureScale("_MainTex", pattern1.Tiling);
                    m_trayMaterials.materials[0].SetTextureOffset("_MainTex", pattern1.Offset);
                    mat1type = MATERIAL_1_TYPE.TILE;
                    pattern1uploaded = true;
                }
                else
                {
                    m_trayMaterials.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());

                    mat1type = MATERIAL_1_TYPE.COLOR;
                }
            }
            file.Close();
        }
        else if (_path.EndsWith(".png") || _path.EndsWith(".jpg") || _path.EndsWith(".jpeg"))
        {
            mat0type = MATERIAL_0_TYPE.TEXTURE;
            mat1type = MATERIAL_1_TYPE.TEXTURE;

            file.Close();
            Texture2D newTex = new Texture2D(2, 2);
            newTex.LoadImage(File.ReadAllBytes(_path));
            m_trayMaterials.materials[0].color = Color.white;
            m_trayMaterials.materials[1].color = Color.white;
            m_trayMaterials.materials[0].SetTexture("_MainTex", newTex);
            m_trayMaterials.materials[1].SetTexture("_MainTex", newTex);
        }
        //#endif
    }
    public void OpenPatternEditor(int _inORout)
    {
        if (_inORout == 0 && pattern0uploaded)
        {
            SetPatternUIImage(_inORout);
            m_patternPropertiesMenu.SetActive(true);
        }
        else if (_inORout == 1 && pattern1uploaded)
        {
            SetPatternUIImage(_inORout);
            m_patternPropertiesMenu.SetActive(true);
        }
    }

    public void SetPatternUIImage(int _inORout)
    {
        currentPattern = _inORout;
        m_patternUIImage.sprite = (_inORout == 0) ? m_InnerPatternImage.sprite : m_OutterPatternImage.sprite;

        m_XTilling.text = ((_inORout == 0) ? pattern0.Tiling.x : pattern1.Tiling.x).ToString("0.00");
        m_YTilling.text = ((_inORout == 0) ? pattern0.Tiling.y : pattern1.Tiling.y).ToString("0.00");

        m_XOffset.text = ((_inORout == 0) ? pattern0.Offset.x : pattern1.Offset.x).ToString("0.00");
        m_YOffset.text = ((_inORout == 0) ? pattern0.Offset.y : pattern1.Offset.y).ToString("0.00");
    }

    public void SetTillingX(string _tilling_x)
    {
        float _val_tilling_x;
        if (float.TryParse(_tilling_x, out _val_tilling_x))
        {
            if (_val_tilling_x > 1.0f)
            {
                if (currentPattern == 0)
                {
                    pattern0.Tiling.x = _val_tilling_x;
                    m_XTilling.text = pattern0.Tiling.x.ToString("0.00");
                    m_trayMaterials.materials[1].SetTextureScale("_MainTex", pattern0.Tiling);
                }
                else
                {
                    pattern1.Tiling.x = _val_tilling_x;
                    m_XTilling.text = pattern1.Tiling.x.ToString("0.00");
                    m_trayMaterials.materials[0].SetTextureScale("_MainTex", pattern1.Tiling);
                }
            }
            else
            {
                if (currentPattern == 0)
                {
                    pattern0.Tiling.x = 1.0f;
                    m_XTilling.text = pattern0.Tiling.x.ToString("0.00");
                    m_trayMaterials.materials[1].SetTextureScale("_MainTex", pattern0.Tiling);
                }
                else
                {
                    pattern1.Tiling.x = 1.0f;
                    m_XTilling.text = pattern1.Tiling.x.ToString("0.00");
                    m_trayMaterials.materials[0].SetTextureScale("_MainTex", pattern1.Tiling);
                }
            }
        }

    }

    public void SetTillingY(string _tilling_y)
    {
        float _val_tilling_y;
        if (float.TryParse(_tilling_y, out _val_tilling_y)) {
            if (_val_tilling_y > 1.0f)
            {
                if (currentPattern == 0)
                {
                    pattern0.Tiling.y = _val_tilling_y;
                    m_YTilling.text = pattern0.Tiling.y.ToString("0.00");
                    m_trayMaterials.materials[1].SetTextureScale("_MainTex", pattern0.Tiling);
                }
                else
                {
                    pattern1.Tiling.y = _val_tilling_y;
                    m_YTilling.text = pattern1.Tiling.y.ToString("0.00");
                    m_trayMaterials.materials[0].SetTextureScale("_MainTex", pattern1.Tiling);
                }
            }

            else
            {
                if (currentPattern == 0)
                {
                    pattern0.Tiling.y = 1.0f;
                    m_YTilling.text = pattern0.Tiling.y.ToString("0.00");
                    m_trayMaterials.materials[1].SetTextureScale("_MainTex", pattern0.Tiling);
                }
                else
                {
                    pattern1.Tiling.y = 1.0f;
                    m_YTilling.text = pattern1.Tiling.y.ToString("0.00");
                    m_trayMaterials.materials[0].SetTextureScale("_MainTex", pattern1.Tiling);
                }
            }
        }
    }

    public void SetOffsetX(string _offset_x)
    {
        float _val_offset_x;
        if (float.TryParse(_offset_x, out _val_offset_x)) {
            if (currentPattern == 0)
            {
                pattern0.Offset.x = _val_offset_x;
                m_XOffset.text = pattern0.Offset.x.ToString("0.00");
                m_trayMaterials.materials[1].SetTextureOffset("_MainTex", pattern0.Offset);
            }
            else
            {
                pattern1.Offset.x = _val_offset_x;
                m_XOffset.text = pattern1.Offset.x.ToString("0.00");
                m_trayMaterials.materials[0].SetTextureOffset("_MainTex", pattern1.Offset);
            }
        }
    }

    public void SetOffsetY(string _offset_y)
    {
        float _val_offset_y;
        if (float.TryParse(_offset_y, out _val_offset_y))
        {
            if (currentPattern == 0)
            {
                pattern0.Offset.y = _val_offset_y;
                m_YOffset.text = pattern0.Offset.y.ToString("0.00");
                m_trayMaterials.materials[1].SetTextureOffset("_MainTex", pattern0.Offset);
            }
            else
            {
                pattern1.Offset.y = _val_offset_y;
                m_YOffset.text = pattern1.Offset.y.ToString("0.00");
                m_trayMaterials.materials[0].SetTextureOffset("_MainTex", pattern1.Offset);
            }
        }
    }

    public void RemovePattern()
    {
        m_trayMaterials.materials[(currentPattern == 0) ? 1 : 0].SetTexture("_MainTex", null);
        m_patternPropertiesMenu.SetActive(false);
        if (currentPattern == 0)
        {
            m_InnerPatternImage.sprite = m_nullImage;
            m_trayMaterials.materials[1].color = m_innerColorPicker.CurrentColor;
            pattern0uploaded = false;
            mat0type = MATERIAL_0_TYPE.COLOR;

        }
        else
        {
            m_OutterPatternImage.sprite = m_nullImage;
            m_trayMaterials.materials[0].color = m_outterColorPicker.CurrentColor;
            pattern1uploaded = false;
            mat1type = MATERIAL_1_TYPE.COLOR;
        }
    }

    public void RotatePattern(float _angle)
    {
        if (currentPattern == 0)
        {
            m_trayMaterials.materials[1].SetTexture("_MainTex", RotateTexture(m_InnerPatternImage.sprite.texture, _angle));
        }
        else
        {
            m_trayMaterials.materials[0].SetTexture("_MainTex", RotateTexture(m_OutterPatternImage.sprite.texture, _angle));
        }
    }
    public void RotatePattern(string _angle)
    {
        float _rot_val;
        if(float.TryParse(_angle, out _rot_val))
        {
            if(currentPattern == 0)
            {
                m_trayMaterials.materials[1].SetTexture("_MainTex", RotateTexture(m_InnerPatternImage.sprite.texture, _rot_val));
            }
            else
            {
                m_trayMaterials.materials[0].SetTexture("_MainTex", RotateTexture(m_OutterPatternImage.sprite.texture, _rot_val));
            }
        }
    }

    Texture2D RotateTexture(Texture2D _original, float _angle)
    {
        float sinAng = Mathf.Sin(Mathf.Deg2Rad*_angle);
        float cosAng = Mathf.Cos(Mathf.Deg2Rad*_angle);

        Texture2D newTex = new Texture2D(_original.width, _original.height);
        Texture2D cpyTex = new Texture2D(_original.width, _original.height);
        cpyTex.LoadImage(_original.EncodeToPNG());
        for (int y = 0; y < cpyTex.height; y++)
        {
            for (int x = 0; x < cpyTex.width; x++)
            {
                if (cpyTex.GetPixel(x, y).a == 0.0f)
                {
                    cpyTex.SetPixel(x,y,((currentPattern==0)?m_innerColorBlock.color:m_outterColorBlock.color));
                }
            }
        }
        cpyTex.Apply();
        for (int y = 0; y < newTex.height; y++)
        {
            for(int x = 0; x < newTex.width; x++)
            {
                newTex.SetPixel((int)((x-(newTex.width/2)) * cosAng - (y - (newTex.height / 2)) * sinAng + x), (int)((x - (newTex.width / 2)) * sinAng - (y - (newTex.height / 2)) * cosAng + y), cpyTex.GetPixel(x, y));
            }
        }
        newTex.Apply();

        return newTex;
    }
}

