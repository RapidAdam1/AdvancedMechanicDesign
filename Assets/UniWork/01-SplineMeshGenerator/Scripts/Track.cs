using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;
using UnityEditor;

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
    [SerializeField] Pillars Pillars;
    [SerializeField] float MinPillarHeight = 1f;
    [SerializeField] Vector2 PillarScale = Vector2.one;
    private void Awake()
    {
        m_SplineContainer = GetComponent<SplineContainer>();
        if(!m_SplineContainer)
            return;
        Spline Sp = m_SplineContainer[0];
        Sp.changed += OnValidate;
        Sleepers.Init(m_SplineContainer, m_Index, SleeperDistance,m_Offset, m_SleeperScale);
        Pillars.Init(m_SplineContainer,m_Index,m_Offset,(int)Sleepers.TotalSteps,MinPillarHeight,PillarScale);
        OuterTrack.Init(m_SplineContainer, m_Index,Resolution, +m_Offset, m_RailScale);
        InnerTrack.Init(m_SplineContainer, m_Index, Resolution,-m_Offset, m_RailScale);
    }


    private void OnValidate()
    {
        m_SplineContainer = GetComponent<SplineContainer>();
        if (!m_SplineContainer)
            return;

        Sleepers.Init(m_SplineContainer, m_Index, SleeperDistance,m_Offset, m_SleeperScale);
        Pillars.Init(m_SplineContainer, m_Index, m_Offset, (int)Sleepers.TotalSteps, MinPillarHeight, PillarScale);
        OuterTrack.Init(m_SplineContainer, m_Index, Resolution, +m_Offset, m_RailScale);
        InnerTrack.Init(m_SplineContainer, m_Index, Resolution, -m_Offset, m_RailScale);
    }
}


[CustomEditor(typeof(Track)), CanEditMultipleObjects]
public class TrackEditor : Editor
{
    private void OnSceneGUI()
    {
        Track CurrentTrack = (Track)target;

        //Button To Draw a completely new spline


        //Label to signify sharp points that may be dodgy
        var Colour = new Color(1, 0, 0, 1);
        GUI.color = Colour;
        Handles.Label(CurrentTrack.transform.position, "This is a Label Handle");
        
        /*        EditorGUI.BeginChangeCheck();
                if (EditorGUI.EndChangeCheck())
                {

                }
        */

        //Drag Tool for X
    }

}