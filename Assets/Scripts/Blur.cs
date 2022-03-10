using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blur : MonoBehaviour
{
    public Camera m_Camera;
    public float m_Blur = 1.0f;
    public Material m_Mat;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Camera == null)
        {
            m_Camera = GetComponent<Camera>();
        }
        if (m_Mat == null)
        {
            m_Mat = new Material(Shader.Find("Custom/Gaussian"));
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        m_Mat.SetFloat("_BlurSize", m_Blur);

        RenderTexture buffer0 = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2);

        Graphics.Blit(src, buffer0, m_Mat, 0);

        RenderTexture buffer1 = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2);

        Graphics.Blit(buffer0, dst, m_Mat, 1);
        
    }
}
