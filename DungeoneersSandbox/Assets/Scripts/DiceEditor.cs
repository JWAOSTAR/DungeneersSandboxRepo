using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEditor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    MeshFilter currentModel;
    [SerializeField]
    MeshRenderer material;
    [SerializeField]
    MeshCollider colider;
    [SerializeField]
    Dice.DiceType currentDiceType;
    [SerializeField]
    Mesh[] diceModels = new Mesh[7];
    [SerializeField]
    DiceAxisMovement diceMover;
    [SerializeField]
    Sprite brush;
    [SerializeField]
    EditorToolPanel toolPanel;

    String currentFilePath;

    private bool m_paintable = true;

    void Start()
    {
        if (File.Exists("./player_profile/temp/die_to_paint.dstd"))
        {
            BinaryReader file = new BinaryReader(File.Open("./player_profile/temp/die_to_paint.dstd", FileMode.Open));
            currentFilePath = file.ReadString();
            file = new BinaryReader(File.Open(currentFilePath, FileMode.Open));
            file.ReadBoolean();
            currentDiceType = (Dice.DiceType)file.ReadInt32();
            colider.sharedMesh = currentModel.mesh = diceModels[(int)currentDiceType];
            String imageName = currentFilePath.Split('/')[currentFilePath.Split('/').Length - 1];
            imageName = imageName.Replace(".dsd", ".png");
            Texture2D newTex = new Texture2D(512, 512);
            newTex.LoadImage(File.ReadAllBytes("./player_profile/temp/" + imageName));
            for (int i = 0; i < material.materials.Length; i++)
            {
               material.materials[i].SetTexture("_MainTex", newTex);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!diceMover.GetMobility() && Input.GetKey(KeyCode.LeftShift))
        {
            diceMover.SetMobility(true);
            m_paintable = false;
        }
        else if(diceMover.GetMobility() && !Input.GetKey(KeyCode.LeftShift))
        {
            diceMover.SetMobility(false);
            m_paintable = true;
        }

        if(m_paintable && Input.GetMouseButton(0))
        {
            Vector3 _toSend = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(_toSend);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                StartCoroutine(DrawBrushOnMesh(_toSend));
                Debug.Log("uv(" + hit.textureCoord.x.ToString("0.00") + ", " + hit.textureCoord.y.ToString("0.00") + ")");
            }
            else
            {
                Debug.Log("Not on top of object");
            }
        }
    }

    private void OnMouseDrag()
    {
        if (m_paintable && Input.GetMouseButton(0))
        {
            Vector3 _toSend = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(_toSend);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                StartCoroutine(DrawBrushOnMesh(_toSend));
                Debug.Log("uv(" + hit.textureCoord.x.ToString("0.00") + ", " + hit.textureCoord.y.ToString("0.00") + ")");
            }
            else
            {
                Debug.Log("Not on top of object");
            }
        }
    }

    private IEnumerator DrawBrushOnMesh(Vector3 vec)
    {
        for(int y = -5; y < 5; y++)
        {
            for(int x = -5; x < 5; x++)
            {
                Ray ray = Camera.main.ScreenPointToRay(vec + new Vector3(x, y, 0.0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //StartCoroutine(PaintPointOnTexture(hit.textureCoord.x, hit.textureCoord.y));
                    for (int i = 0; i < material.materials.Length; i++)
                    {
                        ((Texture2D)(material.materials[i].mainTexture)).SetPixel((int)(hit.textureCoord.x * material.materials[i].mainTexture.width), (int)(hit.textureCoord.y * material.materials[i].mainTexture.height), toolPanel.PrimaryColor);
                        
                    }
                }
            }
        }
        for (int j = 0; j < material.materials.Length; j++)
        {
            ((Texture2D)(material.materials[j].mainTexture)).Apply();
        }
        yield return null;
    }

    private IEnumerator PaintPointOnTexture(float x, float y)
    {
        //if (lastMousePos.x >= 0.0f) {
            for (int i = 0; i < material.materials.Length; i++)
            {
                ((Texture2D)(material.materials[i].mainTexture)).SetPixel((int)(x * material.materials[i].mainTexture.width), (int)(y * material.materials[i].mainTexture.height), toolPanel.PrimaryColor);
                ((Texture2D)(material.materials[i].mainTexture)).Apply();
            }
        //}
        //else
        //{
        //    RaycastHit hit;
        //    Ray ray;
        //    for (int j = 0; j < 10; j++)
        //    {
        //        ray = Camera.main.ScreenPointToRay(new Vector3(Mathf.Lerp(lastMousePos.x, Input.mousePosition.x, j / 10.0f), Mathf.Lerp(lastMousePos.y, Input.mousePosition.y, j / 10.0f), Mathf.Lerp(lastMousePos.z, Input.mousePosition.z, j / 10.0f)));
        //        Physics.Raycast(ray, out hit);
        //        for (int i = 0; i < material.materials.Length; i++)
        //        {
        //            ((Texture2D)(material.materials[i].mainTexture)).SetPixel((int)(hit.textureCoord.x * material.materials[i].mainTexture.width), (int)(hit.textureCoord.y * material.materials[i].mainTexture.height), new Color(0.0f, 1.0f, 1.0f, 1.0f));
        //        }
        //    }
        //    for (int k = 0; k < material.materials.Length; k++)
        //    {
        //        ((Texture2D)(material.materials[k].mainTexture)).Apply();
        //    }
        //}
        yield return null; 
    }
}
