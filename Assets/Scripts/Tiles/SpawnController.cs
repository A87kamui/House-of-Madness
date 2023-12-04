using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] GameObject cursePrefab;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] GameObject curseSpawner;
    [SerializeField] GameObject[] ghostSpawner;
    [SerializeField] Explosion curseExplosion;
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
                pool[i] = Instantiate(cursePrefab, transform);
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
        else if (enemyCount == 2 && !isCursed)
        {
            StartCoroutine(SpawnCurseDelay());       
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
        Debug.Log("SC SpawnGhost enemy count: " + enemyCount);
    }

    IEnumerator SpawnCurseDelay()
    {       
        pool[enemyCount].transform.position = curseSpawner.transform.position;

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
        Debug.Log("SC SpawnGhost enemy count: " + enemyCount);
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
        if (!isCursed)
        {
            if (enemyCount == 2)
            {
                enemyCount -= 1;
                ghostSpawner[enemyCount].GetComponent<Explosion>().ExplosionPlay();
                pool[enemyCount].SetActive(false);
            }
            else if (enemyCount == 1)
            {
                enemyCount -= 1;
                ghostSpawner[enemyCount].GetComponent<Explosion>().ExplosionPlay();
                pool[enemyCount].SetActive(false);
            }
        }
        
        StartCoroutine(SwitchPlayerDelay());
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
            Debug.Log("SC defeatcursedelay enemy count: " + enemyCount);
            curseExplosion.ExplosionPlay();
            pool[enemyCount].SetActive(false);
            enemyCount = 0;  // Reset count
            isCursed = false;
            GameManager.instance.curseCount -= 1;
        }
        StartCoroutine(SwitchPlayerDelay());
    }

    /// <summary>
    /// Wait then call GameManager to check number of curses
    /// </summary>
    /// <returns></returns>
    IEnumerator SwitchPlayerDelay()
    {
        Debug.Log("SC SpawnGhost enemy count: " + enemyCount);
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
    }
}
