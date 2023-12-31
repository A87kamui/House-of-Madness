using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        INTRO,
        WAITING,
        ROLL_DIE,
        FIGHT_GHOST,
        FIGHT_CURSE,
        CURSE_CHECK,
        WIN_CHECK,
        LOSE_CHECK,
        SWITCH_PLAYER,
        WIN,
        GAMEOVER
    }

    [Header("Camera")]
    public GameObject camera;
    public Transform cam1;
    public Transform cam2;
    public Transform cam3;
    Vector2Int roomCamCoordinate;
    Vector3 roomCamPosition;
    public Vector3 roomCamHeight = new Vector3(0, 2, 0);

    [Header("Player")]
    public States state;
    public Player player;
    public int activePlayer = -1;
    public bool switchingPlayer = false;
    public int keyCount = 0;
    bool isTurn = true;
    bool gameOver = false;

    [Header("Dice")]
    public GameObject moveButton;
    public GameObject movementDie;
    DiceRoll dieRoll;
    [HideInInspector] public int dieRolled;
    public DiceCheckZoneScript dieNumber;

    public GameObject fightButton;
    public GameObject fightDie;
    public FightDiceCheckZone fightDieSide;
    FightRoll fightRoll;
    bool ghostFight = false;
    bool curseFight = true;

    [Header("Ghost")]
    [SerializeField] SpawnController[] spawner;
    [SerializeField] List<SpawnController> spawnerTrack;
    int spawnerIndex;  // Store index of which spawner to spawn
    int spawerTrackIndex;  // Store index of which spawnTracker to remove
    public int curseCount = 0;

    // Set instance to GameManager
    // Allows other scripts to access GameManager at any point
    void Awake()
    {
        instance = this;
        dieRoll = movementDie.GetComponent<DiceRoll>();
        fightRoll = fightDie.GetComponent<FightRoll>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ActivateMoveDieButton(false);
        ActivateFightDiceButton(false);
        ActivateFightDice(false);
        keyCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (activePlayer == -1 && !gameOver)
        {
            switch (state)
            {
                case States.INTRO:
                    {
                        camera.transform.position = cam2.transform.position;
                        camera.transform.rotation = cam2.transform.rotation;
                        SpawnStartingGhost();
                    }
                    break;//*/

                case States.SWITCH_PLAYER:
                    {
                        if (isTurn)
                        {
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
            }
        }
        if (activePlayer != -1 && !gameOver && playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            switch (state)
            {
                case States.WAITING:
                    {
                        // Idle
                    }
                    break;
                case States.ROLL_DIE:
                    {
                        //Debug.Log("State: " + state);
                        if (isTurn)
                        {
                            player = playerList[activePlayer].player;
                            camera.transform.position = cam1.transform.position;
                            camera.transform.rotation = cam1.transform.rotation;
                            ActivateMoveDieButton(true);
                            state = States.WAITING; // Wait for dice roll to complete
                        }
                    }
                    break;
                case States.FIGHT_GHOST:
                    {
                        //Debug.Log("State: " + state);
                        ghostFight = true;
                        curseFight = false;

                        ActivateFightDice(true);
                        ActivateFightDiceButton(true);
                        camera.transform.position = cam3.transform.position;
                        camera.transform.rotation = cam3.transform.rotation;
                        state = States.WAITING; // Wait for dice roll to complete
                    }
                    break;
                case States.FIGHT_CURSE:
                    {
                        //Debug.Log("State: " + state);
                        ghostFight = false;
                        curseFight = true;

                        ActivateFightDice(true);
                        ActivateFightDiceButton(true);
                        camera.transform.position = cam3.transform.position;
                        camera.transform.rotation = cam3.transform.rotation;
                        state = States.WAITING; // Wait for dice roll to complete
                    }
                    break;
                case States.CURSE_CHECK:
                    {
                        //Debug.Log("State: " + state);
                        if (curseCount >= 6)
                        {
                            state = States.LOSE_CHECK;
                            //Debug.Log("State: " + state);
                        }
                        else if (player.CheckCursed())
                        {
                            StartCoroutine(ViewPlayer());
                        }
                        else
                        {
                            state = States.ROLL_DIE;
                        }
                    }
                    break;
                case States.WIN_CHECK:
                    {
                        //Debug.Log("State: " + state);
                        keyCount++;
                        InformationKeys.instance.ShowKeysText(keyCount + "");
                        if (keyCount >= 8)
                        {
                            gameOver = true;
                            state = States.WIN;
                        }
                        else
                        {
                            state = States.SWITCH_PLAYER;
                        }
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        //Debug.Log("State: " + state);
                        if (isTurn)
                        {
                            player.SetSelector(false);
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                        //Debug.Log("State: " + state);
                    }
                    break;
                case States.LOSE_CHECK:
                    {
                        gameOver = true;
                        state = States.GAMEOVER;
                        //Debug.Log("State: " + state);
                    }
                    break;
            }
        }
        if (gameOver)
        {
            switch (state)
            {
                case States.WIN:
                    {
                        //Debug.Log("Win");
                        state = States.WAITING;
                        SceneManager.LoadScene("Win");
                    }
                    break;
                case States.GAMEOVER:
                    {
                        //Debug.Log("GameOver");
                        state = States.WAITING;
                        SceneManager.LoadScene("Game Over");
                    }
                    break;
                case States.WAITING:
                    {
                        // Idle
                    }
                    break;
            }
        }           
    }


    /// <summary>
    /// Activate Die roll button in game
    /// </summary>
    /// <param name="on"></param>
    void ActivateMoveDieButton(bool isActive)
    {
        moveButton.SetActive(isActive);
    }

    /// <summary>
    /// Activate Fight Die roll button in game
    /// Pass player to FightDiceCheckZone
    /// </summary>
    /// <param name="isActive"></param>
    void ActivateFightDiceButton(bool isActive)
    {
        fightButton.SetActive(isActive);
        fightDieSide.player = player;
    }

    /// <summary>
    /// Roll die
    /// Deactivate Roll Dice button
    /// </summary>
    public void RollDie()
    {
        dieRoll.RollDie(); // Call Roll die method
        ActivateMoveDieButton(false);
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
    }

    /// <summary>
    /// Spawn Ghost or tower
    /// </summary>
    public void SpawnEnemy(int dieNumber)
    {
        //Need to randomly spawn a ghost and track
        if (spawnerTrack.Count > 0)
        {
            spawnerIndex = Random.Range(0, spawnerTrack.Count + 1);
            spawerTrackIndex = spawnerIndex;

            if (spawnerIndex < spawnerTrack.Count)
            {
                // Get the index of remaining rooms available and spawn ghost
                spawnerIndex = spawnerTrack[spawnerIndex].index;
                spawner[spawnerIndex].SpawnGhost();

                roomCamCoordinate = GridManager.instance.GetCoordinatesFromPosition(spawner[spawnerIndex].roomCamera.transform.position);
                roomCamPosition = GridManager.instance.GetPositionFromCoordinates(roomCamCoordinate);

                camera.transform.position = roomCamPosition + roomCamHeight;
                camera.transform.rotation = spawner[spawnerIndex].roomCamera.transform.rotation;

                // Remove from spawerTrack at random num index
                spawnerTrack.RemoveAt(spawerTrackIndex);
            }
            else 
            {
                SummonMoreGhost(dieNumber); 
            }
        }
        else
        {
            SummonMoreGhost(dieNumber);
        }

        StartCoroutine(SpawnEnemyDelay(dieNumber));
        state = States.WAITING;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dieNumber"></param>
    /// <returns></returns>
    IEnumerator SpawnEnemyDelay(int dieNumber)
    {
        // Wait to show ghost that spawned then call roll die
        yield return new WaitForSeconds(4.0f);
        DieRolled(dieNumber);
    }

    /// <summary>
    /// Add all ghost spawners back to spawner track
    /// </summary>
    void SummonMoreGhost(int dieNumber)
    {
        spawnerTrack.Clear();
        foreach (SpawnController spawnController in spawner)
        {
            spawnerTrack.Add(spawnController);
        }

        //Debug.Log("Shuffle ghost back");
        StartCoroutine(SpawnEnemyDelay(dieNumber));
        state = States.WAITING;
    }

    /// <summary>
    /// Call Spawn starting ghosts delay
    /// </summary>
    void SpawnStartingGhost()
    {
        // Rooms = B D E F       
        spawner[1].SpawnGhost();
        spawner[3].SpawnGhost();
        spawner[4].SpawnGhost();
        spawner[5].SpawnGhost();

        StartCoroutine(SpawnStartingGhostDelay());
        state = States.WAITING;     
    }

    /// <summary>
    /// Spawn starting ghost
    /// Wait
    /// Then change state to ROLL_DIE
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnStartingGhostDelay()
    {
        yield return new WaitForSeconds(4.0f);
        state = States.SWITCH_PLAYER;
    }

    /// <summary>
    /// Activate or deactivate fight dice for ghost or curse
    /// </summary>
    /// <param name="isActive"></param>
    void ActivateFightDice(bool isActive)
    {
        fightRoll.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Roll fight die/dice
    /// Deactivate fight Button
    /// </summary>
    public void FightDieRoll()
    {
        fightRoll.RollDie();   
        ActivateFightDiceButton(false);
    }

    /// <summary>
    /// Create a delay when switching players
    /// </summary>
    /// <returns></returns>
    IEnumerator SwitchPlayer()
    {
        if (switchingPlayer)
        {
            yield break;
        }

        switchingPlayer = true;
        yield return new WaitForSeconds(2.0f);
        // Set next player
        SetNextActivePlayer();

        switchingPlayer = false;
    }

    /// <summary>
    /// Activate the next player
    /// who has not won
    /// </summary>
    void SetNextActivePlayer()
    {
        activePlayer++;
        activePlayer %= playerList.Count;
        //Debug.Log("New active player: " + activePlayer);

        // Set current active player and to do a Curse check
        player = playerList[activePlayer].player;
        InformationKeys.instance.ShowPlayerTurnText(player.name);
        state = States.CURSE_CHECK;
    }

    /// <summary>
    /// View player's location in room before fighting a curse
    /// </summary>
    /// <returns></returns>
    IEnumerator ViewPlayer()
    {
        camera.transform.position = player.transform.position + roomCamHeight;
        camera.transform.rotation = player.currentNode.GetComponentInChildren<SpawnController>().roomCamera.transform.rotation;
        yield return new WaitForSeconds(2.0f);
        state = States.FIGHT_CURSE;
    }
}
