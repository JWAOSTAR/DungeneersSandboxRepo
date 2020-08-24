using System;
using System.Security.Permissions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Forms;

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
    
    int currentModel = 0;
    MATERIAL_0_TYPE mat0type = MATERIAL_0_TYPE.COLOR;
    MATERIAL_1_TYPE mat1type = MATERIAL_1_TYPE.COLOR;

    // Start is called before the first frame update
    void Start()
    {
        m_trayMover.SetMobility(false);
        m_currentMesh.mesh = m_meshOptions[currentModel];
        m_innerColorPicker.gameObject.SetActive(false);
        m_outterColorPicker.gameObject.SetActive(false);
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
        m_trayMaterials.materials[1].color = _color;
        m_innerColorBlock.color = _color;
    }

    public void SetOutterColor(Color _color)
    {
        m_trayMaterials.materials[0].color = _color;
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

                        mat0type = MATERIAL_0_TYPE.TILE;
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

                        mat1type = MATERIAL_1_TYPE.TILE;
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
                    //TODO: Add in file writting when shader is written
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
                    //TODO: Add in file writting when shader is written
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
        BinaryWriter file = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/active_dice_tray.dst", FileMode.OpenOrCreate));

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
                    //TODO: Add in file writting when shader is written
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
                    //TODO: Add in file writting when shader is written
                }
                break;
            default:
                break;
        }

        file.Close();

        //m_manager.ChangeScene("DiceSetSelector");
    }

    public void LoadDiceTray()
    {

    }
}

