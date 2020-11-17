using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using System.Windows.Forms;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class DiceEditorV2 : MonoBehaviour
{
    //[SerializeField]
    Texture originalTexture;
    [SerializeField]
    Shader texturePaintShader;
    [SerializeField]
    Shader iShader;
    [SerializeField]
    Shader oShader;
    public static Vector3 mousePos;
    Camera cam;
    int clearTexture;
    RenderTexture m_renderTarget;
    CommandBuffer m_commandBuffer;
    int numFrams;
    Material edgeMat;

    TexturePaintable albedo;

    [SerializeField]
    MeshCollider colider;
    [SerializeField]
    Dice.DiceType currentDiceType;
    [SerializeField]
    MeshFilter currentModel;
    [SerializeField]
    Mesh[] diceModels = new Mesh[7];
    [SerializeField]
    MeshRenderer material;
    [SerializeField]
    Brush brush;
    [SerializeField]
    EditorToolPanel toolPanel;
    [SerializeField]
    DiceAxisMovement diceMover;
    [SerializeField]
    InputField m_saveDialog;
    [SerializeField]
    SceneChanger m_manager;
    [SerializeField]
    Slider m_brushSizeSlider;
    [SerializeField]
    Slider m_brushSoftnessSlider;
    [SerializeField]
    Toggle m_circleToggle;
    [SerializeField]
    Toggle m_squareToggle;

    String currentFilePath;
    int in_step = 0;
    List<Texture2D> step_back_stack = new List<Texture2D>();
    List<Texture2D> step_forward_stack = new List<Texture2D>();

    bool savePanelOpen = false;
    //int numFramsDown = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Set up base variables
        cam = Camera.main;
        step_back_stack.Capacity = 5;
        step_forward_stack.Capacity = 5;
        m_brushSizeSlider.SetValueWithoutNotify(brush.Size);
        m_brushSoftnessSlider.SetValueWithoutNotify(brush.Hardness);
        m_circleToggle.SetIsOnWithoutNotify(!brush.Square);
        m_squareToggle.SetIsOnWithoutNotify(brush.Square);
#if (UNITY_ANDROID && !UNITY_EDITOR)
        Input.simulateMouseWithTouches = true;
#endif
        //Get path to where the dice to paint file is located
        string path;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/";
#elif (UNITY_ANDROID && !UNITY_EDITOR)
        path = UnityEngine.Application.persistentDataPath + "/DungeoneersSamdbox/temp/";
#endif
        //Read in file
        if (File.Exists(path + "die_to_paint.dstd"))
        {
            BinaryReader file = new BinaryReader(File.Open(path +"die_to_paint.dstd", FileMode.Open));
            currentFilePath = file.ReadString();
            file = new BinaryReader(File.Open(currentFilePath, FileMode.Open));
            file.ReadBoolean();
            currentDiceType = (Dice.DiceType)file.ReadInt32();
            colider.sharedMesh = currentModel.mesh = diceModels[(int)currentDiceType];
            String imageName = currentFilePath.Split('/')[currentFilePath.Split('/').Length - 1];
            imageName = imageName.Replace(".dsd", ".png");
            Texture2D newTex = new Texture2D(512, 512);
            newTex.LoadImage(File.ReadAllBytes(path + imageName));

            originalTexture = newTex;

            Texture2D _tex = new Texture2D(originalTexture.width, originalTexture.height);
            _tex.LoadImage(((Texture2D)originalTexture).EncodeToPNG());
            step_back_stack.Insert(0, _tex);

            m_renderTarget = new RenderTexture(originalTexture.width, originalTexture.height, 0, RenderTextureFormat.R8);

            albedo = new TexturePaintable(newTex, originalTexture.width, originalTexture.height, "_MainTex", texturePaintShader, currentModel.mesh, oShader, m_renderTarget);

            for (int i = 0; i < material.materials.Length; i++)
            {
                material.materials[i].SetTexture(albedo.id, albedo.runTimeTexture);
            }

            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.name = "dicePainter";

            m_commandBuffer.SetRenderTarget(m_renderTarget);
            Material m_diceMat = new Material(iShader);
            m_commandBuffer.DrawMesh(currentModel.mesh, Matrix4x4.identity, m_diceMat);
            cam.AddCommandBuffer(CameraEvent.AfterDepthTexture, m_commandBuffer);

            albedo.SetActiveTexture(cam);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if shift is clicked to change the state of mobility
        if (!diceMover.GetMobility() && Input.GetKey(KeyCode.LeftShift))
        {
            diceMover.SetMobility(true);
        }
        else if(diceMover.GetMobility() && !Input.GetKey(KeyCode.LeftShift))
        {
            diceMover.SetMobility(false);
        }

        //Refresh command buffer on second frame
        if(numFrams > 2)
        {
            cam.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, m_commandBuffer);
        }

        //Incrument frame
        numFrams++;

        albedo.UpdateShaderParams(currentModel.gameObject.transform.localToWorldMatrix);

        
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector4 mouseWorldPos = Vector3.positiveInfinity;
        //Check for mouse click when mobility of on
        if(!savePanelOpen && !diceMover.GetMobility() && (Input.GetMouseButton(0)))
        {
            if(Physics.Raycast(ray, out hit))
            {
                mouseWorldPos = hit.point;
                in_step = 1;

                switch (toolPanel.CurrentTool)
                {
                    case EditorToolPanel.ToolType.Brush:
                        {
                            //numFramsDown++;
                            //if (numFramsDown%6 == 0) 
                            //{
                                mouseWorldPos.w = 1.0f;
                            //}
                            //else
                            //{
                            //    mouseWorldPos.w = 0.0f;
                            //}
                        }
                        break;
                    case EditorToolPanel.ToolType.Bucket:
                        {
                            mouseWorldPos.w = 0.0f;
                            Graphics.SetRenderTarget(albedo.runTimeTexture);
                            GL.Clear(false, true, toolPanel.PrimaryColor);
                            Graphics.SetRenderTarget(albedo.outputTexture);
                            GL.Clear(false, true, toolPanel.PrimaryColor);
                        }
                        break;
                    default:
                        mouseWorldPos.w = 0.0f;
                        break;
                }
            }
        }
        else
        {
            //Incrument the state a brush stroke
            //numFramsDown = 0;
            mouseWorldPos.w = 0.0f;
            if (in_step == 1)
            {
                in_step = 2;
            }
        }
        //Reset for the next brush stroke
        if(in_step == 2)
        {
            TakeSnapshot();
            in_step = 0;
        }
        //Send values to shader
        mousePos = mouseWorldPos;
        Shader.SetGlobalVector("_Mouse", mouseWorldPos);

        Shader.SetGlobalColor("_BrushColor", toolPanel.PrimaryColor);
        Shader.SetGlobalFloat("_BrushOpacity", toolPanel.PrimaryColor.a);
        Shader.SetGlobalFloat("_BrushHardness", brush.Hardness);
        Shader.SetGlobalFloat("_BrushSize", brush.Size*0.004f);
        Shader.SetGlobalInt("_isSquare", (brush.Square) ? 1 : 0);
    }

    /// <summary>
    /// //SaveDialogOpen setes the boolean that tells weather or not the save dialog is open
    /// </summary>
    /// <param name="_open">Boolean value of the bool savePanelOpen variable</param>
    public void SaveDialogOpen(bool _open)
    {
        savePanelOpen = _open;
    }

    /// <summary>
    /// SaveFile saves the current version of the dice properties to a .dsd(DS Dice file) and save it to the game's directory
    /// </summary>
    public void SaveFile()
    {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        if (currentFilePath.Split('/')[currentFilePath.Split('/').Length - 1].Contains("untitled"))
        {
            if (m_saveDialog.text == "untitled")
            {
                m_saveDialog.transform.parent.gameObject.SetActive(true);
                savePanelOpen = true;
                return;
            }
            else
            {
                currentFilePath = currentFilePath.Replace("untitled", m_saveDialog.text);
            }
        }
        if (currentFilePath.Split('/')[6] == "temp")
        {
            currentFilePath = currentFilePath.Replace("temp", "dice/skins");
        }
        BinaryWriter file = new BinaryWriter(File.Open(currentFilePath, FileMode.OpenOrCreate));

        file.Write(true);
        file.Write((int)currentDiceType);

        Texture2D _newText = new Texture2D(material.materials[0].mainTexture.width, material.materials[0].mainTexture.height);
        RenderTexture.active = albedo.runTimeTexture;
        _newText.ReadPixels(new Rect(0, 0, albedo.runTimeTexture.width, albedo.runTimeTexture.height), 0, 0);
        _newText.Apply();

        file.Write(_newText.EncodeToPNG().Length);
        file.Write(albedo.runTimeTexture.width);
        file.Write(albedo.runTimeTexture.height);
        file.Write(_newText.EncodeToPNG());

        file.Close();
#endif
        //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        //saveFileDialog1.InitialDirectory = "C:/Users/" + Environment.UserName + "/Documents/";
        //saveFileDialog1.Filter = "DS Dice files (*.dsd)|*.dsd";
        //saveFileDialog1.FilterIndex = 0;
        //saveFileDialog1.RestoreDirectory = false;
        //saveFileDialog1.FileName = m_saveDialog.text + ".dsd";

        //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        //{
        //    file = new BinaryWriter(saveFileDialog1.OpenFile());
        //    file.Write(true);
        //    file.Write((int)currentDiceType);
        //    file.Write(_newText.EncodeToPNG().Length);
        //    file.Write(albedo.runTimeTexture.width);
        //    file.Write(albedo.runTimeTexture.height);
        //    file.Write(_newText.EncodeToPNG());
        //    file.Close();
        //}
        UnityEngine.Cursor.SetCursor(null, new Vector2(0.0f, 0.0f), CursorMode.Auto);
        m_manager.ChangeScene("Main");
    }

    /// <summary>
    /// Loads in and textures the next snapshot taken from the current one on the die
    /// </summary>
    public void StepForwards()
    {
        if (step_forward_stack.Count > 0)
        {
            Texture2D _oldText = new Texture2D(step_forward_stack[0].width, step_forward_stack[0].height);
            _oldText.LoadImage(step_forward_stack[0].EncodeToPNG());
            step_back_stack.Insert(0, _oldText);

            for (int i = 0; i < material.materials.Length; i++)
            {
                Texture2D _newText = new Texture2D(step_forward_stack[0].width, step_forward_stack[0].height);
                _newText.LoadImage(step_forward_stack[0].EncodeToPNG());
                //material.materials[i].SetTexture("_MainTex", _newText);
                Graphics.Blit(_newText, albedo.runTimeTexture);
                Graphics.Blit(_newText, albedo.outputTexture);
            }

            step_forward_stack.RemoveAt(0);
        }
    }

    /// <summary>
    /// Loads in and textures the last snapshot taken from the current one on the die
    /// </summary>
    public void StepBackwards()
    {
        if (step_back_stack.Count > 1)
        {
            Texture2D _oldText = new Texture2D(step_back_stack[0].width, step_back_stack[0].height);
            _oldText.LoadImage(step_back_stack[0].EncodeToPNG());
            step_forward_stack.Insert(0, _oldText);

            step_back_stack.RemoveAt(0);

            for (int i = 0; i < material.materials.Length; i++)
            {
                Texture2D _newText = new Texture2D(step_back_stack[0].width, step_back_stack[0].height);
                _newText.LoadImage(step_back_stack[0].EncodeToPNG());
                //material.materials[i].SetTexture("_MainTex", _newText);
                Graphics.Blit(_newText, albedo.runTimeTexture);
                Graphics.Blit(_newText, albedo.outputTexture);
            }
        }
    }

    /// <summary>
    /// TakeSnapshot saves a snapshot of the texture on the die before the next brush stroke is finished
    /// </summary>
    private void TakeSnapshot()
    {
        Texture2D _newText = new Texture2D(material.materials[0].mainTexture.width, material.materials[0].mainTexture.height);
        RenderTexture.active = albedo.runTimeTexture;
        _newText.ReadPixels(new Rect(0, 0, albedo.runTimeTexture.width, albedo.runTimeTexture.height), 0, 0);
        _newText.Apply();
        step_back_stack.Insert(0, _newText);
        if (step_back_stack.Count > 5)
        {
            step_back_stack.RemoveAt(step_back_stack.Count - 1);
        }
        if (step_forward_stack.Count > 0)
        {
            step_forward_stack.Clear();
        }
    }

    /// <summary>
    /// Cancel is called when the scene has exited
    /// </summary>
    public void Cancel()
    {
        UnityEngine.Cursor.SetCursor(null, new Vector2(0.0f, 0.0f), CursorMode.Auto);
    }
}
