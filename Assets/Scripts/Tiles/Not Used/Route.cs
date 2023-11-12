using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    Transform[] childTiles;
    public List<Transform> childTransformList = new List<Transform>();
    public List<GameObject> tilesList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        FillTiles();
        //SetConnections();
        //PrintConnections();
    }

    /// <summary>
    /// Gets the transform of child objects and adds the transform to list
    /// </summary>
    void FillTiles()
    {
        childTransformList.Clear();
        tilesList.Clear();

        childTiles = GetComponentsInChildren<Transform>();

        // Add all nodes that is not the parent object
        foreach (Transform child in childTiles)
        {
            if (child != this.transform && child.tag == "Hallway")    // This = Parent
            {
                childTransformList.Add(child);
                tilesList.Add(child.gameObject);
            }
        }
    }


    /*void SetConnections()
    {
        for(int i = 0; i < tilesList.Count; i++)
        {
            if( i == 0)
            {
                tilesList[i].GetComponent<Node>().nextNode = tilesList[i+1].GetComponent<Node>();
            }
            if (i == tilesList.Count - 1)
            {
                tilesList[i].GetComponent<Node>().previousNode = tilesList[i - 1].GetComponent<Node>();
            }
            else if (i > 0 && i < tilesList.Count - 1)
            {
                tilesList[i].GetComponent<Node>().nextNode = tilesList[i + 1].GetComponent<Node>();
                tilesList[i].GetComponent<Node>().previousNode = tilesList[i - 1].GetComponent<Node>();
            }
        }
    }//*/


    /*void PrintConnections()
    {
        for (int i = 0; i < tilesList.Count; i++)
        {
            Debug.Log("Current: " + tilesList[i]);
            Debug.Log("Previous " + tilesList[i].GetComponent<Node>().previousNode);
            Debug.Log("Next " + tilesList[i].GetComponent<Node>().nextNode);
        }
    }//*/

    /// <summary>
    /// Visualize the nodes connection
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // conneciton color 
        FillTiles();

        for (int i = 0; i < childTransformList.Count; i++)
        {
            Vector3 currentPosition = childTransformList[i].position;
            if (i > 0)
            {
                Vector3 previousPostion = childTransformList[i - 1].position;
                Gizmos.DrawLine(previousPostion, currentPosition);
            }
        }
    }

    /// <summary>
    /// Get index of node based on its transfrom position
    /// </summary>
    /// <param name="tileTransfrom"></param>
    /// <returns></returns>
    public int RequestPosition(Transform tileTransfrom)
    {
        return childTransformList.IndexOf(tileTransfrom);
    }
}
