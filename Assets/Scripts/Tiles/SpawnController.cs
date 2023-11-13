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
    public bool haunted = false;

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
        StartCoroutine(SpawnGhostDelay());

    }

    IEnumerator SpawnGhostDelay()
    {
        if (count < 2 && !haunted)
        {
            Debug.Log("Count: " + count);
            Debug.Log("Haunted: " + haunted);
            pool[count].transform.position = ghostSpawner[count].transform.position;

            // Spawn smoke particle effect

            yield return new WaitForSeconds(1.0f);
            pool[count].SetActive(true);
            count++;
        }
        yield return new WaitForSeconds(1.5f);
    }

    public void SpawnTower()
    {
        if (count == 2)
        {
            Debug.Log("Count: " + count);
            pool[count].transform.position = ghostSpawner[count].transform.position;
            pool[count].SetActive(true);
            haunted = true;

            Debug.Log("Haunted: " + haunted);
            count = 0;  // Reset count
            Debug.Log("Count: " + count);
        }
    }
}
