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
    int enemyHealth;

    // Start is called before the first frame update
    void Start()
    {
        cam = Instantiate(camPrefab);
        cam.Priority = 5;
        cam.Follow = transform;

        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        enemyHealth = 1;
    }

    void Update()
    {
        if(enemyHealth <= 0)
        {
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
        if(other.tag == "projectile")
        {
            Destroy(other.gameObject);
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;
        }

        if (other.tag == "attack" && !active)
        {
            Damage damage = other.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;
            Debug.Log($"Enemy Health: {enemyHealth}");
        }
    }
}
