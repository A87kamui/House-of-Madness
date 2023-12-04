using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheckZoneScript : MonoBehaviour {

	Vector3 diceVelocity;
	public int dieNumber;
    [HideInInspector] public bool dieThrown;

	// Update is called once per frame
	void FixedUpdate () {
		diceVelocity = DiceRoll.diceVelocity;
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
                        DiceNumberTextScript.diceNumber = 6;
                        dieNumber = 6;
                        dieThrown = false;
                        StartCoroutine(RollDieDelay());
                        break;
                    case "Side2":
                        DiceNumberTextScript.diceNumber = 5;
                        dieNumber = 5;
                        dieThrown = false;
                        StartCoroutine(SpawnGhostDelay());
                        break;
                    case "Side3":
                        DiceNumberTextScript.diceNumber = 4;
                        dieNumber = 4;
                        dieThrown = false;
                        StartCoroutine(SpawnGhostDelay());
                        break;
                    case "Side4":
                        DiceNumberTextScript.diceNumber = 3;
                        dieNumber = 3;
                        dieThrown = false;
                        StartCoroutine(SpawnGhostDelay());
                        break;
                    case "Side5":
                        DiceNumberTextScript.diceNumber = 2;
                        dieNumber = 2;
                        dieThrown = false;
                        StartCoroutine(SpawnGhostDelay());
                        break;
                    case "Side6":
                        DiceNumberTextScript.diceNumber = 1;
                        dieNumber = 1;
                        dieThrown = false;
                        StartCoroutine(SpawnGhostDelay());
                        break;
                }
            }
        }		
	}

    /// <summary>
    /// Wait for 1.5 seconds then call GameManager DieRolled method
    /// </summary>
    /// <returns></returns>
    IEnumerator RollDieDelay()
    {
        dieNumber = 6;
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.DieRolled(dieNumber);
    }

    /// <summary>
    /// Wait for 1.5 seconds then call GameManager
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnGhostDelay()
    {
        dieNumber = 6;
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.SpawnEnemy(dieNumber);
    }
}
