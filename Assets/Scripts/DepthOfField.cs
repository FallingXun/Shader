using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthOfField : MonoBehaviour
{
    public Camera m_Camera;
    public float m_Far = 1000f;
    public float m_Near = 0f;
    public float m_Blur = 1f;
    public Material m_Mat;

    void Start()
    {
        if (m_Camera == null)
        {
            m_Camera = GetComponent<Camera>();
            m_Camera.depthTextureMode = DepthTextureMode.Depth;
        }
        if (m_Mat == null)
        {
            m_Mat = new Material(Shader.Find("Custom/DepthOfField"));
        }
    }


    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        m_Mat.SetFloat("_Near", m_Near);
        m_Mat.SetFloat("_Far", m_Far);
        m_Mat.SetFloat("_BlurSize", m_Blur);
        Graphics.Blit(src, dst, m_Mat);
    }
}
