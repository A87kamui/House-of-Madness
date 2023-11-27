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
        INTRO,
        WAITING,
        ROLL_DIE,
        FIGHT_GHOST,
        FIGHT_CURSE,
        CURSE_CHECK,
        SWITCH_PLAYER,
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
    public int activePlayer;
    bool switchingPlayer;
    bool isTurn = true;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            switch (state)
            {
                case States.INTRO:
                    {
                        camera.transform.position = cam2.transform.position;
                        camera.transform.rotation = cam2.transform.rotation;
                        SpawnStartingGhost();
                    }
                break;
                case States.WAITING:
                    {
                        // Idle
                    }
                    break;
                case States.ROLL_DIE:
                    {
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
                        print("Curse Count: " + curseCount);
                        if (curseCount == 6)
                        {
                            state = States.GAMEOVER;
                        }
                        else
                        {
                            //***NEED TO SWITCH PLAYERS***
                            //state = States.SWITCH_PLAYER;
                            state = States.ROLL_DIE;
                        }
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (isTurn)
                        {
                            //StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;
                case States.GAMEOVER:
                    {
                        print("GameOver");
                        state = States.WAITING;
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
                state = States.CURSE_CHECK;
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

        print("Shuffle ghost back");
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
        state = States.CURSE_CHECK;
    }

    /// <summary>
    /// Activate or deactivate fight dice for ghost or curse
    /// </summary>
    /// <param name="isActive"></param>
    void ActivateFightDice(bool isActive)
    {
        fightRoll.gameObject.SetActive(isActive);
        /*if (ghostFight)
        {
            //Debug.Log("Ghost fight die");
            fightRoll1.gameObject.SetActive(isActive);
        }
        if (curseFight)
        {
            //Debug.Log("Curse fight dice");
            fightRoll1.gameObject.SetActive(isActive);
            fightRoll2.gameObject.SetActive(isActive);
        } //*/  
    }

    /// <summary>
    /// Roll fight die/dice
    /// Deactivate fight Button
    /// </summary>
    public void FightDieRoll()
    {
        fightRoll.RollDie();
        /*if (ghostFight)
        {
            fightRoll1.RollDie();
        }
        if (curseFight)
        {
            fightRoll1.RollDie();
            fightRoll2.RollDie();
        }//*/    
        ActivateFightDiceButton(false);
    }

    /// <summary>
    /// Reset dice positions after getting value. 
    /// </summary>
    /*public void ResetDicePosition()
    {
        Debug.Log("Reset position");
        GameManager.instance.fightDie1.transform.position = fightDie1.GetComponentInParent<Transform>().position + diePostion1;
        //GameManager.instance.fightDie2.transform.position = fightDie2.GetComponentInParent<Transform>().position + diePostion2;
        ActivateFightDice(false);
    }//*/
}
