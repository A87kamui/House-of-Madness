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
        FIGHT_HAUNTED,
        SWITCH_PLAYER
    }

    [Header("Camera")]
    public GameObject camera;
    public Transform cam1;
    public Transform cam2;
    Vector2Int roomCamCoordinate;
    Vector3 roomCamPosition;
    Vector3 roomCamHeight = new Vector3(0, 2, 0);

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
    [SerializeField] SpawnController[] spawner;
    [SerializeField] List<SpawnController> spawnerTrack;
    int spawnerIndex;  // Store index of which spawner to spawn
    int spawerTrackIndex;  // Store index of which spawnTracker to remove

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
                        ActivateButton(true);
                        state = States.WAITING; // Wait for dice roll to complete
                    }
                }
                    break;
                case States.FIGHT_GHOST:
                {
                        // Idle
                        Debug.Log("Fight ghost");
                        state = States.ROLL_DIE;
                }
                    break;
                case States.FIGHT_HAUNTED:
                    {
                        // Idle
                        Debug.Log("Fight Haunted");
                        state = States.ROLL_DIE;
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
        //Debug.Log("Spawn Ghost/tower");
        //Debug.Log("SapwnTracker count: " + spawnerTrack.Count);
        Debug.Log("SpawnTracker is empty: " + spawnerTrack == null);

        //Need to randomly spawn a ghost and track
        if (spawnerTrack.Count > 0)
        {
            spawnerIndex = Random.Range(0, spawnerTrack.Count + 1);
            spawerTrackIndex = spawnerIndex;
            Debug.Log("Random spawnerIndex number: " + spawnerIndex);

            if (spawnerIndex < spawnerTrack.Count)
            {
                // Get the index of remaining rooms available
                spawnerIndex = spawnerTrack[spawnerIndex].index;
                Debug.Log("spawnerIndex from track: " + spawnerIndex);
                spawnerTrack.RemoveAt(spawerTrackIndex);
            }
            
            Debug.Log("SpawnTracker count after remove: " + spawnerTrack.Count);
        }

        if (spawerTrackIndex < spawnerTrack.Count)
        {
            spawner[spawnerIndex].SpawnGhost();

            roomCamCoordinate = GridManager.instance.GetCoordinatesFromPosition(spawner[spawnerIndex].roomCamera.transform.position);
            roomCamPosition = GridManager.instance.GetPositionFromCoordinates(roomCamCoordinate);


            camera.transform.position = roomCamPosition + roomCamHeight;
            camera.transform.rotation = spawner[spawnerIndex].roomCamera.transform.rotation;
        }
        else if (spawnerIndex == spawnerTrack.Count || spawnerTrack == null)
        {
            Debug.Log("SHUFFLE");
            spawnerTrack.Clear();
            foreach(SpawnController spawnController in spawner)
            {
                spawnerTrack.Add(spawnController);
            }
        }
        
        StartCoroutine(SpawnEnemyDelay(dieNumber));
        state = States.WAITING;
    }

    IEnumerator SpawnEnemyDelay(int dieNumber)
    {
        // Wait to show ghost that spawned then call roll die
        yield return new WaitForSeconds(4.0f);
        DieRolled(dieNumber);
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
        state = States.ROLL_DIE;
    }

    

}
