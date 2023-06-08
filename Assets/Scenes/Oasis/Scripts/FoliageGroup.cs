using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


[Serializable]
public struct FoliageElement
{
    public GameObject prefab;
    public float weight;
    public Vector2 minMaxScale;
    public float angleVariance;
}

public class FoliageGroup : MonoBehaviour
{
    public List<FoliageElement> m_FoliageElements = new List<FoliageElement>();

    public float AngleVariance;
    
    public float m_Radius;
    public int m_Count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reproject()
    {
        
        foreach (Transform child in transform)
        {
            PutOnTerrain(child, new Vector3(child.position.x, transform.position.y, child.position.z));
            
            child.Rotate(transform.up, Random.value * 360);
            child.Rotate(transform.right, (Random.value * AngleVariance)-0.5f*AngleVariance);
            
            //Debug.Log("This is a " + m_TransformToFoliageElement[child].prefab.name + "!");
        }
    }

    public void Shuffle()
    {
        
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        for(int i = 0; i < m_Count; i++)
        {
            FoliageElement foliageElement = GetRandomElement();//m_FoliageElements[Random.Range(0, m_FoliageElements.Count)];
            
            Vector2 pointInCirkle = RandomPointInCirkle();
            float scale = Random.Range(foliageElement.minMaxScale.x, foliageElement.minMaxScale.y);

            if (foliageElement.prefab == null)
            {
                Debug.LogError("Foliage element does not have prefab assigned");
                continue;
            }

            GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(foliageElement.prefab); //Instantiate(foliageElement.prefab, position, Quaternion.identity, transform);
            instance.transform.parent = transform;
            instance.transform.position = new Vector3(pointInCirkle.x, 0, pointInCirkle.y) + transform.position;
            instance.transform.localScale = Vector3.one * scale;
        }
    }

    public void Place()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        for(int i = 0; i < m_Count; i++)
        {
            FoliageElement foliageElement = GetRandomElement();//m_FoliageElements[Random.Range(0, m_FoliageElements.Count)];
            
            Vector2 pointInCirkle = RandomPointInCirkle();
            float scale = Random.Range(foliageElement.minMaxScale.x, foliageElement.minMaxScale.y);

            if (foliageElement.prefab == null)
            {
                Debug.LogError("Foliage element does not have prefab assigned");
                continue;
            }

            GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(foliageElement.prefab); //Instantiate(foliageElement.prefab, position, Quaternion.identity, transform);
            
            
            instance.transform.parent = transform;
            PutOnTerrain(instance.transform, pointInCirkle);
            
            instance.transform.Rotate(transform.up, Random.value * 360);
            instance.transform.Rotate(transform.right, (Random.value * foliageElement.angleVariance)-0.5f*foliageElement.angleVariance);
            //instance.transform.rotation = rotation;
            instance.transform.localScale = Vector3.one * scale;
        }
        
    }

    //Picks a random element based on weight
    private FoliageElement GetRandomElement()
    {
        float[] sumArray = new float[m_FoliageElements.Count];
        float totalWeight = 0;

        for(int i = 0; i < m_FoliageElements.Count; i++)
        {
            float previousSum = i > 0 ? sumArray[i - 1] : 0;
            sumArray[i] = previousSum + m_FoliageElements[i].weight;
            totalWeight += m_FoliageElements[i].weight;
        }
        
        float randomValue = Random.Range(0, totalWeight);
        
        for(int i = 0; i < m_FoliageElements.Count; i++)
        {
            if (randomValue < sumArray[i])
            {
                return m_FoliageElements[i];
            }
        }

        throw new Exception("Could not find random element");
    }

    private void PutOnTerrain(Transform elementTransform, Vector3 point)
    {
        if(Physics.Raycast(point, Vector3.down, out RaycastHit hit))
        {
            point = hit.point;
            elementTransform.position = point;
            
            elementTransform.up = Vector3.Normalize(Vector3.up + hit.normal);
        }
    }
    
    private Vector2 RandomPointInCirkle()
    {
        float r = m_Radius * Mathf.Sqrt(Random.value);
        float theta = Random.value * 2 * Mathf.PI;
        
        float x = r * Mathf.Cos(theta);
        float y = r * Mathf.Sin(theta);
        
        return new Vector2(x, y);
    }

}