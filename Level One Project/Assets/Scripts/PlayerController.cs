using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//These represent independent states that cannot coexist.
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
    public List<GameObject> enemies;
    private int playerLevel = 1;
    private int playerXP;
    private bool attackingFlag;
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
        //Get the TurnBasedSystem script
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("enemy");
        foreach(GameObject objects in tempArray)
        {
            enemies.Add(objects);
        }

        //Sets it equal to whatever difficulty is selected.
        //********************TODO!***************************
        playerHealth = 1;
        shieldHealth = 1;
    }

    void FixedUpdate()
    {
        //Let's you enter attack mode. Change when needed.
        if(playerXP >= 100)
        {
            playerXP -= 100;
            playerLevel++;
            Debug.Log($"XP:{playerXP}, Level:{playerLevel}");
            //TODO Add in the level up splash screen.
        }
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
                    //This should ensure that you can attack only once a turn.
                    if(!attackingFlag)
                    {
                        attackScript.Attack();
                    }
                    else
                    {
                        //Resets the state
                        state = States.NavMesh;
                    }
                    break;
            }
        }
        else if(!active)
        {
            //This resets the attacking flag when it isn't the players turn.
            attackingFlag = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && state == States.NavMesh)
        {
            state = States.Attacking;
        }
        else if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) && state == States.Attacking)
        {
            //This will wait until the end of the frame before we change the state
            StartCoroutine(NavMeshCoRountine());
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
            other.enabled = false;
        }
        //Checks if the tag is right and that the player is in the right state
        else if (other.gameObject.tag == "enemy" && (state == States.Attacking))
        {
            //Sees if the enemy is attacking or the player.
            if (!active)
            {
                //Now we get the 
                //If there are no shields, it will damage the player
                if (shieldHealth == 0)
                {
                    playerHealth--;
                }
                else //Otherwise the shields will be hurt.
                {
                    shieldHealth--;
                }
            }
            else
            {
                //This grabs the enemy's controller
                EnemyController enemy = other.gameObject.GetComponent<EnemyController>();

                //If the enemy is dead
                if (enemy.enemyHealth < 0)
                {
                    //Adds to your xp which is based on the enemy level and the player level. We multiply by 50 to give a basis for XP.
                    //So if you are level 1, and so are the enemies, you will get 50 xp each enemy, but if you are level 2 you only get 25 xp.
                    playerXP += (enemy.level/playerLevel)*50;
                }

                Debug.Log(playerXP);
                attackingFlag = true;
            }
            
        }
    }

    /*May not need this block of code.
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
    } */

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

    private IEnumerator NavMeshCoRountine()
    {
        yield return new WaitForEndOfFrame();

        //Code needed
        state = States.NavMesh;
    }
}
