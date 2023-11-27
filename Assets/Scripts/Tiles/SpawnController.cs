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

    int enemyCount = 0;

    public Transform roomCamera;
    public bool isCursed = false;
    public bool spawnedGhost = false;
    public int index;
    

    private void Awake()
    {
        PopulatePool();
        enemyCount = 0;
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
        if (enemyCount < 2 && !isCursed)
        {
            StartCoroutine(SpawnGhostDelay());
            
        }
        if (enemyCount == 2 && !isCursed)
        {
            StartCoroutine(SpawnTowerDelay());       
        }
    }

    IEnumerator SpawnGhostDelay()
    {
        pool[enemyCount].transform.position = ghostSpawner[enemyCount].transform.position;

        yield return new WaitForSeconds(2.0f);
        // Spawn smoke particle effect
        pool[enemyCount].SetActive(true);
        spawnedGhost = true;
        isCursed = false;
        enemyCount++;
        print("Name: " + this.name);
    }

    IEnumerator SpawnTowerDelay()
    {       
        pool[enemyCount].transform.position = towerSpawner.transform.position;

        //particle effect for ghost to be removed

        yield return new WaitForSeconds(2.0f);
        // Deactivate ghost
        for (int i = 0; i < 2; i++)
        {
            pool[i].SetActive(false);
            // Spawn smoke particle effect
        }
        spawnedGhost = false;

        pool[enemyCount].SetActive(true);
        isCursed = true;
        GameManager.instance.curseCount += 1;
        print("Name: " + this.name);
    }

    /// <summary>
    /// Start a delay to remove ghost in room
    /// </summary>
    public void DefeatGhost()
    {
        StartCoroutine(DefeatGhostDelay());
        
    }

    /// <summary>
    /// Remove ghost(s) in room
    /// </summary>
    /// <returns></returns>
    IEnumerator DefeatGhostDelay()
    {
        yield return new WaitForSeconds(2.0f);
        print("Count: " + enemyCount);
        if (enemyCount == 2)
        {
            enemyCount -= 1;
            pool[enemyCount].SetActive(false);
        }
        else if (enemyCount == 1)
        {
            enemyCount -= 1;
            pool[enemyCount].SetActive(false);
        }

        StartCoroutine(CurseCheckDelay());
    }

    /// <summary>
    /// Start a delay to remove tower in room
    /// </summary>
    public void DefeatCurse()
    {
        StartCoroutine(DefeatCurseDelay());

    }

    /// <summary>
    /// Remove Tower in room
    /// </summary>
    /// <returns></returns>
    IEnumerator DefeatCurseDelay()
    {
        yield return new WaitForSeconds(2.0f);

        if (isCursed)
        {
            pool[enemyCount].SetActive(false);
            enemyCount = 0;  // Reset count
            isCursed = false;
            GameManager.instance.curseCount -= 1;
        }

        StartCoroutine(CurseCheckDelay());
    }

    /// <summary>
    /// Wait then call GameManager to check number of curses
    /// </summary>
    /// <returns></returns>
    IEnumerator CurseCheckDelay()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.state = GameManager.States.CURSE_CHECK;
    }
}
