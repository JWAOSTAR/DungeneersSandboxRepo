using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    GameObject m_tile;

    public List<GameObject> objects = new List<GameObject>();
    public GameObject tile { get { return m_tile; } set { m_tile = value; } }
}
