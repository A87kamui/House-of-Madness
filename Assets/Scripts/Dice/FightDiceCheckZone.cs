using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightDiceCheckZone : MonoBehaviour
{
    Vector3 diceVelocity;
    [HideInInspector] public bool dieThrown;
    public Player player;
    Transform roomCamera;

    // Update is called once per frame
    void FixedUpdate()
    {
        diceVelocity = FightRoll.diceVelocity;
    }

    /// <summary>
    /// Check which side collided with DiceCheckzone collider to determine what diceface is facing up
    /// </summary>
    /// <param name="col"></param>
    public void OnTriggerStay(Collider col)
    {
        if (dieThrown)
        {
            if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
            {
                switch (col.gameObject.name)
                {
                    case "Side1":
                    case "Side4":
                    case "Side6":
                        //dieNumber = 6/3/1;
                        dieThrown = false;
                        StartCoroutine(DefeatGhostDelay());
                        break;
                    case "Side2":
                    case "Side5":
                        //dieNumber = 5/2;
                        dieThrown = false;
                        StartCoroutine(DefeatCurseDelay());
                        break;
                    case "Side3":
                        //dieNumber = 4;
                        dieThrown = false;
                        StartCoroutine(CurseCheckDelay());
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Wait for 1.5 seconds then call GameManager 
    /// </summary>
    /// <returns></returns>
    IEnumerator CurseCheckDelay()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.state = GameManager.States.CURSE_CHECK;
    }

    /// <summary>
    /// Reposition camera to view room
    /// Wait for 1.5 seconds then reset position of dice
    /// Call method to remove ghost
    /// </summary>
    /// <returns></returns>
    IEnumerator DefeatGhostDelay()
    {
        yield return new WaitForSeconds(1.5f);
        roomCamera = player.currentNode.GetComponentInChildren<SpawnController>().roomCamera.transform;
        GameManager.instance.camera.transform.position = roomCamera.position;
        GameManager.instance.camera.transform.rotation = roomCamera.rotation;
        player.currentNode.GetComponentInChildren<SpawnController>().DefeatGhost();
    }

    /// <summary>
    /// Reposition camera to view room
    /// Wait for 1.5 seconds then reset position of dice
    /// Call method to remove ghost
    /// </summary>
    /// <returns></returns>
    IEnumerator DefeatCurseDelay()
    {
        yield return new WaitForSeconds(1.5f);
        roomCamera = player.currentNode.GetComponentInChildren<SpawnController>().roomCamera.transform;
        GameManager.instance.camera.transform.position = roomCamera.position;
        GameManager.instance.camera.transform.rotation = roomCamera.rotation;
        player.currentNode.GetComponentInChildren<SpawnController>().DefeatCurse();
    }
}
