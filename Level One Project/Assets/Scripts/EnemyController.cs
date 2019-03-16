using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public bool active = false;
    float movementSpeed = 5.0f;
    private CinemachineVirtualCamera cam;
    public CinemachineVirtualCamera camPrefab;
    private EnemyAI ai;
    private NavMeshAgent agent;
    public int enemyType = 1;
    Rigidbody rb;
    public int enemyHealth;
    public int enemyLevel;
    TurnBasedSystem turn;
    PlayerController pController;
    public Animator anim;
    CinemachineTransposer transposer;
    private GameObject cameraFollow;

    // Start is called before the first frame update
    void Start()
    {
        //Get animator of the enemy
        anim = GetComponent<Animator>();

        //Create an empty gameobject at the enemies position
        cameraFollow = new GameObject("cameraFollow");
        cameraFollow.transform.position = new Vector3(this.gameObject.transform.position.x, (this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2)), this.gameObject.transform.position.z);
        cameraFollow.transform.rotation = this.gameObject.transform.rotation;

        cam = Instantiate(camPrefab);
        cam.Priority = 5;
        cam.Follow = cameraFollow.transform;
        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
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
                    transposer.m_FollowOffset = new Vector3((-0.5f * zoomAmount), (1.5f * zoomAmount), (-0.25f * zoomAmount));
                }
            }
        }
    }

    public void ToggleEnemy(bool isOn)
    {
        if (isOn)
        {
            active = true;
            cam.Priority = 15;
            ai.enabled = true;
            ai.AI(enemyType);
        }
        else
        {
            active = false;
            cam.Priority = 5;
            ai.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Activates when the players projectile hits the enemy
        if(other.tag == "playerProjectile")
        {
            Destroy(other.gameObject);
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;
            pController.PlayEnemySounds(1);
        }

        //Activates when the player hits the enemy
        if (other.tag == "attack")
        {
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;
            pController.PlayEnemySounds(1);
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
}
