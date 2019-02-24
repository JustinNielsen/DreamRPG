﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum States { NavMesh, WASD, MeleeAttack, RangeAttack, Neutral };

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public Vector3[] checkpointLocations;
    public float maxDistance;
    public bool active = true;
    public TurnBasedSystem turn;
    public States state;
    public PlayerMovement movement;
    public Levels level;
    public int health;
    public int hitChance = 4;
    public AttackScript Attack;
    public LevelController lController;
    bool attackingFlag = false;
    int mouseWheelLocation;
    States[] mouseWheelStates;

    public int playerLevel;
    public int playerXP;

    //Bool to check if the melee attack has already been used this turn
    public bool meleeAttacked;

    //Keeps track of the maxMana and remainingMana
    public float maxMana;
    public float remainingMana;

    CinemachineBrain camBrain;

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
        //Initialize attack script
        Attack = GetComponent<AttackScript>();
        //Set the mouse wheel point to 0
        mouseWheelLocation = 0;
        //Initilize the mouseWheelStates array
        mouseWheelStates = new States[3] { States.NavMesh, States.MeleeAttack, States.RangeAttack };
        //Initilize checkpoints
        checkpointLocations = new Vector3[3] { new Vector3(8.43f, 9.2f, -0.92f), new Vector3(-347.35f, 300.6f, -635.83f), new Vector3(62.17f, 2.7f, 98.26f) };
        //Initilize level controller
        lController = GameObject.FindGameObjectWithTag("turn").GetComponent<LevelController>();
        //Initilize camBrain
        camBrain = cam.GetComponent<CinemachineBrain>();
        //Sets the player level to 1 and xp to 0
        playerLevel = 1;
        playerXP = 0;
        meleeAttacked = false;
        //Initilize maxMana and remaining mana
        maxMana = 100f;
        remainingMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        //Only allow the scroll wheel to change states in combat mode
        if (state != States.WASD && !movement.isMoving)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                ScrollWheel(true);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ScrollWheel(false);
            }
        }

        //Checks if the player should level up
        if(playerXP >= 100)
        {
            //Removes the xp and adds to the player level
            playerXP -= 100;
            playerLevel++;
            SavePlayer();

            //TODO Add in Level Up Screen
        }

        if(health == 0)
        {
            //TODO - Add a game over screen
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            switch (state) //Switch states if active
            {
                case States.NavMesh:
                    movement.NavMeshMovement();
                    mouseWheelLocation = 0;
                    break;
                case States.WASD:
                    movement.KeyboardMovement();
                    break;
                case States.MeleeAttack:
                    Attack.MeleeAttackMode();
                    mouseWheelLocation = 1;
                    break;
                case States.RangeAttack:
                    Attack.RangeAttackMode();
                    mouseWheelLocation = 2;
                    break;
            }
        }
        else
        {
            //Reset max move distance and meleeAttacked after turn is done
            movement.maxDistance = 10f;
            meleeAttacked = false;
        }
    }

    private void ScrollWheel(bool up)
    {
        if (up)//Scrolled up
        {
            //Make sure the location is less than the max length of the array
            if(mouseWheelLocation == mouseWheelStates.Length - 1)
            {
                mouseWheelLocation = 0;
            }
            else
            {
                mouseWheelLocation++;
            }
        }
        else //Scrolled down
        {
            //Make sure the location is greater than 0
            if(mouseWheelLocation == 0)
            {
                mouseWheelLocation = mouseWheelStates.Length - 1;
            }
            else
            {
                mouseWheelLocation--;
            }
        }

        //Switch the state to the indicated integer location
        SwitchStates();
    }

    private void SwitchStates()
    {
        //Reset line and destroy hitbox
        movement.line.positionCount = 0;
        Destroy(Attack.hitbox);
        //Set the state to the indicated location
        state = mouseWheelStates[mouseWheelLocation];
    }

    //Triggers when entering a collider
    private void OnTriggerEnter(Collider other)
    {       
        //Turn on the navmesh if the tag is navMesh
        if (other.gameObject.tag == "navMesh")
        {
            //turn on the navMeshAgent and set the state to NavMesh
            movement.agent.enabled = true;
            mouseWheelLocation = 0;
            state = States.NavMesh;
            other.enabled = false;
            turn.ResetArrays();

            //Delete the collider
            Destroy(other.gameObject);
            //Change camera blend mode to ease
            camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        }

        if (other.gameObject.tag == "enemyProjectile")
        {
            //Gets a random hit chance
            int random = Random.Range(1, hitChance);

            switch (random)
            {
                case 1:
                    Debug.Log("Hit");

                    //Damage the player according to level of enemy
                    DamagePlayer(other.gameObject.transform.parent.GetComponent<EnemyController>());
                    break;
                case 2:
                case 3:
                case 4:
                    Debug.Log("Miss");
                    break;
            }

            //Destroys the projectile
            Destroy(other.gameObject);
        }

        //Change level if you walk into door collider
        if (other.gameObject.CompareTag("Door"))
        {
            switch (lController.levels)
            {
                case Levels.Level1:
                    lController.levels = Levels.Level2;
                    break;
                case Levels.Level2:
                    lController.levels = Levels.Level3;
                    break;
                /*case Levels.Level3:
                    lController.levels = Levels.Level4;
                    break;
                case Levels.Level4:
                    lController.levels = Levels.EndCredits;
                    break;*/
            }
        }

        if (other.gameObject.CompareTag("enemy"))
        {
            if (!active)
            {
                //Currently, we will just have every enemy do one damage
                health--;
            }

            /*
            else
            {
                //Grabs the enemy controller
                EnemyController eController = other.gameObject.GetComponent<EnemyController>();

                //Checks to see if the enemy has health left
                if(eController.enemyHealth < 0)
                {
                    //Uses a simple formula to find the xp. It should work for the most part
                    playerXP += eController.enemyLevel / playerLevel * 50;
                    Debug.Log($"Player XP: {playerXP}");
                }
            }
            */
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

    //Damages the player
    public void DamagePlayer(EnemyController enemy)
    {
        //TODO - Implement a better damage system based on the level of the enemy
        health--;
    }

    public void SavePlayer()
    {
        Checkpoint.SavePlayer(this);
    }

    public Levels LoadPlayer()
    {
        //Get Player data
        Data data = Checkpoint.LoadPlayer();
        //if there is a save file load the level
        if (data != null)
        {
            //levels = data.level;
            return data.level;
        }
        else
        {
            return Levels.MainMenu;
        }
    }

}
