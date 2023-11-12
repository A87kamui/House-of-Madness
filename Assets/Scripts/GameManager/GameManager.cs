using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager.Entity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Entity> playerList = new List<Entity>();

    public PlayerTypes playerTypes;

    public bool hasWon;
    bool startTileTaken;

    [System.Serializable]
    public class Entity
    {
        public string playerName;
        public Player player;
        public bool hasTurn;

        public enum PlayerTypes
        {
            HUMAN,
            NO_PLAYER
        }

        public PlayerTypes playerType;
        public bool hasWon;
    }

    // STATEMACHINE to keep track of what state game is at
    public enum States
    {
        WAITING,
        ROLL_DIE,
        SWITCH_PLAYER
    }

    [Header("Camera")]
    public GameObject camera;
    public Transform cam1;
    public Transform cam2;

    [Header("Player")]
    public States state;
    public Player player;
    public int activePlayer;
    bool switchingPlayer;
    bool isTurn = true;

    [Header("Die")]
    public GameObject rollButton;
    public GameObject die;
    DiceRoll dieRoll;
    [HideInInspector] public int dieRolled;
    public DiceCheckZoneScript dieNumber;

    [Header("Ghost")]
    int[] roomNumber = new int[13];

    // Set instance to GameManager
    // Allows other scripts to access GameManager at any point
    void Awake()
    {
        instance = this;
        dieRoll = die.GetComponent<DiceRoll>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ActivateButton(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            switch (state)
            {
                case States.ROLL_DIE:
                    {
                        if (isTurn)
                        {
                            player = playerList[activePlayer].player;
                            camera.transform.position = cam1.transform.position;
                            camera.transform.rotation = cam1.transform.rotation;
                            ActivateButton(true);
                            state = States.WAITING; // Wait for dice roll to complete
                        }
                    }
                    break;
                case States.WAITING:
                    {
                        // Idle
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (isTurn)
                        {
                            // Deactivate Button
                            // Deactivate Highlights

                            //StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Activate Die roll button in game
    /// </summary>
    /// <param name="on"></param>
    void ActivateButton(bool isActive)
    {
        rollButton.SetActive(isActive);
    }

    /// <summary>
    /// Roll die
    /// Deactivate Roll Dice button
    /// </summary>
    public void RollDie()
    {
        dieRoll.RollDie(); // Call Roll dice method
        ActivateButton(false);
    }

    /// <summary>
    /// Reposition camera to view gameboard
    /// Activate current player's selector
    /// Highlight tiles to move to
    /// </summary>
    /// <param name="dieNumber"></param>
    public void DieRolled(int dieNumber)
    {
        camera.transform.position = cam2.transform.position;
        camera.transform.rotation = cam2.transform.rotation;
        player.SetSelector(true);

        // TODO Check value of dice to add ghost to rooms
        // TODO Check number of ghost in a room to add ghost Towers
        // TODO Display possible locations to move to

        // Check if startTile is taken
        startTileTaken = false;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].player.currentNode == playerList[i].player.startNode)
            {
                startTileTaken = true;
                break;
            }
        }

        // If player is not out of main base
        if (!player.isOutOfBase)
        {
            if (startTileTaken && dieNumber == 1)
            {
                state = States.SWITCH_PLAYER;
            }
            else
            {
                Debug.Log("startNode: " + player.startNode + " coordinates: " + player.startNode.coordinates);
                //Node node = GridManager.instance.GetNode(player.startNode.coordinates);
                //Node node = player.startNode;
                GridManager.instance.HightLightTiles(dieNumber, player.startNode, player.baseNode);
            }
        }
        // If player is out of main base
        else
        {
            Node playerNode = player.currentNode;
            if (playerNode.topNode != null)
            {
                GridManager.instance.HightLightTiles(dieNumber, playerNode.topNode, playerNode);
            }
            if (playerNode.bottomNode != null)
            {
                GridManager.instance.HightLightTiles(dieNumber, playerNode.bottomNode, playerNode);
            }
            if (playerNode.rightNode != null)
            {
                GridManager.instance.HightLightTiles(dieNumber, playerNode.rightNode, playerNode);
            }
            if (playerNode.leftNode != null)
            {
                GridManager.instance.HightLightTiles(dieNumber, playerNode.leftNode, playerNode);
            }
        }

        // If at starting location
        switch (dieNumber)
        {
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
            case 6:

                break;
        }      
    }
}
