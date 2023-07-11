using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct PerformanceTestStage
{
    public string SceneName;
    public Vector3 CameraPosition;
    public Quaternion CameraRotation;
}

public enum TestState
{
    Idle,
    Loading,
    Waiting,
    Capturing,
    TestFinished,
}

public struct TestResult
{
    public string testName;
    public float avgFPS;
}

public class PerformanceTest : MonoBehaviour
{
    public bool m_AutoStart;
    public List<PerformanceTestStage> m_Stages;

    [SerializeField]
    private TestState m_State;
    [SerializeField]
    private float m_WaitTime;
    [SerializeField]
    private int m_FramesToCapture;

    private float[] m_FrameTimes;
    private float m_ElapsedWaitTime;
    private int m_CaptureIndex;
    private int m_CurrentStage;

    private Transform m_TestCamera;
    
    private static PerformanceTest m_Instance;

    private List<TestResult> m_TestResults;

    public static bool RunningBenchmark()
    {
        return m_Instance != null && m_Instance.m_State != TestState.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Destroy if assigned
        if (m_Instance != null)
        {
            Destroy(this);
            return;
        }
        
        m_Instance = this;
        
        m_State = TestState.Idle; 
        DontDestroyOnLoad(this);

        if (m_AutoStart)
        {
            StartTests();
        }
        
        MakeBgTex();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case TestState.Waiting:
                if(m_ElapsedWaitTime > m_WaitTime)
                {
                    m_State = TestState.Capturing;
                }
                m_ElapsedWaitTime += Time.deltaTime;
                break;
            case TestState.Capturing:
                m_FrameTimes[m_CaptureIndex] = Time.deltaTime;
                m_CaptureIndex++;
                if(m_CaptureIndex >= m_FramesToCapture)
                {
                    SaveTestResult();
                    if(m_CurrentStage < m_Stages.Count - 1)
                    {
                        m_CurrentStage++;
                        StartTest(m_CurrentStage);
                    }
                    else
                    {
                        m_State = TestState.TestFinished;
                    }
                }
                break;
        }
    }
    
    public void StartTests()
    {
        StartTest(0);

        m_TestResults = new List<TestResult>();
    }

    private void CreateCamera()
    {
        GameObject go = new GameObject("TestCamera");
        go.AddComponent<Camera>();
        var additionalData = go.AddComponent<UniversalAdditionalCameraData>();
        additionalData.renderPostProcessing = true;
        m_TestCamera = go.transform;
        DontDestroyOnLoad(go);
    }
    
    private Texture2D bktex;
    private void MakeBgTex()
    {
        int w = 2;
        int h = 2;
        Color col = new Color(0,0,0,1);
        Color[] pix = new Color[w * h];
        for( int i = 0; i < pix.Length; ++i )
        {
            pix[ i ] = col;
        }
        bktex = new Texture2D( w, h );
        bktex.SetPixels( pix );
        bktex.Apply();
    }

    private void OnGUI()
    {
        
        int guiWidth = 1000;
        int guiFontSize = 32;
        
        
        float scale = Screen.height / 1080f;

        GUI.skin.label.fontSize = Mathf.RoundToInt ( guiFontSize * scale );
        GUI.skin.box.normal.background = bktex;
        ResetGUIBgColor();
        GUI.contentColor = Color.white;
        GUI.color = Color.white;
        int padding = 5;
        //Width
        float w = guiWidth;
        w *= scale;
        //Height
        float h = Screen.height - padding * 2;

        float x = (Screen.width - w) * 0.5f;
        float y = padding;
        
        GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

        StringBuilder names = new StringBuilder();
        StringBuilder frameTimes = new StringBuilder();

        names.AppendLine("<b>Test Name</b>");
        frameTimes.AppendLine("<b>FPS</b>");

        foreach (var result in m_TestResults)
        {
            names.AppendLine(result.testName);
            frameTimes.AppendLine(result.avgFPS.ToString("0.00"));
        }
        
        GUILayout.BeginHorizontal();
        GUILayout.Label(names.ToString());
        GUILayout.Label(frameTimes.ToString());
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();

        switch (m_State)
        {
            case TestState.Waiting:
                GUILayout.Label("Test "+ m_CurrentStage + " currently <b>waiting</b>. (" + m_ElapsedWaitTime.ToString("0.00") + " / " + m_WaitTime.ToString("0.00") + ")");
                break;
            case TestState.Capturing:
                GUILayout.Label("Test "+ m_CurrentStage + " currently <b>capturing</b>. (" + m_CaptureIndex + " / " + m_FramesToCapture + ")");
                break;
            case TestState.Loading:
                GUILayout.Label("Loading scene " + m_Stages[m_CurrentStage].SceneName);
                break;
            case TestState.TestFinished:
                GUILayout.Label("Test finished.");
                break;
        }
        
        
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
    
    private void ResetGUIBgColor()
    {
        float bgAlpha = 0.8f;
        
        //Need to hack the gamma <-> linear like this
        float a = bgAlpha; // the color i want is (1,1,1,0.8)
        Color col = new Color(a,a,a,1f);
        col = QualitySettings.activeColorSpace == ColorSpace.Linear? col.gamma : col;
        col = QualitySettings.activeColorSpace == ColorSpace.Linear? col.gamma : col;
        col.a = col.r;
        col.r = 1;
        col.g = 1;
        col.b = 1;
        GUI.backgroundColor = col;
    }

    private void SaveTestResult()
    {
        TestResult result = new TestResult();
        PerformanceTestStage currentStage = m_Stages[m_CurrentStage];
        result.testName = "Test " + m_CurrentStage + " - " + currentStage.SceneName;

        float sum = 0;
        for(int i = 0 ; i < m_FrameTimes.Length; i++)
        {
            sum += m_FrameTimes[i];
        }
        result.avgFPS = m_FrameTimes.Length / sum;

        m_TestResults.Add(result);
    }
    
    

    private void StartTest(int i)
    {
        if(m_TestCamera == null) CreateCamera();
        
        m_CaptureIndex = 0;
        m_ElapsedWaitTime = 0;
        
        PerformanceTestStage stage = m_Stages[i];
        m_TestCamera.position = stage.CameraPosition;
        m_TestCamera.rotation = stage.CameraRotation;

        m_State = TestState.Loading;
        SceneManager.LoadScene(stage.SceneName, LoadSceneMode.Single);
        
        DisableCamerasInScene();
        

        m_State = TestState.Waiting;
        m_FrameTimes = new float[m_FramesToCapture];
    }

    private void DisableCamerasInScene()
    {
        //Camera[] cameras = GameObject.FindObjectsOfTypeAll(typeof(Camera)) as Camera;

        foreach (var camera in FindObjectsOfTypeAll(typeof(Camera)) as Camera[])
        {
            Debug.Log("Found camera: " + camera.gameObject.name);
            if (camera.gameObject != m_TestCamera.gameObject)
            {
                camera.enabled = false;
            }
        }
/*
        foreach (var camera in cameras)
        {
            
        }
        */
    }
}
