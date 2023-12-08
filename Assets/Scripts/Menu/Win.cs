using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    /// <summary>
    /// Reset Game
    /// </summary>
    /// <param name="sceneName"></param>
    public void BackToMain(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
