using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// ��դɨ���㷨��8SSEDT���� 8-point Signed Sequential Euclidean Distance Transform��
/// </summary>
public class RasterScanning
{
    public static int m_Width;
    public static int m_Height;
    public static Grid m_GridMax;
    public static Grid m_GridMin;

    public static float[,] CreateSDF(bool[,] color)
    {
        m_Width = color.GetLength(0);
        m_Height = color.GetLength(1);
        m_GridMax = new Grid(m_Width, m_Height);
        m_GridMin = new Grid(0, 0);

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

        // �����ɫ�ڵ㵽��ɫ�ڵ�
        var g1 = Compute(positiveMaps);
        // �����ɫ�ڵ㵽��ɫ�ڵ�
        var g2 = Compute(negativeMaps);

        float[,] distance = new float[m_Width, m_Height];
        float max = float.MinValue;
        float min = float.MaxValue;
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                float d = GetDistance(g1[i, j], g2[i, j]);
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

    public static float GetDistance(Grid g1, Grid g2)
    {
        double d1 = Math.Sqrt(g1.GetDistance());
        double d2 = Math.Sqrt(g2.GetDistance());
        // ��ɫ����ɫΪ�����룬��ɫ����ɫΪ������
        float d = (float)(d1 - d2);
        return d;
    }

    public static Grid[,] Init(bool[,] maps)
    {
        var gridMaps = new Grid[m_Width, m_Height];
        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                if (maps[i, j] == true)
                {
                    // Ŀ���Ϊ����Զֵ
                    gridMaps[i, j] = m_GridMax;
                }
                else
                {
                    // ��Ŀ���Ϊ0
                    gridMaps[i, j] = m_GridMin;
                }
            }
        }
        return gridMaps;
    }

    public static Grid[,] Compute(bool[,] maps)
    {
        var gridMaps = Init(maps);

        for (int j = 0; j < m_Height; j++)
        {
            for (int i = 0; i < m_Width; i++)
            {
                // 1.�������£��������ң�����ÿ���ڵ㵽���ϡ����ϡ����Ͻڵ�ľ��룬������ǰ�ڵ����Ϊ������С�Ľڵ�

                gridMaps[i, j] = Compare(gridMaps, i, j, -1, 0);    //�ھ�-��
                gridMaps[i, j] = Compare(gridMaps, i, j, 0, -1);    //�ھ�-��
                gridMaps[i, j] = Compare(gridMaps, i, j, -1, -1);   //�ھ�-����
                gridMaps[i, j] = Compare(gridMaps, i, j, 1, -1);    //�ھ�-����
            }

            for (int i = m_Width - 1; i >= 0; i--)
            {
                // 2.�������£��������󣬼���ÿ���ڵ㵽�ҽڵ�ľ��룬������ǰ�ڵ����Ϊ������С�Ľڵ�

                gridMaps[i, j] = Compare(gridMaps, i, j, 1, 0);     //�ھ�-��
            }
        }

        for (int j = m_Height - 1; j >= 0; j--)
        {
            for (int i = m_Width - 1; i >= 0; i--)
            {
                // 3.�������ϣ��������󣬼���ÿ���ڵ㵽�ҡ��¡����¡����½ڵ�ľ��룬������ǰ�ڵ����Ϊ������С�Ľڵ�

                gridMaps[i, j] = Compare(gridMaps, i, j, 1, 0);    //�ھ�-��
                gridMaps[i, j] = Compare(gridMaps, i, j, 0, 1);    //�ھ�-��
                gridMaps[i, j] = Compare(gridMaps, i, j, -1, 1);    //�ھ�-����
                gridMaps[i, j] = Compare(gridMaps, i, j, 1, 1);    //�ھ�-����

            }

            for (int i = 0; i < m_Width; i++)
            {
                // 2.�������ϣ��������ң�����ÿ���ڵ㵽��ڵ�ľ��룬������ǰ�ڵ����Ϊ������С�Ľڵ�
                gridMaps[i, j] = Compare(gridMaps, i, j, -1, 0);    //�ھ�-��
            }
        }

        return gridMaps;
    }

    public static Grid Compare(Grid[,] gridMaps, int i, int j, int offset_i, int offset_j)
    {
        Grid result;
        Grid grid = GetGrid(gridMaps, i, j);

        int compare_i = i + offset_i;
        int compare_j = i + offset_j;
        if (compare_i < 0 || compare_i >= m_Width || compare_j < 0 || compare_j >= m_Height)
        {
            // �����߽�Ľڵ㲻���бȽ�
            result = grid;
        }
        else
        {
            Grid grid_compare = GetGrid(gridMaps, i + offset_i, j + offset_j);
            // �Ƚϵ�Ŀ��ڵ㣬����ƫ����Ϣ������Դ�ڵ��ƶ���Ŀ��ڵ����Ϣ
            grid_compare.Offset(offset_i, offset_j);

            int d = grid.GetDistance();
            int d_compare = grid_compare.GetDistance();

            // ���Ŀ��ڵ�ľ���С���򷵻�Ŀ��ڵ�
            if (d_compare < d)
            {
                result = grid_compare;
            }
            else
            {
                result = grid;
            }
        }
        return result;
    }


    public static Grid GetGrid(Grid[,] gridMaps, int i, int j)
    {
        if (i < 0 || i >= m_Width || j < 0 || j >= m_Height)
        {
            // �����߽������Ϊ����Զ�����Բ��ñ��������߽�ĵ㣩
            return m_GridMax;
        }
        return gridMaps[i, j];
    }


    public struct Grid
    {
        public int dx;
        public int dy;

        public Grid(int dx, int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public void Offset(int x, int y)
        {
            dx += x;
            dy += y;
        }

        public int GetDistance()
        {
            return dx * dx + dy * dy;
        }
    }
}


