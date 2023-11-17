using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] GameObject towerSpawner;
    [SerializeField] GameObject[] ghostSpawner;
    [SerializeField] int poolSize = 3;
    GameObject[] pool;

    int count = 0;

    public Transform roomCamera;
    public bool haunted = false;
    public bool spawnedGhost = false;
    public int index;
    

    private void Awake()
    {
        PopulatePool();
        count = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// IOnstantiate objects to pool array
    /// </summary>
    private void PopulatePool()
    {
        pool = new GameObject[poolSize];

        for (int i = 0; i < pool.Length; i++)
        {
            // Instantiate an enemy at Spawner's transform position
            if (i < 2)
            {
                pool[i] = Instantiate(ghostPrefab, transform);
            }
            else
            {
                pool[i] = Instantiate(towerPrefab, transform);
            }
            pool[i].SetActive(false);
        }
    }

    /// <summary>
    /// Spawn Ghost in room
    /// </summary>
    public void SpawnGhost()
    {
        if (count < 2 && !haunted)
        {
            StartCoroutine(SpawnGhostDelay());
            
        }
        if (count == 2 && !haunted)
        {
            StartCoroutine(SpawnTowerDelay());       
        }
        //Debug.Log("Count: " + count);
        //Debug.Log("Haunted: " + haunted);
    }

    IEnumerator SpawnGhostDelay()
    {

        pool[count].transform.position = ghostSpawner[count].transform.position;

        yield return new WaitForSeconds(2.0f);
        // Spawn smoke particle effect
        pool[count].SetActive(true);
        spawnedGhost = true;
        count++;
    }

    IEnumerator SpawnTowerDelay()
    {
        
        pool[count].transform.position = towerSpawner.transform.position;

        //particle effect for ghost to be removed

        yield return new WaitForSeconds(2.0f);
        // Deactivate ghost
        for (int i = 0; i < 2; i++)
        {
            pool[i].SetActive(false);
            // Spawn smoke particle effect
        }
        spawnedGhost = false;

        pool[count].SetActive(true);
        haunted = true;
        count = 0;  // Reset count
        
    }
}
