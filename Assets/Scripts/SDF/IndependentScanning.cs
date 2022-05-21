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
                // 白色节点为true，黑色节点为false
                if (color[i, j] == true)
                {
                    positiveMaps[i, j] = true;  // 正向map，计算白色节点（目标点）到黑色节点（非目标点）的距离
                    negativeMaps[i, j] = false; // 反向map，计算黑色节点（目标点）到白色节点（非目标点）的距离
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
        // 计算最大值到最小值的差值
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
                    // 将距离映射为[0,1]
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
        // 1. 对每一行，计算目标点到非目标点的距离，即得到每个点(i,j)的水平距离
        for (int j = 0; j < m_Height; j++)
        {
            // 是否有非目标点
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
        // 2. 对每一列，计算每个点(i,j)与每一行(i,y)的竖直距离，即得到当前点(i, j)的竖直距离数组
        // 3. 将当前点(i, j)的竖直距离数组和当前列的对应点(i, y)的水平距离相加，其中的最小值即为当前点(i,j)的距离
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
