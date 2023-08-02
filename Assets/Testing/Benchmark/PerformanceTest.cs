using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Benchmarking
{
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
        private Button changeDataButton;

        public void SetCurrentFPS(float fps)
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
            changeDataButton = rootVE.Q<Button>(name: "ChangeDataButton");

            var testList = rootVE.Q<VisualElement>(name: "TestsList");

            for (int i = 0; i < m_Stages.Count; i++)
            {
                var stage = m_Stages[i];

                stage.InstantiateVisualElement(m_TestDataVisualTreeReference, testList);

                if (i < m_Stages.Count - 1)
                    stage.SetFinishedAction(m_Stages[i + 1].Start);
                else
                    stage.SetFinishedAction(FinalizeTests);
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
}