using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CBuffer : MonoBehaviour
{
    public Material m_Mat;
    public RenderTexture m_RT;
    public MeshRenderer m_Render;

    // Start is called before the first frame update
    void OnEnable()
    {
        m_RT = RenderTexture.GetTemporary(Screen.width, Screen.height);
        m_Render = GetComponentInChildren<MeshRenderer>();
        //var mat = render.sharedMaterial;
        CommandBuffer cb = new CommandBuffer();
        //cb.SetRenderTarget(m_RT);
        //cb.ClearRenderTarget(true, true, Color.black);
        cb.DrawRenderer(m_Render, m_Render.sharedMaterial);

        Camera.main.AddCommandBuffer(CameraEvent.AfterImageEffects, cb);
        m_Render.enabled = false;
    }

    private void OnDisable()
    {
        if (m_RT != null)
        {
            m_RT.Release();
            m_RT = null;
            m_Render.enabled = true;
        }
    }
}
