using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using M11_Vissen;
public class EnvironmentSpawner : MonoBehaviour
{
    
    #region variables
    #pragma warning disable 649
    
    [SerializeField] private GameObject[] rockObjects;
    [SerializeField] private GameObject[] bubbleObjects;
    [SerializeField] private GameObject[] seaweedObjects;
    [SerializeField] private Transform pier;
    [SerializeField] private Vector3 pierSpeed;

    [SerializeField] private Transform rockStartRange = null;
    [SerializeField] private Transform rockEndRange = null;
    [SerializeField] private Transform bubbleStartRange = null;
    [SerializeField] private Transform bubbleEndRange = null;
    [SerializeField] private Transform seaweedStartRange = null;
    [SerializeField] private Transform seaweedEndRange = null;

    [SerializeField] private float rockFrequency = 1.0f;
    [SerializeField] private float bubbleFrequency = 1.0f;
    [SerializeField] private float seaweedFrequency = 1.0f;

    [SerializeField] private M11_Manager manager = null;
    [SerializeField] private Transform parent = null;

    private float rockTimer = 0.0f;
    private float bubbleTimer = 0.0f;
    private float seaweedTimer = 0.0f;

    private bool spawnAllowed = false;
    private Vector3 offset = Vector3.zero;

    #pragma warning restore 649
    #endregion
    
    private void Start()
    {
        offset = parent.position - transform.position;
    }
    private void Update()
    {
        transform.position = new Vector3(parent.position.x - offset.x, transform.position.y, parent.position.z - offset.z);

        if (spawnAllowed)
        {
            if (rockTimer <= 0.0f)
            {
                rockTimer = 1 / rockFrequency;
                SpawnObject(rockStartRange, rockEndRange, rockObjects[Random.Range(0, rockObjects.Length)]);
            }
            else
            {
                rockTimer -= Time.deltaTime;
            }

            if (bubbleTimer <= 0.0f)
            {
                bubbleTimer = 1 / bubbleFrequency;
                SpawnObject(bubbleStartRange, bubbleEndRange, bubbleObjects[Random.Range(0, bubbleObjects.Length)]);
            }
            else
            {
                bubbleTimer -= Time.deltaTime;
            }

            if (seaweedTimer <= 0.0f)
            {
                seaweedTimer = 1 / seaweedFrequency;
                SpawnObject(seaweedStartRange, seaweedEndRange, seaweedObjects[Random.Range(0, seaweedObjects.Length)]);
            }
            else
            {
                seaweedTimer -= Time.deltaTime;
            }

            pier.position += pierSpeed * Time.deltaTime;
        }
    }

    public void SetSpawning(bool shouldSpawn)
    {
        if (shouldSpawn != spawnAllowed)
        {
            spawnAllowed = shouldSpawn;
        }
    }
    private void SpawnObject(Transform rangeStart, Transform rangeEnd, GameObject objectToSpawn)
    {
        float position = Random.Range(rangeStart.position.y, rangeEnd.position.y);
        GameObject spawnedObject = Instantiate(objectToSpawn, new Vector3(transform.position.x, position, transform.position.z), objectToSpawn.transform.rotation);
        spawnedObject.GetComponent<FishBehaviour>().SetManager(manager);
        spawnedObject.GetComponent<FishBehaviour>().Move(Vector3.zero);
    }
}
