using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool actionDone = true;
    GameObject player;
    EnemyController enemyController;
    NavMeshAgent agent;
    NavMeshPath path;
    float beginningDistance = 0;
    float remaining;
    bool mageActive = false;
    bool bruteActive = false;
    bool rangeAttack = false;
    GameObject[] waypoints;
    Quaternion targetRotation;

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
            Vector3 distanceToPlayer = player.transform.position - transform.position;
            float distance = distanceToPlayer.magnitude;
            remaining = agent.remainingDistance;

            //Stop moving if the enemy hasn't moved more than 10 of if the distance to the player is less than 7
            if (beginningDistance - remaining >= 10f || distance < 12f && distance > 7)
            {
                agent.ResetPath();
                remaining = 0;
                mageActive = false;
                StartCoroutine(RangeAttackWait());
                targetRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                //RangeAttack();
            }
        }

        if (bruteActive) //Activates if the enemy is a brute
        {
            Vector3 distanceToPlayer = player.transform.position - transform.position;           
            float distance = distanceToPlayer.magnitude;
            remaining = agent.remainingDistance;

            if (beginningDistance - remaining >= 10f || distanceToPlayer.magnitude <= 3)
            {
                remaining = 0;
                agent.ResetPath();
                bruteActive = false;
            }
        }

        if (rangeAttack)
        {
            //float random = Random.Range(-4, 4);
            ////transform.LookAt(player.transform);
            //Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
            //transform.Rotate(0, random, 0);

            //TODO - Find a way to check when done rotating

            rangeAttack = false;
            RaycastHit hit;
            Debug.Log("asdfasdf");

           if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
           {

               if (hit.collider.tag == "player")
               {
                    //Debug.Log("Random #: " + random);
                    Debug.DrawLine(transform.position, hit.transform.position, Color.red, 5f);
                    //Debug.Log("Hit Player");
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
        MageMove();
        //RangeAttack();
    }

    private void BruteAI()
    {
        BruteMove();
        //MeleeAttack();
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
        float random = Random.Range(-4, 4);
        ////transform.LookAt(player.transform);
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5 * Time.deltaTime);
        transform.Rotate(0, random, 0);

        if (Mathf.Abs(transform.rotation.y - targetRotation.y) < 1)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
            {

                if (hit.collider.tag == "player")
                {
                    //Debug.Log("Random #: " + random);
                    Debug.DrawLine(transform.position, hit.transform.position, Color.red, 5f);
                    //Debug.Log("Hit Player");
                }
                else
                {
                    //Debug.Log("Miss: " + random);
                }
            }
        }
    }

    //Attack Player through close combat
    private void MeleeAttack()
    {

    }

    IEnumerator RangeAttackWait()
    {
        yield return new WaitForSeconds(0.4f);
        rangeAttack = true;
    }
}
