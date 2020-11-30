using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


[Serializable]
public class ColorPickerColorEvent : UnityEvent<Color> { }
[Serializable]
public class ColorPickerEvent : UnityEvent { }
public class ColorPicker : MonoBehaviour
{
    //Components of a color slider bar
    public struct ColorSlider
    {
        public Image bar;
        public RectTransform handle;
        public RectTransform initialHandleTransform;
    };

    Color originColor;
    Color defaultColor;
    [SerializeField]
    Color currentColor;
    public Color CurrentColor { get { return currentColor;} }
    [SerializeField]
    Image previousColorPanel = null;
    [SerializeField]
    Image currentColorPanel = null;
    [Space]
    [Header("Color Box")]
    public Image colorBlock;
    public RectTransform colorBlockHandle;
    RectTransform initialColorBlockHandlePosition;
    ColorSlider colorBlockSlider;
    ColorSlider[] colorSlider = new ColorSlider[4];
    public Slider2D colorBoxSlider;
    [Space]
    [Header("Color Bars")]
    public Image colorHueBar;
    public RectTransform colorHueHandle;
    public Slider colorHueSlider;
    [Space]
    public Image[] colorBars;
    [Space]
    public RectTransform[] colorHandles;
    [Space]
    public Slider[] colorSliders;
    [Space]
    public InputField[] colorInputFields = new InputField[4];
    Color[] hues = { Color.red, new Color(1.0f, 0.0f, 1.0f, 1.0f) ,Color.blue, new Color(0.0f, 1.0f, 1.0f, 1.0f), Color.green, new Color(1.0f, 1.0f, 0.0f, 1.0f), Color.red };
    bool DontUpdate = false;
    [Space]
    [SerializeField]
    Text m_coordinates;

    [Space]

    [SerializeField]
    bool m_hideOnConfermationOrCancelation = false;

    [Space]

    [SerializeField]
    private ColorPickerColorEvent m_onEnter = new ColorPickerColorEvent();
    [SerializeField]
    private ColorPickerEvent m_onEscape = new ColorPickerEvent();
    public ColorPickerColorEvent onEnter { get { return m_onEnter; } set { m_onEnter = value; } }
    public ColorPickerEvent onEscape { get { return m_onEscape; } set { m_onEscape = value; } }
    protected ColorPicker() { }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        //Set up base varibles and components
        previousColorPanel.color = currentColor;
        currentColorPanel.color = currentColor;
        defaultColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        initialColorBlockHandlePosition = colorBlockHandle;
        if ((originColor == Color.white) || (originColor == Color.black) || ((originColor.r == originColor.g) && (originColor.g == originColor.b)))
        {
            originColor = GetBaseColor(currentColor);
        }
        colorBlock.sprite = Sprite.Create(new Texture2D((int)colorBlock.rectTransform.rect.width, (int)colorBlock.rectTransform.rect.height), new Rect(0, 0, colorBlock.rectTransform.rect.width, colorBlock.rectTransform.rect.height), new Vector2(0, 0));
        colorHueBar.sprite = Sprite.Create(new Texture2D((int)colorHueBar.rectTransform.rect.width, (int)colorHueBar.rectTransform.rect.height), new Rect(0, 0, colorHueBar.rectTransform.rect.width, colorHueBar.rectTransform.rect.height), new Vector2(0, 0));
        for (int i = 0; i < colorBars.Length; i++)
        {
            colorSlider[i].bar = colorBars[i];
            colorSlider[i].initialHandleTransform = colorSlider[i].handle = colorHandles[i];
            colorSlider[i].bar.sprite = Sprite.Create(new Texture2D((int)colorSlider[i].bar.rectTransform.rect.width, (int)colorSlider[i].bar.rectTransform.rect.height), new Rect(0, 0, colorSlider[i].bar.rectTransform.rect.width, colorSlider[i].bar.rectTransform.rect.height), new Vector2(0, 0));
            //colorSlider[i].handle.localPosition = colorSlider[i].initialHandleTransform.localPosition + new Vector3((148.0f * currentColor[i]), 0.0f, 0.0f);
        }

        //Set the value in the color sliders text input feilds
        for(int j = 0; j < colorBars.Length; j++)
        {
            colorSliders[j].value = currentColor[j];
            colorInputFields[j].text = currentColor[j].ToString("0.0000");
        }
        colorSliders[3].value = currentColor.a;
        //colorSlider[0].handle.transform.localPosition
        
        //Refresh the color picker with new variable and component values
        UpdateColorPicker();

        //FillColorBlockSegments(hues, ref colorHueBar, true);

        //FillColorBlockSegment(hues[1], hues[1], hues[0], hues[0], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*0.0f), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*1.0f));
        //FillColorBlockSegment(hues[2], hues[2], hues[1], hues[1], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*1.0f), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*2.0f));
        //FillColorBlockSegment(hues[3], hues[3], hues[2], hues[2], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*2.0f), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*3.0f));
        //FillColorBlockSegment(hues[4], hues[4], hues[3], hues[3], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*3.0f), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*4.0f));
        //FillColorBlockSegment(hues[5], hues[5], hues[4], hues[4], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*4.0f), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*5.0f));
        //FillColorBlockSegment(hues[6], hues[6], hues[5], hues[5], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*5.0f), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / 6.0f)*6.0f));
        //for(int k = 0; k < (hues.Length - 1); k++)
        //{
        //    FillColorBlockSegment(hues[k+1], hues[k+1], hues[k], hues[k], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / (float)(hues.Length - 1)) * (k)), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / (float)(hues.Length - 1)) * (k+1)));
        //}

        //Fill the vertical slider bar used to change the base hue to a diffrent color
        FillColorBlockSegments(hues, ref colorHueBar, true, true);

        //Color test = GetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), currentColor, new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f), colorBlock, (int)colorBlock.rectTransform.rect.width, (int)colorBlock.rectTransform.rect.height);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Un-does any changes done to the color picker during the current active session and sets active false
    /// </summary>
    public void onCancel()
    {
        //Set current color back to previouse
        currentColor = previousColorPanel.color;
        //Reseting the base hue
        originColor = GetBaseColor(currentColor);
        //Refreshing components
        for (int j = 0; j < 4; j++)
        {
            colorSliders[j].SetValueWithoutNotify(currentColor[j]);
        }
        UpdateHueMarker();
        UpdateColorPicker();
        m_onEscape.Invoke();
        if (m_hideOnConfermationOrCancelation)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Applies color change to all objects listening for the confirmation event
    /// </summary>
    public void onConferm()
    {
        previousColorPanel.color = currentColor;
        m_onEnter.Invoke(currentColor);
        if(m_hideOnConfermationOrCancelation)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Bilinearly interpolates four colors over the four points of an Image object
    /// </summary>
    /// <param name="topLeft">Color for the top left side of the refrenced image</param>
    /// <param name="topRight">Color for the top right side of the refrenced image</param>
    /// <param name="bottomRight">Color for the bottom right side of the refrenced image</param>
    /// <param name="bottomLeft">Color for the bottom left side of the refrenced image</param>
    /// <param name="block">Refrence to the image that is being filled</param>
    /// <param name="mark">Boolean indicating if the handle for the interpolated area should be repositioned in this fill</param>
    void FillColorBlock(Color topLeft, Color topRight, Color bottomRight, Color bottomLeft, ref Image block, bool mark)
    {
        if (mark)
        {
            //Check if all current color values(RGB) are equal then set handle to a ratio based position between the top left corner(White in interpolated block for the 2D slider) or the bottom left corner(Black in interpolated block for the 2D slider)
            if ((Mathf.Floor(10.0f * currentColor.r) == Mathf.Floor(10.0f * currentColor.g)) && (Mathf.Floor(10.0f * currentColor.g) == Mathf.Floor(10.0f * currentColor.b)))
            {
                colorBlockHandle.localPosition = /*initialColorBlockHandlePosition.position +*/ new Vector3(-75.0f, (currentColor.r * 148.0f) - 75.0f, 0.0f);
                colorBoxSlider.SetValueWithoutNotify(0, (currentColor.r * 148.0f));
                //mark = false;
            }
            //Check if the base hue is equal to the current color then set the handle to the top right(Base hue/color in interpolated block for the 2D slider)
            else if (((Mathf.Floor(10.0f * topRight.r) == Mathf.Floor(10.0f * currentColor.r)) && (Mathf.Floor(10.0f * topRight.g) == Mathf.Floor(10.0f * currentColor.g)) && (Mathf.Floor(10.0f * topRight.b) == Mathf.Floor(10.0f * currentColor.b))) || (topRight == currentColor))
            {
                colorBlockHandle.localPosition = new Vector3(73.0f, 73.0f, 0.0f);
                colorBoxSlider.SetValueWithoutNotify(148.0f, 148.0f);
                //mark = false;
            }
            //Check if any of the current color values are set to 100% then place the handle at the top and a ration left 
            else if ((Mathf.Floor(currentColor.r) == 1.0f || Mathf.Floor(currentColor.g) == 1.0f || Mathf.Floor(currentColor.b) == 1.0f) && ((currentColor != Color.white) && (currentColor != Color.black)))
            {
                float r = 0.0f;
                if((((Mathf.Floor(currentColor.r) == Mathf.Floor(currentColor.g)) && Mathf.Floor(currentColor.r) == 1.0f) || ((Mathf.Floor(currentColor.r) == Mathf.Floor(currentColor.b)) && Mathf.Floor(currentColor.r) == 1.0f) || ((Mathf.Floor(currentColor.b) == Mathf.Floor(currentColor.g)) && Mathf.Floor(currentColor.b) == 1.0f)))
                {
                    if (Mathf.Floor(currentColor.r) == Mathf.Floor(currentColor.g))
                    {
                        r = ((1.0f - currentColor.b) * 148.0f);
                    }
                    else if (Mathf.Floor(currentColor.r) == Mathf.Floor(currentColor.b))
                    {
                        r = ((1.0f - currentColor.g) * 148.0f);
                    }
                    else if (Mathf.Floor(currentColor.b) == Mathf.Floor(currentColor.g))
                    {
                        r = ((1.0f - currentColor.r) * 148.0f);
                    }
                }
                else
                {
                    r = ((1.0f - Mathf.Min(currentColor[0], currentColor[1], currentColor[2])) * 148.0f);
                }
                //colorBlockHandle.localPosition = new Vector3(r - 75.0f, 73.0f, 0.0f);
                colorBlockHandle.localPosition = new Vector3(r - 75.0f, 73.0f, 0.0f);
                colorBoxSlider.SetValueWithoutNotify(r, 148.0f);
                //mark = false;
            }
            //else if ((currentColor == Color.white) && (currentColor == Color.black))
            //{
            //    colorBlockHandle.localPosition = (currentColor == Color.white) ? new Vector3(-75.0f, 73.0f, 0.0f) : new Vector3(-75.0f, -75.0f, 0.0f);
            //}

        }

        //Fill the image
        for (int y = 0; y < Mathf.Floor(block.sprite.rect.height); y++)
        {
            for (int x = 0; x < Mathf.Floor(block.sprite.rect.width); x++)
            {
                Color _c = Color.LerpUnclamped(Color.LerpUnclamped(bottomLeft, bottomRight, (x / Mathf.Floor(block.sprite.rect.width))), Color.LerpUnclamped(topLeft, topRight, (x / Mathf.Floor(block.sprite.rect.width))), (y / Mathf.Floor(block.sprite.rect.height)));
                if (mark)
                {
                    if ((Mathf.Floor(10.0f * _c.r) == Mathf.Floor(10.0f * currentColor.r)) && (Mathf.Floor(10.0f * _c.g) == Mathf.Floor(10.0f * currentColor.g)) && (Mathf.Floor(10.0f * _c.b) == Mathf.Floor(10.0f * currentColor.b)))
                    {
                        //colorBlockHandle.localPosition = new Vector3(x - 75.0f, y - 75.0f, 0.0f);
                        colorBlockHandle.localPosition = new Vector3(x - 75.0f, y - 75.0f, 0.0f);
                        colorBoxSlider.SetValueWithoutNotify(x, y);
                    }
                }
                block.sprite.texture.SetPixel(x, y, _c);
            }
        }

        //Apply changes to refrenced image
        block.sprite.texture.Apply();
    }

    /// <summary>
    /// Bilinearly interpolates two colors from a pair of points to the opposing points of an Image object
    /// </summary>
    /// <param name="left">Color of the left/top color</param>
    /// <param name="right">Color of the right/bottom two corners</param>
    /// <param name="block">Refrence to the image that is being filled</param>
    /// <param name="top_bottom">Boolean determining wheather or not the color is interpolated from the top two corners to the bottom two or the left two to the right two</param>
    void FillColorBlock(Color left, Color right, ref Image block, bool top_bottom)
    {
        if (!top_bottom)
        {
            FillColorBlock(left, right, right, left, ref block, false);
        }
        else
        {
            FillColorBlock(left, left, right, right, ref block, false);
        }
    }

    /// <summary>
    /// Bilinearly interpolate four colors between four points of a designated segment of an Image object
    /// </summary>
    /// <param name="topLeft">Color for the top left side of the refrenced image</param>
    /// <param name="topRight">Color for the top right side of the refrenced image</param>
    /// <param name="bottomRight">Color for the bottom right side of the refrenced image</param>
    /// <param name="bottomLeft">Color for the bottom left side of the refrenced image</param>
    /// <param name="block">Refrence to the image that is being filled</param>
    /// <param name="minX">Lowest horizantal position value(X Value) of the four points</param>
    /// <param name="minY">Lowest vertical position value(Y Value) of the four points</param>
    /// <param name="maxX">Highest horizantal position value(X Value) of the four points</param>
    /// <param name="maxY">Highest vertical position value(Y Value) of the four points</param>
    /// <param name="mark">Boolean indicating if the handle for the interpolated area should be repositioned in this fill</param>
    void FillColorBlockSegment(Color topLeft, Color topRight, Color bottomRight, Color bottomLeft, ref Image block, int minX, int minY, int maxX, int maxY, bool mark)
    {
        for (float y = minY; y < maxY; y++)
        {
            for (float x = minX; x < maxX; x++)
            {
                Color _c = Color.Lerp(Color.Lerp(bottomLeft, bottomRight, ((x - (float)minX) / (maxX - minX))), Color.Lerp(topLeft, topRight, ((x - (float)minX) / (maxX - minX))), ((y - (float)minY) / (maxY - minY)));
                if (mark)
                {
                    if ((Mathf.Floor(10.0f * _c.r) == Mathf.Floor(10.0f * originColor.r)) && (Mathf.Floor(10.0f * _c.g) == Mathf.Floor(10.0f * originColor.g)) && (Mathf.Floor(10.0f * _c.b) == Mathf.Floor(10.0f * originColor.b)))
                    {
                        colorHueSlider.value = 1.0f - (y / block.rectTransform.rect.height);
                    }
                }
                block.sprite.texture.SetPixel((int)x, (int)y, _c);
            }
        }
        block.sprite.texture.Apply();
    }

    /// <summary>
    /// Get the color of a point in a Image object with four colors bilinearly interpolated between four points
    /// </summary>
    /// <param name="topLeft">Color for the top left side of the image</param>
    /// <param name="topRight">Color for the top right side of the image</param>
    /// <param name="bottomRight">Color for the bottom right side of the image</param>
    /// <param name="bottomLeft">Color for the bottom left side of the image</param>
    /// <param name="block">Tmage that has being filled</param>
    /// <param name="x">The horizantal coordinate value from where the color is to be retrived from the image</param>
    /// <param name="y">The vertical coordinate value from where the color is to be retrived from the image</param>
    /// <returns>The color found at the given coordinates</returns>
    public Color GetColor(Color topLeft, Color topRight, Color bottomRight, Color bottomLeft, Image block, float x, float y)
    {
        return Color.Lerp(Color.Lerp(bottomLeft, bottomRight, (x / block.sprite.rect.width)), Color.Lerp(topLeft, topRight, (x / block.sprite.rect.width)), (y / block.sprite.rect.height));
    }

    /// <summary>
    /// Bilinearly interpolate two colors from a pair of points to the opposing points of a designated segment of an Image object
    /// </summary>
    /// <param name="left">Color of the left/top color</param>
    /// <param name="right">Color of the right/bottom two corners</param>
    /// <param name="block">Refrence to the image that is being filled</param>
    /// <param name="minX">Lowest horizantal position value(X Value) of the four points</param>
    /// <param name="minY">Lowest vertical position value(Y Value) of the four points</param>
    /// <param name="maxX">Highest horizantal position value(X Value) of the four points</param>
    /// <param name="maxY">Highest vertical position value(Y Value) of the four points</param>
    /// <param name="top_bottom">Boolean determining wheather or not the color is interpolated from the top two corners to the bottom two or the left two to the right two</param>
    /// <param name="mark">Boolean indicating if the handle for the interpolated area should be repositioned in this fill</param>
    void FillColorBlockSegment(Color left, Color right, ref Image block, int minX, int minY, int maxX, int maxY, bool top_bottom, bool mark)
    {
        if (!top_bottom)
        {
            FillColorBlockSegment(left, right, right, left, ref block, minX, minY, maxX, maxY, mark);
        }
        else
        {
            FillColorBlockSegment(left, left, right, right, ref block, minX, minY, maxX, maxY, mark);
        }
    }

    /// <summary>
    /// Bilinearly interpolate a colection of colors between evenly destributed segments of an Image object
    /// </summary>
    /// <param name="colors">Collection of colors to be filled into the refrenced image</param>
    /// <param name="block">Refrence to the image that is being filled</param>
    /// <param name="top_bottom">Boolean determining wheather or not the color is interpolated from the top two corners to the bottom two or the left two to the right two</param>
    /// <param name="mark">Boolean indicating if the handle for the interpolated area should be repositioned in this fill</param>
    void FillColorBlockSegments(Color[] colors, ref Image block, bool top_bottom, bool mark)
    {
        for (int i = 0; i < (colors.Length - 1); i++)
        {
            if (!top_bottom)
            {
                FillColorBlockSegment(colors[i], colors[i + 1], colors[i + 1], colors[i], ref colorHueBar, (int)((colorHueBar.rectTransform.rect.width / (float)(colors.Length - 1)) * (i)), 0, (int)((colorHueBar.rectTransform.rect.width / (float)(colors.Length - 1)) * ((i + 1))), (int)colorHueBar.rectTransform.rect.height, mark);
            }
            else
            {
                FillColorBlockSegment(colors[i + 1], colors[i + 1], colors[i], colors[i], ref colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / (float)(colors.Length - 1)) * (i)), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / (float)(colors.Length - 1)) * ((i + 1))), mark);
            }
        }
    }

    //void RGBToHex(Color rgbColor, out uint hexOutput)
    //{

    //}

    /// <summary>
    /// Updates marker on the hue bar that determins the main hue color of the color picker
    /// </summary>
    public void UpdateHueMarker()
    {
        DontUpdate = true;
        for (int i = 0; i < (hues.Length - 1); i++)
        {
            UpdateHueMarker(hues[i + 1], hues[i + 1], hues[i], hues[i], colorHueBar, 0, (int)((colorHueBar.rectTransform.rect.height / (float)(hues.Length - 1)) * (i)), (int)colorHueBar.rectTransform.rect.width, (int)((colorHueBar.rectTransform.rect.height / (float)(hues.Length - 1)) * ((i + 1))));
        }
        DontUpdate = false;
        currentColorPanel.color = currentColor;
    }

    /// <summary>
    /// Updates marker on the hue bar that determins the main hue color of the color picker
    /// </summary>
    /// <param name="topLeft">Color for the top left side of the image</param>
    /// <param name="topRight">Color for the top right side of the image</param>
    /// <param name="bottomRight">Color for the bottom right side of the image</param>
    /// <param name="bottomLeft">Color for the bottom left side of the image</param>
    /// <param name="block">Refrence to the image that is being filled</param>
    /// <param name="minX">Lowest horizantal position value(X Value) of the four points</param>
    /// <param name="minY">Lowest vertical position value(Y Value) of the four points</param>
    /// <param name="maxX">Highest horizantal position value(X Value) of the four points</param>
    /// <param name="maxY">Highest vertical position value(Y Value) of the four points</param>
    public void UpdateHueMarker(Color topLeft, Color topRight, Color bottomRight, Color bottomLeft, Image block, int minX, int minY, int maxX, int maxY)
    {
        for (float y = minY; y < maxY; y++)
        {
            for (float x = minX; x < maxX; x++)
            {
                Color _c = Color.Lerp(Color.Lerp(bottomLeft, bottomRight, ((x - (float)minX) / (maxX - minX))), Color.Lerp(topLeft, topRight, ((x - (float)minX) / (maxX - minX))), ((y - (float)minY) / (maxY - minY)));
                if ((Mathf.Floor(10.0f * _c.r) == Mathf.Floor(10.0f * originColor.r)) && (Mathf.Floor(10.0f * _c.g) == Mathf.Floor(10.0f * originColor.g)) && (Mathf.Floor(10.0f * _c.b) == Mathf.Floor(10.0f * originColor.b)))
                {
                    colorHueSlider.value = 1.0f - (y / block.rectTransform.rect.height);
                }
            }
        }
    }

    /// <summary>
    /// Sets the color value of the current color of the color picker
    /// </summary>
    /// <param name="_color">Color value the current color is to be set to</param>
    /// <param name="changePrevious">Boolean that indicates wheather or not to set the previouse color indicator to the current color before it changes</param>
    public void SetCurrentColor(Color _color, bool changePrevious = false)
    {
        SetCurrentColorRed(_color.r);
        SetCurrentColorGreen(_color.g);
        SetCurrentColorBlue(_color.b);
        SetCurrentColorAlpha(_color.a);
        if(changePrevious)
        {
            previousColorPanel.color = _color;
        }
    }
    
    /// <summary>
    /// Set the red value of the current color
    /// </summary>
    /// <param name="_color_values"></param>
    public void SetCurrentColorRed(float _color_values)
    {
        currentColor.r = _color_values;
        colorSliders[0].SetValueWithoutNotify(currentColor.r);
        colorInputFields[0].text = _color_values.ToString("0.0000");
        originColor = GetBaseColor(currentColor);
        UpdateColorPicker();
        UpdateCoordinates();
    }

    /// <summary>
    /// Set the red value of the current color
    /// </summary>
    public void SetCurrentColorRed()
    {
        currentColor.r = (float)double.Parse(colorInputFields[0].text);
        if(currentColor.r > 1.0f)
        {
            currentColor.r = 1.0f;
            colorInputFields[0].text = currentColor.r.ToString("0.0000");
        }
        if (currentColor.r < 0.0f)
        {
            currentColor.r = 0.0f;
            colorInputFields[0].text = currentColor.r.ToString("0.0000");
        }
        colorSliders[0].SetValueWithoutNotify(currentColor.r);
        originColor = GetBaseColor(currentColor);
        UpdateColorPicker();
        UpdateCoordinates();
    }

    /// <summary>
    /// Set the green value of the current color
    /// </summary>
    /// <param name="_color_values"></param>
    public void SetCurrentColorGreen(float _color_values)
    {
        currentColor.g = _color_values;
        colorSliders[1].SetValueWithoutNotify(currentColor.g);
        colorInputFields[1].text = _color_values.ToString("0.0000");
        originColor = GetBaseColor(currentColor);
        UpdateColorPicker();
        UpdateCoordinates();
    }
    /// <summary>
    /// Set the green value of the current color
    /// </summary>
    public void SetCurrentColorGreen()
    {
        currentColor.g = (float)double.Parse(colorInputFields[1].text);
        if (currentColor.g > 1.0f)
        {
            currentColor.g = 1.0f;
            colorInputFields[1].text = currentColor.g.ToString("0.0000");
        }
        if (currentColor.g < 0.0f)
        {
            currentColor.g = 0.0f;
            colorInputFields[1].text = currentColor.g.ToString("0.0000");
        }
        colorSliders[1].SetValueWithoutNotify(currentColor.g);
        originColor = GetBaseColor(currentColor);
        UpdateColorPicker();
        UpdateCoordinates();
    }
    /// <summary>
    /// Set the blue value of the current color
    /// </summary>
    /// <param name="_color_values"></param>
    public void SetCurrentColorBlue(float _color_values)
    {
        currentColor.b = _color_values;
        colorSliders[2].SetValueWithoutNotify(currentColor.b);
        colorInputFields[2].text = _color_values.ToString("0.0000");
        originColor = GetBaseColor(currentColor);
        UpdateColorPicker();
        UpdateCoordinates();
    }
    /// <summary>
    /// Set the blue value of the current color
    /// </summary>
    public void SetCurrentColorBlue()
    {
        currentColor.b = (float)double.Parse(colorInputFields[2].text);
        if (currentColor.b > 1.0f)
        {
            currentColor.b = 1.0f;
            colorInputFields[2].text = currentColor.b.ToString("0.0000");
        }
        if (currentColor.b < 0.0f)
        {
            currentColor.b = 0.0f;
            colorInputFields[2].text = currentColor.b.ToString("0.0000");
        }
        colorSliders[2].SetValueWithoutNotify(currentColor.b);
        originColor = GetBaseColor(currentColor);
        UpdateColorPicker();
        UpdateCoordinates();
    }
    /// <summary>
    /// Set the alpha/opacity value of the current color
    /// </summary>
    /// <param name="_color_values"></param>
    public void SetCurrentColorAlpha(float _color_values)
    {
        currentColor.a = _color_values;
        colorSliders[3].SetValueWithoutNotify(currentColor.a);
        colorInputFields[3].text = _color_values.ToString("0.0000");
        //originColor = GetBaseColor(currentColor);
        FillColorBlock(new Color(0.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(0.0f, currentColor.g, currentColor.b, 1.0f), ref colorSlider[0].bar, false);
        FillColorBlock(new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), ref colorSlider[1].bar, false);
        FillColorBlock(new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), ref colorSlider[2].bar, false);
        //FillColorBlock(new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), ref colorSlider[3].bar, false);
        currentColorPanel.color = currentColor;
        UpdateCoordinates();
    }
    /// <summary>
    /// Set the alpha/opacity value of the current color
    /// </summary>
    public void SetCurrentColorAlpha()
    {
        currentColor.a = float.Parse(colorInputFields[3].text);
        if (currentColor.a > 1.0f)
        {
            currentColor.a = 1.0f;
            colorInputFields[3].text = currentColor.r.ToString("0.0000");
        }
        if (currentColor.a < 0.0f)
        {
            currentColor.a = 0.0f;
            colorInputFields[3].text = currentColor.r.ToString("0.0000");
        }
        colorSliders[3].SetValueWithoutNotify(currentColor.a);
        //originColor = GetBaseColor(currentColor);
        FillColorBlock(new Color(0.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(0.0f, currentColor.g, currentColor.b, 1.0f), ref colorSlider[0].bar, false);
        FillColorBlock(new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), ref colorSlider[1].bar, false);
        FillColorBlock(new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), ref colorSlider[2].bar, false);
        //FillColorBlock(new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), ref colorSlider[3].bar, false);
        currentColorPanel.color = currentColor;
        UpdateCoordinates();
    }
    /// <summary>
    /// Updates the current color value of the color picker based on change in gui value
    /// </summary>
    public void UpdateCurrentColor()
    {
        currentColor = GetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), originColor, new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f), colorBlock, colorBoxSlider.xValue, colorBoxSlider.yValue);
        for (int j = 0; j < 4; j++)
        {
            colorSliders[j].SetValueWithoutNotify(currentColor[j]);
            colorInputFields[j].text = currentColor[j].ToString("0.0000");
        }
        UpdateColorPicker();
        UpdateCoordinates();
    }
    /// <summary>
    /// Sets the base hue color with color from he hue bar
    /// </summary>
    /// <param name="_color">float value with which to set the slider to</param>
    public void SetOriginColor(float _color)
    {
        if (!DontUpdate) {
            int access = (hues.Length - 1);
            for (float i = 0; i < (hues.Length - 1); i++)
            {
                if (((i / (hues.Length - 1)) <= _color) && (((i + 1) / (hues.Length - 1)) > _color))
                {
                    float min = (i / (hues.Length - 1));
                    float max = ((i + 1) / (hues.Length - 1));
                    access -= (int)(i + 1);
                    originColor = Color.Lerp(hues[access + 1], hues[access], (_color - min) / (max - min));
                    break;
                }
            }
            float alpha = currentColor.a;
            currentColor = GetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), originColor, new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f), colorBlock, colorBlockHandle.localPosition.x + 75.0f, colorBlockHandle.localPosition.y + 75.0f);
            currentColor.a = alpha;
            for (int j = 0; j < 4; j++)
            {
                colorSliders[j].SetValueWithoutNotify(currentColor[j]);
                colorInputFields[j].text = currentColor[j].ToString("0.0000");
            }
            currentColorPanel.color = currentColor;
            FillColorBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), originColor, new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f), ref colorBlock, false);
            FillColorBlock(new Color(0.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(0.0f, currentColor.g, currentColor.b, 1.0f), ref colorSlider[0].bar, false);
            FillColorBlock(new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), ref colorSlider[1].bar, false);
            FillColorBlock(new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), ref colorSlider[2].bar, false);
            FillColorBlock(new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), ref colorSlider[3].bar, false);
        }
    }
    /// <summary>
    /// Updates all the sliders in the color picker
    /// </summary>
    void UpdateColorPicker()
    {
        FillColorBlock(new Color(1.0f, 1.0f, 1.0f, 1.0f), originColor, new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f), ref colorBlock, true);
        FillColorBlock(new Color(0.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f), new Color(0.0f, currentColor.g, currentColor.b, 1.0f), ref colorSlider[0].bar, false);
        FillColorBlock(new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 1.0f, currentColor.b, 1.0f), new Color(currentColor.r, 0.0f, currentColor.b, 1.0f), ref colorSlider[1].bar, false);
        FillColorBlock(new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 1.0f, 1.0f), new Color(currentColor.r, currentColor.g, 0.0f, 1.0f), ref colorSlider[2].bar, false);
        FillColorBlock(new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f), new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f), ref colorSlider[3].bar, false);
        currentColorPanel.color = currentColor;
    }
    /// <summary>
    /// Calculate the base color of the color picker's 2D slider area from a given color value
    /// </summary>
    /// <param name="_color">Color value used ot calculate base color</param>
    /// <returns>The calculated base color</returns>
    Color GetBaseColor(Color _color)
    {
        Color _base = defaultColor;
        //Check if all current color values(RGB) are equal then set handle to a ratio based position between the top left corner(White in interpolated block for the 2D slider) or the bottom left corner(Black in interpolated block for the 2D slider)
        if ((Mathf.Floor(10.0f*_color.r) == Mathf.Floor(10.0f * _color.g)) && (Mathf.Floor(10.0f * _color.g) == Mathf.Floor(10.0f * _color.b)))
        {
            colorBlockHandle.localPosition = /*initialColorBlockHandlePosition.position +*/ new Vector3(-75.0f, (_color.r * 148.0f) - 75.0f, 0.0f);
            colorBoxSlider.SetValueWithoutNotify(0.0f, (_color.r*148.0f));
        }
        //Check if two of the colors are equal to eachother and if so set the base color to the combination of the two equal color values or the inequal color depending on which is higher
        else if ((Mathf.Floor(10.0f * _color.r) == Mathf.Floor(10.0f * _color.g)) || (Mathf.Floor(10.0f * _color.g) == Mathf.Floor(10.0f * _color.b)) || (Mathf.Floor(10.0f * _color.r) == Mathf.Floor(10.0f * _color.b)))
        {
            if (Mathf.Floor(10.0f * _color.r) == Mathf.Floor(10.0f * _color.g))
            {
                _base = (_color.r > _color.b) ? new Color(1.0f, 1.0f, 0.0f, 1.0f) : Color.blue;
            }
            else if(Mathf.Floor(10.0f * _color.g) == Mathf.Floor(10.0f * _color.b))
            {
                _base = (_color.g > _color.r) ? Color.cyan : Color.red;
            }
            else if (Mathf.Floor(10.0f * _color.r) == Mathf.Floor(10.0f * _color.b))
            {
                _base = (_color.r > _color.g) ? Color.magenta : Color.green;
            }
        }
        //Set the mediam color value to value based the max and min values
        else
        {
            _base = _color;

            float min_color_val = Mathf.Min(Mathf.Min(_color.r,_color.g), _color.b);
            float max_color_val = Mathf.Max(Mathf.Max(_color.r, _color.g), _color.b);
            float min_diff_val = min_color_val;
            float max_diff_val = 1.0f - max_color_val;
            float diff_diff = max_diff_val - min_diff_val;
            bool[] colorsChanged = {false, false, false};

            if (min_color_val == _color.r)
            {
                _base.r -= min_diff_val;
                colorsChanged[0] = true;
            }
            else if(min_color_val == _color.g)
            {
                _base.g -= min_diff_val;
                colorsChanged[1] = true;
            }
            else if(min_color_val == _color.b)
            {
                _base.b -= min_diff_val;
                colorsChanged[2] = true;
            }

            if (max_color_val == _color.r)
            {
                _base.r += max_diff_val;
                colorsChanged[0] = true;
            }
            else if (max_color_val == _color.g)
            {
                _base.g += max_diff_val;
                colorsChanged[1] = true;
            }
            else if (max_color_val == _color.b)
            {
                _base.b += max_diff_val;
                colorsChanged[2] = true;
            }

            for (int i = 0; i < 3; i++)
            {
                if (!colorsChanged[i] )
                {
                    if (Mathf.Floor((_base[0] + _base[1] + _base[2]) - _base[i]) != 1.0f)
                    {
                        float num_1 = Mathf.Max(_base[i], max_color_val) - Mathf.Min(_base[i], max_color_val);
                        float num_2 = Mathf.Max(_base[i], min_color_val) - Mathf.Min(_base[i], min_color_val);
                        _base[i] = /*(_base[i] < 0.5f) ? (*/1.0f - (Mathf.Max(num_1, num_2) + Mathf.Min(num_1, num_2))/*) : (Mathf.Max(num_1, num_2) + Mathf.Min(num_1, num_2))*/;
                    }
                    else
                    {
                        _base[i] = currentColor[i];
                    }
                }
            }
        }
        return _base;
    }
    /// <summary>
    /// Change the coordinate values indicating the position of the 2D slider handle
    /// </summary>
    public void UpdateCoordinates()
    {
        m_coordinates.text = "X:" + (colorBlockHandle.localPosition.x + 75.0f).ToString("0.00") + " Y:" + (colorBlockHandle.localPosition.y + 75.0f).ToString("0.00");
    }
}