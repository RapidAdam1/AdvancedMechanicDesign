using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineMesh : MonoBehaviour
{
    //Spline Stuff
    [SerializeField] private SplineContainer m_SplineContainer;
    [SerializeField] private int m_Index;

    private float3 Position;
    private float3 Tangent;
    private float3 UpVector;


    //TrackGeneration Variables
    [SerializeField] [Range(3,500)]int Resolution;
    [SerializeField] [Range(-4,4)]float m_Offset;
    [SerializeField] Vector2 Scale = Vector2.one;


    private MeshFilter m_Filter;
    private Mesh m_Mesh;

    private List<Vector3> m_Verts;
    private List<int> m_Tris;


    private void Awake()
    {
        m_Filter = GetComponent<MeshFilter>();
        m_Mesh = new Mesh { name = "Track Mesh" };
        GenerateMesh();
        m_Filter.mesh = m_Mesh;
    }
    public void OnValidate()
    {
        if (m_Mesh == null)
        {
            m_Filter = GetComponent<MeshFilter>();
            m_Mesh = new Mesh { name = "Track Mesh" };
        }
        GenerateMesh();
        m_Filter.mesh = m_Mesh;
    }

    private void GenerateMesh()
    {
        m_Mesh = new Mesh();
        m_Verts = new List<Vector3>();
        m_Tris = new List<int>();

        float step = 1f / (float)Resolution;

        for (int i = 0; i < Resolution; i++)
        {
            float t = step * i;
            m_SplineContainer.Evaluate(m_Index, t, out Position, out Tangent, out UpVector);
            float3 right = Vector3.Cross(Tangent, UpVector).normalized;
            Vector3 Point = Position + (right * m_Offset);
            BuildMesh(Point,m_Verts.Count);
        }

        if (!m_SplineContainer.Spline.Closed)
        {
            BuildEnds(m_Verts.Count);
        }

        m_Mesh.Clear();
        m_Mesh.SetVertices(m_Verts);
        m_Mesh.SetTriangles(m_Tris, 0);
        m_Mesh.RecalculateNormals();
    }

    private void BuildMesh(Vector3 SegmentPos,int RefTri)
    {
        // Z Face
        Vector3 Right = Vector3.Cross(Tangent, UpVector).normalized;
        Vector3 Up = (Vector3)UpVector;
        m_Verts.Add(SegmentPos + (-Right * Scale.x + Up * Scale.y)); //TL 0
        m_Verts.Add(SegmentPos + (Right * Scale.x + Up * Scale.y)); //TR 1
        m_Verts.Add(SegmentPos + (Right * Scale.x + -Up * Scale.y)); //BR 2
        m_Verts.Add(SegmentPos + (-Right * Scale.x + -Up * Scale.y)); //BL 3
        if(RefTri > 3)
        {
            //Top face 0 1 , 4 5
            m_Tris.AddRange(new int[] {0, 1, 4 }); // Outer
            m_Tris.AddRange(new int[] {4, 1, 5 }); // Inner

            //Left Face 1 2 ,5 6
            m_Tris.AddRange(new int[] {1, 2, 5 }); //Bottom
            m_Tris.AddRange(new int[] {5, 2, 6 }); //Top

            // Outer Face 0 3, 4 7
            m_Tris.AddRange(new int[] {0, 4, 3 }); //Bottom
            m_Tris.AddRange(new int[] {3, 4, 7 }); //Top

            //Bottom 2 3, 5 7
            m_Tris.AddRange(new int[] { 6, 2, 3}); // Outer
            m_Tris.AddRange(new int[] { 6, 3, 7 }); // Inner
        }
    }

    void BuildEnds(int Verts)
    {
        //First
        m_Tris.AddRange(new int[] { 3, 1, 0 });
        m_Tris.AddRange(new int[] { 3, 2, 1 });

        //Last
        m_Tris.AddRange(new int[] { Verts -2,   Verts -4,   Verts-3 });
        m_Tris.AddRange(new int[] { Verts -2,   Verts -1,   Verts-4 });

    }

    private void OnDrawGizmos()
    {
/*        Handles.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < m_Verts.Count; i++)
        {
            Handles.SphereHandleCap(i, m_Verts[i], Quaternion.identity, 0.1f, EventType.Repaint);
        }*/
    }
}
