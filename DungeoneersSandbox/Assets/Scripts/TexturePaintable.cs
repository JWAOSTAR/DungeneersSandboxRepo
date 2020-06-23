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

    public void SetActiveTexture(Camera _cam)
    {
        _cam.AddCommandBuffer(CameraEvent.AfterDepthTexture, command_buffer);
    }

    public void SetInactiveTexture(Camera _cam)
    {
        _cam.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, command_buffer);
    }

    public void UpdateShaderParams(Matrix4x4 _WorldMatrix)
    {
        m_paintable.SetMatrix("_WorldMatrix", _WorldMatrix);
    }
}
