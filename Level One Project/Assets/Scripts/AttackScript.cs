﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    //hitbox is the child of the player
    private GameObject player;
    public GameObject hitbox;
    public Collider hitboxCollider;
    //prefab
    public GameObject[] prefab;
    //Collider
    PlayerController pController;
    public GameObject mageShot;
    LineRenderer line;

    //Start Function
    private void Start()
    {
        //Initialize the boi game object
        player = GameObject.FindGameObjectWithTag("player");
        //Initilaize player controller
        pController = GetComponent<PlayerController>();
        //Initilize LineRendrer
        line = player.GetComponent<LineRenderer>();
    }

    //The main attacking function
    public void MeleeAttackMode()
    {
        //Choose the attack. Runs once per R press /// TODOODODODODODOD
        if (hitbox == null)
        {
            switch (1)
            {
                case 1:
                    //Basic attack... Kind of.
                    CallCollider(-2, 0.5f, 0, prefab[0]);
                    break;
            }
        }

        //Turns the player towards the mouse pointer
        LookAtMouse();

        //Enables the collider
        if (Input.GetMouseButtonDown(0))
        {
            hitboxCollider.enabled = true;
            Destroy(hitbox, 1);
            pController.state = States.NavMesh;
        }

        //This turns attack mode off.
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(hitbox);
            pController.state = States.NavMesh;
        }
    }

    public void RangeAttackMode()
    {
        //Set the lines color to blue
        line.material.color = Color.blue;

        //Looks at the mosue pointer
        LookAtMouse();

        //Sets the line to two points and sets the first point to the players location
        line.positionCount = 2;
        line.SetPosition(0, transform.position);

        RaycastHit hit;

        //Draws the line 20 units forward unless obstructed by an object
        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
        {
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, transform.forward * 20 + transform.position);
        }

        //Shoot the projectile in the players forward direction
        if (Input.GetMouseButtonDown(0))
        {
            LaunchProjectile();
        }

        if (Input.GetMouseButtonDown(1))
        {
            pController.state = States.NavMesh;
            line.positionCount = 0;
        }
    }

    private void LookAtMouse()
    {
        Ray cameraRay = pController.cam.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 14;

        RaycastHit rayHit;

        if (Physics.Raycast(cameraRay, out rayHit, Mathf.Infinity, layerMask))
        {
            if (rayHit.collider.tag == "ground")
            {
                Vector3 pointToLook = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
                player.transform.LookAt(pointToLook);
            }
        }
    }

    private void CallCollider(float offsetX, float offsetY, float offsetZ, GameObject prefab)
    {
        //Creates a vector at the player's current position.
        Vector3 pos = player.transform.position + (transform.forward * 2);
        pos.y += 0.5f;
        //Creates Collider at player's position, with the player as a parent.
        hitbox = Instantiate(prefab, pos, player.transform.rotation, player.transform);
        //Disables the collider
        hitboxCollider = hitbox.GetComponent<Collider>();
    }

    private void LaunchProjectile()
    {
        Vector3 pos = transform.position + transform.forward;
        GameObject projectile = Instantiate(mageShot, pos, transform.rotation);
        pController.state = States.NavMesh;
        Destroy(projectile, 4f);
    }



}