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
    public int enemyHealth;
    // Start is called before the first frame update
    void Start()
    {
        cam = Instantiate(camPrefab);
        cam.Priority = 5;
        cam.Follow = transform;

        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(enemyHealth == 0)
        {
            //This will destroy this object, therefore killing it. Potentially add animation before.
            Destroy(this);
        }
        if (active)
        {
            //Forward and backward movement
            transform.position += transform.forward * Time.deltaTime * movementSpeed * Input.GetAxis("Vertical");

            //Left and right movement
            transform.position += transform.right * Time.deltaTime * movementSpeed * Input.GetAxis("Horizontal");
        }
    }

    public void ToggleEnemy(bool isOn)
    {
        if (isOn)
        {
            active = true;
            cam.Priority = 15;
            agent.enabled = true;
            ai.AI(1);
        }
        else
        {
            active = false;
            cam.Priority = 5;
            agent.enabled = false;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
     Debug.Log("Enemy Collision with: " + collision.gameObject.tag);
        if(collision.gameObject.tag == "attack")
        {
            Damage damage = collision.gameObject.GetComponent<Damage>();
            enemyHealth -= damage.damage;
            Debug.Log($"Enemy Health: {enemyHealth}");
        }
        
    }
}
