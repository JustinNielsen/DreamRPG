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
    private int playerHealth;
    private int shieldHealth;
    public AttackScript attackScript;

    private void Start()
    {
        //Initialize cam to main camera
        cam = Camera.main;
        //Initialize Player movement class object
        movement = GetComponent<PlayerMovement>();
        //Initialize Player attack class object
        attackScript = GetComponent<AttackScript>();
        //Turn off the NavMeshAgent at the beginning of the game
        movement.agent.enabled = false;
        //Set the state to WASD at the beginning of the game
        state = States.WASD;
        //Get the TurnBasedSystem script from the turnObj
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
        //Sets it equal to whatever difficulty is selected.
        //********************TODO!***************************
        playerHealth = 1;
        shieldHealth = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //Let's you enter attack mode. Change when needed.

        if(playerHealth > 0)
        {
            //TODO Sets the gameover sequence
        }
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
                    attackScript.Attack();
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && state == States.NavMesh)
        {
            state = States.Attacking;
        }
        else if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) && state == States.Attacking)
        {
            state = States.NavMesh;
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
        //TODO - Check if the tag is right, potentially change method of damage.
        else if(other.gameObject.tag == "enemy")
        {
            Debug.Log("HIT!");
            //If there are no shields, it will damage the player
            if(shieldHealth == 0)
            {
                playerHealth--;
            }
            else //Otherwise the shields will be hurt.
            {
                shieldHealth--;
            }
        }
    }

    //Triggers when exiting a collider
    private void OnTriggerExit(Collider other)
    {
        //Turn off navMesh if exiting a collider with a tag of navMesh
        if (other.gameObject.tag == "navMesh")
        {
            //turn off the navMeshAgent and set the state to WASD
            movement.agent.enabled = false;
            state = States.WASD;
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
}
