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
    public bool Square { get { return m_square; } set { m_square = value; } }
    public Texture2D BrushGraphic { get { return m_graphic; } set { m_graphic = value; } }

    public float Size { get { return m_size; } set { m_size = value; } }
    public float Hardness { get { return m_hardness; } set { m_hardness = value; } }
}
