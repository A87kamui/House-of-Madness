using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour
{
    Transform[] childTiles;
    public List<Transform> childTransformList = new List<Transform>();
    public List<GameObject> roomTilesList = new List<GameObject>();
    public List<GameObject> roomDoorList = new List<GameObject>();
    public List<GameObject> roomConnectionList = new List<GameObject>();
    public List<GameObject> roomList = new List<GameObject>();

    private void Awake()
    {
        FillTiles();
    }
    // Start is called before the first frame update
    void Start()
    {
        //FillTiles();
    }

    /// <summary>
    /// Gets the transform of child objects and adds the transform to list
    /// </summary>
    void FillTiles()
    {
        childTransformList.Clear();

        childTiles = GetComponentsInChildren<Transform>();

        // Add all nodes that is not the parent object
        foreach (Transform child in childTiles)
        {
            if (child != this.transform && child.tag == "RoomTile")    // This = Parent
            {
                childTransformList.Add(child);
                roomTilesList.Add(child.gameObject);
            }
            if (child != this.transform && child.tag == "DoorTile")
            {
                roomDoorList.Add(child.gameObject);
            }
            if (child != this.transform && child.tag == "RoomConnectionTile")
            {
                roomConnectionList.Add(child.gameObject);
            }
            roomList.Add(child.gameObject);
        }
    }
}
