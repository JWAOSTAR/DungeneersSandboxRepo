using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceEditor : MonoBehaviour
{
    // Start is called before the first frame update

    private class Stroke
    {
        public Queue<Vector3> points;
        public List<Vector3> line_points;
        public float brushSize;
        LineRenderer trail;
        public Stroke()
        {
            points = new Queue<Vector3>();
            trail = new LineRenderer();
            line_points = new List<Vector3>();
            brushSize = 1.0f;
        }

        public void GenerateTrail(LineRenderer _line)
        {
            trail = Instantiate(_line);
            trail.sortingLayerName = "FrontFacing";
            trail.positionCount = line_points.Count;
            trail.SetPositions(line_points.ToArray());
        }

        public void DestroyLine()
        {
            Destroy(trail.gameObject);
        }
    }

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
    Brush brush;
    [SerializeField]
    EditorToolPanel toolPanel;
    [SerializeField]
    InputField m_saveDialog;
    [SerializeField]
    LineRenderer m_instnace_line;
    [SerializeField]
    GameObject m_notification_box;
    [SerializeField]
    SceneChanger m_manager;

    String currentFilePath;
    List<Texture2D> step_back_stack = new List<Texture2D>();
    List<Texture2D> step_forward_stack = new List<Texture2D>();
    bool in_step = false;
    Queue<Stroke> m_strokes_to_process = new Queue<Stroke>();

    private bool m_paintable = true;
    bool CR_LINE_GEN = false;

    void Start()
    {
        step_back_stack.Capacity = 5;
        step_forward_stack.Capacity = 5;
        m_strokes_to_process.Enqueue(new Stroke());
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
            for (int i = 0; i < material.materials.Length; i++)
            {
               material.materials[i].SetTexture("_MainTex", newTex);
            }
        }
        Texture2D _newText = new Texture2D(material.materials[0].mainTexture.width, material.materials[0].mainTexture.height);
        _newText.LoadImage(((Texture2D)(material.materials[0].mainTexture)).EncodeToPNG());
        step_back_stack.Insert(0, _newText);
    }

    // Update is called once per frame
    void Update()
    {
        if(!diceMover.GetMobility() && Input.GetKey(KeyCode.LeftShift))
        {
            if (!CR_LINE_GEN)
            {
                diceMover.SetMobility(true);
            }
            else
            {
                StartCoroutine(DisplayNotification());
            }
            m_paintable = false;
        }
        else if(diceMover.GetMobility() && !Input.GetKey(KeyCode.LeftShift))
        {
            if (diceMover.GetMobility()) {
                diceMover.SetMobility(false);
            }
            m_paintable = true;
        }
        else if(!diceMover.GetMobility() && !Input.GetKey(KeyCode.LeftShift))
        {
            m_paintable = true;
        }

        if(m_paintable && Input.GetMouseButton(0))
        {
            Vector3 _toSend = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(_toSend);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if (!m_strokes_to_process.Peek().points.Contains(_toSend))
                {
                    if(m_strokes_to_process.Peek().points.Count == 0)
                    {
                        m_strokes_to_process.Peek().brushSize = brush.Size;
                    }
                    m_strokes_to_process.Peek().points.Enqueue(_toSend);
                    m_strokes_to_process.Peek().line_points.Add(hit.point + new Vector3(0.0f, 0.0f, -0.01f)); ;
                }

                Debug.Log("uv(" + hit.textureCoord.x.ToString("0.00") + ", " + hit.textureCoord.y.ToString("0.00") + ")");
                if (!in_step)
                {
                    in_step = true;
                }
            }
            else
            {
                Debug.Log("Not on top of object");
            }

        }

        if(m_paintable && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Z))
        {
            StepBackwards();
        }

        if (m_paintable && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Y))
        {
            StepForwards();
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            for(int i = 0; i < step_back_stack.Count; i++)
            {
                BinaryWriter file = new BinaryWriter(File.Open("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/temp/step_back_" + i.ToString() + ".png", FileMode.OpenOrCreate));
                file.Write(step_back_stack[i].EncodeToPNG());
                file.Close();
            }
        }

        if (in_step && !Input.GetMouseButton(0))
        {
            m_strokes_to_process.Peek().GenerateTrail(m_instnace_line);
            switch (toolPanel.CurrentTool)
            {
                case EditorToolPanel.ToolType.Brush:
                    StartCoroutine(DrawQueueedStroksOnMesh());
                    break;
                case EditorToolPanel.ToolType.Bucket:
                    StartCoroutine(DrawFillBucketOnMesh());
                    break;
                default:
                    break;
            }
            
            m_strokes_to_process.Enqueue(new Stroke());
            in_step = false;
        }
    }

    private void TakeSnapshoot()
    {
        Texture2D _newText = new Texture2D(material.materials[0].mainTexture.width, material.materials[0].mainTexture.height);
        _newText.LoadImage(((Texture2D)(material.materials[0].mainTexture)).EncodeToPNG());
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
                material.materials[i].SetTexture("_MainTex", _newText);
            }

            step_forward_stack.RemoveAt(0);
        }
    }

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
                material.materials[i].SetTexture("_MainTex", _newText);
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
                if (!m_strokes_to_process.Peek().points.Contains(_toSend)) {
                    if (m_strokes_to_process.Peek().points.Count == 0)
                    {
                        m_strokes_to_process.Peek().brushSize = brush.Size;
                    }
                    m_strokes_to_process.Peek().points.Enqueue(_toSend);
                    m_strokes_to_process.Peek().line_points.Add(hit.point);
                }

                Debug.Log("uv(" + hit.textureCoord.x.ToString("0.00") + ", " + hit.textureCoord.y.ToString("0.00") + ")");
                if (!in_step)
                {
                    in_step = true;
                }
            }
            else
            {
                Debug.Log("Not on top of object");
            }
        }
    }

    private IEnumerator DrawQueueedStroksOnMesh()
    {
        for (int i = m_strokes_to_process.Count; i > 0; i--) { 
            Stroke cur_stroke = m_strokes_to_process.Dequeue();
            for (int l = cur_stroke.points.Count; l > 0; l--)
            {
                for (float y = -(cur_stroke.brushSize * 0.5f); y < (cur_stroke.brushSize * 0.5f); y++)
                {
                    for (float x = -(cur_stroke.brushSize * 0.5f); x < (cur_stroke.brushSize * 0.5f); x++)
                    {
                        CR_LINE_GEN = true;
                        Ray ray = Camera.main.ScreenPointToRay(cur_stroke.points.Peek() + new Vector3(x, y, 0.0f));
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {

                            bool draw = (brush.Square) ? true : (Mathf.Sqrt(((cur_stroke.points.Peek().x - (x + cur_stroke.points.Peek().x)) * (cur_stroke.points.Peek().x - (x + cur_stroke.points.Peek().x))) + ((cur_stroke.points.Peek().y - (y + cur_stroke.points.Peek().y)) * (cur_stroke.points.Peek().y - (y + cur_stroke.points.Peek().y)))) <= (cur_stroke.brushSize * 0.5f));
                            //StartCoroutine(PaintPointOnTexture(hit.textureCoord.x, hit.textureCoord.y));
                            if (draw)
                            {
                                //Debug.Log("(" + x.ToString() + ", " + y.ToString() + ") Printing");
                                for (int j = 0; j < material.materials.Length; j++)
                                {
                                    ((Texture2D)(material.materials[j].mainTexture)).SetPixel((int)(hit.textureCoord.x * material.materials[j].mainTexture.width), (int)(hit.textureCoord.y * material.materials[j].mainTexture.height), toolPanel.PrimaryColor);

                                }
                            }
                        }
                    }
                    yield return null;
                }
                cur_stroke.points.Dequeue();
            }
            cur_stroke.DestroyLine();
            for (int k = 0; k < material.materials.Length; k++)
            {
                ((Texture2D)(material.materials[k].mainTexture)).Apply();
            }
            CR_LINE_GEN = false;
            TakeSnapshoot();
        }
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

    private IEnumerator DrawFillBucketOnMesh()
    {
        for(int y = 0; y < material.materials[0].mainTexture.height; y++)
        {
            for(int x = 0; x < material.materials[0].mainTexture.width; x++)
            {
                if (((Texture2D)(material.materials[0].mainTexture)).GetPixel(x, y).a != 0.0f) 
                {
                    ((Texture2D)(material.materials[0].mainTexture)).SetPixel(x, y, toolPanel.PrimaryColor);
                }
            }
        }

        ((Texture2D)(material.materials[0].mainTexture)).Apply();

        for (int i = 1; i < material.materials.Length; i++)
        {
            Texture2D _newTex = new Texture2D(material.materials[0].mainTexture.width, material.materials[0].mainTexture.height);
            _newTex.LoadImage(((Texture2D)(material.materials[0].mainTexture)).EncodeToPNG());

            material.materials[i].SetTexture("_MainTex", _newTex);
        }
        TakeSnapshoot();
        yield return null;
    }

    private IEnumerator DisplayNotification()
    {
        m_notification_box.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        m_notification_box.SetActive(false);
    }

    public void SaveFile()
    {
        if(currentFilePath.Split('/')[currentFilePath.Split('/').Length - 1].Contains("untitled"))
        {
            if (m_saveDialog.text == "untitled") {
                m_saveDialog.transform.parent.gameObject.SetActive(true);
                return;
            }
            else
            {
                currentFilePath = currentFilePath.Replace("untitled", m_saveDialog.text);
            }
        }
        if(currentFilePath.Split('/')[6] == "temp")
        {
            currentFilePath = currentFilePath.Replace("temp", "dice/skins");
        }
        BinaryWriter file = new BinaryWriter(File.Open(currentFilePath, FileMode.OpenOrCreate));
        
        file.Write(true);
        file.Write((int)currentDiceType);
        file.Write(((Texture2D)(material.materials[0].mainTexture)).EncodeToPNG().Length);
        file.Write(material.materials[0].mainTexture.width);
        file.Write(material.materials[0].mainTexture.height);
        file.Write(((Texture2D)(material.materials[0].mainTexture)).EncodeToPNG());

        file.Close();

        m_manager.ChangeScene("Main");
    }
}
