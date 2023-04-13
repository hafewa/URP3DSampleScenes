using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Anamorphic
{

    // TODOS
    // 1. We should probably make sure this does not serialize it's textures
    // 2. More modern URP friendly blit?

    public class BakedFixedPointReflection : MonoBehaviour
    {
        public float groundPlaneHeight;
        public GameObject referenceGeometry;
        public Vector2Int textureResolution = new(512, 512);
        public float captureFuzziness;
        public Texture2D reflectionTexture;
        Vector3 m_EyePoint;

        float ReferenceHeight
        {
            get
            {
                var GroundBase = 0f;
                if (referenceGeometry != null)
                {
                    var mr = referenceGeometry.GetComponent<MeshRenderer>();
                    if (mr != null)
                        GroundBase = mr.bounds.min.y;
                    else
                        GroundBase = mr.transform.position.y;
                }

                return GroundBase + groundPlaneHeight;
            }
        }

        public float Yaw
        {
            get
            {
                // note this needs to be the same as the yaw values in the shader
                var fl = gameObject.transform.forward;
                fl.y = 0;
                fl.Normalize();
                return Mathf.Atan2(fl.x, fl.z);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            var h = transform.position.y - ReferenceHeight;
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(new Vector3(0, -1 * h, 0), new Vector3(1, 0, 1));
            Gizmos.color = Color.yellow;
            Gizmos.DrawFrustum(Vector3.zero, 135f, .25f, -.25f, 1f);
            Gizmos.DrawSphere(Vector3.zero, 0.0625f);
        }

        void OnValidate() { }

        [ExecuteInEditMode]
        public void Capture(bool saveToDisk)
        {
#if UNITY_EDITOR

            var delta = gameObject.transform.position.y - ReferenceHeight;
            var eyepoint = gameObject.transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, -2 * delta, 0));
            var probename = gameObject.name;

            BakeReflection(eyepoint, Yaw, textureResolution, captureFuzziness, out reflectionTexture);

            if (saveToDisk)
            {
                AssetDatabase.CreateAsset(reflectionTexture, $"Assets/{probename}_FixedReflection.asset");
                AssetDatabase.SaveAssets();
            }
#endif
        }

        public void Capture(Vector2Int resolution, float fuzziness)
        {
#if UNITY_EDITOR

            var delta = gameObject.transform.position.y - ReferenceHeight;
            var eyepoint = gameObject.transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, -2 * delta, 0));
            var probename = gameObject.name;

            BakeReflection(eyepoint, Yaw, resolution, fuzziness, out reflectionTexture);
#endif

        }

        static Shader s_CachedShader;

        static Shader s_CaptureShader
        {
            get
            {
                if (s_CachedShader is null)
                {
                    s_CachedShader = Shader.Find("Anamorphic/AnamorphicSampler");
                    if (s_CaptureShader is null) Debug.LogError("Could not find AnamorphicSampler shader");
                    return s_CachedShader;
                }

                return s_CachedShader;
            }
        }

#if UNITY_EDITOR
        public static void BakeReflection(Vector3 eyePoint, float yaw, Vector2Int textureResolution, float captureFuzziness, out Texture2D reflectionTexture)
        {
            var cachedRT = RenderTexture.active;

            // create and bake the probe
            var probeGameObject = new GameObject();
            var newProbe = probeGameObject.AddComponent<ReflectionProbe>();
            var rawRes = Mathf.Max(textureResolution.x, textureResolution.y);
            var exp = Mathf.Log(rawRes, 2);
            exp = Mathf.Ceil(exp);
            newProbe.resolution = Mathf.FloorToInt(Mathf.Pow(2, exp));
            newProbe.mode = ReflectionProbeMode.Baked;
            newProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            newProbe.nearClipPlane = 1;
            newProbe.farClipPlane = 10000;

            var m = new Material(s_CaptureShader);
            var q = Quaternion.AngleAxis(yaw, Vector3.down);
            probeGameObject.transform.position = eyePoint;
            Lightmapping.BakeReflectionProbe(newProbe, "Assets/tmp.exr");
            AssetDatabase.Refresh();

            // convert from cubemap to anamorphic map
            var output = new RenderTexture(textureResolution.x, textureResolution.y, 0);
            RenderTexture.active = output;
            m.SetTexture("_Cube", newProbe.bakedTexture);
            m.SetFloat("_Yaw", yaw);
            m.SetFloat("_Fuzz", captureFuzziness);
            Graphics.Blit(newProbe.bakedTexture, output, m, 0);

            var tempTex2D = new Texture2D(textureResolution.x, textureResolution.y, TextureFormat.RGB24, false);
            tempTex2D.ReadPixels(new Rect(0, 0, textureResolution.x, textureResolution.y), 0, 0);
            tempTex2D.Apply();
            RenderTexture.active = cachedRT;

            // save as BC6H for best dynamic range
            EditorUtility.CompressTexture(tempTex2D, TextureFormat.BC6H, TextureCompressionQuality.Best);
            reflectionTexture = tempTex2D;

            // cleanup the temp textures and the probe
            AssetDatabase.DeleteAsset("Assets/tmp.exr");
            AssetDatabase.Refresh();
            DestroyImmediate(probeGameObject);
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(BakedFixedPointReflection))]
    public class BakedFixedPointReflectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update")) ((BakedFixedPointReflection)target).Capture(false);
            if (GUILayout.Button("Save as PNG")) ((BakedFixedPointReflection)target).Capture(true);
            if (GUILayout.Button("View From Here"))
            {
                var sceneCam = SceneView.lastActiveSceneView;
                if (sceneCam is not null)
                {
                    var self = (BakedFixedPointReflection)target;
                    sceneCam.AlignViewToObject(self.transform);
                }
            }
        }
    }
#endif

}