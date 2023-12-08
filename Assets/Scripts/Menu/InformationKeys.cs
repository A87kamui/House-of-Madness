using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationKeys : MonoBehaviour
{
    public static InformationKeys instance;
    public TextMeshProUGUI playerTurnText;
    public TextMeshProUGUI redPlayerText;
    public TextMeshProUGUI greenPlayerText;
    public TextMeshProUGUI yellowPlayerText;
    public TextMeshProUGUI bluePlayerText;
    public TextMeshProUGUI keysText;

    private void Awake()
    {
        instance = this;
        keysText.text = "Keys: ";
    }

    private void Start()
    {
        keysText.text = "Keys: " + GameManager.instance.keyCount;
        playerTurnText.text = "Player's Turn: ";
    }

    /// <summary>
    /// Update number of keys returned to entrance
    /// </summary>
    /// <param name="text"></param>
    public void ShowKeysText(string text)
    {
        keysText.text = "Keys: " + text;
    }

    /// <summary>
    /// Update player turn text
    /// </summary>
    public void ShowPlayerTurnText(string name)
    {
        switch (name)
        {
            case "Red":
                {
                    DeactivatePlayersText();
                    redPlayerText.gameObject.SetActive(true);
                }
                break;
            case "Green":
                {
                    DeactivatePlayersText();
                    greenPlayerText.gameObject.SetActive(true);
                }
                break;
            case "Yellow":
                {
                    DeactivatePlayersText();
                    yellowPlayerText.gameObject.SetActive(true);
                }
                break;
            case "Blue":
                {
                    DeactivatePlayersText();
                    bluePlayerText.gameObject.SetActive(true);
                }
                break;
        }
        
    }

    /// <summary>
    /// Deactivate all player text
    /// </summary>
    public void DeactivatePlayersText()
    {
        redPlayerText.gameObject.SetActive(false);
        greenPlayerText.gameObject.SetActive(false);
        yellowPlayerText.gameObject.SetActive(false);
        bluePlayerText.gameObject.SetActive(false);
    }   
}
