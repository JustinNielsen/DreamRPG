using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    public bool active = false;
    float movementSpeed = 5.0f;
    private CinemachineVirtualCamera cam;
    public CinemachineVirtualCamera camPrefab;
    public EnemyAI ai;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    public int enemyType = 1;
    Rigidbody rb;
    public int enemyMaxHealth;
    public int enemyHealth;
    public Canvas healthIndictor;
    public int enemyLevel;
    TurnBasedSystem turn;
    PlayerController pController;
    public Animator anim;
    CinemachineTransposer transposer;
    private GameObject cameraFollow;
    public TextMeshProUGUI healthText;
    public Slider healthBar;

    // Start is called before the first frame update
    void Start()
    {
        //Get animator of the enemy
        anim = GetComponent<Animator>();

        //Initilize the enemies health to the max health and update the indicator
        enemyHealth = enemyMaxHealth;
        healthText.text = "Health: " + enemyHealth;
        healthBar.value = 1;

        //Create an empty gameobject at the enemies position
        cameraFollow = new GameObject("cameraFollow");
        cameraFollow.transform.position = new Vector3(this.gameObject.transform.position.x, (this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2)), this.gameObject.transform.position.z);
        cameraFollow.transform.rotation = new Quaternion(0, 180, 0, 0);

        //Create and intilize camera and various enemy components
        cam = Instantiate(camPrefab);
        cam.Priority = 5;
        cam.Follow = cameraFollow.transform;
        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        rb = GetComponent<Rigidbody>();

        transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        //Initlize turn based system script
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
        pController = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerController>();

        //Changes the size of the enemy based on the type it is
        switch (enemyType)
        {
            case 1:
                transform.localScale = pController.gameObject.transform.localScale + new Vector3(0.25f, 0.25f, 0.25f);
                break;
            case 2:
                transform.localScale = pController.gameObject.transform.localScale;
                break;
            case 3:
                //transform.localScale = pController.gameObject.transform.localScale + new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case 4:
                transform.localScale = pController.gameObject.transform.localScale;
                break;
        }

        if(pController.lController.levels == Levels.Space)
        {
            transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = new Vector3(-0.5f, 1.5f, -0.25f);
        }
    }

    void Update()
    {
        if(enemyHealth <= 0)
        {
            //Uses a simple formula to find the xp. It should work for the most part
            pController.playerXP += ((float)enemyLevel / (float)pController.playerLevel) * 50;
            Debug.Log("Added " + (((float)enemyLevel / (float)pController.playerLevel) * 50) + " XP");

            turn.ResetArrays();

            pController.PlayEnemySounds(2);

            Destroy(this.gameObject);
            //Resets the text
        }
        else
        {
            anim.SetFloat("Speed", (agent.velocity.magnitude / this.gameObject.transform.localScale.y));

            healthIndictor.transform.LookAt(pController.cam.transform.position, Vector3.up);

            if (active)
            {
                //position between player and enemy
                Vector3 middlePoint = ((this.gameObject.transform.position + pController.gameObject.transform.position) / 2);
                middlePoint.y = (this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2));
                Vector3 distance = this.gameObject.transform.position - pController.gameObject.transform.position;
                float distanceToPlayer = distance.magnitude;
                float zoomAmount = distanceToPlayer / 2;

                if(pController.lController.levels == Levels.Space && zoomAmount > 0.8f)
                {
                    zoomAmount *= 0.6f;
                }

                if(pController.lController.levels != Levels.Space && zoomAmount < 5.5f)
                {
                    zoomAmount = 5.5f;
                }

                if(zoomAmount < 0.8f)
                {
                    zoomAmount = 0.8f;
                }

                Debug.Log(zoomAmount);
                cameraFollow.transform.position = middlePoint;

                if(pController.lController.levels == Levels.Space)
                {
                    transposer.m_FollowOffset = new Vector3((-0.75f * zoomAmount), (1.75f * zoomAmount), (-0.325f * zoomAmount));
                }
                else
                {
                    zoomAmount *= 0.15f;
                    transposer.m_FollowOffset = new Vector3((-6f * zoomAmount), (12f * zoomAmount), (-3f * zoomAmount));
                }
            }
        }
    }

    public void ToggleEnemy(bool isOn)
    {
        if (isOn)
        {
            //obstacle.enabled = false;
            //agent.enabled = true;
            StartCoroutine(NavMeshToggle(true));

            //active = true;
            //cam.Priority = 15;
            //ai.enabled = true;
            //ai.AI(enemyType);            
        }
        else
        {
            //agent.enabled = false;
            //obstacle.enabled = true;
            StartCoroutine(NavMeshToggle(false));

            //active = false;
            //cam.Priority = 5;
            //ai.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Activates when the players projectile hits the enemy
        if(other.tag == "playerProjectile")
        {
            //Destroy the projectile
            Destroy(other.gameObject);

            //Get the damage value and apply it to the enemy health
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;

            //Update the health indicator
            healthText.text = "Health: " + enemyHealth;
            healthBar.value = (1.0f / (float)enemyMaxHealth) * (float)enemyHealth;

            //Play enemy hit sound
            pController.PlayEnemySounds(2);
        }

        //Activates when the player hits the enemy
        if (other.tag == "attack")
        {
            //Get the damage value and apply it to the enemy health
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;

            //Update the health indicator
            healthText.text = "Health: " + enemyHealth;
            healthBar.value = (1.0f / (float)enemyMaxHealth) * (float)enemyHealth;

            //Play enemy hit sound
            pController.PlayEnemySounds(2);
        }
    }

    private void OnMouseOver()
    {
        if (enemyHealth > 0)
        {
            //Updates the enemyStates text
            pController.hud.enemyStats.text = $"Enemy Health: {enemyHealth}\nEnemy Level: {enemyLevel}";
        }
        else
        {
            //Resets the text
            pController.hud.enemyStats.text = "";
        }
    }

    private void OnMouseExit()
    {
        //Resets the text
        pController.hud.enemyStats.text = "";
    }

    IEnumerator NavMeshToggle(bool state)
    {
        if (state)
        {
            if(pController.lController.levels != Levels.Space)
            {
                obstacle.enabled = false;
            }

            yield return null;
            agent.enabled = true;

            active = true;
            cam.Priority = 15;
            ai.enabled = true;
            ai.AI(enemyType);
        }
        else
        {

            if(pController.lController.levels != Levels.Space)
            {
                agent.enabled = false;
                obstacle.enabled = true;
            }

            active = false;
            cam.Priority = 5;
            ai.enabled = false;
        }
    }
}
