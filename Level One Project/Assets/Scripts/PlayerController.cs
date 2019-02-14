using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum States { NavMesh, WASD, Attacking, Neutral };

public class PlayerController : MonoBehaviour
{
    private Camera cam;
    public float maxDistance;
    public bool active = true;
    public TurnBasedSystem turn;
    public States state;
    public PlayerMovement movement;
    public int level;
    public int health;
    
    private void Start()
    {
        //Initialize cam to main camera
        cam = Camera.main;
        //Initialize Player movement class object
        movement = GetComponent<PlayerMovement>();
        //Turn off the NavMeshAgent at the beginning of the game
        movement.agent.enabled = false;
        //Set the state to WASD at the beginning of the game
        state = States.WASD;
        //Get the TurnBasedSystem script from the turnObj
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (active)
        {
            switch (state)
            {
                case States.NavMesh:
                    movement.NavMeshMovement();
                    break;
                case States.WASD:
                    movement.KeyboardMovement();
                    break;
                case States.Attacking:
                    break;
            }
        }
    }

    //Triggers when entering a collider
    private void OnTriggerEnter(Collider other)
    {
        //Destroy the other object if its tag is waypoint
        if (other.gameObject.tag == "waypoint")
        {
            Destroy(other.gameObject);
        } //Turn on the navmesh if the tag is navMesh
        else if (other.gameObject.tag == "navMesh")
        {
            //turn on the navMeshAgent and set the state to NavMesh
            movement.agent.enabled = true;
            state = States.NavMesh;
        }
    }

    //Triggers when exiting a collider
    private void OnTriggerExit(Collider other)
    {
        //Turn off navMesh if exiting a collider with a tag of navMesh
        if (other.gameObject.tag == "navMesh")
        {
            movement.line.positionCount = 0;
            //turn off the navMeshAgent and set the state to WASD
            movement.agent.enabled = false;
            state = States.WASD;
        }
        else if(other.gameObject.tag == "projectile")
        {
            Destroy(other.gameObject);
            Debug.Log("Lose Health");
        }
    } 

    //Turns on and off the player according to the bool parameter
    public void TogglePlayer(bool isOn)
    {
        //Sets the player active state to true
        if (isOn)
        {
            active = true;
        }
        else //Sets the player active state to false
        {
            active = false;
            movement.line.positionCount = 0;
        }
    }

    public void SavePlayer()
    {
        Checkpoint.SavePlayer(this);
    }

    public void LoadPlayer()
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
