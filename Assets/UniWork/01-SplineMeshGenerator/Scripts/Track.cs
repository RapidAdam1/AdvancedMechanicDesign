using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField][Range(-4, 4)] float m_Offset;
    [SerializeField][Range(1, 500)] int Resolution;

    [SerializeField] private SplineContainer m_SplineContainer;
    [SerializeField] private int m_Index;

    //Rails
    [SerializeField] Vector2 m_RailScale = new Vector2(.1f,.1f);
    [SerializeField] SplineMesh OuterTrack;
    [SerializeField] SplineMesh InnerTrack;


    //Sleepers
    [SerializeField] SleeperMesh Sleepers;
    [SerializeField] [Min(0)]float SleeperDistance;
    [SerializeField] Vector2 m_SleeperScale = Vector2.one;

    //Supports
    //Class Name
    [SerializeField] float MinPillarHeight = 1f;
    [SerializeField] Vector2 PillarScale = Vector2.one;
    private void Awake()
    {
        m_SplineContainer = GetComponent<SplineContainer>();
        if(!m_SplineContainer)
            return;

        Sleepers.Init(m_SplineContainer, m_Index, SleeperDistance, m_SleeperScale);
        OuterTrack.Init(m_SplineContainer, m_Index,Resolution, +m_Offset, m_RailScale);
        InnerTrack.Init(m_SplineContainer, m_Index, Resolution,-m_Offset, m_RailScale);
    }


    private void OnValidate()
    {
        m_SplineContainer = GetComponent<SplineContainer>();
        if (!m_SplineContainer)
            return;

        Sleepers.Init(m_SplineContainer, m_Index, SleeperDistance, m_SleeperScale);
        OuterTrack.Init(m_SplineContainer, m_Index, Resolution, +m_Offset, m_RailScale);
        InnerTrack.Init(m_SplineContainer, m_Index, Resolution, -m_Offset, m_RailScale);
    }
}
