using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool actionDone = true;
    public GameObject mageShot;
    GameObject player;
    EnemyController enemyController;
    NavMeshAgent agent;
    NavMeshPath path;
    float beginningDistance = 0;
    float remaining;
    bool mageActive = false;
    bool bruteActive = false;
    bool rotate = false;
    GameObject[] waypoints;
    Quaternion targetRotation;
    int classType;
    Vector3 distanceToPlayer;
    float rangeDifference;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        waypoints = GameObject.FindGameObjectsWithTag("waypoint");
    }

    // Update is called once per frame
    void Update()
    {
        if (mageActive) //Activate if the enemy is a mage - Stays within distance to do range attacks to the player
        {
            distanceToPlayer = player.transform.position - transform.position;
            float distance = distanceToPlayer.magnitude;
            remaining = agent.remainingDistance;

            //Stop moving if the enemy hasn't moved more than 10 of if the distance to the player is less than 7
            if (beginningDistance - remaining >= 10f || distance < 12f && distance > 7)
            {
                agent.ResetPath();
                remaining = 0;
                mageActive = false;
                targetRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                StartCoroutine(AttackWait());
            }
        }

        if (bruteActive) //Activates if the enemy is a brute
        {
            distanceToPlayer = player.transform.position - transform.position;           
            float distance = distanceToPlayer.magnitude;
            remaining = agent.remainingDistance;

            if (beginningDistance - remaining >= 10f || distanceToPlayer.magnitude <= 3)
            {
                agent.ResetPath();
                remaining = 0;
                bruteActive = false;
                targetRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                StartCoroutine(AttackWait());
            }
        }

        if (rotate)
        {
            Vector3 targetDir = player.transform.position - transform.position;
            float step = 2f * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            Debug.Log(Quaternion.Dot(transform.rotation, targetRotation));

            if(classType == 1)
            {
                rangeDifference = 0.0004f;
            }
            else
            {
                rangeDifference = 0.008f;
            }

            if (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) >= 1f - rangeDifference)
            {
                Debug.Log("Test");
                if (classType == 1)
                {
                    transform.LookAt(player.transform);
                    rotate = false;

                    RangeAttack();                    
                }
                else
                {
                    rotate = false;

                    if (distanceToPlayer.magnitude <= 3.5)
                    {
                        MeleeAttack();
                    }
                
                }

            }
            
        }

        if(beginningDistance == 0)
        {
            beginningDistance = agent.remainingDistance;
        }
    }

    public void AI(int aiIndex)
    {
        switch (aiIndex)
        {
            case 1: //Mage AI
                MageAI();
                break;
            case 2: // Brute AI
                BruteAI();
                break;
            case 3: // Assassin AI
                AssassinAI();
                break;
        }
    }

    private void MageAI()
    {
        classType = 1;
        MageMove();
    }

    private void BruteAI()
    {
        classType = 2;
        BruteMove();
    }

    private void AssassinAI()
    {
        int random = Random.Range(0, 1);

        if(random == 0)
        {
            BruteMove();
        }
        else
        {
            MageMove();
        }
    }

    private void BruteMove()
    {
        agent.ResetPath();
        agent.SetDestination(player.transform.position);
        beginningDistance = agent.remainingDistance;
        bruteActive = true; //Always moves towards the player and tries to attack him
    }

    private void MageMove()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        float distance = distanceToPlayer.magnitude;

        if (distance < 7f) //If the distance to the player is less than 7 run away
        {
            //Initilize nearest waypoint to the first waypoint in the waypoints array
            GameObject nearestWaypoint = waypoints[0];
            //Find the waypoint that is furthest away from the player and move towards it.
            foreach (GameObject waypoint in waypoints)
            {
                float distToPlayer = Vector3.Distance(player.transform.position, waypoint.transform.position);
                float nearestDist = Vector3.Distance(player.transform.position, nearestWaypoint.transform.position);

                if (distToPlayer > nearestDist)
                {
                    nearestWaypoint = waypoint;
                }
            }

            agent.ResetPath();
            agent.SetDestination(nearestWaypoint.transform.position);

        }
        else
        {
            agent.ResetPath();
            agent.SetDestination(player.transform.position);
        }

        beginningDistance = agent.remainingDistance;
        //Debug.Log("Distance" + distance);

        mageActive = true; //Stays within range for range attacks
    }

    //Attack Player from a range
    private void RangeAttack()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
        {
            if (hit.collider.tag == "player")
            {
                Debug.DrawLine(transform.position, hit.transform.position, Color.red, 5f);
                //Debug.Log("Hit Player");
            }
        }

        LaunchProjectile();
    }

    //Attack Player through close combat
    private void MeleeAttack()
    {
        Vector3 distance = player.transform.position - transform.position;

        if(distance.magnitude <= 3.5f)
        {
            int random = Random.Range(1, 5);
            if(random < 3)
            {
                Debug.Log("Hit");
            }
            else
            {
                Debug.Log("Miss");
            }
        }
    }

    private void LaunchProjectile()
    {
        Vector3 pos = transform.position + transform.forward;
        GameObject projectile = Instantiate(mageShot, transform.position, transform.rotation);
        Destroy(projectile, 4f);
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(0.4f);
        rotate = true;
    }
}
