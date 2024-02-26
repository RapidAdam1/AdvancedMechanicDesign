using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;
using UnityEditor;
using Codice.CM.Common.Merge;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

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
    [SerializeField] public float MinPillarHeight = 1f;
    [SerializeField] Vector2 PillarScale = Vector2.one;


    private void Awake()
    {
        OnValidate();
    }

    public void ToggleLoopedTrack()
    {
        m_SplineContainer.Spline.Closed = !m_SplineContainer.Spline.Closed;
        OnValidate();
    }

    public void AdjustTrackWidth(float Change)
    {
        m_TrackWidth = Mathf.Clamp(m_TrackWidth + Change, 1, 4);
        OnValidate();
    }

    public void SetPillarHeight(float Value)
    {
        MinPillarHeight = Value;
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
public class ProcPlaneEditorWindow : EditorWindow
{
    float tempValue = 0.4f;


    [MenuItem("Window/SplineEditor")]
    public static void ShowWindow()
    {
        GetWindow<ProcPlaneEditorWindow>("SplineEditor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Spline Editor", EditorStyles.boldLabel);
        float Scale = 0.2f;

        Track target = Selection.gameObjects[0].GetComponent<Track>();
        if (target != null)
        {
            //Loop Track
            GUILayout.BeginHorizontal();
            GUILayout.Label("Looped Track");
            if (GUILayout.Button("Toggle"))
            {
                target.ToggleLoopedTrack();
            }
            GUILayout.EndHorizontal();


            //Scale Track Width
            GUILayout.BeginHorizontal();
            GUILayout.Label("Track Width");
            if (GUILayout.Button("+"))
            {
                target.AdjustTrackWidth(+0.2f);
            }
            if (GUILayout.Button("-"))
            {
                target.AdjustTrackWidth(-0.2f);
            };
            GUILayout.EndHorizontal();


            //Scale Pillar Height
            GUILayout.Label($"Minimum Pillar Height: {(Mathf.Round(target.MinPillarHeight * 100)) / 100.0}", EditorStyles.boldLabel);
            float PreviousTempValue = tempValue;
            tempValue = GUILayout.HorizontalSlider(tempValue, 0, 5);
            if (tempValue != PreviousTempValue)
            {
                target.SetPillarHeight(tempValue);
            }
        }
    }
}