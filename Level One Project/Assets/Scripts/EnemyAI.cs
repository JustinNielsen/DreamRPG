using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool actionDone = true;
    private GameObject player;
    private EnemyController enemyController;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private float enemyMoveDistance = 10f;

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
        MoveToPlayer(10);
    }

    private void BruteAI()
    {

    }

    private void AssassinAI()
    {

    }

    private void MoveToPlayer(float distanceToMove)
    {
        agent.SetDestination(player.transform.position);
        float beginningDistance = agent.remainingDistance;
        float remaining = 0;

        while(agent.remainingDistance - remaining < enemyMoveDistance)
        {
            remaining = agent.remainingDistance;
        }

        agent.ResetPath();
    }
}
