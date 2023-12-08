using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [SerializeField] Vector2Int gridSize;
    [SerializeField] Material doorMaterial;
    [SerializeField] Material hallwayMaterail;
    [SerializeField] Material hightlightMaterail;
    [SerializeField] List<Node> allTiles = new List<Node>();  // Holds all tiles in game

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int[] orthogonalDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    public List<Node> tilesHighlighted = new List<Node>(); // Tracks highlited tiles
    Transform[] childTilesTransfrom;

    /// <summary>
    /// Property to get grid
    /// </summary>
    public Dictionary<Vector2Int, Node> GetGrid { get { return grid; } }

    /// <summary>
    /// Get instance of grid manager
    /// </summary>
    private void Awake()
    {
        instance = this;        
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        SetConnections();
    }

    /// <summary>
    /// Create nodes and place into grid dictionary
    /// Created nodes contain the x, y value coordinate location on the grid
    /// </summary>
    private void CreateGrid()
    {
        childTilesTransfrom = GetComponentsInChildren<Transform>();

        foreach (Transform child in childTilesTransfrom)
        {
            if (child.gameObject.GetComponent<Node>() != null)
            {
                grid.Add(child.gameObject.GetComponent<Node>().coordinates, child.gameObject.GetComponent<Node>());
            }     
        }
    }

    /// <summary>
    /// Set tile connections for each tile
    /// </summary>
    void SetConnections()
    {
        foreach (KeyValuePair<Vector2Int, Node> entity in grid)
        {
            for (int i = 0; i < orthogonalDirections.Length; i++)
            {
                Vector2Int neighborCoordinates = entity.Key + orthogonalDirections[i];
                if (Vector2Int.right == orthogonalDirections[i] && grid.ContainsKey(neighborCoordinates))
                {
                    entity.Value.rightNode = GetNode(neighborCoordinates);
                }
                if (Vector2Int.left == orthogonalDirections[i] && grid.ContainsKey(neighborCoordinates))
                {
                    entity.Value.leftNode = GetNode(neighborCoordinates);
                }
                if (Vector2Int.up == orthogonalDirections[i] && grid.ContainsKey(neighborCoordinates))
                {
                    entity.Value.topNode = GetNode(neighborCoordinates);
                }
                if (Vector2Int.down == orthogonalDirections[i] && grid.ContainsKey(neighborCoordinates))
                {
                    entity.Value.bottomNode = GetNode(neighborCoordinates);
                }
            }
        }
    }

    /// <summary>
    /// Hightlight tiles that player is able to move to base on die roll
    /// </summary>
    /// <param name="dieNumber"></param>
    /// <param name="hightlightNode"></param>
    /// <param name="playerNode"></param>
    public void HightLightTiles(int dieNumber, Node hightlightNode, Node playerNode)
    {
        if (dieNumber <= 0 || hightlightNode == null)
        {
            if (tilesHighlighted.Count <= 0)
            {
                //Debug.Log("No place to move");
                GameManager.instance.player.CheckGhostOrCurse();
            }
            else
            {
                return;
            }      
        }
        // Check hightlightNode is not taken and Player is not at hightlightNode
        else if (!hightlightNode.isTaken && hightlightNode != playerNode)
        {
            hightlightNode.gameObject.GetComponent<MeshRenderer>().material = hightlightMaterail;
            hightlightNode.isMoveOption = true;
            tilesHighlighted.Add(hightlightNode);
        }
        dieNumber -= 1;
        HightLightTiles(dieNumber, hightlightNode.topNode, playerNode);
        HightLightTiles(dieNumber, hightlightNode.bottomNode, playerNode);
        HightLightTiles(dieNumber, hightlightNode.leftNode, playerNode);
        HightLightTiles(dieNumber, hightlightNode.rightNode, playerNode);
    }

    /// <summary>
    /// Reset Nodes back to initial material
    /// </summary>
    public void ResetHighlight()
    {
        foreach(Node node in tilesHighlighted)
        {
            if(node.tag == "Hallway" || node.tag == "EntranceTile")
            {
                node.gameObject.GetComponent<MeshRenderer>().material = hallwayMaterail;
            }
            if (node.tag == "SpawnTile")
            {
                node.gameObject.GetComponent<MeshRenderer>().material = doorMaterial;
            }

        }
        tilesHighlighted.Clear();   // Reset list
    }

    /// <summary>
    /// Retruns the node from passed in coordinates if it exist
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public Node GetNode(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            return grid[coordinates];
        }
        return null;
    }

    /// <summary>
    /// Get the coordinates from a position in the world grid
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector2Int GetCoordinatesFromPosition(Vector3 position)
    {
        Vector2Int coordinates = new Vector2Int();

        // Gets the x and y coordinate based on parent object position location
        coordinates.x = (int)(position.x);
        coordinates.y = (int)(position.z);

        return coordinates;
    }

    /// <summary>
    /// Get the world grid position from coordinates
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public Vector3 GetPositionFromCoordinates(Vector2Int coordinates)
    {
        Vector3 position = new Vector3();

        // Gets the x and z position based on coordinate location
        position.x = coordinates.x;
        position.z = coordinates.y;

        return position;
    }

    /// <summary>
    /// Loop through every node in world grid
    /// Reset the node to its initial state of unexplored/not connected to another node in pathfinding
    /// </summary>
    public void ResetNodes()
    {
        foreach (KeyValuePair<Vector2Int, Node> entry in grid)
        {
            entry.Value.isExplored = false;
            entry.Value.isPath = false;
            entry.Value.connectedTo = null;
            entry.Value.isMoveOption = false;
        }
    }

    /// <summary>
    /// Print grid key and value
    /// </summary>
    void PrintGrid()
    {
        foreach(KeyValuePair< Vector2Int, Node> entity in grid)
        {
            Debug.Log("entity: " + entity.Key + " " + entity.Value);
        }
    }

    /// <summary>
    /// View all tiles conneced
    /// </summary>
    void PrintConnections()
    {
        foreach (KeyValuePair<Vector2Int, Node> entity in grid)
        {
            Debug.Log("Node: " + entity);
            if (entity.Value.topNode != null)
            {
                Debug.Log("Top: " + entity.Value.topNode);
            }
            if (entity.Value.bottomNode != null)
            {
                Debug.Log("Bottom: " + entity.Value.bottomNode);
            }
            if (entity.Value.leftNode != null)
            {
                Debug.Log("left: " + entity.Value.leftNode);
            }
            if (entity.Value.rightNode != null)
            {
                Debug.Log("right: " + entity.Value.rightNode);
            }
        }
    }
}
