using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : MonoBehaviour
{
    public Camera m_Camera;
    public Material m_Mat;
    public float m_Intensity = 1f;
    public float m_Threshold = 0f;
    public float m_BlurSize = 1f;

    private void Start()
    {
        if(m_Camera == null)
        {
            m_Camera = GetComponent<Camera>();
        }

        if(m_Mat == null)
        {
            m_Mat = new Material(Shader.Find("Custom/Bloom"));
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        m_Mat.SetFloat("_Intensity", m_Intensity);
        m_Mat.SetFloat("_Threshold", m_Threshold);
        m_Mat.SetFloat("_BlurSize", m_BlurSize);

        RenderTexture rt1 = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2);
        Graphics.Blit(src, rt1, m_Mat, 0);
        RenderTexture rt2 = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2);
        Graphics.Blit(rt1, rt2, m_Mat, 1);

        m_Mat.SetTexture("_BloomTex", rt2);
        Graphics.Blit(src, dst, m_Mat, 2);
        RenderTexture.ReleaseTemporary(rt1);
        RenderTexture.ReleaseTemporary(rt2);
    }
}
