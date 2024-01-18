using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SleeperMesh : MonoBehaviour
{
    
    private MeshFilter m_Filter;
    private Mesh m_Mesh;
    
    private List<Vector3> m_Vertices; 
    private List<int> m_Tris;

    [SerializeField] Vector2 ChunkScale;
    [SerializeField] private Vector3 m_CellOffset;
    [SerializeField] int SleeperCount;

    
    [SerializeField]private SplineContainer m_SplineContainer;
    private int m_Index;

    [SerializeField] float SleeperWidth = 1;
    private float3 Position;
    private float3 Tangent;
    private float3 UpVector;

    public Vector3 CellOffset
    {
        get { return m_CellOffset; }
        set { m_CellOffset = value; }
    }

    private void Awake()
    {
        m_Filter = GetComponent<MeshFilter>();
        m_Mesh = new Mesh {name = "ProceduralMesh" };
        GenMesh();
        m_Filter.mesh = m_Mesh;
    }

    public void OnValidate()
    {
        if (m_Mesh == null)
        {
            m_Filter = GetComponent<MeshFilter>();
            m_Mesh = new Mesh { name = "Proc Mesh" };
        }
        GenMesh();
        m_Filter.mesh = m_Mesh;
    }

    private void GenMesh()
    {
        m_Vertices = new List<Vector3>();
        m_Tris = new List<int>();
        Vector3 Offset = m_CellOffset + (Vector3)ChunkScale *2;

        m_Mesh = new Mesh();
        m_Vertices = new List<Vector3>();
        m_Tris = new List<int>();

        float step = 1f / (float)SleeperCount;

        for (int i = 0; i < SleeperCount; i++)
        {
            float t = step * i;
            m_SplineContainer.Evaluate(m_Index, t, out Position, out Tangent, out UpVector);
            Vector3 Point1 = Position;
            GenCube(Point1,m_Vertices.Count);
        }

        //GenerateFirstMeshEnd


        //GenerateLastMeshEnd
        m_Mesh.Clear();
        m_Mesh.SetVertices(m_Vertices);
        m_Mesh.SetTriangles(m_Tris, 0);
        //m_Mesh.SetNormals(m_Vertices);
        m_Mesh.RecalculateNormals();
    }
    private void GenCube(Vector3 RefPos,int RefTri)
    {
        Vector3 right = Vector3.Cross(Tangent, UpVector).normalized;
        Vector3 Up = (Vector3)UpVector;
        Vector3 ForwardVector = Tangent;
        // - Z Face
        m_Vertices.Add(RefPos + (-right * SleeperWidth + Up * ChunkScale.y + -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + Up * ChunkScale.y + -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + -Up * ChunkScale.y + -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + -Up * ChunkScale.y + -ForwardVector * ChunkScale.x));
        m_Tris.Add(RefTri + 0);
        m_Tris.Add(RefTri + 1);
        m_Tris.Add(RefTri + 3);
        m_Tris.Add(RefTri + 1);
        m_Tris.Add(RefTri + 2);
        m_Tris.Add(RefTri + 3);

        // + Z Face
        m_Vertices.Add(RefPos + (-right * SleeperWidth + Up * ChunkScale.y + ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + -Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + -Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Tris.Add(RefTri + 4);
        m_Tris.Add(RefTri + 7);
        m_Tris.Add(RefTri + 5);
        m_Tris.Add(RefTri + 5);
        m_Tris.Add(RefTri + 7);
        m_Tris.Add(RefTri + 6);

        // - X Face
        m_Vertices.Add(RefPos + (-right * SleeperWidth + Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + Up * ChunkScale.y+ -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + -Up * ChunkScale.y+ -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + -Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Tris.Add(RefTri + 8);
        m_Tris.Add(RefTri + 9);
        m_Tris.Add(RefTri + 11);
        m_Tris.Add(RefTri + 9);
        m_Tris.Add(RefTri + 10);
        m_Tris.Add(RefTri + 11);

        // + X Face
        m_Vertices.Add(RefPos + (right * SleeperWidth + Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + Up * ChunkScale.y+ -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + -Up * ChunkScale.y+ -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + -Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Tris.Add(RefTri + 12);
        m_Tris.Add(RefTri + 15);
        m_Tris.Add(RefTri + 13);
        m_Tris.Add(RefTri + 13);
        m_Tris.Add(RefTri + 15);
        m_Tris.Add(RefTri + 14);

        // - Y Face
        m_Vertices.Add(RefPos + (-right * SleeperWidth + -Up * ChunkScale.y + -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + -Up * ChunkScale.y + -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + -Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + -Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Tris.Add(RefTri + 16);
        m_Tris.Add(RefTri + 17);
        m_Tris.Add(RefTri + 19);
        m_Tris.Add(RefTri + 17);
        m_Tris.Add(RefTri + 18);
        m_Tris.Add(RefTri + 19);


        // + Y Face
        m_Vertices.Add(RefPos + (-right * SleeperWidth + Up * ChunkScale.y+ -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + Up * ChunkScale.y+ -ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (right * SleeperWidth + Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Vertices.Add(RefPos + (-right * SleeperWidth + Up * ChunkScale.y+ ForwardVector * ChunkScale.x));
        m_Tris.Add(RefTri + 20);
        m_Tris.Add(RefTri + 23);
        m_Tris.Add(RefTri + 21);
        m_Tris.Add(RefTri + 21);
        m_Tris.Add(RefTri + 23);
        m_Tris.Add(RefTri + 22);

    }
}
