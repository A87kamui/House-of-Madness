using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Vector2Int startCoordinates;
    public Vector2Int destinationCoordinates;

    // Property to get startCoordinates
    public Vector2Int StartCoordinates { get { return startCoordinates; } }
    // Property to get destinationCoordinates
    public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

    Node startNode; 
    Node destinationNode;   
    Node currentSearchNode;

    Queue<Node> openQueue = new Queue<Node>();
    Dictionary<Vector2Int, Node> explored = new Dictionary<Vector2Int, Node>();
    Vector2Int[] searchDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    bool isRunning;

    void Awake()
    {
        if (GridManager.instance != null)
        {
            grid = GridManager.instance.GetGrid;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Get path to destination
    /// </summary>
    /// <param name="startCoordinates"></param>
    /// <returns></returns>
    public List<Node> GetPath(Vector2Int startCoordinates, Vector2Int destionationCoordinates)
    {
        GridManager.instance.ResetNodes();
        destinationNode = GridManager.instance.GetNode(destionationCoordinates);
        BreadthFirstSearch(startCoordinates, destinationCoordinates);
        return BuildPath();
    }

    /// <summary>
    /// Search neighbor nodes tiles until destination is found to create a path
    /// </summary>
    private void BreadthFirstSearch(Vector2Int startCoordinates, Vector2Int destinationCoordinates)
    {
        openQueue.Clear();
        explored.Clear();

        isRunning = true;
        
        // Add start node to Queue
        openQueue.Enqueue(grid[startCoordinates]);

        // Add start coordinate and its node to explored
        explored.Add(startCoordinates, grid[startCoordinates]);

        while (openQueue.Count > 0 && isRunning)
        {
            currentSearchNode = openQueue.Dequeue();
            currentSearchNode.isExplored = true;

            ExploreNeighbors();

            if (currentSearchNode.coordinates == destinationCoordinates)
            {
                isRunning = false;
            }
        }
    }

    /// <summary>
    /// Search available node tiles to move to
    /// </summary>
    private void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();

        // Get neighbor node tiles based on search directions
        foreach (Vector2Int direction in searchDirections)
        {
            Vector2Int neighborCoordinates = currentSearchNode.coordinates + direction;

            if (grid.ContainsKey(neighborCoordinates))
            {
                neighbors.Add(grid[neighborCoordinates]);
            }
        }

        // Add neighbors to Open queue if have not been explored
        // Add neighbor to explored to show it has been checked
        foreach (Node neighbor in neighbors)
        {
            if (!explored.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchNode;

                explored.Add(neighbor.coordinates, neighbor);
                openQueue.Enqueue(neighbor);
            }
        }
    }

    /// <summary>
    /// Create path for player to navigate through
    /// </summary>
    /// <returns></returns>
    private List<Node> BuildPath()
    {
        // List for the path found
        List<Node> path = new List<Node>();
        Node currentNode = destinationNode;

        path.Add(currentNode);

        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;

            path.Add(currentNode);
            currentNode.isPath = true;
        }

        // Reverise the list so it goes from starting to destintion
        path.Reverse();
        return path;
    }
}
