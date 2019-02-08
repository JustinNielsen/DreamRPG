using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedSystem : MonoBehaviour
{

    public GameObject[] turnArr;
    public int turn = 0;

    public List<Character> charList;

    //public PlayerController pController;
    //public EnemyController[] eController;

    // Start is called before the first frame update
    private void Start()
    {
        //InitializeTurnArray();
        //InitializeEnemyArr();
        charList = new List<Character>();
        InitializeCharacterList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchTurn();
        }
    }

    private void InitializeCharacterList()
    {
        foreach(GameObject obj in turnArr)
        {
            charList.Add(new Character(obj));
        }
    }

    /*
    private void InitializeTurnArray()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        turnArr = new GameObject[enemies.Length + 1];

        turnArr[0] = GameObject.FindGameObjectWithTag("player");

        for(int i = 1; i < turnArr.Length; i++)
        {
            turnArr[i] = enemies[i - 1];
        }
    }

    
    private void InitializeEnemyArr()
    {
        pController = turnArr[0].GetComponent<PlayerController>();

        eController = new EnemyController[turnArr.Length - 1];

        for(int i = 1; i < turnArr.Length; i++)
        {
            eController[i-1] = turnArr[i].GetComponent<EnemyController>();
        }
    }
    */

    public void SwitchTurn()
    {
        if(turn == turnArr.Length - 1)
        {
            turn = 0;
        }
        else
        {
            turn++;
        }

        if(turn == 0)
        {
            foreach(Character classObj in charList)
            {
                if (classObj.Obj.CompareTag("player"))
                {
                    classObj.PController.TogglePlayer(true);
                }
                else
                {
                    classObj.EController.ToggleEnemy(false);
                }
            }
        }
        else
        {
            foreach (Character classObj in charList)
            {
                if (classObj.Obj.CompareTag("player"))
                {
                    classObj.PController.TogglePlayer(false);
                }
                else
                {
                    classObj.EController.ToggleEnemy(false);
                }
            }

            charList[turn].EController.ToggleEnemy(true);
        }
    }
}
