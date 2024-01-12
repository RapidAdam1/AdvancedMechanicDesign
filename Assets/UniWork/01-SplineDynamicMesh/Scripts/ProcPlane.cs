using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProcPlane : MonoBehaviour
{
    
    private MeshFilter m_Filter;
    private Mesh m_Mesh;
    
    private List<Vector3> m_Vertices; 
    private List<int> m_Tris;

    [SerializeField] Vector3 ChunkScale;

    [SerializeField] private Vector3Int m_GridSize;
    [SerializeField] private Vector3 m_CellOffset;

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
        Vector3 Offset = m_CellOffset + ChunkScale *2;

        for(int i = 0; i < m_GridSize.x; i++)
        {
            for(int j = 0; j < m_GridSize.y; j++)
            {
                for (int k = 0; k < m_GridSize.z; k++)
                {
                    GenCube(new Vector3(i * Offset.x, j * Offset.y, k * Offset.z), m_Vertices.Count);
                }
            }
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
        // - Z Face
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, -ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, -ChunkScale.y, -ChunkScale.z));
        m_Tris.Add(RefTri + 0);
        m_Tris.Add(RefTri + 1);
        m_Tris.Add(RefTri + 3);
        m_Tris.Add(RefTri + 1);
        m_Tris.Add(RefTri + 2);
        m_Tris.Add(RefTri + 3);

        // + Z Face
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, -ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, -ChunkScale.y, ChunkScale.z));
        m_Tris.Add(RefTri + 4);
        m_Tris.Add(RefTri + 7);
        m_Tris.Add(RefTri + 5);
        m_Tris.Add(RefTri + 5);
        m_Tris.Add(RefTri + 7);
        m_Tris.Add(RefTri + 6);

        // - X Face
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, -ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, -ChunkScale.y, ChunkScale.z));
        m_Tris.Add(RefTri + 8);
        m_Tris.Add(RefTri + 9);
        m_Tris.Add(RefTri + 11);
        m_Tris.Add(RefTri + 9);
        m_Tris.Add(RefTri + 10);
        m_Tris.Add(RefTri + 11);

        // + X Face
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, -ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, -ChunkScale.y, ChunkScale.z));
        m_Tris.Add(RefTri + 12);
        m_Tris.Add(RefTri + 15);
        m_Tris.Add(RefTri + 13);
        m_Tris.Add(RefTri + 13);
        m_Tris.Add(RefTri + 15);
        m_Tris.Add(RefTri + 14);

        // - Y Face
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, -ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, -ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, -ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, -ChunkScale.y, ChunkScale.z));
        m_Tris.Add(RefTri + 16);
        m_Tris.Add(RefTri + 17);
        m_Tris.Add(RefTri + 19);
        m_Tris.Add(RefTri + 17);
        m_Tris.Add(RefTri + 18);
        m_Tris.Add(RefTri + 19);


        // + Y Face
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, ChunkScale.y, -ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(ChunkScale.x, ChunkScale.y, ChunkScale.z));
        m_Vertices.Add(RefPos + new Vector3(-ChunkScale.x, ChunkScale.y, ChunkScale.z));
        m_Tris.Add(RefTri + 20);
        m_Tris.Add(RefTri + 23);
        m_Tris.Add(RefTri + 21);
        m_Tris.Add(RefTri + 21);
        m_Tris.Add(RefTri + 23);
        m_Tris.Add(RefTri + 22);

    }
}


[CustomEditor(typeof(ProcPlane)), CanEditMultipleObjects]
public class ProcPlaneEditor : Editor
{
    private void OnSceneGUI()
    {
        ProcPlane Plane = (ProcPlane)target;
        EditorGUI.BeginChangeCheck();
        Vector3 newCellOffset = Handles.PositionHandle(Plane.transform.position + Plane.transform.TransformVector(Plane.CellOffset), Plane.transform.rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Plane, $"Changed the Cell Offset of {Plane.gameObject.name}");
            Plane.CellOffset= Plane.transform.InverseTransformVector(newCellOffset - Plane.transform.position);
            Plane.OnValidate();

        }

    }

}