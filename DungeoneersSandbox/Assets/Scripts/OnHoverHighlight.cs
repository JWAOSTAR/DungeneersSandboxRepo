﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class OnHoverHighlight : MonoBehaviour
{
    MeshRenderer m_meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Change shader based on whether the the mouse is over this object or not
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == this.gameObject)
        {
            for(int i = 0; i < m_meshRenderer.materials.Length; i++)
            {
                //Color temp = new Color(m_meshRenderer.materials[i].color.r, m_meshRenderer.materials[i].color.g, m_meshRenderer.materials[i].color.b);
                m_meshRenderer.materials[i].shader = Shader.Find("DS/OutlineShader");
                m_meshRenderer.materials[i].SetFloat("_OutlineThickness", 0.02f);
            }
        }
        else if (m_meshRenderer.material.shader == Shader.Find("DS/OutlineShader"))
        {
            for (int i = 0; i < m_meshRenderer.materials.Length; i++)
            {
                m_meshRenderer.materials[i].shader = Shader.Find("Standard");
            }
        }
    }
}
