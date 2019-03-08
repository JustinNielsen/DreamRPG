using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{

    public float speed = 0.5f;
    public PlayerController pController;

    private void Start()
    {
        pController = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerController>();

        speed = 0.5f * pController.gameObject.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed;
    }
}
