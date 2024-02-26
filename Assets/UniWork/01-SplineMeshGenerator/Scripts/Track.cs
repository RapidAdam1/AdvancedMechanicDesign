using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;
using UnityEditor;
using Codice.CM.Common.Merge;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

public class Track : MonoBehaviour
{
    float m_TrackWidth = 1;
    [SerializeField][Range(1, 500)] int Resolution;


    public float TrackWidth
    {
        get { return m_TrackWidth; }
        set { m_TrackWidth = value; }
    }
    public float MinimumPillarHeight
    {
        get { return m_MinPillarHeight; }
        set { m_MinPillarHeight = value; }
    }

    public float PillarScale
    {
        get { return m_PillarScale.x; }
        set { m_PillarScale = new Vector2(value, value); }
    }

    public Vector2 RailScale
    {
        get { return m_RailScale; }
        set { m_RailScale = value; }
    }

    public Vector2 SleeperScale
    {
        get { return m_SleeperScale; }
        set { m_SleeperScale = value; }
    }

    [SerializeField] private SplineContainer m_SplineContainer;
    private int m_Index = 0;

    //Rails
    [SerializeField] SplineMesh OuterTrack;
    [SerializeField] SplineMesh InnerTrack;
    [SerializeField] public SleeperMesh Sleepers;
    [SerializeField] Pillars Pillars;


    //Sleepers
    [SerializeField] [Min(0)]float SleeperOffset;

    //Supports
    float m_MinPillarHeight = 1f;
    Vector2 m_PillarScale = Vector2.one;
    Vector2 m_SleeperScale = Vector2.one;
    Vector2 m_RailScale = new Vector2(.1f,.1f);


    private void Awake()
    {
        OnValidate();
    }

    public void ToggleLoopedTrack()
    {
        m_SplineContainer.Spline.Closed = !m_SplineContainer.Spline.Closed;
        OnValidate();
    }

    public void OnValidate()
    {
        m_SplineContainer = GetComponent<SplineContainer>();
        if (!m_SplineContainer)
            return;
        Spline Sp = m_SplineContainer[0];
        Sp.changed += OnValidate;
        Sleepers.Init(m_SplineContainer, m_Index, SleeperOffset,m_TrackWidth, m_SleeperScale);
        Pillars.Init(m_SplineContainer, m_Index, m_TrackWidth, (int)Sleepers.TotalSteps, m_MinPillarHeight, m_PillarScale);
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
    float tempMinHeight = 0.4f;
    float tempPillarScale = 0.25f;
    string SleeperX = "";
    string SleeperY = "";
    string RailX = "";
    string RailY = "";

    Track SelectedTarget;
    [MenuItem("Window/SplineEditor")]
    public static void ShowWindow()
    {
        GetWindow<ProcPlaneEditorWindow>("SplineEditor");
    }


    private void OnGUI()
    {
        if (Selection.activeGameObject == null)
        {
            GUILayout.Label("Select an Object to Use Tool");
            return;
        }
        Track target = Selection.gameObjects[0].GetComponent<Track>();
        if(SelectedTarget != target)
        {
            SelectedTarget = target;
            SleeperX = SelectedTarget.SleeperScale.x.ToString();
            SleeperY = SelectedTarget.SleeperScale.y.ToString();

            RailX = SelectedTarget.RailScale.x.ToString();
            RailY= SelectedTarget.RailScale.y.ToString();

        }

        if (SelectedTarget != null)
        {
            GUILayout.Label("Spline Editor", EditorStyles.boldLabel);
            //Loop Track
            GUILayout.BeginHorizontal();
            GUILayout.Label("Looped Track");
            if (GUILayout.Button("Toggle"))
            {
                SelectedTarget.ToggleLoopedTrack();
            }
            GUILayout.EndHorizontal();


            //Scale Track Width
            GUILayout.BeginHorizontal();
            GUILayout.Label("Track Width");
            if (GUILayout.Button("+"))
            {
                SelectedTarget.TrackWidth = Mathf.Clamp(SelectedTarget.TrackWidth += 0.2f, 1, 4);
                SelectedTarget.OnValidate();
            }
            if (GUILayout.Button("-"))
            {
                SelectedTarget.TrackWidth = Mathf.Clamp(SelectedTarget.TrackWidth -= 0.2f, 1, 4);
                SelectedTarget.OnValidate();
            };
            GUILayout.EndHorizontal();


            //Scale Pillar Height
            float PreviousTempValue = SelectedTarget.MinimumPillarHeight;
            GUILayout.Label($"Minimum Pillar Height: {(Mathf.Round(PreviousTempValue * 100)) / 100.0}", EditorStyles.boldLabel);
            tempMinHeight = GUILayout.HorizontalSlider(tempMinHeight, 0, 5);
            if (tempMinHeight != PreviousTempValue)
            {
                SelectedTarget.MinimumPillarHeight = tempMinHeight;
                SelectedTarget.OnValidate();
            }

            //Scale Pillar
            float PillarScale = SelectedTarget.PillarScale;
            GUILayout.Label($"\nPillar Scale: {(Mathf.Round(PillarScale * 100)) / 100.0}", EditorStyles.boldLabel);
            tempPillarScale = GUILayout.HorizontalSlider(tempPillarScale, 0, 1);
            if (tempPillarScale != PillarScale)
            {
                SelectedTarget.PillarScale = tempPillarScale;
                SelectedTarget.OnValidate();
            }

            //Sleeper Scale
            GUILayout.Label("");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sleeper Scale");
            GUILayout.Label("X");
            SleeperX = GUILayout.TextField($"{SleeperX}");
            GUILayout.Label("Y");
            SleeperY = GUILayout.TextField($"{SleeperY}");
            GUILayout.EndHorizontal();


            //Rail Scale
            GUILayout.Label("");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sleeper Scale");
            GUILayout.Label("X");
            RailX = GUILayout.TextField($"{RailX}");
            GUILayout.Label("Y");
            RailY = GUILayout.TextField($"{RailY}");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Validate"))
            {
                float SleeperXVal;
                float SleeperYVal;
                bool SuccessfullyParsedX = float.TryParse(SleeperX, out  SleeperXVal);
                bool SuccessfullyParsedY = float.TryParse(SleeperY, out  SleeperYVal);
                if (SuccessfullyParsedX && SuccessfullyParsedY)
                {
                    SelectedTarget.SleeperScale = new Vector2(SleeperXVal, SleeperYVal);
                }

                float RailXVal;
                float RailYVal;
                bool SuccessfullyParsedRailX = float.TryParse(RailX, out RailXVal);
                bool SuccessfullyParsedRailY = float.TryParse(RailY, out RailYVal);
                if (SuccessfullyParsedRailX && SuccessfullyParsedRailY)
                {
                    SelectedTarget.RailScale = new Vector2(RailXVal, RailYVal);
                }
                SelectedTarget.OnValidate();

            }
        }
        
        else 
        { 
            if(GUILayout.Button("Find Spline"))
            {
                Track PotentialTrack = FindFirstObjectByType<Track>();
                if(PotentialTrack != null && Selection.activeGameObject != null)
                {
                    Selection.activeGameObject = PotentialTrack.gameObject;
                }
                Debug.Log(PotentialTrack);
            }
        }
    }
}