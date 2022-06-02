using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class QuadricErrorMetrics
{
    private static List<Vector3> m_Vertices = null;
    private static Dictionary<int, List<Plane>> m_Planes = null;

    public static void Simplification(Mesh mesh)
    {
        MinHeap heap = new MinHeap();
        int limit = 1;
        float threshold = 0f;
        HashSet<Pair> sides = new HashSet<Pair>();
        m_Vertices = mesh.vertices.ToList();
        m_Planes = new Dictionary<int, List<Plane>>();
        // 1. 计算最初的网格中，每个顶点的 Q 矩阵（Q 为经过顶点的所有三角面的 Kp 之和）
        int length = mesh.triangles.Length;
        for (int i = 0; i < length; i += 3)
        {
            int index1 = mesh.triangles[i];
            int index2 = mesh.triangles[i + 1];
            int index3 = mesh.triangles[i + 2];
            Plane plane = new Plane(index1, index2, index3, m_Vertices);

            m_Planes.TryGetValue(index1, out List<Plane> l1);
            if (l1 == null)
            {
                l1 = new List<Plane>();
            }
            m_Planes[index1] = l1;
            l1.Add(plane);

            m_Planes.TryGetValue(index1, out List<Plane> l2);
            if (l2 == null)
            {
                l2 = new List<Plane>();
            }
            m_Planes[index2] = l2;
            l2.Add(plane);

            m_Planes.TryGetValue(index1, out List<Plane> l3);
            if (l3 == null)
            {
                l3 = new List<Plane>();
            }
            m_Planes[index3] = l3;
            l3.Add(plane);

            Pair side12 = new Pair(index1, index2);
            Pair side13 = new Pair(index1, index3);
            Pair side23 = new Pair(index2, index3);
            sides.Add(side12);
            sides.Add(side13);
            sides.Add(side23);

        }

        //HashSet<Pair> pairs = new HashSet<Pair>();
        //// 2. 选择所有有效的顶点对
        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //    for (int j = 0; j < mesh.vertexCount; j++)
        //    {
        //        if (i == j)
        //        {
        //            continue;
        //        }
        //        Pair s = new Pair(i, j);
        //        if (sides.Contains(s) == false)
        //        {
        //            Vector3 vi = mesh.vertices[i];
        //            Vector3 vj = mesh.vertices[j];
        //            Vector3 v = vj - vi;
        //            float d = v.x * v.x + v.y * v.y + v.z + v.z;
        //            if (d > threshold)
        //            {
        //                continue;
        //            }
        //        }
        //        pairs.Add(s);
        //    }
        //}
        // 3. 计算所有队的误差度量，加入堆中
        foreach (var pair in sides)
        {
            var node = CreateNode(pair);
            heap.Push(node);
        }
        // 4.
        HashSet<int> removeList = new HashSet<int>();
        while (heap.Count > 0 && limit > 0)
        {
            var node = heap.Pop();
            // 如果当前对的其中一个顶点已经替换为新顶点，则此对已经更新为其他点对，不再处理
            if (removeList.Contains(node.p.index1) || removeList.Contains(node.p.index2))
            {
                continue;
            }
            var list = new List<Plane>();
            int[] removeIndex = new int[] { node.p.index1, node.p.index2 };
            removeList.Add(node.p.index1);
            removeList.Add(node.p.index2);
            // 删除顶点
            foreach (var index in removeIndex)
            {
                var l = m_Planes[index];
                foreach (var item in l)
                {
                    if (item.index1 == index)
                    {
                        if(item.index2 == index || item.index3 == index)
                        {
                            list.Remove(item);
                        }
                        else
                        {
                            item.index1 = node.i;
                            list.Add(item);
                        }
                    }
                    else if (item.index2 == index)
                    {
                        if (item.index1 == index || item.index3 == index)
                        {
                            list.Remove(item);
                        }
                        else
                        {
                            item.index2 = node.i;
                            list.Add(item);
                        }
                    }
                    else if (item.index3 == index)
                    {
                        if (item.index1 == index || item.index2 == index)
                        {
                            list.Remove(item);
                        }
                        else
                        {
                            item.index3 = node.i;
                            list.Add(item);
                        }
                    }
                }
                m_Planes.Remove(index);
            }
            m_Planes[node.i] = list;
            // 将新产生的边再加入堆中
            foreach (var item in list)
            {
                var side12 = new Pair(item.index1, item.index2);
                var side13 = new Pair(item.index1, item.index3);
                var side23 = new Pair(item.index2, item.index3);
                if (sides.Contains(side12) == false)
                {
                    sides.Add(side12);
                    var n = CreateNode(side12);
                    heap.Push(n);
                }
                if (sides.Contains(side13) == false)
                {
                    sides.Add(side13);
                    var n = CreateNode(side13);
                    heap.Push(n);
                }
                if (sides.Contains(side23) == false)
                {
                    sides.Add(side23);
                    var n = CreateNode(side23);
                    heap.Push(n);
                }
            }
            sides.Remove(node.p);
            --limit;
        }

        int curIndex = 0;
        Dictionary<int, int> map = new Dictionary<int, int>();
        HashSet<Triangle> hash = new HashSet<Triangle>();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Plane> planes = new List<Plane>();
        foreach (var item in m_Planes)
        {
            map[item.Key] = curIndex++;
            foreach (var p in item.Value)
            {
                if (map.ContainsKey(p.index1) && map.ContainsKey(p.index2) && map.ContainsKey(p.index3))
                {
                    var tri = new Triangle(p.index1, p.index2, p.index3);
                    if (hash.Contains(tri) == false)
                    {
                        hash.Add(tri);
                        planes.Add(p);
                    }
                }
            }
            verts.Add(m_Vertices[item.Key]);
        }
        foreach (var item in planes)
        {
            tris.Add(map[item.index1]);
            tris.Add(map[item.index2]);
            tris.Add(map[item.index3]);
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
    }

    private static Node CreateNode(Pair pair)
    {
        int index1 = pair.index1;
        int index2 = pair.index2;
        int index12 = m_Vertices.Count;
        Vector3 v1 = m_Vertices[index1];
        Vector3 v2 = m_Vertices[index2];
        Vector3 v12 = (v1 + v2) / 2;
        var Q1 = CalculateQ(index1);
        var Q2 = CalculateQ(index2);
        var Q12 = AddMatrix(Q1, Q2);
        float e1 = CalulateErrorMetric(v1, Q1);
        float e2 = CalulateErrorMetric(v2, Q2);
        float e12 = CalulateErrorMetric(v12, Q12);

        Vector3 v;
        float e;
        if (e1 < e2)
        {
            v = v1;
            e = e1;
        }
        else
        {
            v = v2;
            e = e2;
        }
        if (e12 < e)
        {
            v = v12;
            e = e12;
        }
        int index = m_Vertices.Count;
        m_Vertices.Add(v);
        Node node = new Node(index1, index2, index, e);
        return node;
    }

    private static Matrix4x4 AddMatrix(Matrix4x4 m1, Matrix4x4 m2)
    {
        Matrix4x4 m = Matrix4x4.zero;
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                m[i, j] = m1[i, j] + m2[i, j];
            }
        }
        return m;
    }

    private static float CalulateErrorMetric(int index)
    {
        Matrix4x4 Q = CalculateQ(index);
        float error = CalulateErrorMetric(m_Vertices[index], Q);
        return error;
    }

    private static Matrix4x4 CalculateQ(int index)
    {
        var list = m_Planes[index];
        Vector3 vertex = m_Vertices[index];
        Vector4 v = new Vector4(vertex.x, vertex.y, vertex.z, 1);
        Matrix4x4 Q = Matrix4x4.zero;
        foreach (var item in list)
        {
            Q = AddMatrix(Q, item.Kp);
        }
        return Q;
    }

    private static float CalulateErrorMetric(Vector3 vertex, Matrix4x4 Q)
    {
        Vector4 v = new Vector4(vertex.x, vertex.y, vertex.z, 1);
        Vector4 temp = Q * v;
        float error = temp.x * v.x + temp.y * v.y + temp.z * v.z + temp.w + v.w;
        return error;
    }
}


public class Plane
{
    public List<Vector3> vertices;

    public int index1;
    public int index2;
    public int index3;

    private float A;
    private float B;
    private float C;
    private float D;

    /// <summary>
    /// 基本误差二次曲面（fundamental error quadric）
    /// </summary>
    public Matrix4x4 Kp;

    public Plane(int index1, int index2, int index3, List<Vector3> vertices)
    {
        this.index1 = index1;
        this.index2 = index2;
        this.index3 = index3;
        this.vertices = vertices;

        Vector3 v1 = vertices[index1];
        Vector3 v2 = vertices[index2];
        Vector3 v3 = vertices[index3];
        // 平面法向量n = 向量v1v2 x 向量 v1v3 = (A, B, C) 
        // 对于平面上任意两点 MM', 有 n * MM' = 0 
        // 则平面方程为 A * (x - x0) + B * (y - y0) + C * (z - z0) = 0
        // 即 A * x + B * y + C * z - (A * x0 + B * y0 + C * z0) = 0
        A = (v2.y - v1.y) * (v3.z - v1.z) - (v3.y - v1.y) * (v2.z - v1.z);
        B = (v2.x - v1.x) * (v3.z - v1.z) - (v3.x - v1.x) * (v2.z - v1.z);
        C = (v2.x - v1.x) * (v3.y - v1.y) - (v3.x - v1.x) * (v2.y - v1.y);
        D = -(A * v1.x + B * v1.y + C * v1.z);

        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetRow(0, new Vector4(A * A, A * B, A * C, A * D));
        matrix.SetRow(1, new Vector4(A * B, B * B, B * C, B * D));
        matrix.SetRow(2, new Vector4(A * C, B * C, C * C, C * D));
        matrix.SetRow(3, new Vector4(A * D, B * D, C * D, D * D));

        Kp = matrix;
    }

    public bool IsSide(Pair p)
    {
        int i1 = p.index1;
        int i2 = p.index2;
        if (i1 == i2)
        {
            return false;
        }
        if ((i1 != index1) && (i1 != index2) && (i1 != index3))
        {
            return false;
        }
        if ((i2 != index1) && (i2 != index2) && (i2 != index3))
        {
            return false;
        }
        return true;
    }
}

public struct Pair
{
    public int index1;
    public int index2;

    public Pair(int i1, int i2)
    {
        index1 = Math.Min(i1, i2);
        index2 = Math.Max(i1, i2);
    }
}

public struct Triangle
{
    public int index1;
    public int index2;
    public int index3;

    public Triangle(int i1, int i2, int i3)
    {
        if (i1 < i2)
        {
            if (i1 < i3)
            {
                index1 = i1;
                if (i2 < i3)
                {
                    index2 = i2;
                    index3 = i3;
                }
                else
                {
                    index2 = i3;
                    index3 = i2;
                }
            }
            else
            {
                index1 = i3;
                index2 = i1;
                index3 = i2;
            }
        }
        else
        {
            // i2 < i1 < i3
            if (i1 < i3)
            {
                index1 = i2;
                index2 = i1;
                index3 = i3;
            }
            else
            {
                // i2 < i3 < i1
                if (i2 < i3)
                {
                    index1 = i2;
                    index2 = i3;
                    index3 = i1;
                }
                else
                {
                    // i3 < i2 < i1
                    index1 = i3;
                    index2 = i2;
                    index3 = i1;
                }
            }
        }
    }
}

public struct Node
{
    public Pair p;
    public int i;
    public float e;

    public Node(int index1, int index2, int index12, float error)
    {
        p = new Pair(index1, index2);
        i = index12;
        e = error;
    }

    public static bool operator >(Node left, Node right)
    {
        if (left.e > right.e)
        {
            return true;
        }
        return false;
    }

    public static bool operator <(Node left, Node right)
    {
        if (left.e < right.e)
        {
            return true;
        }
        return false;
    }
}

public class MinHeap
{
    private List<Node> nodes = new List<Node>();

    public int Count
    {
        get
        {
            return nodes.Count;
        }
    }

    private bool IsLeaf(int index)
    {
        if (LeftChildIndex(index) < 0)
        {
            return true;
        }
        return false;
    }

    private int LeftChildIndex(int index)
    {
        int left = 2 * index + 1;
        if (left >= nodes.Count)
        {
            left = -1;
        }
        return left;
    }

    private int RightChildIndex(int index)
    {
        int right = 2 * index + 2;
        if (right >= nodes.Count)
        {
            right = -1;
        }
        return right;
    }

    private int ParentIndex(int index)
    {
        int parent = (index - 1) / 2;
        if (index == 0)
        {
            parent = -1;
        }
        return parent;
    }

    public void Push(Node n)
    {
        nodes.Add(n);
        int index = nodes.Count - 1;
        while (index > 0)
        {
            int parent = ParentIndex(index);
            if (nodes[index] < nodes[parent])
            {
                var temp = nodes[parent];
                nodes[parent] = nodes[index];
                nodes[index] = temp;
                index = parent;
            }
            else
            {
                break;
            }
        }
    }

    public Node Pop()
    {
        var min = nodes[0];
        int index = 0;
        nodes[index] = nodes[nodes.Count - 1];
        nodes.RemoveAt(nodes.Count - 1);
        while (IsLeaf(index) == false)
        {
            int left = LeftChildIndex(index);
            int right = RightChildIndex(index);
            if (right < 0)
            {
                if (nodes[index] > nodes[left])
                {
                    var temp = nodes[index];
                    nodes[index] = nodes[left];
                    nodes[left] = temp;
                    index = left;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (nodes[left] < nodes[right])
                {
                    if (nodes[index] > nodes[left])
                    {
                        var temp = nodes[index];
                        nodes[index] = nodes[left];
                        nodes[left] = temp;
                        index = left;
                    }
                }
                else
                {
                    if (nodes[index] > nodes[right])
                    {
                        var temp = nodes[index];
                        nodes[index] = nodes[right];
                        nodes[right] = temp;
                        index = right;
                    }
                }
            }
        }
        return min;
    }
}
