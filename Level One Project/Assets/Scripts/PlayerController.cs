using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum States { NavMesh, WASD, MeleeAttack, RangeAttack, Neutral };

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
    public int hitChance = 4;
    RangeAttack rAttack;
    AttackScript mAttack;
    bool attackingFlag = false;
    
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
        //Initilize range attack script
        rAttack = GetComponent<RangeAttack>();
        //Initialize melee attack script
        mAttack = GetComponent<AttackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            state = States.RangeAttack;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            state = States.MeleeAttack;
        }
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
                case States.MeleeAttack:
                    //This should ensure that you can attack only once a turn.
                    if (!attackingFlag)
                    {
                        mAttack.Attack();
                    }
                    else
                    {
                        //Resets the state
                        state = States.NavMesh;
                    }
                    break;
                case States.RangeAttack:
                    rAttack.active = true;
                    break;
            }
        }
    }

    //Triggers when entering a collider
    private void OnTriggerEnter(Collider other)
    {       
        //Turn on the navmesh if the tag is navMesh
        if (other.gameObject.tag == "navMesh")
        {
            //turn on the navMeshAgent and set the state to NavMesh
            movement.agent.enabled = true;
            state = States.NavMesh;
            other.enabled = false;
        }
    }

    //Triggers when exiting a collider
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "projectile")
        {
            Destroy(other.gameObject);
            int random = Random.Range(1, hitChance);

            switch (random)
            {
                case 1:
                    Debug.Log("Hit");
                    break;
                case 2:
                case 3:
                case 4:
                    Debug.Log("Miss");
                    break;
            }
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
