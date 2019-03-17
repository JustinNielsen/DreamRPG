using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum States { NavMesh, WASD, MeleeAttack, RangeAttack, Shielding, Neutral };

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public Vector3[] checkpointLocations;
    public AttackScript attack;
    public LevelController lController;
    public HUD hud;
    public PlayerMovement movement;
    public TurnBasedSystem turn;
    public LevelUp levelUp;
    public float maxDistance;
    public bool active = true;
    public States state;
    public Levels level;
    
    public int health;
    public bool shieldActive;
    public int hitChance = 4;
    public GameObject gameOver;

    int mouseWheelLocation;
    States[] mouseWheelStates;

    //Level 2 doors
    public GameObject entryDoor;
    public GameObject exitDoor;

    //In loving memory of Matt...
    public AudioSource matt;
    public AudioSource enemySource;
    public AudioSource playerSource;
    public AudioClip[] mattVoiceArray;
    public AudioClip[] playerSounds;
    public AudioClip[] enemySounds;

    public int playerLevel;
    public float playerXP;
    public GameObject levelUpMenu;
    public bool canAttack = false;

    //Bool to check if the melee attack has already been used this turn
    public bool meleeAttacked = false;

    //Keeps track of the maxMana and remainingMana
    public float maxMana;
    public float remainingMana;

    //Bool to check when the player has already added mana after a turn reset
    bool addedMana = false;

    //Bool to check if the poster trigger has already triggered.
    bool posterTriggerBool = true;


    //Shield gameobject
    public GameObject shield;

    CinemachineBrain camBrain;

    public CinemachineVirtualCamera NormalCamera;

    public CinemachineVirtualCamera ThirdPersonCamera;

    public CinemachineVirtualCamera TopCamera;

    private void Start()
    {
        //Lock the mouse to the game window
        Cursor.lockState = CursorLockMode.Confined;
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
        attack = GetComponent<AttackScript>();
        //Set the mouse wheel point to 0
        mouseWheelLocation = 0;
        //Initilize the mouseWheelStates array
        mouseWheelStates = new States[4] { States.NavMesh, States.MeleeAttack, States.RangeAttack, States.Shielding };
        //Initilize checkpoints
        checkpointLocations = new Vector3[4] { new Vector3(8.43f, 9.2f, -0.92f), new Vector3(-347.35f, 299.68f, -635.83f), new Vector3(62.17f, 1.74f, 98.26f), new Vector3(10.133f, 3.422f, -7.163f) };
        //Initilize level controller
        lController = GameObject.FindGameObjectWithTag("turn").GetComponent<LevelController>();
        //Initilize camBrain
        camBrain = cam.GetComponent<CinemachineBrain>();
        //Sets the player level to 1 and xp to 0
        playerLevel = 1;
        playerXP = 0f;
        meleeAttacked = false;
        //Initilize maxMana and remaining mana
        maxMana = 100f;
        remainingMana = maxMana;
        //Initilize player health
        health = 3;
        //Initilize the shield to false
        shieldActive = false;
        //Initilize maxDistance according to the players size
        maxDistance = 10f * this.gameObject.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Only allow the scroll wheel to change states in combat mode
        if (state != States.WASD && !movement.isMoving && active)
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

        //Checks to see if you hit the left mouse button
        if (Input.GetMouseButtonDown(0) && state == States.Shielding && active)
        {
            //Makes sure you can only have one shield
            if (!shieldActive)
            {
                if (remainingMana > 40) //Checks if the player has enough mana to get a shield
                {
                    //Subtract mana
                    hud.DecreaseManaBar(40);
                    //Activates the shield
                    shieldActive = true;
                    //Disables the visable shield
                    shield.SetActive(false);
                    //Updates the hudhealth, which essentially adds another 
                    hud.HUDHealth();
                    //Updates player stats
                    hud.DisplayStats();
                    //Play Shield Sound
                    PlayPlayerSounds(1);
                }
                else
                {
                    StartCoroutine(hud.DisplayError("Not Enough Mana"));
                }
            }
            else
            {
                //Ya' Done Gooof'd!
                StartCoroutine(hud.DisplayError("Already Have a Shield"));
            }
        }

        //Checks if the player should level up
        if (playerXP >= 100)
        {
            //Removes the xp and adds to the player level
            playerXP -= 100;
            playerLevel++;
            Debug.Log($"Level: {playerLevel}");
            canAttack = false;

            //pause game and show level up screen
            levelUp.PlayLevelSound(2);
            levelUpMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        if(health == 0 && gameOver.activeSelf == false)
        {
            //Show game over screen and pause the game
            gameOver.SetActive(true);
            Time.timeScale = 0f;
        }

        if (active && Input.GetKeyDown(KeyCode.Return))
        {
            //Reset line and destroy hitbox and disable shield
            movement.line.positionCount = 0;
            Destroy(attack.hitbox);
            shield.SetActive(false);

            turn.SwitchTurn();
        }
    }

    void FixedUpdate()
    {
        //If it is this players turn
        if (active)
        {
            switch (state) //Switch states if active
            {
                case States.NavMesh:
                    movement.NavMeshMovement();
                    mouseWheelLocation = 0;
                    hud.stateIndicator.text = "Combat Movement";
                    break;
                case States.WASD:
                    movement.KeyboardMovement();
                    hud.stateIndicator.text = "WASD Movement";
                    break;
                case States.MeleeAttack:
                    attack.MeleeAttackMode();
                    mouseWheelLocation = 1;
                    hud.stateIndicator.text = "Melee Attack";
                    movement.rb.velocity = Vector3.zero;
                    movement.anim.SetFloat("Speed", (movement.agent.velocity.magnitude / gameObject.transform.localScale.y));
                    canAttack = true;
                    break;
                case States.RangeAttack:
                    attack.RangeAttackMode();
                    mouseWheelLocation = 2;
                    hud.stateIndicator.text = "Mage Attack";
                    movement.rb.velocity = Vector3.zero;
                    movement.anim.SetFloat("Speed", (movement.agent.velocity.magnitude / gameObject.transform.localScale.y));
                    canAttack = true;
                    break;
                case States.Shielding:
                    //Changes the mouse wheel location and tells us which state we are in.
                    mouseWheelLocation = 3;
                    hud.stateIndicator.text = "Shield";
                    movement.rb.velocity = Vector3.zero;
                    movement.anim.SetFloat("Speed", (movement.agent.velocity.magnitude / gameObject.transform.localScale.y));

                    //look at the mouse if active
                    if (active)
                    {
                        attack.LookAtMouse();
                    }

                    //Activates the visable shield if it isn't active
                    if (!shieldActive)
                    {
                        shield.SetActive(true);
                    }
                    break;
            }

            //Display the stats for the current state
            hud.DisplayStats();
            addedMana = false;
        }
        else
        {
            ResetPlayerCombat();
        }
    }

    //Reset the mana and move distance for the player after a combat turn
    public void ResetPlayerCombat()
    {
        //Adds mana after each turn
        if (!addedMana && remainingMana != 100f)
        {
            if (remainingMana <= (maxMana - 35f))
            {
                remainingMana += 35f;
            }
            else
            {
                remainingMana = 100f;
            }

            addedMana = true;
            hud.DecreaseManaBar(0);
        }

        //Reset max move distance and meleeAttacked after turn is done
        movement.maxDistance = 4f * this.gameObject.transform.localScale.y;
        meleeAttacked = false;

        //Delete line, hitbox, and shield
        attack.line.positionCount = 0;

        if (attack.hitbox != null)
        {
            Destroy(attack.hitbox);
        }
    }

    private void ScrollWheel(bool up)
    {
        //This makes it so you can only attack once per turn... For any attack. 
        //If we would rather let you range attack as many times as you want per turn, we can change it.
        
            if (up)//Scrolled up
            {
                //Make sure the location is less than the max length of the array
                if (mouseWheelLocation == mouseWheelStates.Length - 1)
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
                if (mouseWheelLocation == 0)
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
        //Reset line and destroy hitbox and disable shield
        movement.line.positionCount = 0;
        Destroy(attack.hitbox);
        shield.SetActive(false);

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

            //Switches based on the level so that we know what clip to play, as they are level based.
            switch (lController.levels)
            {
                case Levels.Level1:
                    {
                        MattVoiceOver(1);
                        active = false;
                        hud.Tutorial();
                        break;
                    }
                case Levels.Level2:
                    {
                        MattVoiceOver(4);
                        break;
                    }
                case Levels.Level3:
                    {

                            MattVoiceOver(8);

                        break;
                    }
            }
        }

        if (other.gameObject.tag == "enemyProjectile")
        {
            //Gets a random hit chance
            int random = Random.Range(1, hitChance + 1);

            switch (random)
            {
                case 1:
                    Debug.Log("Hit");

                    //Damage the player according to level of enemy
                    DamagePlayer(other.gameObject.transform.parent.GetComponent<EnemyController>(), true);
                    break;
                case 2:
                case 3:
                case 4:
                    Debug.Log("Miss");
                    break;
            }

            //Switch turn
            StartCoroutine(other.gameObject.transform.parent.GetComponent<EnemyController>().ai.SwitchTurn());

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
                case Levels.Level3:
                    lController.levels = Levels.Space;
                    break;
                case Levels.Space:
                    //This will end the game
                    MattVoiceOver(10);
                    //state = States.Neutral;

                    StartCoroutine(WinGame());
                    //hud.WinGame();
                    //lController.levels = Levels.MainMenu;
                    break;
            }
        }

        if (other.gameObject.CompareTag("doorWay"))
        {
            camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
            ThirdPersonCamera.Priority = 25;

            if(entryDoor == null && exitDoor == null)
            {
                entryDoor = GameObject.FindGameObjectWithTag("enterDoor");
                exitDoor = GameObject.FindGameObjectWithTag("exitDoor");
            }

            //Open the first door
            entryDoor.SetActive(false);
        }

        if (other.gameObject.CompareTag("wallCollider"))
        {
            TopCamera.Priority = 20;
        }

        if (other.gameObject.CompareTag("PosterTrigger") && posterTriggerBool)
        {
            posterTriggerBool = false;
            MattVoiceOver(7);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("doorWay"))
        {
            ThirdPersonCamera.Priority = 10;
            other.gameObject.SetActive(false);
            entryDoor.SetActive(true);
            exitDoor.SetActive(true);
        }

        if (other.gameObject.CompareTag("wallCollider"))
        {
            TopCamera.Priority = 10;
        }
    }

    //Used to determine which voice over bit to play
    public void MattVoiceOver(int i)
    {
        if (i < mattVoiceArray.Length)
        {
            //Goes through each needed audio clip
            matt.clip = mattVoiceArray[i];
            //PLAY THAT SUCKA!
            matt.Play();
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
    public void DamagePlayer(EnemyController enemy, bool playSound)
    {
        //TODO - Implement a better damage system based on the level of the enemy

        if (!shieldActive && playSound) //subtract health and play sound if hit by a range attack
        {
            health--;

            //Play enemy player grunt
            MattVoiceOver(12);
        }
        else if(shieldActive) //if the shield is active disable it and play the shield sound
        {
            shieldActive = false;
            PlayPlayerSounds(2);
        }
        else if (!playSound)//Only subtract the health if the player is hit by an enemy melee attack
        {
            health--;
        }

        if (health == 0)
        {
            //Activate GameOver Screen
        }

        hud.HUDHealth();
    }

    public void PlayEnemySounds(int i)
    {
        enemySource.clip = enemySounds[i];
        enemySource.Play();
    }

    public void PlayPlayerSounds(int i)
    {
        playerSource.clip = playerSounds[i];
        playerSource.Play();
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
            //Initilize player stats
            attack.damage = data.damage;
            attack.spellCost = (int)data.spellCost;
            playerLevel = data.playerLevel;
            playerXP = data.playerXP;

            //levels = data.level;
            return data.level;
        }
        else
        {
            return Levels.MainMenu;
        }
    }

    IEnumerator WinGame()
    {
        yield return new WaitForSeconds(3.5f);

        hud.WinGame();
        lController.levels = Levels.MainMenu;
    }

    //Waits one second before calling the SwitchTurn method in the turnBasedSystem Script
    IEnumerator SwitchTurn()
    {
        yield return new WaitForSeconds(1f);

        turn.SwitchTurn();
    }

}
