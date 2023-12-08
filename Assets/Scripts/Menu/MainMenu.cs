using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject rulesMenu;
    public GameObject rules2Menu;
    public GameObject rules3Menu;
    public GameObject creditsMenu;

    public Button quitButton;

    /// <summary>
    /// Start game
    /// Go to Game scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void StartTheGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMain()
    {
        CloseAllMenu();
        mainMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// View rules menu
    /// </summary>
    public void ViewRules()
    {
        CloseAllMenu();
        rulesMenu.SetActive(true);
    }

    /// <summary>
    /// View rules 2 menu
    /// </summary>
    public void ViewRules2()
    {
        CloseAllMenu();
        rules2Menu.SetActive(true);
    }

    /// <summary>
    /// View rules 3 menu
    /// </summary>
    public void ViewRules3()
    {
        CloseAllMenu();
        rules3Menu.SetActive(true);
    }

    /// <summary>
    /// View credits menu
    /// </summary>
    public void CreditsMenu()
    {
        CloseAllMenu();
        creditsMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Close all menu objects
    /// </summary>
    public void CloseAllMenu()
    {
        mainMenu.gameObject.SetActive(false);
        rulesMenu.gameObject.SetActive(false);
        rules2Menu.gameObject.SetActive(false);
        rules3Menu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Player quit game");
    }
}
