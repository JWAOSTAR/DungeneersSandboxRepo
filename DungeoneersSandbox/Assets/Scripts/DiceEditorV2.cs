using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

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

    String currentFilePath;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        if (File.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/die_to_paint.dstd"))
        {
            BinaryReader file = new BinaryReader(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/die_to_paint.dstd", FileMode.Open));
            currentFilePath = file.ReadString();
            file = new BinaryReader(File.Open(currentFilePath, FileMode.Open));
            file.ReadBoolean();
            currentDiceType = (Dice.DiceType)file.ReadInt32();
            colider.sharedMesh = currentModel.mesh = diceModels[(int)currentDiceType];
            String imageName = currentFilePath.Split('/')[currentFilePath.Split('/').Length - 1];
            imageName = imageName.Replace(".dsd", ".png");
            Texture2D newTex = new Texture2D(512, 512);
            newTex.LoadImage(File.ReadAllBytes("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/" + imageName));

            originalTexture = newTex;
            //originalTexture.width = newTex.width;
            //originalTexture.height = newTex.height;
            //((Texture2D)originalTexture).LoadImage(File.ReadAllBytes("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/" + imageName));

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
        if (!diceMover.GetMobility() && Input.GetKey(KeyCode.LeftShift))
        {
            diceMover.SetMobility(true);
        }
        else if(diceMover.GetMobility() && !Input.GetKey(KeyCode.LeftShift))
        {
            diceMover.SetMobility(false);
        }

        if(numFrams > 2)
        {
            cam.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, m_commandBuffer);
        }

        numFrams++;

        albedo.UpdateShaderParams(currentModel.gameObject.transform.localToWorldMatrix);

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector4 mouseWorldPos = Vector3.positiveInfinity;

        if(!diceMover.GetMobility() && Input.GetMouseButton(0))
        {
            if(Physics.Raycast(ray, out hit))
            {
                mouseWorldPos = hit.point;
            }
            mouseWorldPos.w = 1.0f;
        }
        else
        {
            mouseWorldPos.w = 0.0f;
        }

        mousePos = mouseWorldPos;
        Shader.SetGlobalVector("_Mouse", mouseWorldPos);

        Shader.SetGlobalColor("_BrushColor", toolPanel.PrimaryColor);
        Shader.SetGlobalFloat("_BrushOpacity", toolPanel.PrimaryColor.a);
        Shader.SetGlobalFloat("_BrushHardness", brush.Hardness);
        Shader.SetGlobalFloat("_BrushSize", brush.Size*0.004f);
    }
}
