using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    //Boi is the child of the player
    public GameObject boi;
    private GameObject player;
    //prefab
    public GameObject[] prefab;
    //Collider
    private Collider boiCollider;


    //Start Function
    private void Start()
    {
        //Initialize the boi game object
        player = GameObject.FindGameObjectWithTag("player"); 
    }
    

    //The main attacking function
    public void Attack()
    {
        //Choose the attack. Runs once per R press /// TODOODODODODODOD
        if (boi == null)
        {
            switch (1)
            {
                case 1:
                    //Basic attack... Kind of.
                    CallCollider(0, 0, 1, prefab[0]);
                    
                    break;

            }
        }
        //Follows your mouse
        //Creates a ray of the current mouse position from the main camera
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Creates a plane for the 
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        //Fancy code to create a an intersection between the ray and plane
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            //This creates the world position where the ray currently is
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            //This is for the programmers reference. Getting rid of it wouldn't change anything.
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            //Look at the ray point
            player.transform.LookAt(pointToLook);
        }
        //This turns attack mode off.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Destroy(boi);           
        }
        //Enables the collider
        if (Input.GetKeyDown(KeyCode.E))
        {
            boiCollider.enabled = true;
            Destroy(boi);
        }

    }

    private void CallCollider(int offsetX, int offsetY, int offsetZ, GameObject prefab)
    {
        //Creates a vector at the player's current position.
        Vector3 pos = new Vector3(player.transform.position.x + offsetX, player.transform.position.y + offsetY, 
            player.transform.position.z + offsetZ);
        //Creates Collider at player's position, with the player as a parent.
        boi = Instantiate(prefab, pos, Quaternion.identity, player.transform);

        //Disables the collider
        boiCollider = boi.GetComponent<Collider>();
        boiCollider.enabled = false;
    }

}
