using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    /// <summary>
    /// Go to Game scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void StartTheGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
