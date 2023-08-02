using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public class PerformanceTestStage
{
    [FormerlySerializedAs("SceneName")]
    public string sceneName;
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;

    public bool useFullTimeline = true;

    private List<FrameData> _frameDatas;
    private FrameData _minFrameData, _maxFrameData, _avgFrameData, _medianFrameData, _lowerQuartileFrameData, _upperQuartileFrameData;

    private List<float> m_allFrameTimes;
    private float m_cumulatedFrameTime = 0, m_avgFrameTime = 0, m_minFrameTime = 99999999, m_maxFrameTime = 0, m_medianFrameTime=0, m_upperQuartileFrameTime=0, m_lowerQuartileFrameTime=0;
    private int recordingIndex = 0;

    private Camera m_testCamera => PerformanceTest.instance.testCamera;
    private Action m_finishedAction;
    private PlayableDirector m_playableDirector;
    private float m_intermediateCaptureTime;

    private VisualElement visualElementRoot;
    private Label testNameLabel, minFPSLabel, maxFPSLabel, avgFPSLabel, lowerQuartileFPSLabel, medianFPSLabel, upperQuartileFPSLabel;
    private VisualElement timingsGraphContainerVE;
    private StatsGraphVE timingsGraphVE;

    public float avgFrameTime => m_avgFrameTime;
    public float minFrameTime => m_minFrameTime;
    public float maxFrameTime => m_maxFrameTime;
    public float medianFrameTime => m_medianFrameTime;
    public float upperQuartileFrameTime => m_upperQuartileFrameTime;
    public float lowerQuartileFrameTime => m_lowerQuartileFrameTime;

    public float avgFPS => 1.0f / avgFrameTime;
    public float minFPS => 1.0f / m_maxFrameTime;
    public float maxFPS => 1.0f / m_minFrameTime;
    public float medianFPS => 1.0f / m_medianFrameTime;
    public float upperQuartileFPS => 1.0f / m_lowerQuartileFrameTime;
    public float lowerQuartileFPS => 1.0f / m_upperQuartileFrameTime;

    public void Init()
    {
        var initialListSize = PerformanceTest.instance.m_FramesToCapture;
        if (m_playableDirector != null && useFullTimeline)
            initialListSize = (int) m_playableDirector.duration * 120;

        m_allFrameTimes = new List<float>();

        m_avgFrameTime = 0;
        m_minFrameTime = 99999999;
        m_maxFrameTime = 0;
        recordingIndex = 0;
        
        _frameDatas = new List<FrameData>();
        _maxFrameData
            = _avgFrameData
            = _medianFrameData
            = _upperQuartileFrameData
            = _lowerQuartileFrameData
            = new FrameData(0f);
        _minFrameData = new FrameData(Mathf.Infinity);
    }

    public void RecordTiming ( float deltaTime )
    {
        var currentFrameData = new FrameData(deltaTime * 1000f);
        _frameDatas.Add(currentFrameData);
        _minFrameData.MinWith(currentFrameData);
        _maxFrameData.MaxWith(currentFrameData);

        // Debug.Log("Current frame: " + currentFrameData.ToString());
        // Debug.Log($"CpuTimerFrequency: {FrameTimingManager.GetCpuTimerFrequency()}, GpuTimerFrequency: {FrameTimingManager.GetGpuTimerFrequency()}");

        m_allFrameTimes.Add( deltaTime );
        recordingIndex++;
        m_cumulatedFrameTime += deltaTime;
        m_avgFrameTime = m_cumulatedFrameTime / recordingIndex;
        m_minFrameTime = Mathf.Min(m_minFrameTime, deltaTime);
        m_maxFrameTime = Mathf.Max(m_maxFrameTime, deltaTime);

        timingsGraphVE.SetData(_frameDatas.Select(v => v.frameTime / _maxFrameData.frameTime).ToList(), true );

        minFPSLabel.text = minFPS.ToString();
        maxFPSLabel.text = maxFPS.ToString();
        avgFPSLabel.text = avgFPS.ToString();
    }

    public void FinishTest()
    {
        CalculateValues();
        lowerQuartileFPSLabel.text = lowerQuartileFPS.ToString();
        medianFPSLabel.text = medianFPS.ToString();
        upperQuartileFPSLabel.text = upperQuartileFPS.ToString();
    }

    public void InstantiateVisualElement(VisualTreeAsset referenceVisuaTree, VisualElement parent = null)
    {
        if (referenceVisuaTree == null)
            return;

        visualElementRoot = referenceVisuaTree.Instantiate();
        testNameLabel           = visualElementRoot.Q<Label>(name: "TestName");
        minFPSLabel             = visualElementRoot.Q<Label>(name: "MinFPS");
        maxFPSLabel             = visualElementRoot.Q<Label>(name: "MaxFPS");
        avgFPSLabel             = visualElementRoot.Q<Label>(name: "AvgFPS");
        lowerQuartileFPSLabel   = visualElementRoot.Q<Label>(name: "LowerQuartileFPS");
        medianFPSLabel          = visualElementRoot.Q<Label>(name: "MedianFPS");
        upperQuartileFPSLabel   = visualElementRoot.Q<Label>(name: "UpperQuartileFPS");

        timingsGraphContainerVE          = visualElementRoot.Q(name: "TimingsGraph");
        timingsGraphVE = new StatsGraphVE();
        timingsGraphContainerVE.Add(timingsGraphVE);

        testNameLabel.text = sceneName;

        if (parent != null)
        {
            parent.Add(visualElementRoot);
        }
    }

    public void CalculateValues(bool recalculateRange = false)
    {
        if ( recalculateRange)
        {
            m_minFrameTime = m_allFrameTimes.Min();
            m_maxFrameTime = m_allFrameTimes.Max();
        }
        CalculateValues( m_minFrameTime, m_minFrameTime );
    }

    public void CalculateValues( float min, float max )
    {
        var orderedData = new List<float>(m_allFrameTimes);
        orderedData.Sort();
        var lowerQuartileIndexF = m_allFrameTimes.Count * 0.25f;
        var medianIndexF = m_allFrameTimes.Count * 0.5f;
        var upperQuartileIndexF = m_allFrameTimes.Count * 0.75f;

        var lowerQuartileIndexI = (int)lowerQuartileIndexF;
        lowerQuartileIndexF -= lowerQuartileIndexI;
        var medianIndexI = (int)medianIndexF;
        medianIndexF -= medianIndexI;
        var upperQuartileIndexI = (int)upperQuartileIndexF;
        upperQuartileIndexF -= upperQuartileIndexI;

        m_lowerQuartileFrameTime = Mathf.Lerp( orderedData[lowerQuartileIndexI], orderedData[lowerQuartileIndexI+1], lowerQuartileIndexF );
        m_medianFrameTime = Mathf.Lerp(orderedData[medianIndexI], orderedData[medianIndexI + 1], medianIndexF);
        m_upperQuartileFrameTime = Mathf.Lerp(orderedData[upperQuartileIndexI], orderedData[upperQuartileIndexI + 1], upperQuartileIndexF);
    }

    public void Start()
    {
        Start(null);
    }

    public void Start( Action finishedAction )
    {
        if (finishedAction != null)
            m_finishedAction = finishedAction;

        Debug.Log("Called start for : " + sceneName);

        PerformanceTest.instance.StartCoroutine(ProcessTest());
    }
    public void SetFinishedAction (Action finishedAction ) { m_finishedAction = finishedAction; }

    IEnumerator ProcessTest()
    {
        yield return LoadAndInit();
        yield return new WaitForSeconds(PerformanceTest.instance.m_WaitTime );
        yield return RunTest();
        yield return End();

        if (m_finishedAction != null)
        {
            Debug.Log("Invoking Finish action");
            m_finishedAction.Invoke();
        }
    }

    IEnumerator LoadAndInit()
    {
        Debug.Log($"Start test {sceneName}");

        m_testCamera.transform.position = cameraPosition;
        m_testCamera.transform.rotation = cameraRotation;

         SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        // wait one frame for scene object to be loaded in memory
        yield return null;

        DisableCamerasInScene();

        var directors = Resources.FindObjectsOfTypeAll<PlayableDirector>();
        Debug.Log($"Found {directors.Length} playable director(s)");

        m_playableDirector = (directors.Length > 1) ? directors.Single(d => d.gameObject.name == "CinematicTimeline") : directors[0];

        if (m_playableDirector != null)
        {
            m_playableDirector.gameObject.SetActive(true);
            var playable = m_playableDirector.playableAsset;
            var cinemachineTrack = playable.outputs.Single(o => o.outputTargetType == typeof(CinemachineBrain)).sourceObject;
            m_playableDirector.SetGenericBinding(cinemachineTrack, m_testCamera.GetComponent<CinemachineBrain>());

            var duration = (float)m_playableDirector.duration;
            m_intermediateCaptureTime = duration / (PerformanceTest.instance.m_FramesToCapture + 1);

            m_playableDirector.Pause();
            m_playableDirector.extrapolationMode = DirectorWrapMode.None;
        }

        Init();
    }

    IEnumerator RunTest()
    {
        bool noIntermediateTime = useFullTimeline && m_playableDirector != null;

        if (m_playableDirector != null)
            m_playableDirector.Play();

        while (recordingIndex < m_allFrameTimes.Count || (useFullTimeline && m_playableDirector != null && m_playableDirector.state != PlayState.Paused) )
        {
            PerformanceTest.instance.SetCurrentFPS(1.0f / Time.deltaTime);
            RecordTiming(Time.deltaTime);
            if (noIntermediateTime)
                yield return null;
            else
                yield return new WaitForSeconds(m_intermediateCaptureTime);
        }
    }

    IEnumerator End()
    {
        Debug.Log($"Test {sceneName} finished and captured {m_allFrameTimes.Count} frames timings");
        FinishTest();
        yield return null;
    }

    private void DisableCamerasInScene()
    {
        foreach (var camera in UnityEngine.Object.FindObjectsOfType<Camera>())
        {
            //Debug.Log("Found camera: " + camera.gameObject.name);
            if (camera.gameObject != m_testCamera.gameObject)
            {
                camera.enabled = false;
            }
        }
    }
}

public enum TestState
{
    Idle,
    Loading,
    Waiting,
    Capturing,
    TestFinished,
}

public class PerformanceTest : MonoBehaviour
{
    private static PerformanceTest m_instance;
    public static PerformanceTest instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindAnyObjectByType<PerformanceTest>();

            return m_instance;
        }
    }

    public bool m_AutoStart;
    public List<PerformanceTestStage> m_Stages;

    [SerializeField]
    private TestState m_State;
    [SerializeField]
    public float m_WaitTime;
    [SerializeField]
    public int m_FramesToCapture;

    public Gradient frameTimingGradient = new Gradient();

    [SerializeField]
    private VisualTreeAsset m_TestDataVisualTreeReference;

    private int m_CurrentStageIndex;
    private PerformanceTestStage m_CurrentStage => m_Stages[m_CurrentStageIndex];

    private Camera m_testCamera;
    public Camera testCamera
    {
        get
        {
            if (m_testCamera == null)
                CreateCamera();

            return m_testCamera;
        }
    }

    private UIDocument m_UIDocument;
    private TextElement currentFPSText;

    public void SetCurrentFPS( float fps )
    {
        currentFPSText.text = fps.ToString();
    }

    public static bool RunningBenchmark()
    {
        return m_instance != null;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Destroy if assigned
        if (m_instance != null)
        {
            Destroy(this);
            return;
        }

        Time.maximumDeltaTime = 120;

        m_instance = this;
        
        m_State = TestState.Idle; 
        DontDestroyOnLoad(this);

        m_UIDocument = GetComponent<UIDocument>();
        var rootVE = m_UIDocument.rootVisualElement;
        currentFPSText = rootVE.Q<TextElement>(name: "CurrentFPS");

        var testList = rootVE.Q<VisualElement>(name: "TestsList");

        for (int i = 0; i<m_Stages.Count; i++)
        {
            var stage = m_Stages[i];

            stage.InstantiateVisualElement(m_TestDataVisualTreeReference, testList);

            if (i < m_Stages.Count - 1)
                stage.SetFinishedAction( m_Stages[i + 1].Start);
            else
                stage.SetFinishedAction( FinalizeTests );
        }
        testList.MarkDirtyRepaint();

        m_Stages[0].Start();
    }

    private void CreateCamera()
    {
        GameObject go = new GameObject("TestCamera");
        m_testCamera = go.AddComponent<Camera>();
        var additionalData = go.AddComponent<UniversalAdditionalCameraData>();
        additionalData.renderPostProcessing = true;
        DontDestroyOnLoad(go);

        go.AddComponent<CinemachineBrain>();
    }

    private void FinalizeTests()
    {

    }
}
