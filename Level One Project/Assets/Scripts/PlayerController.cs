using System.Collections;
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
    }

    void FixedUpdate()
    {
        if (active)
        {
            switch (state)
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
            movement.maxDistance = 10f;
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

    /*
    public void SavePlayer()
    {
        Checkpoint.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        //Get Player data
        Data data = Checkpoint.LoadPlayer();
        //if there is a save file load the level
        if (data != null)
        {
            level = data.level;
            lController.levels = level;
        }
    }
    */
}
