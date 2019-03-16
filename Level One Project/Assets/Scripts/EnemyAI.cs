using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool actionDone = true;
    public GameObject mageShot;
    public float maxMovementRange;
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
    PlayerController pController;

    //Distance variables according to enemy size
    float maxRangeDistance;
    float minRangeDistance;
    float meleeDistance;
    float meleeAttackDistance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        pController = player.GetComponent<PlayerController>();
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        waypoints = GameObject.FindGameObjectsWithTag("waypoint");
        maxMovementRange = 6.5f * this.gameObject.transform.localScale.y;
        agent.speed = 1.4f * this.gameObject.transform.localScale.y;

        if (pController.lController.levels == Levels.Space)
        {
            maxRangeDistance = (12 * 0.15f);
            minRangeDistance = (7 * 0.15f);
            meleeDistance = (3 * 0.15f);
            meleeAttackDistance = (3.5f * 0.15f);
        }
        else
        {
            maxRangeDistance = 12;
            minRangeDistance = 7;
            meleeDistance = 4;
            meleeAttackDistance = 3.5f;
        }
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
            if (beginningDistance - remaining >= maxMovementRange || distance < maxRangeDistance && distance > minRangeDistance)
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

            if (beginningDistance - remaining >= maxMovementRange || distanceToPlayer.magnitude <= meleeDistance)
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

            //Debug.Log(Quaternion.Dot(transform.rotation, targetRotation));

            if(classType == 1)
            {
                rangeDifference = 0.001f;
            }
            else
            {
                rangeDifference = 0.008f;
            }

            if (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) >= 1f - rangeDifference)
            {
                if (classType == 1)
                {
                    transform.LookAt(player.transform);
                    rotate = false;

                    RangeAttack();                    
                }
                else
                {
                    rotate = false;

                    if (distanceToPlayer.magnitude <= meleeDistance)
                    {
                        MeleeAttack();
                    }
                    else
                    {
                        StartCoroutine(SwitchTurn());
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
            case 1: //Brute AI
                BruteAI();
                break;
            case 2: // Mage AI
                MageAI();
                break;
            case 3: // Assassin AI
                AssassinAI();
                break;
        }
    }

    private void BruteAI()
    {
        classType = 2;
        BruteMove();
    }

    private void MageAI()
    {
        classType = 1;
        MageMove();
    }

    private void AssassinAI()
    {
        agent = GetComponent<NavMeshAgent>();

        int random = Random.Range(0, 2);

        if(random == 0)
        {
            Debug.Log("Brute");
            classType = 2;
            BruteMove();
        }
        else
        {
            Debug.Log("Mage");
            classType = 1;
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
        waypoints = GameObject.FindGameObjectsWithTag("waypoint");

        if (distance < minRangeDistance) //If the distance to the player is less than 7 run away
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

        Vector3 lineStart = transform.position;
        lineStart.y = transform.position.y + (this.transform.localScale.y * 1.3f);

        if (Physics.Raycast(lineStart, transform.forward, out hit, 20f))
        {
            if (hit.collider.tag == "player")
            {
                Debug.DrawLine(transform.position, hit.transform.position, Color.red, 5f);
                //Debug.Log("Hit Player");
                StartCoroutine(LaunchProjectile());
            }
            else
            {
                StartCoroutine(SwitchTurn());
            }
        }
        else
        {
            StartCoroutine(SwitchTurn());
        }

    }

    //Attack Player through close combat
    private void MeleeAttack()
    {
        Vector3 distance = player.transform.position - transform.position;

        if(distance.magnitude <= meleeAttackDistance + (transform.localScale.y * 2))
        {
            int random = Random.Range(1, pController.hitChance + 1);

            enemyController.anim.SetTrigger("Attack");

            switch (random)
            { //TODO - Notify player when an attack hits or misses
                case 1:
                    Debug.Log("Hit");
                    pController.PlayEnemySounds(0);

                    //play grunt if shield in inactive
                    if (!pController.shieldActive)
                    {
                        pController.MattVoiceOver(12);
                    }

                    StartCoroutine(MeleeAttackWait());
                    //pController.DamagePlayer(this.gameObject.GetComponent<EnemyController>());
                    break;
                case 2:
                case 3:
                case 4:
                    Debug.Log("Miss");
                    break;
            }
        }

        StartCoroutine(SwitchTurn());
    }

    IEnumerator EnemyStuckCheck()
    {
        yield return new WaitForSeconds(10f);

        //Switches the turn if the enemy is still active
        if (enemyController.active)
        {
            StartCoroutine(SwitchTurn());
        }
    }

    IEnumerator MeleeAttackWait()
    {
        yield return new WaitForSeconds(0.8f);

        //Damage the player
        pController.DamagePlayer(this.gameObject.GetComponent<EnemyController>(), false);
    }

    IEnumerator LaunchProjectile()
    {
        //Vector3 pos = transform.position + transform.forward;


        //Vector3 lineStart = transform.position;
        //lineStart.y = transform.position.y + (this.transform.localScale.y * 1.3f);

        //GameObject projectile = Instantiate(mageShot, lineStart, transform.rotation, this.transform);
        //Destroy(projectile, 4f);

        //Start the animation
        enemyController.anim.SetTrigger("Attack");

        //Wait for the animation to catch up
        yield return new WaitForSeconds(0.8f);

        if (pController.lController.levels == Levels.Space)
        {
            Vector3 lineStart = transform.position + (transform.forward * pController.gameObject.transform.localScale.y);
            lineStart.y = transform.position.y + (player.transform.localScale.y * 1.3f);

            GameObject projectile = Instantiate(mageShot, lineStart, transform.rotation, this.transform);
            Destroy(projectile, 1.6f);
        }
        else
        {
            Vector3 lineStart = transform.position + (transform.forward * 3f);
            lineStart.y = transform.position.y + (player.transform.localScale.y * 1.3f);

            GameObject projectile = Instantiate(mageShot, lineStart, transform.rotation, this.transform);
            Destroy(projectile, 1.6f);
        }

        pController.PlayEnemySounds(3);
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(0.4f);
        rotate = true;
    }

    IEnumerator SwitchTurn()
    {
        yield return new WaitForSeconds(2f);

        pController.turn.SwitchTurn();
    }
}
