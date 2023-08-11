using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Benchmarking
{
    public class BenchmarkShortcutManager : MonoBehaviour
    {
        private static BenchmarkShortcutManager s_instance;
        public static BenchmarkShortcutManager instance
        {
            get
            {
                if (s_instance == null)
                    GetOrCreate();

                return s_instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void GetOrCreate()
        {
            s_instance = FindObjectOfType<BenchmarkShortcutManager>();
            if (s_instance == null)
            {
                var go = new GameObject("Benchmark Shortcut Manager");
                s_instance = go.AddComponent<BenchmarkShortcutManager>();
            }

            DontDestroyOnLoad(s_instance.gameObject);
        }

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(s_instance.gameObject);

#if ENABLE_INPUT_SYSTEM

#endif
        }

#if ENABLE_INPUT_SYSTEM
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                if (Input.GetKeyDown(KeyCode.B) && !PerformanceTest.RunningBenchmark)
                    StartBenchmark();
        }
#endif

        static void StartBenchmark()
        {
            SceneManager.LoadScene("BenchmarkScene");
        }
    }
}