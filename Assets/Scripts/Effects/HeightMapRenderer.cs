using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = System.Object;

[ExecuteInEditMode]
public class HeightMapRenderer : MonoBehaviour
{
    public string m_Path;
    public float m_Size;
    public Vector2 m_HeightBounds;
    public int m_Resolution;

    public LayerMask m_CullingMask;

    private RenderTexture temporaryTexture;
    private Camera m_HeightmapCamera;

    public void RenderHeightMap()
    {
        if (temporaryTexture != null)
        {
            DestroyImmediate(temporaryTexture);
            temporaryTexture = null;
        }
        temporaryTexture = new RenderTexture(m_Resolution, m_Resolution, GraphicsFormat.B10G11R11_UFloatPack32,
            GraphicsFormat.D24_UNorm_S8_UInt);

        if (m_HeightmapCamera == null)
        {
            GameObject cameraGO = new GameObject("[Heightmap Camera]");
            cameraGO.transform.parent = transform;
            cameraGO.hideFlags = HideFlags.HideAndDontSave;

            cameraGO.transform.localPosition = new Vector3(0, 100, 0);
            cameraGO.transform.SetLocalPositionAndRotation(new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0));
            m_HeightmapCamera = cameraGO.AddComponent<Camera>();
            m_HeightmapCamera.AddComponent<UniversalAdditionalCameraData>().SetRenderer(2);
            m_HeightmapCamera.enabled = false;
        }
        m_HeightmapCamera.orthographic = true;
        m_HeightmapCamera.orthographicSize = m_Size * 0.5f;
        m_HeightmapCamera.cullingMask = m_CullingMask;

        m_HeightmapCamera.targetTexture = temporaryTexture;
        m_HeightmapCamera.Render();
    }

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    void OnEndCameraRendering(ScriptableRenderContext Context, Camera camera)
    {
        if (camera != m_HeightmapCamera) return;

        RenderTexture rt = camera.targetTexture;
        
        WriteRenderTextureToPNG(rt, m_Path);

        SetImportSettings(m_Path);
    }

    void WriteRenderTextureToPNG(RenderTexture rt, string path)
    {
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAHalf, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] bytes = ImageConversion.EncodeToPNG(tex);
        
        DestroyImmediate(tex);

        string absolutePath = Application.dataPath + "/" + path;

        File.WriteAllBytes(absolutePath, bytes);
        AssetDatabase.Refresh();
    }

    void SetImportSettings(string path)
    {
        string assetsPath = "Assets/" + m_Path;
        TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(assetsPath);
        importer.sRGBTexture = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
    }
}

[CustomEditor(typeof(HeightMapRenderer))]
public class HeightMapRendererEditor : Editor
{
    
    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Render"))
        {
            HeightMapRenderer renderer = (HeightMapRenderer) target;
            renderer.RenderHeightMap();
        }
    }
    
    
}