using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{

    public float speed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed;
    }
}
