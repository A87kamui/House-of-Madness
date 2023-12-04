using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] float speed = 1.5f;
    [SerializeField] float height = 0.15f;
    Vector3 position;

    private void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = Mathf.Sin(Time.time * speed) * height + position.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    
}
