using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;
using UnityEditor;
using Codice.CM.Common.Merge;

public class Track : MonoBehaviour
{
    [SerializeField][Range(1, 4)] float m_TrackWidth;
    [SerializeField][Range(1, 500)] int Resolution;


    public float TrackWidth
    {
        get { return m_TrackWidth; }
        set { m_TrackWidth = value; }
    }

    [SerializeField] private SplineContainer m_SplineContainer;
    private int m_Index = 0;

    //Rails
    [SerializeField] Vector2 m_RailScale = new Vector2(.1f,.1f);
    [SerializeField] SplineMesh OuterTrack;
    [SerializeField] SplineMesh InnerTrack;
    [SerializeField] public SleeperMesh Sleepers;
    [SerializeField] Pillars Pillars;


    //Sleepers
    [SerializeField] [Min(0)]float SleeperOffset;
    [SerializeField] Vector2 m_SleeperScale = Vector2.one;

    //Supports
    [SerializeField] float MinPillarHeight = 1f;
    [SerializeField] Vector2 PillarScale = Vector2.one;
    private void Awake()
    {
        OnValidate();
    }


    private void OnValidate()
    {
        m_SplineContainer = GetComponent<SplineContainer>();
        if (!m_SplineContainer)
            return;
        Spline Sp = m_SplineContainer[0];
        Sp.changed += OnValidate;
        Sleepers.Init(m_SplineContainer, m_Index, SleeperOffset,m_TrackWidth, m_SleeperScale);
        Pillars.Init(m_SplineContainer, m_Index, m_TrackWidth, (int)Sleepers.TotalSteps, MinPillarHeight, PillarScale);
        OuterTrack.Init(m_SplineContainer, m_Index, Resolution, +m_TrackWidth, m_RailScale);
        InnerTrack.Init(m_SplineContainer, m_Index, Resolution, -m_TrackWidth, m_RailScale);
    }
}


[CustomEditor(typeof(Track)), CanEditMultipleObjects]
public class TrackEditor : Editor
{

    private void OnSceneGUI()
    {
        Track CurrentTrack = (Track)target;


        //Button To Draw a completely new spline
        EditorGUI.BeginChangeCheck();
        //Vector3 newCellOffset = Handles.PositionHandle(CurrentTrack.transform.position + CurrentTrack.transform.TransformVector(CurrentTrack.CellOffset), CurrentTrack.transform.rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(CurrentTrack, $"Changed the Cell Offset of {CurrentTrack.gameObject.name}");
            //CurrentTrack.TrackWidth = CurrentTrack.transform.InverseTransformVector(newCellOffset - CurrentTrack.transform.position);
            //CurrentTrack.OnValidate();

        }

        //Label to Show how many Sleepers
        var Colour = Color.green;
        GUI.color = Colour;
        if (CurrentTrack.Sleepers.TotalSteps != 0)
        {
            Handles.Label(CurrentTrack.transform.position, "Sleeper Count: " + CurrentTrack.Sleepers.TotalSteps.ToString());
        }
        
        

    }
}