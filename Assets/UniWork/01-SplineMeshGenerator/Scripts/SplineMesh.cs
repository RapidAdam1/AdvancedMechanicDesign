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
    private SplineContainer m_SplineContainer;
    private int m_Index;

    private float3 Position;
    private float3 Tangent;
    private float3 UpVector;


    //TrackGeneration Variables
    int m_Resolution;
    protected float m_Offset;
    protected Vector2 m_Scale = Vector2.one;


    private MeshFilter m_Filter;
    private Mesh m_Mesh;

    private List<Vector3> m_Verts;
    private List<int> m_Tris;

    public void Init(SplineContainer Spline,int SplineIndex,int Resolution, float Offset, Vector2 Scale)
    {
        m_SplineContainer = Spline;
        m_Resolution = Resolution;
        m_Offset = Offset;
        m_Scale = Scale;

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

        float step = 1f / (float)m_Resolution;

        for (int i = 0; i < m_Resolution; i++)
        {
            float t = step * i;
            m_SplineContainer.Evaluate(m_Index, t, out Position, out Tangent, out UpVector);
            float3 right = Vector3.Cross(Tangent, UpVector).normalized;
            Vector3 Point1 = Position + (right * m_Offset);
            BuildMesh(Point1);
        }
        
        BuildEnds(m_SplineContainer.Spline.Closed);

        m_Mesh.Clear();
        m_Mesh.SetVertices(m_Verts);
        m_Mesh.SetTriangles(m_Tris, 0);
        m_Mesh.RecalculateNormals();
    }

    private void BuildMesh(Vector3 SegmentPos)
    {
        // Z Face
        Vector3 Right = Vector3.Cross(Tangent, UpVector).normalized;
        Vector3 Up = (Vector3)UpVector;
        Right.Normalize();
        Up.Normalize();
        m_Verts.Add(SegmentPos + (-Right * m_Scale.x + Up * m_Scale.y)); //TR 0
        m_Verts.Add(SegmentPos + (Right * m_Scale.x + Up * m_Scale.y)); //TL 1
        m_Verts.Add(SegmentPos + (Right * m_Scale.x + -Up * m_Scale.y)); //BL 2
        m_Verts.Add(SegmentPos + (-Right * m_Scale.x + -Up * m_Scale.y)); //BR 3
        int RefTri = m_Verts.Count -1;
        if(RefTri >= 7)
        {
            //Red   0 & 4
            //Blue  1 & 5
            //Yell  2 & 6
            //Green 3 & 7

            //Right Face 
            m_Tris.AddRange(new int[] { RefTri - 7 , RefTri - 3, RefTri - 4 }); 
            m_Tris.AddRange(new int[] { RefTri - 3, RefTri - 0 , RefTri - 4 }); 
           
            m_Tris.AddRange(new int[] { RefTri -2 , RefTri -3 , RefTri -6  }); 
            m_Tris.AddRange(new int[] { RefTri -3 , RefTri -7 , RefTri -6  }); 

            m_Tris.AddRange(new int[] { RefTri - 5, RefTri - 4 , RefTri - 1 }); 
            m_Tris.AddRange(new int[] { RefTri - 4, RefTri - 0, RefTri - 1  });

            m_Tris.AddRange(new int[] { RefTri - 2, RefTri - 6, RefTri - 1 });
            m_Tris.AddRange(new int[] { RefTri -6 , RefTri -5 , RefTri - 1 });
        }
    }

    void BuildEnds(bool Looped)
    {
        int RefTri = m_Verts.Count - 1;
        if (Looped)
        {
            m_Tris.AddRange(new int[] { RefTri -3 , 0 , RefTri});
            m_Tris.AddRange(new int[] { 0, 3, RefTri});

            m_Tris.AddRange(new int[] { RefTri-2 , 1 , 0});
            m_Tris.AddRange(new int[] { 0, RefTri - 3, RefTri - 2});

            m_Tris.AddRange(new int[] { 1, RefTri-2,2});
            m_Tris.AddRange(new int[] { RefTri - 2, RefTri - 1, 2});

            m_Tris.AddRange(new int[] { 2, RefTri - 1, RefTri });
            m_Tris.AddRange(new int[] { RefTri, 3,2});
        }
        else
        {
            //First
            m_Tris.AddRange(new int[] { 1, 0, 2 });
            m_Tris.AddRange(new int[] { 2, 0, 3 });

           //Last
            m_Tris.AddRange(new int[] { RefTri-3,RefTri-2,RefTri });
            m_Tris.AddRange(new int[] { RefTri- 2, RefTri-1, RefTri});

        }
        

    }

/*    private void OnDrawGizmos()
    {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.color = Color.red;Handles.SphereHandleCap(0, m_Verts[0], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.blue;Handles.SphereHandleCap(0, m_Verts[1], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.yellow;Handles.SphereHandleCap(0, m_Verts[2], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.green;Handles.SphereHandleCap(0, m_Verts[3], Quaternion.identity, 0.1f, EventType.Repaint);

        Handles.color = Color.red; Handles.SphereHandleCap(0, m_Verts[4], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.blue; Handles.SphereHandleCap(0, m_Verts[5], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.yellow;Handles.SphereHandleCap(0, m_Verts[6], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.green;Handles.SphereHandleCap(0, m_Verts[7], Quaternion.identity, 0.1f, EventType.Repaint);

                for (int i = 1; i < m_Verts.Count-1; i++)
                {
                    Handles.SphereHandleCap(i, m_Verts[i], Quaternion.identity, 0.03f, EventType.Repaint);
                }
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, m_Verts[m_Verts.Count-1], Quaternion.identity, 0.1f, EventType.Repaint);
        
    }*/
}
