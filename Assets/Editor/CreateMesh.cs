using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateMesh : Editor
{
    [MenuItem("Mesh/SaveMesh")]
    public static void SaveMesh()
    {
        var go = Selection.activeGameObject;
        if (go == null)
        {
            return;
        }
        var filter = go.GetComponent<MeshFilter>();
        if (filter == null)
        {
            return;
        }
        var mesh = new Mesh();
        mesh.vertices = filter.sharedMesh.vertices;
        mesh.triangles = filter.sharedMesh.triangles;
        AssetDatabase.CreateAsset(mesh, string.Format("Assets/Mesh/{0}.asset", go.name));
        AssetDatabase.Refresh();
    }

    [MenuItem("Mesh/CreateMeshCube")]
    public static void CreateMeshCube()
    {
        var mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(1, 1, 0);
        vertices[3] = new Vector3(0, 1, 0);
        vertices[4] = new Vector3(0, 0, 1);
        vertices[5] = new Vector3(1, 0, 1);
        vertices[6] = new Vector3(1, 1, 1);
        vertices[7] = new Vector3(0, 1, 1);
        mesh.vertices = vertices;

        int[] triangles = new int[6 * 2 * 3];
        int i = 0;
        triangles[i++] = 0;
        triangles[i++] = 4;
        triangles[i++] = 1;

        triangles[i++] = 4;
        triangles[i++] = 5;
        triangles[i++] = 1;

        triangles[i++] = 1;
        triangles[i++] = 5;
        triangles[i++] = 2;

        triangles[i++] = 5;
        triangles[i++] = 6;
        triangles[i++] = 2;

        triangles[i++] = 2;
        triangles[i++] = 6;
        triangles[i++] = 3;

        triangles[i++] = 6;
        triangles[i++] = 7;
        triangles[i++] = 3;

        triangles[i++] = 3;
        triangles[i++] = 7;
        triangles[i++] = 0;

        triangles[i++] = 0;
        triangles[i++] = 7;
        triangles[i++] = 4;

        triangles[i++] = 4;
        triangles[i++] = 7;
        triangles[i++] = 5;

        triangles[i++] = 7;
        triangles[i++] = 6;
        triangles[i++] = 5;

        triangles[i++] = 3;
        triangles[i++] = 0;
        triangles[i++] = 1;

        triangles[i++] = 3;
        triangles[i++] = 1;
        triangles[i++] = 2;

        mesh.triangles = triangles;
        AssetDatabase.CreateAsset(mesh, string.Format("Assets/Mesh/{0}.asset", "Cube"));
        AssetDatabase.Refresh();
    }


    [MenuItem("Mesh/CreateMeshPyramid")]
    public static void CreateMeshPyramid()
    {
        var mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, -1);
        vertices[1] = new Vector3(0, 0, 1);
        vertices[2] = new Vector3(1.732f, 0, 0f);
        vertices[3] = new Vector3(0.577f, 1.732f, 0);
        mesh.vertices = vertices;

        int[] triangles = new int[4 * 3];
        int i = 0;
        triangles[i++] = 0;
        triangles[i++] = 3;
        triangles[i++] = 2;

        triangles[i++] = 0;
        triangles[i++] = 1;
        triangles[i++] = 3;

        triangles[i++] = 2;
        triangles[i++] = 3;
        triangles[i++] = 1;

        triangles[i++] = 0;
        triangles[i++] = 2;
        triangles[i++] = 1;

        mesh.triangles = triangles;
        AssetDatabase.CreateAsset(mesh, string.Format("Assets/Mesh/{0}.asset", "Pyramid"));
        AssetDatabase.Refresh();
    }

    [MenuItem("Mesh/SubdivisionMesh")]
    public static void Subdivision()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(AssetDatabase.GetAssetPath(obj));
        if (mesh == null)
        {
            return;
        }

        List<int> triList = new List<int>();
        List<Vector3> vertList = mesh.vertices.ToList();
        Dictionary<Side, SideData> sideDic = new Dictionary<Side, SideData>();
        HashSet<int>[] vertMap = new HashSet<int>[mesh.vertices.Length];
        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int index1 = triangles[i];
            if (vertMap[index1] == null)
            {
                vertMap[index1] = new HashSet<int>();
            }
            int index2 = triangles[i + 1];
            if (vertMap[index2] == null)
            {
                vertMap[index2] = new HashSet<int>();
            }
            int index3 = triangles[i + 2];
            if (vertMap[index3] == null)
            {
                vertMap[index3] = new HashSet<int>();
            }
            var v1 = mesh.vertices[index1];
            var v2 = mesh.vertices[index2];
            var v3 = mesh.vertices[index3];
            Side side12 = new Side(index1, index2);
            if (sideDic.TryGetValue(side12, out SideData data12) == false)
            {
                var v12 = (v1 + v2);
                data12 = new SideData(vertList.Count);
                data12.SetIndex(index3);
                vertList.Add(v12);
                sideDic[side12] = data12;
                vertMap[index1].Add(data12.index_size);
                vertMap[index2].Add(data12.index_size);
            }
            else
            {
                data12.SetIndex(index3);
            }
            Side side13 = new Side(index1, index3);
            if (sideDic.TryGetValue(side13, out SideData data13) == false)
            {
                var v13 = (v1 + v3);
                data13 = new SideData(vertList.Count);
                data13.SetIndex(index2);
                vertList.Add(v13);
                sideDic[side13] = data13;
                vertMap[index1].Add(data13.index_size);
                vertMap[index3].Add(data13.index_size);
            }
            else
            {
                data12.SetIndex(index2);
            }
            Side side23 = new Side(index2, index3);
            if (sideDic.TryGetValue(side23, out SideData data23) == false)
            {
                var v23 = (v2 + v3);
                data23 = new SideData(vertList.Count);
                data23.SetIndex(index1);
                vertList.Add(v23);
                sideDic[side23] = data23;
                vertMap[index2].Add(data23.index_size);
                vertMap[index3].Add(data23.index_size);
            }
            else
            {
                data23.SetIndex(index1);
            }

            triList.Add(index1);
            triList.Add(data12.index_size);
            triList.Add(data13.index_size);

            triList.Add(data12.index_size);
            triList.Add(index2);
            triList.Add(data23.index_size);

            triList.Add(data23.index_size);
            triList.Add(index3);
            triList.Add(data13.index_size);

            triList.Add(data12.index_size);
            triList.Add(data23.index_size);
            triList.Add(data13.index_size);
        }

        foreach (var item in sideDic)
        {
            if (item.Value.index_min < 0)
            {
                vertList[item.Value.index_size] = (vertList[item.Key.index_min] + vertList[item.Key.index_max]) / 2;
            }
            else
            {
                vertList[item.Value.index_size] = (vertList[item.Key.index_min] + vertList[item.Key.index_max]) * 3 / 8 + (vertList[item.Value.index_min] + vertList[item.Value.index_max]) / 8;
            }
        }

        for (int i = 0; i < vertMap.Length; i++)
        {
            float n = vertMap[i].Count;
            float u = (5f / 8 - Mathf.Pow((3f / 8 + 1f / 4 * Mathf.Cos(2 * Mathf.PI / n)), 2)) / n;
            Vector3 pos = vertList[i] * (1 - n * u);
            foreach (var index in vertMap[i])
            {
                pos += u * vertList[index];
            }
            vertList[i] = pos;
        }

        mesh.vertices = vertList.ToArray();
        mesh.triangles = triList.ToArray();

    }

    [MenuItem("Mesh/SimplificationMesh")]
    public static void Simplification()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(AssetDatabase.GetAssetPath(obj));
        if (mesh == null)
        {
            return;
        }
        QuadricErrorMetrics.Simplification(mesh);
    }
}

public struct Side
{
    public int index_min;
    public int index_max;

    public Side(int index1, int index2)
    {
        index_min = Mathf.Min(index1, index2);
        index_max = Mathf.Max(index1, index2);
    }
}

public struct SideData
{
    public int index_size;
    public int index_min;
    public int index_max;

    public SideData(int index)
    {
        index_size = index;
        index_min = -1;
        index_max = -1;
    }

    public void SetIndex(int index)
    {
        if (index < 0)
        {
            return;
        }
        if (index_max == index)
        {
            return;
        }
        if (index_max < 0)
        {
            index_max = index;
        }
        else
        {
            index_min = index;
        }
    }
}