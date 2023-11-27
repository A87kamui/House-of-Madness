using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("SELECTOR")]
    public GameObject selector;

    [Header("NODES")]
    public Node baseNode;   // Base node for player before entering playarea
    public Node startNode;  // Starting node in playarea
    public Node currentNode;
    public Node goalNode;

    [Header("BOOLS")]
    public bool isOutOfBase;  // In the game
    bool isMoving;
    public bool hasTurn;

    PathFinder pathFinder;
    List<Node> path = new List<Node>();
    public Vector2Int currentCoordinates;
    public Transform playerCam;
    Vector3 playerCamHeight = new Vector3(0, 2, 0);
    int pathIndex;   // index in path
    int startNodeIndex;

    // Arc Movement
    float speed = 4.0f;
    float amplitude = 0.75f; // hight
    float curveTime = 0.0f; // Track time it takes to move from one node to the next

    private void Awake()
    {
                
    }
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = GetComponent<PathFinder>();
        // Deactivate selector on board
        SetSelector(false);
        currentCoordinates = GridManager.instance.GetCoordinatesFromPosition(transform.position);
        currentNode = GridManager.instance.GetNode(currentCoordinates);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Set selection on/off
    /// </summary>
    /// <param name="isActive"></param>
    public void SetSelector(bool isActive)
    {
        selector.SetActive(isActive);
        hasTurn = isActive;
    }

    /// <summary>
    /// Sets isOutOfBase
    /// Starts coroutine and calls move
    /// </summary>
    public void LeaveBase(Vector2Int destinationCoordinates)
    {
        isOutOfBase = true;
        pathIndex = 0;  // Reset pathIndex
        currentCoordinates = startNode.coordinates;
        path = pathFinder.GetPath(currentCoordinates, destinationCoordinates);
        GameManager.instance.camera.transform.rotation = this.playerCam.rotation;
        StartCoroutine(Move());
    }

    /// <summary>
    /// Gets path to destination
    /// </summary>
    /// <param name="destinationCoordinates"></param>
    public void DestinationPath(Vector2Int destinationCoordinates)
    {
        pathIndex = 0;  // Reset pathIndex
        currentCoordinates = GridManager.instance.GetCoordinatesFromPosition(transform.position);
        path = pathFinder.GetPath(currentCoordinates, destinationCoordinates);
        GameManager.instance.camera.transform.rotation = this.playerCam.rotation;
        StartCoroutine(Move());
    }

    /// <summary>
    /// Move to next node
    /// </summary>
    /// <returns></returns>
    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;     // Stop the Coroutine
        }

        isMoving = true;
        selector.SetActive(false);
        
        while(pathIndex < path.Count)
        {
            // Code here to pathfind to get path
            // Code here to do arc move
            Vector3 startPosition = transform.position;
            Vector3 nextPosition = path[pathIndex].transform.position;

            if (startPosition != nextPosition)
            {
                // while moving
                while (MoveInArcToNextNode(startPosition, nextPosition, speed))
                {
                    yield return null;
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            curveTime = 0;
            pathIndex++;
        }

        // Reset node of player's last postion
        if (currentNode != null)
        {
            currentNode.isTaken = false;
        }

        // Update goal node to be the the node player moved to
        goalNode = path[path.Count - 1];
        goalNode.player = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;
        isMoving = false;

        yield return new WaitForSeconds(2.0f);

        // Check for ghost to fight
        if (CheckFightGhost())
        {
            GameManager.instance.state = GameManager.States.FIGHT_GHOST;
        }

        // Check if haunted to fight but skip movement
        else if (CheckCursed())
        {
            GameManager.instance.state = GameManager.States.FIGHT_CURSE;
        }

        else if (!CheckFightGhost() && !CheckCursed())
        {
            // Report back to game manager to check curse count
            GameManager.instance.state = GameManager.States.CURSE_CHECK;
        }
        
    }

    /// <summary>
    /// Move in an arc to goal
    /// Move camera with player
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="goalPosition"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    bool MoveInArcToNextNode(Vector3 startPosition, Vector3 goalPosition, float speed)
    {
        GameManager.instance.camera.transform.position = this.playerCam.position;
        curveTime += speed * Time.deltaTime;

        // Track player position between two points
        Vector3 myPosition = Vector3.Lerp(startPosition, goalPosition, curveTime);

        // Creates arc movement on player's y position
        myPosition.y = amplitude * Mathf.Sin(Mathf.Clamp01(curveTime) * Mathf.PI);

        return goalPosition != (transform.position = Vector3.Lerp(transform.position, myPosition, curveTime));
    }

    /// <summary>
    /// Check if fighting a ghost
    /// </summary>
    bool CheckFightGhost()
    {
        if (currentNode.tag == "SpawnTile")
        {
            return currentNode.GetComponentInChildren<SpawnController>().spawnedGhost;
        }
        return false;
    }

    /// <summary>
    /// Check if fighting a curse
    /// </summary>
    /// <returns></returns>
    bool CheckCursed()
    {
        if (currentNode.tag == "SpawnTile")
        {
            return currentNode.GetComponentInChildren<SpawnController>().isCursed;
        }
        return false;
    }
}
