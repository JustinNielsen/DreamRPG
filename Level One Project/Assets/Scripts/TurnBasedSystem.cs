using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedSystem : MonoBehaviour
{

    public GameObject[] turnArr;
    public int turn = 0;
    public List<Character> charList;

    // Start is called before the first frame update
    private void Start()
    {

    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchTurn();
        }
    }

    public void ResetArrays()
    {
        InitializeTurnArray();
        //Create an empty list for charList
        charList = new List<Character>();
        InitializeCharacterList();
    }

    private void InitializeCharacterList()
    {
        //Initilizes charList with objects from the turnArr
        foreach(GameObject obj in turnArr)
        {
            charList.Add(new Character(obj));
        }
    }

    private void InitializeTurnArray()
    {
        //Creates an array of enemy objects
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        //Creates the turnArr
        turnArr = new GameObject[enemies.Length + 1];
        //Sets the first element in the turnArr to the player
        turnArr[0] = GameObject.FindGameObjectWithTag("player");
        //Adds the enemies to the turnArr
        for(int i = 1; i < turnArr.Length; i++)
        {
            turnArr[i] = enemies[i - 1];
        }
    }
    
    //Switches the turn to a differnt character
    public void SwitchTurn()
    {
        //If turn is on turnArr's final value then set turn to 0
        if(turn == turnArr.Length - 1)
        {
            turn = 0;
        }
        else //Otherwise advance turn by 1
        {
            turn++;
        }

        //If turn = 0 activate the player object and deactivate the enemy objects
        if(turn == 0)
        {
            for (int i = 0; i < charList.Count; i++)
            {
                //Grabs the character wanted
                Character classObj = charList[i];
                //Makes sure that the object grabbed isn't destroyed. If it is destroyed, it will just ignore it.
                if (classObj.Obj != null)
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
                else
                {
                    //This will remove the troublesome character.
                    charList.Remove(classObj);
                }
            }
        }
        else //Otherwise if turn != 0 deactivate everything and activate the object whose turn it is
        {
            //Deactivate everything

            for (int i = 0; i < charList.Count; i++)
            {
                //Grabs the character wanted
                Character classObj = charList[i];
                //Makes sure that the object grabbed isn't destroyed. If it is destroted, it will just ignore it.
                if (classObj.Obj != null)
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
                else
                {
                    //This will remove the troublesome character.
                    charList.Remove(classObj);
                }

            }

            try
            {
                //Activate the object based on the turn using the charList
                if (charList[turn] == null)
                {
                    Debug.Log("Dead, but in");
                    SwitchTurn();
                }
                else
                {
                    Debug.Log("In");
                    //Switches to the first available turn.
                    charList[turn].EController.ToggleEnemy(true);
                }
            }
            catch
            {
                Debug.Log("Dead");
                SwitchTurn();
            }
        }
    }
}
