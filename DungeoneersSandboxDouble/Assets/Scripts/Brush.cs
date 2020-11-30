using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBrush", menuName = "DungoneersSandbox/Brush", order = 1)]
public class Brush : ScriptableObject
{
    [SerializeField]
    float m_size = 1.0f;
    [SerializeField]
    float m_hardness = 1.0f; 
    [SerializeField]
    bool m_square = false;
    [SerializeField]
    Texture2D m_graphic;

    /// <summary>
    /// Boolean determining wheather the brush object is squar or circular
    /// </summary>
    public bool Square { get { return m_square; } set { m_square = value; } }

    //Not Implumented
    /// <summary>
    /// Graphic to paint onto paintable surface
    /// </summary>
    public Texture2D BrushGraphic { get { return m_graphic; } set { m_graphic = value; } }
    
    /// <summary>
    /// Size of the brush
    /// </summary>
    public float Size { get { return m_size; } set { m_size = value; } }

    /// <summary>
    /// Hardness/Opacity of the brush
    /// </summary>
    public float Hardness { get { return m_hardness; } set { m_hardness = value; } }
}
