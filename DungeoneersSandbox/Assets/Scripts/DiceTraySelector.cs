using System;
using System.Security.Permissions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DiceTraySelector : MonoBehaviour
{
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

    public void SaveDiceTray()
    {
        BinaryWriter file = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/active_dice_tray.dss", FileMode.OpenOrCreate));

        file.Write(currentModel);
        
        file.Write(m_trayMaterials.materials[1].color.r);
        file.Write(m_trayMaterials.materials[1].color.g);
        file.Write(m_trayMaterials.materials[1].color.b);
        file.Write(m_trayMaterials.materials[1].color.a);

        file.Write(m_trayMaterials.materials[0].color.r);
        file.Write(m_trayMaterials.materials[0].color.g);
        file.Write(m_trayMaterials.materials[0].color.b);
        file.Write(m_trayMaterials.materials[0].color.a);

        file.Close();

        //m_manager.ChangeScene("DiceSetSelector");
    }
}

