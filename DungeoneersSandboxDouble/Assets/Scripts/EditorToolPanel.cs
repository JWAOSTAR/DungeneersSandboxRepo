using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorToolPanel : MonoBehaviour
{
    public enum ToolType
    {
        Brush = 0,
        Bucket,
        EyeDropper
    }

    [SerializeField]
    Image m_primaryPanel;
    [SerializeField]
    ColorPicker m_primaryColorPicker;
    [Space]
    [SerializeField]
    Image m_secondaryPanel;
    [SerializeField]
    ColorPicker m_secondaryColorPicker;
    ToolType m_tool;
    [SerializeField]
    Texture2D[] m_cursors;

    /// <summary>
    /// The primary color for the tool editor's panel
    /// </summary>
    public Color PrimaryColor { get { return m_primaryPanel.color; } }
    /// <summary>
    /// The secondary color for the tool editor's panel
    /// </summary>
    public Color SecondaryColor { get { return m_secondaryPanel.color; } }
    /// <summary>
    /// The current tool of the tool editor
    /// </summary>
    public ToolType CurrentTool { get { return m_tool; } }

    // Start is called before the first frame update
    void Start()
    {
        m_primaryColorPicker.gameObject.SetActive(false);
        m_secondaryColorPicker.gameObject.SetActive(false);
        m_tool = ToolType.Brush;
        Cursor.SetCursor(m_cursors[0], new Vector2(8.0f, 24.0f), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_tool == ToolType.EyeDropper && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(EyeDropperClicked());  
        }
    }

    /// <summary>
    /// Set the default values for the colors and color panel
    /// </summary>
    public void SetPanelsToDefault()
    {
        m_primaryPanel.color = Color.black;
        m_secondaryPanel.color = Color.white;

        m_primaryColorPicker.SetCurrentColor(Color.black, true);
        m_secondaryColorPicker.SetCurrentColor(Color.white, true);

    }

    /// <summary>
    /// Swap the primery and secondary color values
    /// </summary>
    public void SwitchColors()
    {
        Color temp_color = m_primaryPanel.color;
        m_primaryPanel.color = m_secondaryPanel.color;
        m_secondaryPanel.color = temp_color;

        m_primaryColorPicker.SetCurrentColor(m_primaryPanel.color, true);
        m_secondaryColorPicker.SetCurrentColor(m_secondaryPanel.color, true);
    }

    /// <summary>
    /// Set the color value of the primary color
    /// </summary>
    /// <param name="_color">Color to set the primary color to</param>
    public void SetPrimaryColor(Color _color)
    {
        m_primaryPanel.color = _color;
        m_primaryColorPicker.SetCurrentColor(m_primaryPanel.color, true);
    }

    /// <summary>
    /// Set the color value of the primary color
    /// </summary>
    /// <param name="_r">Float value for the red channel of the primary color</param>
    /// <param name="_g">Float value for the green channel of the primary color</param>
    /// <param name="_b">Float value for the blue channel of the primary color</param>
    /// <param name="_a">Float value for the alpha channel of the primary color</param>
    public void SetPrimaryColor(float _r, float _g, float _b, float _a)
    {
        m_primaryPanel.color = new Color(_r, _g, _b, _a);
    }

    /// <summary>
    /// Set the color value of the secondary color
    /// </summary>
    /// <param name="_color"></param>
    public void SetSecondaryColor(Color _color)
    {
        m_secondaryPanel.color = _color;
        m_secondaryColorPicker.SetCurrentColor(m_secondaryPanel.color, true);
    }

    /// <summary>
    /// Set the color value of the secondary color
    /// </summary>
    /// <param name="_r">Float value for the red channel of the secondary color</param>
    /// <param name="_g">Float value for the green channel of the secondary color</param>
    /// <param name="_b">Float value for the blue channel of the secondary color</param>
    /// <param name="_a">Float value for the alpha channel of the secondary color</param>
    public void SetSecondaryColor(float _r, float _g, float _b, float _a)
    {
        m_secondaryPanel.color = new Color(_r, _g, _b, _a);
    }

    /// <summary>
    /// Set the current tool of the editor tool's panel
    /// </summary>
    /// <param name="_tool">Integer representing the enum value of the ToolType enum</param>
    public void SetTool(int _tool)
    {
        m_tool = (ToolType)_tool;
        Cursor.SetCursor(m_cursors[_tool], ((_tool == 0) ? new Vector2(8.0f, 24.0f) : new Vector2(0.0f, 0.0f)), CursorMode.ForceSoftware);
    }

    /// <summary>
    /// A parallel process used when the eye droper is clicked
    /// </summary>
    IEnumerator EyeDropperClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Texture2D _newText = new Texture2D((hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture).width, (hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture).height);
            RenderTexture.active = (RenderTexture)(hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture);
            _newText.ReadPixels(new Rect(0, 0, _newText.width, _newText.height), 0, 0);
            _newText.Apply();
            SetPrimaryColor(_newText.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y));
        }
        else
        {
            Texture2D capture = ScreenCapture.CaptureScreenshotAsTexture();
            SetPrimaryColor(capture.GetPixelBilinear(Input.mousePosition.x / (float)capture.width, Input.mousePosition.y / (float)capture.height));
        }
        yield return new WaitForSeconds(0.5f);
        m_tool = ToolType.Brush;
    }
}
