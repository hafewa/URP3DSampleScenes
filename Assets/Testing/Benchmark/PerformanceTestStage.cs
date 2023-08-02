using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Benchmarking
{

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

        private int _recordingIndex = 0;

        private Camera _testCamera => PerformanceTest.instance.testCamera;
        private Action _finishedAction;
        private PlayableDirector _playableDirector;
        private float _intermediateCaptureTime;

        private VisualElement _visualElementRoot;
        private Label _testNameLabel, _minLabel, _maxLabel, _avgLabel, _lowerQuartileLabel, _medianLabel, _upperQuartileLabel;
        private VisualElement _timingsGraphContainerVE;
        private StatsGraphVE _timingsGraphVE;

        private TestStageStatus _status = TestStageStatus.Waiting;
        public TestStageStatus status => _status;


        private DataType _displayedDataType = DataType.FrameTime;
        public DataType displayedDataType
        {
            get
            {
                return _displayedDataType;
            }

            set
            {
                SetDisplayedDataType(value);
            }
        }

        public void Init()
        {
            var initialListSize = PerformanceTest.instance._framesToCapture;
            if (_playableDirector != null && useFullTimeline)
                initialListSize = (int)_playableDirector.duration * 120;
            _recordingIndex = 0;

            _frameDatas = new List<FrameData>();
            _maxFrameData
                = _avgFrameData
                = _medianFrameData
                = _upperQuartileFrameData
                = _lowerQuartileFrameData
                = new FrameData(0f);
            _minFrameData = new FrameData(Mathf.Infinity);
        }

        public void RecordTiming(FrameData currentFrameData)
        {
            _frameDatas.Add(currentFrameData);
            if (_recordingIndex == 0)
            {
                _minFrameData = _maxFrameData = _avgFrameData = currentFrameData;
            }
            else
            {
                _minFrameData.MinWith(currentFrameData, true);
                _maxFrameData.MaxWith(currentFrameData, true);
                _avgFrameData.AverageWith(currentFrameData, _recordingIndex + 1, true);
            }

            // Debug.Log("Current frame: " + currentFrameData.ToString());
            // Debug.Log($"CpuTimerFrequency: {FrameTimingManager.GetCpuTimerFrequency()}, GpuTimerFrequency: {FrameTimingManager.GetGpuTimerFrequency()}");

            _recordingIndex++;

            _minLabel.text = _minFrameData.GetValueString(_displayedDataType);
            _maxLabel.text = _maxFrameData.GetValueString(_displayedDataType);
            _avgLabel.text = _avgFrameData.GetValueString(_displayedDataType);
        }

        public void InstantiateVisualElement(VisualTreeAsset referenceVisuaTree, VisualElement parent = null)
        {
            if (referenceVisuaTree == null)
                return;

            _visualElementRoot = referenceVisuaTree.Instantiate();
            _testNameLabel = _visualElementRoot.Q<Label>(name: "TestName");
            _minLabel = _visualElementRoot.Q<Label>(name: "MinFPS");
            _maxLabel = _visualElementRoot.Q<Label>(name: "MaxFPS");
            _avgLabel = _visualElementRoot.Q<Label>(name: "AvgFPS");
            _lowerQuartileLabel = _visualElementRoot.Q<Label>(name: "LowerQuartileFPS");
            _medianLabel = _visualElementRoot.Q<Label>(name: "MedianFPS");
            _upperQuartileLabel = _visualElementRoot.Q<Label>(name: "UpperQuartileFPS");

            _timingsGraphContainerVE = _visualElementRoot.Q(name: "TimingsGraph");
            _timingsGraphVE = new StatsGraphVE();
            _timingsGraphContainerVE.Add(_timingsGraphVE);

            _testNameLabel.text = sceneName;

            if (parent != null)
            {
                parent.Add(_visualElementRoot);
            }
        }

        public void CalculateValues(bool recalculateRange = false)
        {
            if (recalculateRange)
            {
                _minFrameData = FrameData.MinMultiple(_frameDatas);
                _maxFrameData = FrameData.MaxMultiple(_frameDatas);
            }

            var orderedData = new List<FrameData>(_frameDatas).OrderBy(v => v.frameTime).ToArray();
            var lowerQuartileIndexF = _frameDatas.Count * 0.25f;
            var medianIndexF = _frameDatas.Count * 0.5f;
            var upperQuartileIndexF = _frameDatas.Count * 0.75f;

            var lowerQuartileIndexI = (int)lowerQuartileIndexF;
            lowerQuartileIndexF -= lowerQuartileIndexI;

            var medianIndexI = (int)medianIndexF;
            medianIndexF -= medianIndexI;

            var upperQuartileIndexI = (int)upperQuartileIndexF;
            upperQuartileIndexF -= upperQuartileIndexI;

            _lowerQuartileFrameData = FrameData.Lerp(orderedData[lowerQuartileIndexI], orderedData[lowerQuartileIndexI + 1], lowerQuartileIndexF);
            _medianFrameData = FrameData.Lerp(orderedData[medianIndexI], orderedData[medianIndexI + 1], medianIndexF);
            _upperQuartileFrameData = FrameData.Lerp(orderedData[upperQuartileIndexI], orderedData[upperQuartileIndexI + 1], upperQuartileIndexF);

            float tmpFPS = _lowerQuartileFrameData.fps;
            _lowerQuartileFrameData.SetFPSOverride(_upperQuartileFrameData.fps);
            _upperQuartileFrameData.SetFPSOverride(tmpFPS);
        }

        public void Start()
        {
            Start(null);
        }

        public void Start(Action finishedAction)
        {
            if (finishedAction != null)
                _finishedAction = finishedAction;

            Debug.Log("Called start for : " + sceneName);

            PerformanceTest.instance.StartCoroutine(ProcessTest());
        }
        public void SetFinishedAction(Action finishedAction) { _finishedAction = finishedAction; }

        IEnumerator ProcessTest()
        {
            yield return LoadAndInit();
            yield return new WaitForSeconds(PerformanceTest.instance._waitTime);
            yield return RunTest();
            yield return End();

            if (_finishedAction != null)
            {
                Debug.Log("Invoking Finish action");
                _finishedAction.Invoke();
            }
        }

        IEnumerator LoadAndInit()
        {
            Debug.Log($"Start test {sceneName}");

            _testCamera.transform.position = cameraPosition;
            _testCamera.transform.rotation = cameraRotation;

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            // wait one frame for scene object to be loaded in memory
            yield return null;

            DisableCamerasInScene();

            var directors = Resources.FindObjectsOfTypeAll<PlayableDirector>();
            Debug.Log($"Found {directors.Length} playable director(s)");

            _playableDirector = (directors.Length > 1) ? directors.Single(d => d.gameObject.name == "CinematicTimeline") : directors[0];

            if (_playableDirector != null)
            {
                _playableDirector.gameObject.SetActive(true);
                var playable = _playableDirector.playableAsset;
                var cinemachineTrack = playable.outputs.Single(o => o.outputTargetType == typeof(CinemachineBrain)).sourceObject;
                _playableDirector.SetGenericBinding(cinemachineTrack, _testCamera.GetComponent<CinemachineBrain>());

                var duration = (float)_playableDirector.duration;
                _intermediateCaptureTime = duration / (PerformanceTest.instance._framesToCapture + 1);

                _playableDirector.Pause();
                _playableDirector.extrapolationMode = DirectorWrapMode.None;
            }

            Init();
        }

        IEnumerator RunTest()
        {
            _status = TestStageStatus.Running;

            bool noIntermediateTime = useFullTimeline && _playableDirector != null;

            if (_playableDirector != null)
                _playableDirector.Play();

            while (_recordingIndex < _frameDatas.Count || (useFullTimeline && _playableDirector != null && _playableDirector.state != PlayState.Paused))
            {
                FrameData currentFrameData = FrameData.GetCurrentFrameData();

                PerformanceTest.instance.SetCurrentTiming(currentFrameData);
                RecordTiming(currentFrameData);
                UpdateGraph(_displayedDataType);
                if (noIntermediateTime)
                    yield return null;
                else
                    yield return new WaitForSeconds(_intermediateCaptureTime);
            }
        }

        IEnumerator End()
        {
            _status = TestStageStatus.Finished;

            Debug.Log($"Test {sceneName} finished and captured {_frameDatas.Count} frames timings");

            CalculateValues();
            _lowerQuartileLabel.text = _lowerQuartileFrameData.GetValueString(_displayedDataType);
            _medianLabel.text = _medianFrameData.GetValueString(_displayedDataType);
            _upperQuartileLabel.text = _upperQuartileFrameData.GetValueString(_displayedDataType);

            yield return null;
        }

        private void DisableCamerasInScene()
        {
            foreach (var camera in UnityEngine.Object.FindObjectsOfType<Camera>())
            {
                // Debug.Log("Found camera: " + camera.gameObject.name);
                if (camera.gameObject != _testCamera.gameObject)
                {
                    camera.enabled = false;
                }
            }
        }

        private void SetDisplayedDataType( DataType dataType )
        {
            if (dataType != _displayedDataType)
            {

            }

            _displayedDataType = dataType;

            if (status == TestStageStatus.Finished )
            {
                RefreshDisplayedData(dataType);
            }
        }

        private void RefreshDisplayedData( DataType dataType )
        {
            _minLabel.text = _minFrameData.GetValueString(dataType);
            _maxLabel.text = _maxFrameData.GetValueString(dataType);
            _avgLabel.text = _avgFrameData.GetValueString(dataType);
            _lowerQuartileLabel.text = _lowerQuartileFrameData.GetValueString(dataType);
            _upperQuartileLabel.text = _lowerQuartileFrameData.GetValueString(dataType);
            _medianLabel.text = _medianFrameData.GetValueString(dataType);
            UpdateGraph(dataType);
        }

        private void UpdateGraph( DataType dataType )
        {
            _timingsGraphVE.SetData(_frameDatas.Select(v => v.GetValue(dataType) / _maxFrameData.GetValue(dataType)).ToList(), true);
        }
    }

    public enum TestStageStatus
    {
        Waiting,
        Running,
        Finished,
        Canceled
    }
}