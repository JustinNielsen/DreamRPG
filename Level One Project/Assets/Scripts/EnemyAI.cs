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
    float beginningDistance;
    float remaining;
    bool mageActive = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mageActive)
        {
            Vector3 distanceToPlayer = player.transform.position - transform.position;
            float distance = distanceToPlayer.magnitude;

            if (beginningDistance - remaining >= 10f || distanceToPlayer.magnitude <= 3)
            {
                remaining = 0;
                agent.ResetPath();
                mageActive = false;
            }
            else
            {
                remaining = agent.remainingDistance;
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
            MoveToPlayer(); 
    }

    private void BruteAI()
    {

    }

    private void AssassinAI()
    {

    }

    private void MoveToPlayer()
    {
        agent.SetDestination(player.transform.position);
        remaining = 0;
        beginningDistance = agent.remainingDistance;
        mageActive = true;
    }
}
