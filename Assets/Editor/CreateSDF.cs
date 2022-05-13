using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateSDF : Editor
{
    [MenuItem("SDF/Create")]
    public static void Create()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
        if (tex == null)
        {
            return;
        }
        Color target = Color.white;
        int width = tex.width;
        int height = tex.height;
        bool[,] color = new bool[width, height];
        float[,] distance = new float[width, height];
        float dis_min = float.MaxValue;
        float dis_max = float.MinValue;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var col = tex.GetPixel(i, j);
                if (col == target)
                {
                    color[i, j] = true;
                }
                else
                {
                    color[i, j] = false;
                }
            }
        }

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (color[i, j] == false)
                {
                    int min = int.MaxValue;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x == i && y == j)
                            {
                                continue;
                            }
                            if (color[x, y] == false)
                            {
                                continue;
                            }
                            int dis = (x - i) * (x - i) + (y - j) * (y - j);
                            if (dis < min)
                            {
                                min = dis;
                                //Debug.LogFormat("({0},{1})({2}, {3}) {4}", i, j, x, y, min);
                            }
                        }
                    }
                    distance[i, j] = -Mathf.Sqrt(min);
                    dis_min = Mathf.Min(dis_min, distance[i, j]);
                    dis_max = Mathf.Max(dis_max, distance[i, j]);

                }
                else
                {
                    int min = int.MaxValue;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x == i && y == j)
                            {
                                continue;
                            }
                            if (color[x, y] == true)
                            {
                                continue;
                            }
                            int dis = (x - i) * (x - i) + (y - j) * (y - j);
                            if (dis < min)
                            {
                                min = dis;
                                //Debug.LogFormat("({0},{1})({2}, {3}) {4}", i, j, x, y, min);
                            }
                        }
                    }
                    distance[i, j] = Mathf.Sqrt(min);
                    dis_min = Mathf.Min(dis_min, distance[i, j]);
                    dis_max = Mathf.Max(dis_max, distance[i, j]);
                }
            }
        }

        var sdf = new Texture2D(width, height);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                distance[i, j] = Mathf.Max(0, distance[i, j] - dis_min);
                float a = distance[i, j] / (dis_max - dis_min);
                sdf.SetPixel(i, j, new Color(1, 1, 1, a));
                //Debug.Log(distance[i, j]);
            }
        }

        AssetDatabase.CreateAsset(sdf, string.Format("Assets/SDF/{0}.asset", obj.name));
    }

    [MenuItem("SDF/Create_Saito")]
    public static void CreateSDF_Saito()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
        if (tex == null)
        {
            return;
        }
        Color target = Color.white;
        int width = tex.width;
        int height = tex.height;
        bool[,] color = new bool[width, height];
        int[,] distance = new int[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var col = tex.GetPixel(i, j);
                if (col == target)
                {
                    color[i, j] = true;
                }
                else
                {
                    color[i, j] = false;
                }
            }
        }

        int[] temp_width = new int[width];
        for (int j = 0; j < height; j++)
        {
            bool hasBlack = false;
            for (int i = 0; i < width; i++)
            {
                if (color[i, j] == true)
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
                    hasBlack = true;
                    temp_width[i] = 0;
                }
            }

            for (int i = width - 1; i >= 0; i--)
            {
                if (hasBlack == false)
                {
                    temp_width[i] = width;
                }
                else
                {
                    if (color[i, j] == true)
                    {
                        if (i < width - 1)
                        {
                            temp_width[i] = Mathf.Min(temp_width[i + 1] + 1, temp_width[i]);
                        }
                    }
                    else
                    {
                        temp_width[i] = 0;
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                distance[i, j] = temp_width[i] * temp_width[i];
            }
        }

        float max = 1;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int min = int.MaxValue;
                int d = distance[i, j];
                for (int y = 0; y < height; y++)
                {
                    var dis = d + (j - y) * (j - y);
                    min = Mathf.Min(dis, min);
                }
                temp_width[i] = min;
            }

            for (int i = 0; i < width; i++)
            {
                distance[i, j] = temp_width[i];
                max = Mathf.Max(max, distance[i, j]);
            }
        }
        max = Mathf.Sqrt(max);

        var sdf = new Texture2D(width, height);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float a = Mathf.Sqrt(distance[i, j]) / max;
                sdf.SetPixel(i, j, new Color(1, 1, 1, a));
                //Debug.Log(distance[i, j]);
            }
        }

        AssetDatabase.CreateAsset(sdf, string.Format("Assets/SDF/Saito_{0}.asset", obj.name));
    }
}


