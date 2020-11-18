using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class TexturePaintable
{
    public string id;
    public RenderTexture runTimeTexture;
    public RenderTexture outputTexture;
    public CommandBuffer command_buffer;

    Material m_paintable;
    Material m_edges;
    RenderTexture m_finalTex;

    /// <summary>
    /// Constructor to set up the texture to be painted
    /// </summary>
    /// <param name="_baseTex">Texture to copy onto the texture to be painted</param>
    /// <param name="_width">Width of the texture</param>
    /// <param name="_height">Height of the texture</param>
    /// <param name="_id">Numerical identifier for the texture</param>
    /// <param name="_paintShader">Shader dealing with handeling the brush inputs</param>
    /// <param name="_mesh">Mesh of the model being painted</param>
    /// <param name="_edgeShader">Shader dealing with the edge cases</param>
    /// <param name="_renderTarget">Texture to be painted</param>
    public TexturePaintable(Texture _baseTex, int _width, int _height, string _id, Shader _paintShader, Mesh _mesh, Shader _edgeShader, RenderTexture _renderTarget)
    {
        id = _id;

        runTimeTexture = new RenderTexture(_width, _height, 0);
        runTimeTexture.anisoLevel = 0;
        runTimeTexture.useMipMap = false;
        runTimeTexture.filterMode = FilterMode.Bilinear;

        outputTexture = new RenderTexture(_width, _height, 0);
        outputTexture.anisoLevel = 0;
        outputTexture.useMipMap = false;
        outputTexture.filterMode = FilterMode.Bilinear;

        m_finalTex = new RenderTexture(outputTexture.descriptor);

        Graphics.Blit(_baseTex, runTimeTexture);
        Graphics.Blit(_baseTex, outputTexture);

        m_paintable = new Material(_paintShader);
        if (!m_paintable.SetPass(0))
        {
            Debug.LogError("Invalid Shader Pass");
        }
        m_paintable.SetTexture("_MainTex", outputTexture);

        m_edges = new Material(_edgeShader);
        m_edges.SetTexture("_EdgeMap", _renderTarget);
        m_edges.SetTexture("_MainTex", outputTexture);

        command_buffer = new CommandBuffer();
        command_buffer.name = "TexturePaintable" + id;

        command_buffer.SetRenderTarget(runTimeTexture);
        command_buffer.DrawMesh(_mesh, Matrix4x4.identity, m_paintable);

        command_buffer.Blit(runTimeTexture, m_finalTex, m_edges);
        command_buffer.Blit(m_finalTex, runTimeTexture);
        command_buffer.Blit(runTimeTexture, outputTexture);
    }
    /// <summary>
    /// Constructor to set up the texture to be painted
    /// </summary>
    /// <param name="_clearColor">Color to set texture to</param>
    /// <param name="_width">Width of the texture</param>
    /// <param name="_height">Height of the texture</param>
    /// <param name="_id">Numerical identifier for the texture</param>
    /// <param name="_paintShader">Shader dealing with handeling the brush inputs</param>
    /// <param name="_mesh">Mesh of the model being painted</param>
    /// <param name="_edgeShader">Shader dealing with the edge cases</param>
    /// <param name="_renderTarget">Texture to be painted</param>
    public TexturePaintable(Color _clearColor, int _width, int _height, string _id, Shader _paintShader, Mesh _mesh, Shader _edgeShader, RenderTexture _renderTarget)
    {
        id = _id;

        runTimeTexture = new RenderTexture(_width, _height, 0);
        runTimeTexture.anisoLevel = 0;
        runTimeTexture.useMipMap = false;
        runTimeTexture.filterMode = FilterMode.Bilinear;

        outputTexture = new RenderTexture(_width, _height, 0);
        outputTexture.anisoLevel = 0;
        outputTexture.useMipMap = false;
        outputTexture.filterMode = FilterMode.Bilinear;

        m_finalTex = new RenderTexture(outputTexture.descriptor);

        Graphics.SetRenderTarget(runTimeTexture);
        GL.Clear(false, true, _clearColor);
        Graphics.SetRenderTarget(outputTexture);
        GL.Clear(false, true, _clearColor);

        m_paintable = new Material(_paintShader);
        if(!m_paintable.SetPass(0))
        {
            Debug.LogError("Invalid Shader Pass");
        }
        m_paintable.SetTexture("_MainTex", outputTexture);

        m_edges = new Material(_edgeShader);
        m_edges.SetTexture("_EdgeMap", _renderTarget);
        m_edges.SetTexture("_MainTex", outputTexture);

        command_buffer = new CommandBuffer();
        command_buffer.name = "TexturePaintable" + id;

        command_buffer.SetRenderTarget(runTimeTexture);
        command_buffer.DrawMesh(_mesh, Matrix4x4.identity, m_paintable);

        command_buffer.Blit(runTimeTexture, m_finalTex, m_edges);
        command_buffer.Blit(m_finalTex, runTimeTexture);
        command_buffer.Blit(runTimeTexture, outputTexture);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_cam"></param>
    public void SetActiveTexture(Camera _cam)
    {
        _cam.AddCommandBuffer(CameraEvent.AfterDepthTexture, command_buffer);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_cam">Camera being used for painting model</param>
    public void SetInactiveTexture(Camera _cam)
    {
        _cam.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, command_buffer);
    }
    /// <summary>
    /// Changes the world matrix of the paintable in the shader
    /// </summary>
    /// <param name="_WorldMatrix">New world matrix</param>
    public void UpdateShaderParams(Matrix4x4 _WorldMatrix)
    {
        m_paintable.SetMatrix("_WorldMatrix", _WorldMatrix);
    }
}
