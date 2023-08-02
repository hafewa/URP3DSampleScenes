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

        private int recordingIndex = 0;

        private Camera m_testCamera => PerformanceTest.instance.testCamera;
        private Action m_finishedAction;
        private PlayableDirector m_playableDirector;
        private float m_intermediateCaptureTime;

        private VisualElement visualElementRoot;
        private Label testNameLabel, minFPSLabel, maxFPSLabel, avgFPSLabel, lowerQuartileFPSLabel, medianFPSLabel, upperQuartileFPSLabel;
        private VisualElement timingsGraphContainerVE;
        private StatsGraphVE timingsGraphVE;

        private TestStageStatus _status = TestStageStatus.Waiting;
        public TestStageStatus status => _status;

        public void Init()
        {
            var initialListSize = PerformanceTest.instance.m_FramesToCapture;
            if (m_playableDirector != null && useFullTimeline)
                initialListSize = (int)m_playableDirector.duration * 120;
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

        public void RecordTiming(float deltaTime)
        {
            var currentFrameData = new FrameData(deltaTime * 1000f);
            _frameDatas.Add(currentFrameData);
            if (recordingIndex == 0)
            {
                _minFrameData = _maxFrameData = _avgFrameData = currentFrameData;
            }
            else
            {
                _minFrameData.MinWith(currentFrameData, true);
                _maxFrameData.MaxWith(currentFrameData, true);
                _avgFrameData.AverageWith(currentFrameData, recordingIndex + 1, true);
            }

            // Debug.Log("Current frame: " + currentFrameData.ToString());
            // Debug.Log($"CpuTimerFrequency: {FrameTimingManager.GetCpuTimerFrequency()}, GpuTimerFrequency: {FrameTimingManager.GetGpuTimerFrequency()}");

            recordingIndex++;

            timingsGraphVE.SetData(_frameDatas.Select(v => v.frameTime / _maxFrameData.frameTime).ToList(), true);

            minFPSLabel.text = _minFrameData.fps.ToString();
            maxFPSLabel.text = _maxFrameData.fps.ToString();
            avgFPSLabel.text = _avgFrameData.fps.ToString();
        }

        public void InstantiateVisualElement(VisualTreeAsset referenceVisuaTree, VisualElement parent = null)
        {
            if (referenceVisuaTree == null)
                return;

            visualElementRoot = referenceVisuaTree.Instantiate();
            testNameLabel = visualElementRoot.Q<Label>(name: "TestName");
            minFPSLabel = visualElementRoot.Q<Label>(name: "MinFPS");
            maxFPSLabel = visualElementRoot.Q<Label>(name: "MaxFPS");
            avgFPSLabel = visualElementRoot.Q<Label>(name: "AvgFPS");
            lowerQuartileFPSLabel = visualElementRoot.Q<Label>(name: "LowerQuartileFPS");
            medianFPSLabel = visualElementRoot.Q<Label>(name: "MedianFPS");
            upperQuartileFPSLabel = visualElementRoot.Q<Label>(name: "UpperQuartileFPS");

            timingsGraphContainerVE = visualElementRoot.Q(name: "TimingsGraph");
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
                m_finishedAction = finishedAction;

            Debug.Log("Called start for : " + sceneName);

            PerformanceTest.instance.StartCoroutine(ProcessTest());
        }
        public void SetFinishedAction(Action finishedAction) { m_finishedAction = finishedAction; }

        IEnumerator ProcessTest()
        {
            yield return LoadAndInit();
            yield return new WaitForSeconds(PerformanceTest.instance.m_WaitTime);
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
            _status = TestStageStatus.Running;

            bool noIntermediateTime = useFullTimeline && m_playableDirector != null;

            if (m_playableDirector != null)
                m_playableDirector.Play();

            while (recordingIndex < _frameDatas.Count || (useFullTimeline && m_playableDirector != null && m_playableDirector.state != PlayState.Paused))
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
            _status = TestStageStatus.Finished;

            Debug.Log($"Test {sceneName} finished and captured {_frameDatas.Count} frames timings");

            CalculateValues();
            lowerQuartileFPSLabel.text = _lowerQuartileFrameData.fps.ToString();
            medianFPSLabel.text = _medianFrameData.fps.ToString();
            upperQuartileFPSLabel.text = _upperQuartileFrameData.fps.ToString();

            yield return null;
        }

        private void DisableCamerasInScene()
        {
            foreach (var camera in UnityEngine.Object.FindObjectsOfType<Camera>())
            {
                // Debug.Log("Found camera: " + camera.gameObject.name);
                if (camera.gameObject != m_testCamera.gameObject)
                {
                    camera.enabled = false;
                }
            }
        }
    }

    public enum TestStageStatus
    {
        Waiting,
        Running,
        Finished
    }
}