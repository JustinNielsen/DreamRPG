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
}
