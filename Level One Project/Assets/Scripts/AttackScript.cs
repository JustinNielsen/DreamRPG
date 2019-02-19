using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    //Boi is the child of the player
    private GameObject boi;
    private GameObject player;
    //prefab
    public GameObject[] prefab;
    //Collider
    public Collider boiCollider;
    private int currentNum;
    private int mouseNum = 1;
    private int upperBound = 1;
    private int lowerBound = 1;

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
        if (boi == null || mouseNum != currentNum)
        {
            switch (mouseNum)
            {
                case 1:
                    //Basic attack... Kind of.
                    //CallCollider(-2, .5f, -1.5f, prefab[0]);
                    //CallCollider(-1, .5f, -1, prefab[0]);
                    CallCollider(-2, 0.5f, 0, prefab[0]);
                    currentNum = 1;
                    break;

            }
        }
        //Follows your mouse
        //Creates a ray of the current mouse position from the main camera
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 14;
        //Creates a plane for the 
        RaycastHit rayHit;
        //Fancy code to create a an intersection between the ray and plane
        if (Physics.Raycast(cameraRay, out rayHit, Mathf.Infinity, layerMask))

        {
            if(rayHit.collider.tag == "ground")
            {
                //This creates the world position where the ray currently is
                Vector3 pointToLook = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
                //This is for the programmers reference. Getting rid of it wouldn't change anything.
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
                //Look at the ray point
                player.transform.LookAt(pointToLook);
            }
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
            Destroy(boi,1);
        }

    }

    private void CallCollider(float offsetX, float offsetY, float offsetZ, GameObject prefab)
    {
        //Creates a vector at the player's current position.
        Vector3 pos = player.transform.position + (transform.forward * 2);
        pos.y += 0.5f;
        //Creates Collider at player's position, with the player as a parent.
        boi = Instantiate(prefab, pos, player.transform.rotation, player.transform);
        //Disables the collider
        boiCollider = boi.GetComponent<Collider>();
    }

    private void Update()
    {
        //Checks if you are scrolling up or down
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Up goes up
            mouseNum++;
            //Resets cycle
            if(mouseNum > upperBound)
            {
                mouseNum = lowerBound;
            }
        }
        //Same things
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mouseNum--;
            if(mouseNum < lowerBound)
            {
                mouseNum = upperBound;
            }
        }
    }

}
