using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class CoordinateLabeler : MonoBehaviour
{
    TextMeshPro label;
    Vector2Int coordinates = new Vector2Int();
    GridManager gridManager;

    // Default lable color
    [SerializeField] Color defaultColor = Color.black;
    // Color of tiles that are not walkable
    [SerializeField] Color blockColor = Color.gray;
    // Color of tiles that are explored
    [SerializeField] Color exploredColor = Color.yellow;
    // Color of tiles that are paths
    [SerializeField] Color pathColor = new Color(1.0f, 0.5f, 0.0f);

    /// <summary>
    /// Set label to TMP component
    /// Set gridManager and display coordinates
    /// </summary>
    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        label = GetComponent<TextMeshPro>();
        label.enabled = false;

        DisplayCoordinates();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            DisplayCoordinates();
            UpdateObjectName();
            label.enabled = true;
        }

        SetLabelColor();
        ToggleLabels();
    }

    /// <summary>
    /// Update coordinate text
    /// Takes the tranfrom position and converts it to coordinates
    /// </summary>
    void DisplayCoordinates()
    {
        if (gridManager == null)
        {
            return;
        }

        // Gets the updated x and z position based on parent location
        coordinates.x = (int)transform.parent.position.x;
        coordinates.y = (int)transform.parent.position.z;

        label.text = coordinates.y + ", " + coordinates.x;
    }

    /// <summary>
    /// Update label color based on if able to place on tile or not 
    /// </summary>
    void SetLabelColor()
    {
        if (gridManager == null)
        {
            return;
        }

        // Get the coordinates of the coordinateLabeler
        Node node = gridManager.GetNode(coordinates);

        if (node == null)
        {
            return;
        }

        // check each flag of the node (walkable, a path, explored or none)
        if (!node.isWalkable)
        {
            label.color = blockColor;
        }
        else if (node.isPath)
        {
            label.color = pathColor;
        }
        else if (node.isExplored)
        {
            label.color = exploredColor;
        }
        else
        {
            label.color = defaultColor;
        }
    }

    /// <summary>
    /// Update name
    /// </summary>
    void UpdateObjectName()
    {
        //transform.parent.name = coordinates.ToString();
        transform.parent.name = coordinates.y + ", " + coordinates.x;
    }

    /// <summary>
    /// Manually switch corrdinate lables on and off
    /// Based on current state
    /// </summary>
    void ToggleLabels()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            label.enabled = !label.IsActive();
        }
    }

}
