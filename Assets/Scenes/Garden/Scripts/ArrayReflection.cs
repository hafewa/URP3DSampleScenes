using System;
using System.Collections.Generic;
using Anamorphic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Anamorphic
{



    [ExecuteAlways]
    public class ArrayReflection : MonoBehaviour
    {
        public BakedFixedPointReflection[] reflections;
        public bool updateAtRuntime = true;

        public Vector2Int arrayResolution;
        public float fuzziness;
        public Texture2DArray reflectionTextureArray;

        Camera m_CurrentCamera;
        Vector3 m_LastFramePov;
        MaterialPropertyBlock m_Properties;
        MeshRenderer m_Renderer;
        SortedList<float, int> m_InstanceIndex;
        // Start is called before the first frame update

        void Awake()
        {
       
            m_Renderer = GetComponent<MeshRenderer>();
            FindAvailableReflections();
        }

        void Start()
        {
            ApplyProperties();
        }

        public void ApplyProperties()
        {
            m_Properties ??= new MaterialPropertyBlock();
            if (reflectionTextureArray != null)
            {
                m_Properties.SetTexture("_Array", reflectionTextureArray);
            }

            if (m_Renderer != null)
            {
                m_Renderer.SetPropertyBlock(m_Properties);
            }
        }

        protected void FindAvailableReflections()
        {
            reflections ??= new BakedFixedPointReflection[] { };

            if (reflections.Length < 1) reflections = FindObjectsOfType<BakedFixedPointReflection>();

        }

#if UNITY_EDITOR
        void OnValidate()
        {
            ApplyProperties();
        }

#endif
        // Update is called once per frame

        void Update()
        {
            if (!updateAtRuntime) return;
            if (!reflectionTextureArray) return;
                        
            m_CurrentCamera ??= Camera.main;

            var camPoint = Vector3.zero;
#if UNITY_EDITOR

            camPoint = !Application.isPlaying ? SceneView.lastActiveSceneView.camera.transform.position : m_CurrentCamera.transform.position;
#else            
            camPoint =  m_CurrentCamera.transform.position;
#endif

            if (m_LastFramePov == camPoint) return;
            m_LastFramePov = camPoint;
            UpdatePerspective(camPoint);
        }

        void UpdatePerspective(Vector3 camPoint)
        {
            m_InstanceIndex ??= new SortedList<float, int>(reflections.Length);
            m_InstanceIndex.Clear();
            m_InstanceIndex.Capacity = reflections.Length;
            for (int i = 0; i < reflections.Length; i++)
            {
                var d = Vector3.SqrMagnitude(camPoint - reflections[i].transform.position);
                if (!m_InstanceIndex.TryAdd(d,i))
                {
                    m_InstanceIndex.TryAdd(d + Single.Epsilon, i);
                }
            }

            var j = 0;
            var indices = new int[2];
            foreach (var eachItem in m_InstanceIndex)
            {
                if (j > 1) break;
                indices[j] = eachItem.Value;
                j++;
            }

            var a = reflections[indices[0]].transform.position;
            var b = reflections[indices[1]].transform.position;
            
            var baseVector = b - a;
            var camVector = camPoint - a;
            var c = Vector3.Dot(baseVector, camVector);
            var normalized = c / baseVector.sqrMagnitude;
            normalized = Mathf.Clamp(normalized, 0, 1);

            var indexTuple = new Vector4(indices[0], reflections[indices[0]].Yaw, indices[1], reflections[indices[1]].Yaw);
                
            m_Properties.SetFloat("_Blend", normalized);
            m_Properties.SetVector("_Indices", indexTuple);
            m_Renderer.SetPropertyBlock(m_Properties);
            
        }
        
        [ContextMenu("Update Reflection List")]
        void FindReflectionsInScene()
        { 
            reflections = new BakedFixedPointReflection[] { };
           FindAvailableReflections();
        }



        [ContextMenu("Bake Reflections")]
        public void BakeReflections()
        {
#if UNITY_EDITOR
            var result = new Texture2DArray(arrayResolution.x, arrayResolution.y, reflections.Length, TextureFormat.BC6H, false);

            for (int i = 0; i < reflections.Length; i++)
            {
                reflections[i].Capture(arrayResolution, fuzziness);
                result.SetPixelData(reflections[i].reflectionTexture.GetPixelData<byte>(0), 0, i);
            }

            result.Apply();
            reflectionTextureArray = result;
            //AssetDatabase.CreateAsset(result, "Assets/TextureArray.asset");
            ApplyProperties();
#endif
        }

        public void SaveReflections()
        {
#if UNITY_EDITOR
            var result = new Texture2DArray(arrayResolution.x, arrayResolution.y, reflections.Length, TextureFormat.BC6H, false);

            for (int i = 0; i < reflections.Length; i++)
            {
                reflections[i].Capture(arrayResolution, fuzziness);
                result.SetPixelData(reflections[i].reflectionTexture.GetPixelData<byte>(0), 0, i);
            }

            result.Apply();
            reflectionTextureArray = result;
            AssetDatabase.CreateAsset(result, "Assets/TextureArray.asset");
            ApplyProperties();
#endif
        }
    }
}


#if  UNITY_EDITOR
[CustomEditor(typeof(ArrayReflection))]
public class ArrayReflectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var t = (ArrayReflection)target;
        
        if (GUILayout.Button("Bake Reflections"))
        {
            t.BakeReflections();
        }        
        
        if (GUILayout.Button("Save Reflections"))
        {
            t.SaveReflections();
        }
    }
}
#endif