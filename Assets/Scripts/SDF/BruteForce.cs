using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// �����㷨
/// </summary>
public class BruteForce
{
    public static float[,] CreateSDF(bool[,] color)
    {
        int width = color.GetLength(0);
        int height = color.GetLength(1);

        var positiveMaps = new bool[width, height];
        var negativeMaps = new bool[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
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

        float[,] distance = new float[width, height];
        float max = float.MinValue;
        float min = float.MaxValue;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float d = (float)(Math.Sqrt(d1[i, j]) - Math.Sqrt(d2[i, j]));
                distance[i, j] = d;
                max = Math.Max(d, max);
                min = Math.Min(d, min);
            }
        }
        // �������ֵ����Сֵ�Ĳ�ֵ
        float clamp = max - min;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
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
        int width = maps.GetLength(0);
        int height = maps.GetLength(1);

        int[,] distance = new int[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // 1. ����ÿһ����ɫ�ڵ�
                if (maps[i, j] == true)
                {
                    int min = int.MaxValue;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (maps[x, y] == true)
                            {
                                continue;
                            }
                            // 2. ����ÿһ����ɫ�ڵ㣬�����ɫ�ڵ㵽��ɫ�ڵ�ľ��룬��¼��Сֵ
                            int dis = (x - i) * (x - i) + (y - j) * (y - j);
                            if (dis < min)
                            {
                                min = dis;
                            }
                        }
                    }
                    distance[i, j] = min;
                }
            }
        }
        return distance;
    }
}
