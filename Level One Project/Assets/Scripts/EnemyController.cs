using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using TMPro;

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
    // Start is called before the first frame update
    void Start()
    {
        cam = Instantiate(camPrefab);
        cam.Priority = 5;
        cam.Follow = transform;

        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        //Initlize turn based system script
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
        pController = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerController>();
        
    }

    void Update()
    {
        if(enemyHealth <= 0)
        {
            turn.ResetArrays();

            //Uses a simple formula to find the xp. It should work for the most part
            pController.playerXP += enemyLevel / pController.playerLevel * 50;
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            //Forward and backward movement
            transform.position += transform.forward * Time.deltaTime * movementSpeed * Input.GetAxis("Vertical");

            //Left and right movement
            transform.position += transform.right * Time.deltaTime * movementSpeed * Input.GetAxis("Horizontal");
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void ToggleEnemy(bool isOn)
    {
        if (isOn)
        {
            //The enemy is active
            active = true;
            //Changes to the enemy camera.
            cam.Priority = 15;
            //The enemy ai is enabled, and the enemy type is chosen.
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
        }

        //Activates when the player hits the enemy
        if (other.tag == "attack" && !active)
        {
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;
            Debug.Log($"Enemy Health: {enemyHealth}");
        }
    }

    private void OnMouseOver()
    {
        //Updates the enemyStates text.
        pController.hud.enemyStats.text = $"Enemy Health: {enemyHealth}\n" +
            $"Enemy Level: {enemyLevel}";
    }

    private void OnMouseExit()
    {
        //Resets the text.
        pController.hud.enemyStats.text = "";
    }
}
