using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public int level = 2;
    public int health = 3;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * 2.0F);
    }

    public void SavePlayer ()
    {
        Checkpoint.SavePlayer(this);
    }

    public void LoadPlayer ()
    {
        Data data = Checkpoint.LoadPlayer();
        level = data.level;
        health = data.health;

        Vector3 positon;
        positon.x = data.position[0];
        positon.y = data.position[1];
        positon.z = data.position[2];
        transform.position = positon;
    }
    
}