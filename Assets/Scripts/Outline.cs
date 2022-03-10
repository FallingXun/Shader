using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    //private Shader m_OutlineShader_3;
    public Material m_OutlineMat_3;

    public Camera m_Camera;

    public RenderTexture m_RT;

    private void Start()
    {
        m_RT = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        m_Camera.gameObject.SetActive(false);
        m_Camera.CopyFrom(GetComponent<Camera>());
        m_Camera.cullingMask = 1 << 6;
        m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.backgroundColor = new Color(0,0,0,0);
        m_Camera.targetTexture = m_RT;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_OutlineMat_3 != null)
        {
            m_Camera.Render();
            RenderTexture buffer0 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            Graphics.Blit(m_RT, buffer0, m_OutlineMat_3, 0);
            m_OutlineMat_3.SetTexture("_OutlineTex", buffer0);
            RenderTexture.ReleaseTemporary(buffer0);
            Graphics.Blit(source, destination, m_OutlineMat_3, 1);
        }
    }
}
