using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Player player;
    //public Node nextNode;
    //public Node previousNode;
    public Node leftNode;
    public Node rightNode;
    public Node topNode;
    public Node bottomNode;
    /*public Node doorPrevious;
    public Node doorNext;//*/

    public Node connectedTo;    // Used in pathfinding BFS

    public Vector2Int coordinates;

    public bool isWalkable; 
    public bool isExplored; 
    public bool isPath; // Check if node is in path
    public bool isTaken;
    public bool isMoveOption;   // Check if node is a move option

    // Start is called before the first frame update
    void Start()
    {
        coordinates = GridManager.instance.GetCoordinatesFromPosition(transform.position);
        SetIsWalkable();
    }

    /// <summary>
    /// 
    /// </summary>
    void SetIsWalkable()
    {
        if (gameObject.tag == "EmptyTile" || gameObject.tag == "StartTile")
        {
            isWalkable = false;
        }
        if (gameObject.tag == "Hallway" || gameObject.tag == "DoorTile")
        {
            isWalkable = true;
        }
    }

    /// <summary>
    /// Select tile to move to
    /// </summary>
    private void OnMouseDown()
    {
        //Debug.Log("calling On mouse down");
        if (isTaken)
        {
            return;
        }

        if (isMoveOption)
        {
            if (GameManager.instance.player.hasTurn)
            {
                GridManager.instance.ResetHighlight();

                if (!GameManager.instance.player.isOutOfBase)
                {
                    GameManager.instance.player.LeaveBase(coordinates);
                }
                else
                {
                    GameManager.instance.player.DestinationPath(coordinates);
                }
            }
        }
        

    }
}