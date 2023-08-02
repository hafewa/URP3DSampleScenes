using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Benchmarking
{
    public enum DataType
    {
        FrameTime,
        FPS,

        CPUTime,
        CPURenderTime,
        GPUTime,

        Count,
    }

    public class PerformanceTest : MonoBehaviour
    {
        private static PerformanceTest _instance;
        public static PerformanceTest instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<PerformanceTest>();

                return _instance;
            }
        }

        public bool _autoStart = true;
        [FormerlySerializedAs("m_Stages")]
        public List<PerformanceTestStage> _stages;

        [SerializeField]
        public float _waitTime = 5f;
        [SerializeField]
        public int _framesToCapture = 500;

        public Gradient frameTimingGradient = new Gradient();

        [SerializeField]
        [FormerlySerializedAs("m_TestDataVisualTreeReference")]
        private VisualTreeAsset _testDataVisualTreeReference;

        private int _currentStageIndex;
        private PerformanceTestStage _currentStage => _stages[_currentStageIndex];

        private Camera _testCamera;
        public Camera testCamera
        {
            get
            {
                if (_testCamera == null)
                    CreateCamera();

                return _testCamera;
            }
        }

        private UIDocument _UIDocument;
        private TextElement _currentTimingLabel, _currentTimingText;
        private Button _changeDataButton;

        private DataType _displayedDataType = DataType.FrameTime;

        public void SetCurrentTiming(FrameData currentFrameData)
        {
            _currentTimingText.text = currentFrameData.GetValueString(_displayedDataType);
        }

        public static bool RunningBenchmark()
        {
            return _instance != null;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Destroy if assigned
            if (_instance != null)
            {
                Destroy(this);
                return;
            }

            Time.maximumDeltaTime = 120;

            _instance = this;

            DontDestroyOnLoad(this);

            _UIDocument = GetComponent<UIDocument>();
            var rootVE = _UIDocument.rootVisualElement;
            _currentTimingLabel = rootVE.Q<TextElement>(name: "CurrentTimingLabel");
            _currentTimingText = rootVE.Q<TextElement>(name: "CurrentTiming");
            _changeDataButton = rootVE.Q<Button>(name: "ChangeDataButton");
            _changeDataButton.clicked += LoopDisplayedData;
            SetDisplayedData(_displayedDataType);

            var testList = rootVE.Q<VisualElement>(name: "TestsList");

            for (int i = 0; i < _stages.Count; i++)
            {
                var stage = _stages[i];

                stage.InstantiateVisualElement(_testDataVisualTreeReference, testList);

                if (i < _stages.Count - 1)
                    stage.SetFinishedAction(_stages[i + 1].Start);
                else
                    stage.SetFinishedAction(FinalizeTests);
            }
            testList.MarkDirtyRepaint();

            _stages[0].Start();
        }

        private void CreateCamera()
        {
            GameObject go = new GameObject("TestCamera");
            _testCamera = go.AddComponent<Camera>();
            var additionalData = go.AddComponent<UniversalAdditionalCameraData>();
            additionalData.renderPostProcessing = true;
            DontDestroyOnLoad(go);

            go.AddComponent<CinemachineBrain>();
        }

        private void FinalizeTests()
        {

        }

        private void SetDisplayedData(DataType dataType)
        {
            _displayedDataType = dataType;
            foreach (var stage in _stages)
            {
                stage.displayedDataType = dataType;
            }

            _currentTimingLabel.text = $"Current {dataType}:";
        }

        private void LoopDisplayedData()
        {

            int modulo = FrameTimingManager.IsFeatureEnabled() ? (int)DataType.Count : (int)DataType.FPS+1;
            int nextType = ((int)_displayedDataType + 1) % modulo;

            SetDisplayedData((DataType)nextType);
        }
    }
}