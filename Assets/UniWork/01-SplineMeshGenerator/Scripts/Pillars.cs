using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Splines;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class Pillars : MonoBehaviour
{
    [SerializeField] private SplineContainer m_SplineContainer;
    private int m_Index;

    private MeshFilter m_Filter;
    private Mesh m_Mesh;

    private List<Vector3> m_Vertices;
    private List<int> m_Tris;


    private float3 Position;
    private float3 Tangent;
    private float3 UpVector;
   
    RaycastHit HitResult;
    RaycastHit TempHitResult;

    int m_Resolution;
    protected float m_Offset;
    protected Vector2 m_Scale = Vector2.one;
    float PillarHeight = 0.5f;

    public void Init(SplineContainer Spline, int SplineIndex, float Offset,int PillarRes,float MinPillarHeight, Vector2 Scale)
    {
        m_SplineContainer = Spline;
        m_Index = SplineIndex;
        m_Scale = Scale;
        m_Offset = Offset;
        m_Resolution = PillarRes;
        PillarHeight = MinPillarHeight;
        m_Filter = GetComponent<MeshFilter>();
        m_Mesh = new Mesh { name = "Track Mesh" };
        GenerateMesh();
        m_Filter.mesh = m_Mesh;

    }

    private void Awake()
    {
        m_Filter = GetComponent<MeshFilter>();
        m_Mesh = new Mesh { name = "ProceduralMesh" };
        GenerateMesh();
        m_Filter.mesh = m_Mesh;
    }

    public void OnValidate()
    {
        if (m_Mesh == null)
        {
            m_Filter = GetComponent<MeshFilter>();
            m_Mesh = new Mesh { name = "Proc Mesh" };
        }
        GenerateMesh();
        m_Filter.mesh = m_Mesh;
    }
    private void GenerateMesh()
    {
        m_Mesh = new Mesh();
        m_Vertices = new List<Vector3>();
        m_Tris = new List<int>();

        float step = 1f / (float)m_Resolution;

        for (int i = 0; i < m_Resolution; i++)
        {
            float t = step * i;
            m_SplineContainer.Evaluate(m_Index, t, out Position, out Tangent, out UpVector);
            float3 right = Vector3.Cross(Tangent, UpVector).normalized;

            Ray CheckRay = new Ray(Position, Vector3.down);
            if(Physics.Raycast(CheckRay, out HitResult))
            {
                if(HitResult.collider.tag == "Ground")
                {
                    if (HitResult.distance > PillarHeight)
                    {
                        Vector3 Point1 = (Vector3)Position + (Vector3.Normalize(right) * m_Offset);
                        Vector3 Point2 = (Vector3)Position+ (Vector3.Normalize(right) * -m_Offset);

                        GenCube(Point1, m_Vertices.Count);
                        GenCube(Point2 ,m_Vertices.Count);
                    }
                }
            }
        }


        m_Mesh.Clear();
        m_Mesh.SetVertices(m_Vertices);
        m_Mesh.SetTriangles(m_Tris, 0);
        m_Mesh.RecalculateNormals();
    }

    private void GenCube(Vector3 RefPos, int RefTri)
    {
        Ray FloorCheckRay = new Ray(RefPos, Vector3.down);
        Physics.Raycast(FloorCheckRay, out TempHitResult);
        float FloorDistance = TempHitResult.distance + 0.2f;
        
        // - Z Face
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x,0, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x,0, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, -FloorDistance, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, -FloorDistance, -m_Scale.y));
        m_Tris.Add(RefTri + 0);
        m_Tris.Add(RefTri + 1);
        m_Tris.Add(RefTri + 3);
        m_Tris.Add(RefTri + 1);
        m_Tris.Add(RefTri + 2);
        m_Tris.Add(RefTri + 3);

        // + y Face
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, 0, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, 0, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, -FloorDistance, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, -FloorDistance, m_Scale.y));
        m_Tris.Add(RefTri + 4);
        m_Tris.Add(RefTri + 7);
        m_Tris.Add(RefTri + 5);
        m_Tris.Add(RefTri + 5);
        m_Tris.Add(RefTri + 7);
        m_Tris.Add(RefTri + 6);

        // - X Face
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, 0, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, 0, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, -FloorDistance, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, -FloorDistance, m_Scale.y));
        m_Tris.Add(RefTri + 8);
        m_Tris.Add(RefTri + 9);
        m_Tris.Add(RefTri + 11);
        m_Tris.Add(RefTri + 9);
        m_Tris.Add(RefTri + 10);
        m_Tris.Add(RefTri + 11);

        // + X Face
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, 0, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, 0, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, -FloorDistance, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, -FloorDistance, m_Scale.y));
        m_Tris.Add(RefTri + 12);
        m_Tris.Add(RefTri + 15);
        m_Tris.Add(RefTri + 13);
        m_Tris.Add(RefTri + 13);
        m_Tris.Add(RefTri + 15);
        m_Tris.Add(RefTri + 14);

        // - Y Face
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, -FloorDistance, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, -FloorDistance, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, -FloorDistance, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, -FloorDistance, m_Scale.y));
        m_Tris.Add(RefTri + 16);
        m_Tris.Add(RefTri + 17);
        m_Tris.Add(RefTri + 19);
        m_Tris.Add(RefTri + 17);
        m_Tris.Add(RefTri + 18);
        m_Tris.Add(RefTri + 19);


        // + Y Face
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, 0, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, 0, -m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(m_Scale.x, 0, m_Scale.y));
        m_Vertices.Add(RefPos + new Vector3(-m_Scale.x, 0, m_Scale.y));
        m_Tris.Add(RefTri + 20);
        m_Tris.Add(RefTri + 23);
        m_Tris.Add(RefTri + 21);
        m_Tris.Add(RefTri + 21);
        m_Tris.Add(RefTri + 23);
        m_Tris.Add(RefTri + 22);

    }

}
