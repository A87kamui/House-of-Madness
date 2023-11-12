 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;
    [SerializeField] GameObject diceBoard;
    DiceCheckZoneScript dieNumberCheck;

    private void Awake()
    {
        dieNumberCheck = diceBoard.GetComponentInChildren<DiceCheckZoneScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        diceVelocity = rb.velocity;
    }

    /// <summary>
    /// Roll dice
    /// </summary>
    public void RollDie()
    {
        DiceNumberTextScript.diceNumber = 0;
        float dirX = Random.Range(0, 500);
        float dirY = Random.Range(0, 500);
        float dirZ = Random.Range(0, 500);

        // Starting position/rotation at the diceBoard location
        transform.position = new Vector3(diceBoard.transform.position.x, diceBoard.transform.position.y + 3, diceBoard.transform.position.z);
        transform.rotation = Quaternion.identity;

        // Add up force and spin to toss dice
        rb.AddForce(transform.up * 500);
        rb.AddTorque(dirX, dirY, dirZ);

        dieNumberCheck.dieThrown = true;
    }
}
