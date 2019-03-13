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

        speed = 0.25f * pController.gameObject.transform.localScale.y;

        if(pController.lController.levels == Levels.Space)
        {
            this.gameObject.transform.localScale = pController.gameObject.transform.localScale;
            GameObject bullet = GameObject.FindGameObjectWithTag("particles");
            bullet.transform.localScale = new Vector3(2, 2, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed;
    }
}
