using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SmartSpawner : MonoBehaviour
{

    [Header("Settings")]
    [Tooltip("The list of objects you want to spawn.")]
    public List<GameObject> objectsToSpawn;

    [Tooltip("How many times to cycle through the entire list.")]
    public int numberOfCopies = 1;

    [Tooltip("Time in seconds between each spawn.")]
    public float spawnCooldown = 0.5f;


    [Header("Location Overrides")]
    [Tooltip("If true, the objects will spawn half their hight higher.")]
    public bool raiseFromTheGround = false;

    [Tooltip("If true, the spawner will move to 'Target Coordinates' before starting.")]
    public bool useSpecificCoordinates = false;

    [Tooltip("The specific 3 coordinates (X, Y, Z) to spawn from if above is checked.")]
    public Vector3 targetCoordinates;


    [Header("Spacing Logic")]
    [Tooltip("How much space to keep between objects.")]
    [Range(0f, 10f)]
    public float minSpacing = 1.5f;


    private int _currentSpawnCount = 0;
    private int _totalObjectsToSpawn = 0;

    private List<float> _heightOffsets;

    void Start()
    {
        if (objectsToSpawn == null || objectsToSpawn.Count == 0)
        {
            Debug.LogError("SmartSpawner: No objects assigned in the list");
            return;
        }

        ApplyLocationOverride();
        CalculateHeightOffsets();

        _totalObjectsToSpawn = objectsToSpawn.Count * numberOfCopies;

        StartCoroutine(SpawnRoutine());
    }

    void ApplyLocationOverride()
    {
        if (useSpecificCoordinates)
        {
            transform.position = targetCoordinates;
        }
    }

    void CalculateHeightOffsets()
    {
        _heightOffsets = new List<float>();

        for (int i = 0; i < objectsToSpawn.Count; i++)
        {
            float offset = 0f;
            if (raiseFromTheGround && objectsToSpawn[i] != null)
            {
                Renderer r = objectsToSpawn[i].GetComponent<Renderer>();
                if (r == null)
                {
                    r = objectsToSpawn[i].GetComponentInChildren<Renderer>();
                }
                if (r != null)
                {
                    offset = r.bounds.size.y / 2f;
                }
                else
                {
                    Debug.LogWarning("SmartSpawner: Could not find a Renderer on one of the objects to calculate height!");
                }
            }
            _heightOffsets.Add(offset);
        }
    }
    IEnumerator SpawnRoutine()
    {
        while (_currentSpawnCount < _totalObjectsToSpawn)
        {
            SpawnNextEntity();
            _currentSpawnCount++;
            
            if (spawnCooldown > 0)
            {
                yield return new WaitForSeconds(spawnCooldown);
            }
            else
            {
                yield return null;
            }
        }
    }

    void SpawnNextEntity()
    {
        int listIndex = _currentSpawnCount % objectsToSpawn.Count;

        if (objectsToSpawn[listIndex] == null) return;

        Vector3 finalPosition = GetObjectPosition(listIndex);
        
        GameObject newObj = Instantiate(objectsToSpawn[listIndex], finalPosition, Quaternion.identity);
        newObj.transform.parent = transform;
    }

    Vector3 GetObjectPosition(int listIndex)
    {
        Vector3 spawnOffset = CalculateSpiralPoint(_currentSpawnCount);
        float yAdjustment = _heightOffsets[listIndex];
        return transform.position + spawnOffset + new Vector3(0, yAdjustment, 0);
    }

    Vector3 CalculateSpiralPoint(int index)
    {
        if (index == 0) return Vector3.zero;

        float goldenAngle = 2.39996f;

        float radius = minSpacing * Mathf.Sqrt(index);

        float angle = index * goldenAngle;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(x, 0, z);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = useSpecificCoordinates ? targetCoordinates : transform.position;

        int listCount = (objectsToSpawn != null && objectsToSpawn.Count > 0) ? objectsToSpawn.Count : 1;
        int previewCount = numberOfCopies * listCount;

        for (int i = 0; i < previewCount; i++)
        {
            Vector3 point = center + CalculateSpiralPoint(i);
            Gizmos.DrawWireSphere(point, 0.3f);

            /*if (i < previewCount - 1)
            {
                Vector3 nextPoint = center + CalculateSpiralPoint(i + 1);
                Gizmos.DrawLine(point, nextPoint);
            }*/
        }
    }

}
