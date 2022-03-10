using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public Camera m_Camera;
    public Material m_Mat;
    public float m_Edge = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Camera == null)
        {
            m_Camera = GetComponent<Camera>();
        }
        if (m_Mat == null)
        {
            m_Mat = new Material(Shader.Find("Custom/Edge"));
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {

        m_Mat.SetFloat("_Edge", m_Edge);
        Graphics.Blit(src, dst, m_Mat);
    }
}