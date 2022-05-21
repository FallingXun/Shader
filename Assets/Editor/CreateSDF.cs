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


    public static void Log(int[,] distance)
    {
        int width = distance.GetLength(0);
        int height = distance.GetLength(1);
        string log = "";
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                log += distance[i, j] + ", ";
            }
            log += "\n";
        }
        Debug.Log(log);
    }

    [MenuItem("SDF/Create_RasterScanningTest")]
    public static void Create_RasterScanningTest()
    {
        var color1 = new bool[,]
        {
           { false,false,false,false,false,false },
           { false,false,false,false,false,false },
           { true,false,false,false,false,false },
           { false,false,false,false,false,false },
           { false,true,false,false,false,false },
           { false,false,false,true,false,false },
        };
        var color2 = new bool[,]
       {
           { true,true,true,true,true,true },
           { true,true,true,true,true,true },
           { false,true,true,true,true,true },
           { true,true,true,true,true,true },
           { true,false,true,true,true,true },
           { true,true,true,false,true,true },
       };
        var s1 = RasterScanning.Compute(color1);
        var s2 = RasterScanning.Compute(color2);
        int width = s1.GetLength(0);
        int height = s1.GetLength(1);
        string log = "";
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                log += (/*s1[i, j].GetDistance() - */s2[i,j].GetDistance()) + ", ";
            }
            log += "\n";
        }
        Debug.Log(log);
    }

    [MenuItem("SDF/Create_BruteForce")]
    public static void CreateSDF_BruteForce()
    {
        var color = GetColor();
        if (color == null)
        {
            return;
        }
        var sdf = BruteForce.CreateSDF(color);
        int width = sdf.GetLength(0);
        int height = sdf.GetLength(1);
        var tex = new Texture2D(width, height);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                tex.SetPixel(i, j, new Color(1, 1, 1, sdf[i, j]));
            }
        }

        var obj = Selection.activeObject;
        AssetDatabase.CreateAsset(tex, string.Format("Assets/SDF/BruteForce_{0}.asset", obj.name));
        AssetDatabase.Refresh();
    }

    [MenuItem("SDF/Create_IndependentScanning")]
    public static void CreateSDF_IndependentScanning()
    {
        var color = GetColor();
        if (color == null)
        {
            return;
        }
        var sdf = IndependentScanning.CreateSDF(color);
        int width = sdf.GetLength(0);
        int height = sdf.GetLength(1);
        var tex = new Texture2D(width, height);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                tex.SetPixel(i, j, new Color(1, 1, 1, sdf[i, j]));
            }
        }

        var obj = Selection.activeObject;
        AssetDatabase.CreateAsset(tex, string.Format("Assets/SDF/IndependentScanning_{0}.asset", obj.name));
        AssetDatabase.Refresh();
    }

    [MenuItem("SDF/Create_RasterScanning")]
    public static void CreateSDF_RasterScanning()
    {
        var color = GetColor();
        if (color == null)
        {
            return;
        }
        var sdf = RasterScanning.CreateSDF(color);
        int width = sdf.GetLength(0);
        int height = sdf.GetLength(1);
        var tex = new Texture2D(width, height);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                tex.SetPixel(i, j, new Color(1, 1, 1, sdf[i, j]));
            }
        }

        var obj = Selection.activeObject;
        AssetDatabase.CreateAsset(tex, string.Format("Assets/SDF/RasterScanning_{0}.asset", obj.name));
        AssetDatabase.Refresh();
    }


    [MenuItem("SDF/Create_Test")]
    public static void Create_Test()
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
        Texture2D source = tex;
        Texture2D destination = new Texture2D(16, 16);
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        int targetWidth = destination.width;
        int targetHeight = destination.height;

        bool[,] pixels = new bool[sourceWidth, sourceHeight];
        float[,] targetPixels = new float[targetWidth, targetHeight];
        Debug.Log("sourceWidth" + sourceWidth);
        Debug.Log("sourceHeight" + sourceHeight);
        int x, y;
        Color targetColor = Color.white;
        for (y = 0; y < sourceWidth; y++)
        {
            for (x = 0; x < sourceHeight; x++)
            {
                if (source.GetPixel(x, y) == Color.white)
                    pixels[x, y] = true;
                else
                    pixels[x, y] = false;
            }
        }


        int gapX = sourceWidth / targetWidth;
        int gapY = sourceHeight / targetHeight;
        int MAX_SEARCH_DIST = 512;
        int minx, maxx, miny, maxy;
        float max_distance = -MAX_SEARCH_DIST;
        float min_distance = MAX_SEARCH_DIST;

        for (x = 0; x < targetWidth; x++)
        {
            for (y = 0; y < targetHeight; y++)
            {
                int sourceX = x * gapX;
                int sourceY = y * gapY;
                int min = MAX_SEARCH_DIST;
                minx = sourceX - MAX_SEARCH_DIST;
                if (minx < 0)
                {
                    minx = 0;
                }
                miny = sourceY - MAX_SEARCH_DIST;
                if (miny < 0)
                {
                    miny = 0;
                }
                maxx = sourceX + MAX_SEARCH_DIST;
                if (maxx > (int)sourceWidth)
                {
                    maxx = sourceWidth;
                }
                maxy = sourceY + MAX_SEARCH_DIST;
                if (maxy > (int)sourceHeight)
                {
                    maxy = sourceHeight;
                }
                int dx, dy, iy, ix, distance;
                bool sourceIsInside = pixels[sourceX, sourceY];
                if (sourceIsInside)
                {
                    for (iy = miny; iy < maxy; iy++)
                    {
                        dy = iy - sourceY;
                        dy *= dy;
                        for (ix = minx; ix < maxx; ix++)
                        {
                            bool targetIsInside = pixels[ix, iy];
                            if (targetIsInside)
                            {
                                continue;
                            }
                            dx = ix - sourceX;
                            distance = (int)Mathf.Sqrt(dx * dx + dy);
                            if (distance < min)
                            {
                                min = distance;
                            }
                        }
                    }

                    if (min > max_distance)
                    {
                        max_distance = min;
                    }
                    targetPixels[x, y] = min;
                }
                else
                {
                    for (iy = miny; iy < maxy; iy++)
                    {
                        dy = iy - sourceY;
                        dy *= dy;
                        for (ix = minx; ix < maxx; ix++)
                        {
                            bool targetIsInside = pixels[ix, iy];
                            if (!targetIsInside)
                            {
                                continue;
                            }
                            dx = ix - sourceX;
                            distance = (int)Mathf.Sqrt(dx * dx + dy);
                            if (distance < min)
                            {
                                min = distance;
                            }
                        }
                    }

                    if (-min < min_distance)
                    {
                        min_distance = -min;
                    }
                    targetPixels[x, y] = -min;
                }
            }
        }

        //EXPORT texture
        float clampDist = max_distance - min_distance;
        for (x = 0; x < targetWidth; x++)
        {
            for (y = 0; y < targetHeight; y++)
            {
                targetPixels[x, y] -= min_distance;
                float value = targetPixels[x, y] / clampDist;
                destination.SetPixel(x, y, new Color(1, 1, 1, value));
            }
        }

        AssetDatabase.CreateAsset(destination, string.Format("Assets/SDF/Test_{0}.asset", obj.name));
    }

    public static bool[,] GetColor()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            return null;
        }
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
        if (tex == null)
        {
            return null;
        }
        Color target = Color.white;
        int width = tex.width;
        int height = tex.height;
        bool[,] color = new bool[width, height];

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

        return color;
    }
}


