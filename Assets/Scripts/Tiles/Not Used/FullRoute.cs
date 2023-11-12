using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FullRoute : MonoBehaviour
{
    public Material doorMaterial;
    public Material hallwayMaterail;
    public Material hightlightMaterail;
    Transform[] childTiles;
    Vector2Int[] coordinateOfTiles;
    // Vector array for search directions
    Vector2Int[] orthogonalDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    public List<Node> tilesList = new List<Node>();


    // Start is called before the first frame update
    void Start()
    {
        FillTiles();
        SetConnections();
        //PrintConnections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Gets child objects and adds to list
    /// Get child Node and add to Dictionary
    /// </summary>
    private void FillTiles()
    {
        tilesList.Clear();
        int count = 0;

        childTiles = GetComponentsInChildren<Transform>();
        // Add all nodes that is not the parent object
        foreach (Transform child in childTiles)
        {
            if (child.gameObject.GetComponent<Node>() != null)
            {
                tilesList.Add(child.gameObject.GetComponent<Node>());
            }
        }

        coordinateOfTiles = new Vector2Int[tilesList.Count];

        foreach (Node node in tilesList)
        {
            coordinateOfTiles[count] = node.coordinates;
            count++;
        }
    }

    /// <summary>
    /// Set tile connections for each tile
    /// </summary>
    void SetConnections()
    {
        foreach(Vector2Int direction in orthogonalDirections)
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                Vector2Int neighborCoordinates = tilesList[i].coordinates + direction;
                for (int c = 0; c < coordinateOfTiles.Length; c++)
                {
                    if (Vector2Int.right == direction && coordinateOfTiles[c] == neighborCoordinates)
                    {
                        tilesList[i].rightNode = tilesList[c];
                    }
                    if (Vector2Int.left == direction && coordinateOfTiles[c] == neighborCoordinates)
                    {
                        tilesList[i].leftNode = tilesList[c];
                    }
                    if (Vector2Int.up == direction && coordinateOfTiles[c] == neighborCoordinates)
                    {
                        tilesList[i].topNode = tilesList[c];
                    }
                    if (Vector2Int.down == direction && coordinateOfTiles[c] == neighborCoordinates)
                    {
                        tilesList[i].bottomNode = tilesList[c];
                    }
                }
            }
        }
    }

    public void hightAvailableTiles(int dieNumber, Node currentNode)
    {
        //Debug.Log("Die Number: " + dieNumber);
        if (dieNumber <= 0 || currentNode == null)
        {
            return;
        }

        if (!currentNode.isTaken)
        {
            currentNode.gameObject.GetComponent<MeshRenderer>().material = hightlightMaterail;
            currentNode.isMoveOption = true;
        }
        
        //currentNode.GetComponent<Material>().color = hightlightMaterail.color;
        hightAvailableTiles(dieNumber - 1, currentNode.topNode);
        hightAvailableTiles(dieNumber - 1, currentNode.bottomNode);
        hightAvailableTiles(dieNumber - 1, currentNode.leftNode);
        hightAvailableTiles(dieNumber - 1, currentNode.rightNode);
        /*hightAvailableTiles(dieNumber - 1, currentNode.doorNext);
        hightAvailableTiles(dieNumber - 1, currentNode.doorPrevious);//*/
    }

    /// <summary>
    /// View all tiles conneced
    /// </summary>
    void PrintConnections()
    {
        foreach (Node node in tilesList)
        {
            Debug.Log("Node: " + node);
            if (node.topNode != null)
            {
                Debug.Log("Top: " + node.topNode);
            }
            if (node.bottomNode != null)
            {
                Debug.Log("Bottom: " + node.bottomNode);
            }
            if (node.leftNode != null)
            {
                Debug.Log("left: " + node.leftNode);
            }
            if (node.rightNode != null)
            {
                Debug.Log("right: " + node.rightNode);
            }
            /*if (node.doorPrevious != null)
            {
                Debug.Log("right: " + node.doorPrevious);
            }
            if (node.doorNext != null)
            {
                Debug.Log("right: " + node.doorNext);
            }//*/
        }
    }
}
