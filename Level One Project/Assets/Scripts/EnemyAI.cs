using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private int[] actionsArray;
    public bool actionDone = true;

    // Start is called before the first frame update
    void Start()
    {
        //Initizlize actions array
        actionsArray = new int[4] { 1, 2, 3, 4 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AI()
    {
        RandomizeActions();
        int i = 0;

        while(i < actionsArray.Length)
        {

            //actionDone = IsDone();

            if (actionDone)
            {
                switch (actionsArray[i])
                {
                    case 1:
                        //Move();
                        Debug.Log("Move");
                        break;
                    case 2:
                        //MeleeAttack();
                        Debug.Log("MeleeAttack");
                        break;
                    case 3:
                        //MagicAttack();
                        Debug.Log("MagicAttack");
                        break;
                    case 4:
                        //Shield();
                        Debug.Log("Shield");
                        break;
                }

                i++;
            }
        }
    }

    void RandomizeActions()
    {
        for(int i = 0; i < actionsArray.Length; i++)
        {
            int temp = actionsArray[i];
            int randomNum = Random.Range(0, actionsArray.Length - 1);
            actionsArray[i] = actionsArray[randomNum];
            actionsArray[randomNum] = temp;
        }
    }
}
