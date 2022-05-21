using System.Collections;
using System.Collections.Generic;
using System;

public class IndependentScanning
{
    public static int m_Width;
    public static int m_Height;


    public static float[,] CreateSDF(bool[,] color)
    {
        m_Width = color.GetLength(0);
        m_Height = color.GetLength(1);

        var positiveMaps = new bool[m_Width, m_Height];
        var negativeMaps = new bool[m_Width, m_Height];
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                // ��ɫ�ڵ�Ϊtrue����ɫ�ڵ�Ϊfalse
                if (color[i, j] == true)
                {
                    positiveMaps[i, j] = true;  // ����map�������ɫ�ڵ㣨Ŀ��㣩����ɫ�ڵ㣨��Ŀ��㣩�ľ���
                    negativeMaps[i, j] = false; // ����map�������ɫ�ڵ㣨Ŀ��㣩����ɫ�ڵ㣨��Ŀ��㣩�ľ���
                }
                else
                {
                    positiveMaps[i, j] = false;
                    negativeMaps[i, j] = true;
                }
            }
        }

        var d1 = Compute(positiveMaps);
        var d2 = Compute(negativeMaps);

        float[,] distance = new float[m_Width, m_Height];
        float max = float.MinValue;
        float min = float.MaxValue;
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                float d = (float)(Math.Sqrt(d1[i, j]) - Math.Sqrt(d2[i, j]));
                distance[i, j] = d;
                max = Math.Max(d, max);
                min = Math.Min(d, min);
            }
        }
        // �������ֵ����Сֵ�Ĳ�ֵ
        float clamp = max - min;
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                if (clamp <= 0)
                {
                    distance[i, j] = 0;
                }
                else
                {
                    // ������ӳ��Ϊ[0,1]
                    distance[i, j] = (distance[i, j] - min) / clamp;
                }
            }
        }
        return distance;
    }

    public static int[,] Compute(bool[,] maps)
    {
        int[,] distance = new int[m_Width, m_Height];
        int[] temp_width = new int[m_Width];
        // 1. ��ÿһ�У�����Ŀ��㵽��Ŀ���ľ��룬���õ�ÿ����(i,j)��ˮƽ����
        for (int j = 0; j < m_Height; j++)
        {
            // �Ƿ��з�Ŀ���
            bool hasTarget = false;
            for (int i = 0; i < m_Width; i++)
            {
                if (maps[i, j] == true)
                {
                    if (i == 0)
                    {
                        temp_width[i] = int.MaxValue;
                    }
                    else
                    {
                        if (temp_width[i - 1] == int.MaxValue)
                        {
                            temp_width[i] = int.MaxValue;
                        }
                        else
                        {
                            temp_width[i] = temp_width[i - 1] + 1;
                        }
                    }
                }
                else
                {
                    hasTarget = true;
                    temp_width[i] = 0;
                }
            }
            for (int i = m_Width - 1; i >= 0; i--)
            {
                if (hasTarget == false)
                {
                    temp_width[i] = m_Width;
                }
                else
                {
                    if (maps[i, j] == true)
                    {
                        if (i < m_Width - 1)
                        {
                            temp_width[i] = Math.Min(temp_width[i + 1] + 1, temp_width[i]);
                        }
                    }
                    else
                    {
                        temp_width[i] = 0;
                    }
                }
            }

            for (int i = 0; i < m_Width; i++)
            {
                distance[i, j] = temp_width[i] * temp_width[i];
            }
        }
        // 2. ��ÿһ�У�����ÿ����(i,j)��ÿһ��(i,y)����ֱ���룬���õ���ǰ��(i, j)����ֱ��������
        // 3. ����ǰ��(i, j)����ֱ��������͵�ǰ�еĶ�Ӧ��(i, y)��ˮƽ������ӣ����е���Сֵ��Ϊ��ǰ��(i,j)�ľ���
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                int min = int.MaxValue;
                for (int y = 0; y < m_Height; y++)
                {
                    var dis = distance[i, y] + (j - y) * (j - y);
                    min = Math.Min(dis, min);
                }
                temp_width[i] = min;
            }

            for (int i = 0; i < m_Width; i++)
            {
                distance[i, j] = temp_width[i];
            }
        }

        return distance;
    }
}
